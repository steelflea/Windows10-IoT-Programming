using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Gpio;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace IoTBackgroundAppCS
{
    public sealed class StartupTask : IBackgroundTask
    {
        private const int gpioPinNumber = 5;
        private const int msShineDuration = 5000;
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // 
            // TODO: Insert code to perform background work
            //
            // If you start any asynchronous methods here, prevent the task
            // from closing prematurely by using BackgroundTaskDeferral as
            // described in http://aka.ms/backgroundtaskdeferral
            //

            BlinkLed(gpioPinNumber, msShineDuration);
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

        private void BlinkLed(int gpioPinNumber, int msShineDuration)
        {
            GpioPin ledGpioPin = ConfigureGpioPin(gpioPinNumber);

            if (ledGpioPin != null)
            {
                while (true)
                {
                    SwitchGpioPin(ledGpioPin);

                    Task.Delay(msShineDuration).Wait();
                }
            }
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
