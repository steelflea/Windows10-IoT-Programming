using System;
using Windows.Devices.Gpio;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace BlinkyApp
{
    public sealed partial class MainPage : Page
    {
        private const int ledPinNumber = 47;

        private GpioPin ledGpioPin;
        private DispatcherTimer dispatcherTimer;

        private const string stopBlinkingLabel = "Stop blinking";
        private const string startBlinkingLabel = "Start blinking";

        public MainPage()
        {
            InitializeComponent();

            ConfigureGpioPin();
            ConfigureMainButton();
            ConfigureTimer();
        }

        private void ConfigureMainButton()
        {
            MainButton.Content = startBlinkingLabel;

            MainButton.IsEnabled = ledGpioPin != null ? true : false;
        }

        private void UpdateMainButtonLabel()
        {
            var label = MainButton.Content.ToString();

            if (label.Contains(stopBlinkingLabel))
            {
                MainButton.Content = startBlinkingLabel;
            }
            else
            {
                MainButton.Content = stopBlinkingLabel;
            }
        }

        private void ConfigureGpioPin()
        {
            var gpioController = GpioController.GetDefault();

            if (gpioController != null)
            {
                ledGpioPin = gpioController.OpenPin(ledPinNumber);

                if (ledGpioPin != null)
                {
                    ledGpioPin.SetDriveMode(GpioPinDriveMode.Output);
                    ledGpioPin.Write(GpioPinValue.Low);
                }
            }
        }

        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            var msDelay = Convert.ToInt32(Slider.Value);
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(msDelay);
        }

        private void ConfigureTimer()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += DispatcherTimer_Tick;
        }

        private void DispatcherTimer_Tick(object sender, object e)
        {
            Color ellipseBgColor;
            GpioPinValue invertedGpioPinValue;

            var currentPinValue = ledGpioPin.Read();

            if (currentPinValue == GpioPinValue.High)
            {
                invertedGpioPinValue = GpioPinValue.Low;
                ellipseBgColor = Colors.Gray;
            }
            else
            {
                invertedGpioPinValue = GpioPinValue.High;
                ellipseBgColor = Colors.LawnGreen;
            }

            ledGpioPin.Write(invertedGpioPinValue);
            LedEllipse.Fill = new SolidColorBrush(ellipseBgColor);
        }

        private void UpdateTimer()
        {
            if (dispatcherTimer.IsEnabled)
            {
                dispatcherTimer.Stop();
            }
            else
            {
                dispatcherTimer.Start();
            }
        }

        private void MainButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateTimer();

            UpdateMainButtonLabel();
        }
    }
}