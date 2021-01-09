﻿using System;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace WiFiCommunication.Master
{
    public class SenseHatColor
    {
        public SolidColorBrush Brush { get; private set; }

        public double R
        {
            get { return colorComponents[0]; }
            set { UpdateColorComponent(0, Convert.ToByte(value)); }
        }

        public double G
        {
            get { return colorComponents[1]; }
            set { UpdateColorComponent(1, Convert.ToByte(value)); }
        }

        public double B
        {
            get { return colorComponents[2]; }
            set { UpdateColorComponent(2, Convert.ToByte(value)); }
        }

        private byte[] colorComponents;

        private void UpdateColorComponent(int index, byte value)
        {
            colorComponents[index] = value;
            UpdateBrush();
        }

        private void UpdateBrush()
        {
            Brush.Color = Color.FromArgb(255, colorComponents[0], colorComponents[1], colorComponents[2]);
        }

        public SenseHatColor()
        {
            var defaultColor = Colors.Black;

            colorComponents = new byte[]
            {
                defaultColor.R, defaultColor.G, defaultColor.B
            };

            Brush = new SolidColorBrush(defaultColor);
        }
    }
}
