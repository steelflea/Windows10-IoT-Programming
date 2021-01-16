using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace HelloWorldIoTCS_2019.GpioControl
{
    public class LedBlinking
    {
        public bool IsActive { get; private set; } = false;

        public int GpioPinNumber { get; private set; }
        public int MsShineDuration { get; private set; }

        private Task blinkingTask;
        private CancellationTokenSource blinkingCancellationTokenSource;

        private GpioPin gpioPin;

        public LedBlinking(int gpioPinNumber, int msShineDuration)
        {
            GpioPinNumber = gpioPinNumber;
            MsShineDuration = msShineDuration;

            gpioPin = ConfigureGpioPin(GpioPinNumber);

            if(gpioPin == null)
            {
                throw new Exception("GPIO pin unavailable");
            }
        }

        public void Start()
        {
            if (!IsActive)
            {
                InitializeBlinkingTask();

                blinkingTask.Start();

                IsActive = true;
            }
        }

        public void Stop()
        {
            if (IsActive)
            {
                blinkingCancellationTokenSource.Cancel();

                IsActive = false;
            }
        }

        private void InitializeBlinkingTask()
        {
            blinkingCancellationTokenSource = new CancellationTokenSource();

            blinkingTask = new Task(() =>
            {
                while (!blinkingCancellationTokenSource.IsCancellationRequested)
                {
                    if (IsActive)
                    {
                        SwitchGpioPin(gpioPin);

                        Task.Delay(MsShineDuration).Wait();
                    }
                }
            }, blinkingCancellationTokenSource.Token);
        }

        private GpioPin ConfigureGpioPin(int pinNumber)
        {
            var gpioController = GpioController.GetDefault();

            GpioPin pin = null;

            if (gpioController != null)
            {
                pin = gpioController.OpenPin(pinNumber);

                if (pin != null)
                {
                    pin.SetDriveMode(GpioPinDriveMode.Output);
                }
            }

            return pin;
        }

        private void SwitchGpioPin(GpioPin gpioPin)
        {
            var currentPinValue = gpioPin.Read();

            GpioPinValue newPinValue = InvertGpioPinValue(currentPinValue);

            gpioPin.Write(newPinValue);
        }

        private GpioPinValue InvertGpioPinValue(GpioPinValue currentPinValue)
        {
            GpioPinValue invertedGpioPinValue;

            if (currentPinValue == GpioPinValue.High)
            {
                invertedGpioPinValue = GpioPinValue.Low;
            }
            else
            {
                invertedGpioPinValue = GpioPinValue.High;
            }

            return invertedGpioPinValue;
        }
    }
}
