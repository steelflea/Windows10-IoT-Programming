using SenseHatDisplay.Helpers;
using SenseHatIO.SenseHatLedArray;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Media.FaceAnalysis;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

namespace FacialTracking
{
    public sealed partial class MainPage : Page
    {
        private const string previewStartDescription = "Start preview";
        private const string previewStopDescription = "Stop preview";

        private CameraCapture cameraCapture = new CameraCapture();

        private FaceDetector faceDetector;
        private FaceTracker faceTracker;

        private BitmapPixelFormat faceDetectorSupportedPixelFormat;
        private BitmapPixelFormat faceTrackerSupportedPixelFormat;

        private LedArray ledArray;
        private LedPixelPosition previousLedPixelPosition;

        public MainPage()
        {
            InitializeComponent();           

            UpdateUI();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            await InitializeLedArray();
        }

        private async Task InitializeLedArray()
        {
            const byte address = 0x46;
            var device = await I2cHelper.GetI2cDevice(address);

            if (device != null)
            {
                ledArray = new LedArray(device);

                ButtonPreview_Click(null, null);
            }
        }

        private void UpdateUI()
        {
            ButtonPreview.Content = cameraCapture.IsPreviewActive ? previewStopDescription : previewStartDescription;
        }

        private async void ButtonPreview_Click(object sender, RoutedEventArgs e)
        {
            await cameraCapture.Initialize(CaptureElementPreview);

            await InitializeFaceDetection();

            if (cameraCapture.IsInitialized)
            {
                await UpdatePreviewState();

                UpdateUI();
            }
            else
            {
                Debug.WriteLine("Video capture device could not be initialized");
            }
        }

        private async Task InitializeFaceDetection()
        {
            if (FaceDetector.IsSupported)
            {
                if (faceDetector == null)
                {
                    faceDetector = await FaceDetector.CreateAsync();
                    faceDetectorSupportedPixelFormat = FaceDetector.GetSupportedBitmapPixelFormats().FirstOrDefault();
                }
            }
            else
            {
                Debug.WriteLine("Warning. FaceDetector is not supported on this device");
            }

            if (FaceTracker.IsSupported)
            {
                if (faceTracker == null)
                {
                    faceTracker = await FaceTracker.CreateAsync();
                    faceTrackerSupportedPixelFormat = FaceTracker.GetSupportedBitmapPixelFormats().FirstOrDefault();
                }
            }
            else
            {
                Debug.WriteLine("Warning. FaceTracking is not supported on this device");
            }
        }

        private async Task UpdatePreviewState()
        {
            if (!cameraCapture.IsPreviewActive)
            {
                await cameraCapture.Start();

                BeginTracking();
            }
            else
            {
                await cameraCapture.Stop();

                CanvasFaceDisplay.Children.Clear();
            }
        }

        private async void ButtonDetectFaces_Click(object sender, RoutedEventArgs e)
        {
            if (faceDetector != null)
            {
                var inputBitmap = await cameraCapture.CapturePhotoToSoftwareBitmap();

                var facesDetected = await DetectFaces(inputBitmap);

                DisplayFaceLocations(facesDetected);
            }
        }
        private async Task<IList<DetectedFace>> DetectFaces(SoftwareBitmap inputBitmap)
        {
            if (!FaceDetector.IsBitmapPixelFormatSupported(inputBitmap.BitmapPixelFormat))
            {
                inputBitmap = SoftwareBitmap.Convert(inputBitmap, faceDetectorSupportedPixelFormat);
            }

            return await faceDetector.DetectFacesAsync(inputBitmap);
        }

        private void DisplayFaceLocations(IList<DetectedFace> facesDetected)
        {
            for (int i = 0; i < facesDetected.Count; i++)
            {
                var detectedFace = facesDetected[i];
                var detectedFaceLocation = DetectedFaceToString(i + 1, detectedFace.FaceBox);

                AddItemToListBox(detectedFaceLocation);
            }
        }

        private void AddItemToListBox(object item)
        {
            ListBoxInfo.Items.Add(item);
            ListBoxInfo.SelectedIndex = ListBoxInfo.Items.Count - 1;
        }

        private string DetectedFaceToString(int index, BitmapBounds detectedFaceBox)
        {
            return string.Format("Face no: {0}. X: {1}, Y: {2}, Width: {3}, Height: {4}",
                index,
                detectedFaceBox.X,
                detectedFaceBox.Y,
                detectedFaceBox.Width,
                detectedFaceBox.Height);
        }

        private void ButtonClearInfo_Click(object sender, RoutedEventArgs e)
        {
            ListBoxInfo.Items.Clear();
        }

        private void BeginTracking()
        {
            if (faceTracker != null)
            {
                Task.Run(async () =>
                {
                    while (cameraCapture.IsPreviewActive)
                    {
                        await ProcessVideoFrame();
                    }
                });
            }
        }

        private async Task ProcessVideoFrame()
        {
            using (VideoFrame videoFrame = new VideoFrame(faceTrackerSupportedPixelFormat,
                (int)cameraCapture.FrameWidth, (int)cameraCapture.FrameHeight))
            {
                await cameraCapture.MediaCapture.GetPreviewFrameAsync(videoFrame);

                var faces = await faceTracker.ProcessNextFrameAsync(videoFrame);

                if (ledArray == null)
                {
                    DisplayFaces(videoFrame.SoftwareBitmap, faces);
                }
                else
                {
                    TrackFace(faces);
                }
            }
        }

        private void TrackFace(IList<DetectedFace> faces)
        {
            var face = faces.FirstOrDefault();

            if (face != null)
            {
                // LED 픽셀 위치 계산
                var ledPixelPosition = CalculatePosition(face.FaceBox);

                // 위치 표시
                ledArray.SetPixel(ledPixelPosition.X, ledPixelPosition.Y, Colors.Green);

                // 위치 저장
                previousLedPixelPosition = ledPixelPosition;
            }
            else
            {
                // 얼굴이 감지되지 않을 경우, 색상을 빨간색으로 전환
                ledArray.SetPixel(previousLedPixelPosition.X, previousLedPixelPosition.Y, Colors.Red);
            }
        }

        private LedPixelPosition CalculatePosition(BitmapBounds faceBox)
        {
            // 비트맵 LED 배열 스케일러 결정
            var xScaler = (cameraCapture.FrameWidth - faceBox.Width) / (LedArray.Length - 1);
            var yScaler = (cameraCapture.FrameHeight - faceBox.Height) / (LedArray.Length - 1);

            // LED 픽셀 위치 획득
            var xPosition = Convert.ToInt32(faceBox.X / xScaler);
            var yPosition = Convert.ToInt32(faceBox.Y / yScaler);

            // 좌표 보정
            xPosition = CorrectLedCoordinate(LedArray.Length - 1 - xPosition);
            yPosition = CorrectLedCoordinate(yPosition);

            return new LedPixelPosition()
            {
                X = xPosition,
                Y = yPosition
            };
        }

        private int CorrectLedCoordinate(int inputCoordinate)
        {
            inputCoordinate = Math.Min(inputCoordinate, LedArray.Length - 1);
            inputCoordinate = Math.Max(inputCoordinate, 0);

            return inputCoordinate;
        }

        private async void DisplayFaces(SoftwareBitmap displayBitmap, IList<DetectedFace> faces)
        {
            if (Dispatcher.HasThreadAccess)
            {
                var xScalingFactor = CanvasFaceDisplay.ActualWidth / displayBitmap.PixelWidth;
                var yScalingFactor = CanvasFaceDisplay.ActualHeight / displayBitmap.PixelHeight;

                CanvasFaceDisplay.Children.Clear();

                foreach (DetectedFace face in faces)
                {
                    DrawFaceBox(face.FaceBox, xScalingFactor, yScalingFactor);
                }
            }
            else
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    DisplayFaces(displayBitmap, faces);
                });
            }
        }

        private void DrawFaceBox(BitmapBounds faceBox, double xScalingFactor, double yScalingFactor)
        {
            // 경계 상자 준비
            var rectangle = new Rectangle()
            {
                Stroke = new SolidColorBrush(Colors.Yellow),
                StrokeThickness = 5,
                Width = faceBox.Width * xScalingFactor,
                Height = faceBox.Height * yScalingFactor
            };

            // 경계 상자 변환
            var translateTransform = new TranslateTransform()
            {
                X = faceBox.X * xScalingFactor,
                Y = faceBox.Y * yScalingFactor
            };

            rectangle.RenderTransform = translateTransform;

            // 경계 상자 표시
            CanvasFaceDisplay.Children.Add(rectangle);
        }
    }
}
