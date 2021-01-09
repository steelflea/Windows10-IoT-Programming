using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media.Imaging;

namespace ImageProcessing
{
    public static class SoftwareBitmapExtensions
    {
        private static BitmapPixelFormat bitmapPixelFormat = BitmapPixelFormat.Bgra8;

        public static WriteableBitmap ToWriteableBitmap(this SoftwareBitmap softwareBitmap)
        {
            if (softwareBitmap != null)
            {
                if (softwareBitmap.BitmapPixelFormat != bitmapPixelFormat)
                {
                    softwareBitmap = SoftwareBitmap.Convert(softwareBitmap, bitmapPixelFormat);
                }

                var writeableBitmap = new WriteableBitmap(softwareBitmap.PixelWidth, softwareBitmap.PixelHeight);

                softwareBitmap.CopyToBuffer(writeableBitmap.PixelBuffer);

                return writeableBitmap;
            }
            else
            {
                return null;
            }
        }
    }
}
