using System;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;

namespace FacialTracking
{
    public class CameraCapture
    {
        public MediaCapture MediaCapture { get; private set; } = new MediaCapture();

        public bool IsPreviewActive { get; private set; } = false;

        public bool IsInitialized { get; private set; } = false;

        public uint FrameWidth { get; private set; }

        public uint FrameHeight { get; private set; }

        public async Task Initialize(CaptureElement captureElement)
        {
            if (!IsInitialized)
            {
                var settings = new MediaCaptureInitializationSettings()
                {
                    StreamingCaptureMode = StreamingCaptureMode.Video
                };

                try
                {
                    await MediaCapture.InitializeAsync(settings);

                    GetVideoProperties();

                    if (captureElement != null)
                    {
                        captureElement.Source = MediaCapture;

                        IsInitialized = true;
                    }
                }
                catch (Exception)
                {
                    IsInitialized = false;
                }
            }
        }

        public async Task Start()
        {
            if (IsInitialized)
            {
                if (!IsPreviewActive)
                {
                    await MediaCapture.StartPreviewAsync();

                    IsPreviewActive = true;
                }
            }
        }

        public async Task Stop()
        {
            if (IsInitialized)
            {
                if (IsPreviewActive)
                {
                    await MediaCapture.StopPreviewAsync();

                    IsPreviewActive = false;
                }
            }
        }

        public async Task<SoftwareBitmap> CapturePhotoToSoftwareBitmap()
        {
            // 비트맵으로 인코딩된 이미지 생성
            var imageEncodingProperties = ImageEncodingProperties.CreateBmp();

            // 사진 캡처
            var memoryStream = new InMemoryRandomAccessStream();
            await MediaCapture.CapturePhotoToStreamAsync(imageEncodingProperties, memoryStream);

            // 비트맵으로 스트림 디코딩
            var bitmapDecoder = await BitmapDecoder.CreateAsync(memoryStream);

            return await bitmapDecoder.GetSoftwareBitmapAsync();
        }

        private void GetVideoProperties()
        {
            if (MediaCapture != null)
            {
                var videoEncodingProperties = MediaCapture.VideoDeviceController.
                    GetMediaStreamProperties(MediaStreamType.VideoPreview) 
                    as VideoEncodingProperties;

                FrameWidth = videoEncodingProperties.Width;
                FrameHeight = videoEncodingProperties.Height;
            }
        }
    }
}
