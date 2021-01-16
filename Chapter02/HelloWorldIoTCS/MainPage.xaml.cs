using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x412에 나와 있습니다.

namespace HelloWorldIoTCS
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const int gpioPinNumber = 5;
        private const int msShineDuration = 5000;
        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

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
                ledGpioPin.Write(GpioPinValue.Low);

                Task.Delay(msShineDuration).Wait();

                ledGpioPin.Write(GpioPinValue.High);
            }
        }
    }
}
