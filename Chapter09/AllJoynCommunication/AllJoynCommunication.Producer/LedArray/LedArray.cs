using SenseHat.Helpers;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.I2c;
using Windows.UI;

namespace AllJoynCommunication.Producer.SenseHatLedArray
{
    public enum ShapeKind
    {
        None = 0, Line = 2, Triangle = 3, Square = 4, X = 5,
    }

    public sealed class LedArray
    {
        public static byte Length { get; private set; } = 8;

        public static byte ColorChannelCount { get; private set; } = 3;

        private Color[,] Buffer;
        private I2cDevice device;

        private byte[] pixelByteBuffer;
        private byte[] color5BitLut;

        public LedArray(I2cDevice device)
        {
            Check.IsNull(device);

            this.device = device;

            Buffer = new Color[Length, Length];
            pixelByteBuffer = new byte[Length * Length * ColorChannelCount + 1];

            GenerateColorLut();
        }

        public void Reset(Color color)
        {
            ResetBuffer(color);

            UpdateDevice();
        }

        public void SetPixel(int x, int y, Color color)
        {
            CheckPixelLocation(x);
            CheckPixelLocation(y);

            ResetBuffer(Colors.Black);
            Buffer[x, y] = color;

            UpdateDevice();
        }

        public void RgbTest(int msSleepTime)
        {
            Color[] colors = new Color[] { Colors.Red, Colors.Green, Colors.Blue };

            foreach (var color in colors)
            {
                Reset(color);
                Task.Delay(msSleepTime).Wait();
            }
        }

        public void DrawHistogram([ReadOnlyArray] double[] histogram, double minValue, double maxValue)        
        {
            Check.IsNull(histogram);

            for (int i = 0; i < Length; i++)
            {
                var height = SetHeight(histogram[i], minValue, maxValue);

                DrawLine(Length - 1 - i, height);
            }

            UpdateDevice();
        }

        public void DrawShape(ShapeKind shapeKind)
        {
            switch (shapeKind)
            {
                case ShapeKind.Line:
                    DrawLine();
                    break;

                case ShapeKind.Triangle:
                    DrawTriangle(Colors.Red);
                    break;

                case ShapeKind.Square:
                    DrawSquare(Colors.Green);
                    break;

                case ShapeKind.X:
                    DrawX(Colors.Blue);
                    break;

                case ShapeKind.None:
                    Reset(Colors.Red);
                    break;
            }
        }

        private void DrawLine()
        {
            ResetBuffer(Colors.Black);

            DrawLine(Length / 2, Length);

            UpdateDevice();
        }

        private void DrawTriangle(Color color)
        {
            ResetBuffer(Colors.Black);

            for (int i = 0; i < Length; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    Buffer[j, i] = color;
                }
            }

            UpdateDevice();
        }

        private void DrawSquare(Color color)
        {
            ResetBuffer(Colors.Black);

            for (int i = 0; i < Length; i++)
            {
                for (int j = 0; j < Length; j++)
                {
                    if (i == 0 || j == 0 || i == Length - 1 || j == Length - 1)
                    {
                        Buffer[i, j] = color;
                    }
                }
            }

            UpdateDevice();
        }

        private void DrawX(Color color)
        {
            ResetBuffer(Colors.Black);

            for (int i = 0; i < Length; i++)
            {
                for (int j = 0; j < Length; j++)
                {
                    if (i == j || j == Length - 1 - i)
                    {
                        Buffer[i, j] = color;
                    }
                }
            }

            UpdateDevice();
        }

        private int SetHeight(double histogramValue, double minValue, double maxValue)
        {
            double step = (maxValue - minValue) / Length;

            var stretchedValue = Math.Floor((histogramValue - minValue) / step);
            var height = Convert.ToInt32(stretchedValue);

            height = Math.Max(height, 0);
            height = Math.Min(height, Length);

            return height;
        }

        private void DrawLine(int position, int height)
        {
            for (int i = 0; i < Length; i++)
            {
                Buffer[position, i] = GetColor(i, height);
            }
        }

        private Color GetColor(int level, int height)
        {
            const int lowLevel = 3;
            const int mediumLevel = 6;

            var color = Colors.Black;

            if (level < height)
            {
                if (level < lowLevel)
                {
                    color = Colors.Green;
                }
                else if (level < mediumLevel)
                {
                    color = Colors.OrangeRed;
                }
                else
                {
                    color = Colors.Red;
                }
            }

            return color;
        }

        private void GenerateColorLut()
        {
            const float maxValue5Bit = 31.0f; // 2^5 - 1

            int colorLutLength = byte.MaxValue + 1; // 256개의 개별 레벨
            color5BitLut = new byte[colorLutLength];

            for (int i = 0; i < colorLutLength; i++)
            {
                var value5bit = Math.Ceiling(i * maxValue5Bit / byte.MaxValue);

                value5bit = Math.Min(value5bit, maxValue5Bit);

                color5BitLut[i] = Convert.ToByte(value5bit);
            }
        }

        private byte[] ColorToByteArray(Color color)
        {
            return new byte[]
            {
                color5BitLut[color.R],
                color5BitLut[color.G],
                color5BitLut[color.B]
            };
        }

        private void ResetBuffer(Color color)
        {
            for (int x = 0; x < Length; x++)
            {
                for (int y = 0; y < Length; y++)
                {
                    Buffer[x, y] = color;
                }
            }
        }

        private void UpdateDevice()
        {
            Serialize();

            device.Write(pixelByteBuffer);
        }

        private void Serialize()
        {
            int index;
            var widthStep = Length * ColorChannelCount;

            Array.Clear(pixelByteBuffer, 0, pixelByteBuffer.Length);

            for (int x = 0; x < Length; x++)
            {
                for (int y = 0; y < Length; y++)
                {
                    var colorByteArray = ColorToByteArray(Buffer[x, y]);

                    for (int i = 0; i < ColorChannelCount; i++)
                    {
                        index = x + i * Length + y * widthStep + 1;

                        pixelByteBuffer[index] = colorByteArray[i];
                    }
                }
            }
        }

        private void CheckPixelLocation(int location)
        {
            if (location < 0 || location >= Length)
            {
                throw new ArgumentException("LED square array has maximum length of: " + Length);
            }
        }
    }
}
