using FacialTracking;
using ImageProcessingComponent;
using SenseHatDisplay.Helpers;
using SenseHatIO.SenseHatLedArray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace MachineVision
{
    public sealed partial class MainPage : Page
    {
        private CameraCapture cameraCapture = new CameraCapture();
        private LedArray ledArray;

        private BitmapPixelFormat bitmapPixelFormat = BitmapPixelFormat.Bgra8;
        private WriteableBitmap workingBitmap = null;

        private const int maxVerticesCount = 4;
        private const double minBrightness = 100.0d;

        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            await UpdatePreviewState();

            InitializeLedArray();
        }
        
        private async void InitializeLedArray()
        {
            const byte address = 0x46;
            var device = await I2cHelper.GetI2cDevice(address);

            if (device != null)
            {
                ledArray = new LedArray(device);
            }
        }

        private async Task UpdatePreviewState()
        {
            await cameraCapture.Initialize(CaptureElementPreview);

            if (cameraCapture.IsInitialized)
            {
                if (!cameraCapture.IsPreviewActive)
                {
                    await cameraCapture.Start();

                    BeginProcessing();
                }
                else
                {
                    await cameraCapture.Stop();
                }
            }
        }

        private void BeginProcessing()
        {
#pragma warning disable 4014

            if (workingBitmap == null)
            {
                workingBitmap = new WriteableBitmap((int)cameraCapture.FrameWidth, (int)cameraCapture.FrameHeight);
            }

            Task.Run(async () =>
            {
                while (cameraCapture.IsPreviewActive)
                {
                    await ProcessVideoFrame();
                }
            });

#pragma warning restore 4014            
        }

        private async Task ProcessVideoFrame()
        {
            using (VideoFrame videoFrame = new VideoFrame(bitmapPixelFormat,
                (int)cameraCapture.FrameWidth, (int)cameraCapture.FrameHeight))
            {
                await cameraCapture.MediaCapture.GetPreviewFrameAsync(videoFrame);

                var objectDescriptor = await FindLargestObject(videoFrame);

                DisplayDetectedObject(objectDescriptor);
            }
        }

        private async Task<ObjectDescriptor> FindLargestObject(VideoFrame videoFrame)
        {
            if (Dispatcher.HasThreadAccess)
            {
                videoFrame.SoftwareBitmap.CopyToBuffer(workingBitmap.PixelBuffer);

                var objects = OpenCvWrapper.DetectObjects(workingBitmap, false);

                return GetLargestObject(objects);
            }
            else
            {
                ObjectDescriptor objectDescriptor = null;

                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    objectDescriptor = await FindLargestObject(videoFrame);
                });

                return objectDescriptor;
            }
        }

        private ObjectDescriptor GetLargestObject(IList<ObjectDescriptor> objects)
        {
            ObjectDescriptor largestObject = null;

            if (objects != null)
            {
                if (objects.Count() > 0)
                {
                    var sorted = objects.OrderByDescending(s => s.Area);

                    int objectIndex = GetObjectIndex();

                    if (sorted.Count() >= objectIndex + 1)
                    {
                        largestObject = sorted.ElementAt(objectIndex);
                    }
                }
            }

            return largestObject;
        }

        private int GetObjectIndex()
        {
            double brightness = OpenCvWrapper.Brightness(workingBitmap);

            // 이미지 밝기가 충분히 큰 경우, 첫 번째 가짜 객체는 무시한다
            return brightness > minBrightness ? 1 : 0;
        }        

        private void DisplayDetectedObject(ObjectDescriptor objectDescriptor)
        {
            if (ledArray != null)
            {
                var shapeKind = GetShapeKind(objectDescriptor);

                ledArray.DrawShape(shapeKind);
            }
        }

        private ShapeKind GetShapeKind(ObjectDescriptor objectDescriptor)
        {
            var shapeKind = ShapeKind.None;

            if (objectDescriptor != null)
            {
                var objectDescriptorPointsCount = objectDescriptor.Points.Count;

                if (objectDescriptorPointsCount > maxVerticesCount)
                {
                    // 복잡한 객체는 X 기호로 표시된다.
                    shapeKind = ShapeKind.X;
                }
                else
                {
                    if (Enum.IsDefined(typeof(ShapeKind), objectDescriptorPointsCount))
                    {
                        shapeKind = (ShapeKind)objectDescriptorPointsCount;
                    }
                }
            }

            return shapeKind;
        }
    }
}
