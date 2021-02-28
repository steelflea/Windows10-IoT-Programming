using MotorsControl.Enums;
using MotorsControl.PWM;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Servo
{
    public sealed partial class MainPage : Page
    {
        private PcaPwmDriver pwmDriver;

        private byte[] pwmChannels;
        private byte pwmChannel;

        private const byte pwmAddress = 0x40;
        private const int hzFrequency = 100;

        private PcaRegisterValue pwmValue = new PcaRegisterValue();

        private double msPulseDuration;

        private double MsPulseDuration
        {
            get { return msPulseDuration; }
            set
            {
                msPulseDuration = value;
                pwmValue = PcaPwmDriver.PulseDurationToRegisterValue(msPulseDuration, hzFrequency);
            }
        }

        public MainPage()
        {
            InitializeComponent();

            InitializePwmChannels();                   
        }

        private void InitializePwmChannels()
        {
            const byte channelCount = 16;

            pwmChannels = new byte[channelCount];
            
            for(byte b = 0; b < channelCount; b++)
            {
                pwmChannels[b] = b;
            }

            pwmChannel = pwmChannels.First();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            await InitializePwmDriver();

            // 다음 줄의 주석 처리를 제거하면, UI를 사용하지 않고 서보 테스트를 실행한다.
            // ServoTest();
        }

        private async Task InitializePwmDriver()
        {
            pwmDriver = new PcaPwmDriver();

            await pwmDriver.Init(pwmAddress);

            if (pwmDriver.IsInitialized)
            {
                // 오실레이터 활성화
                pwmDriver.SetSleepMode(SleepMode.Normal);

                // 주파수 설정
                pwmDriver.SetFrequency(hzFrequency);
            }
        }

        private void ButtonUpdateChannel_Click(object sender, RoutedEventArgs e)
        {
            if (pwmDriver.IsInitialized)
            {
                pwmDriver.SetChannelValue(pwmChannel, pwmValue);
            }
        }    

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            if (pwmDriver.IsInitialized)
            {
                pwmDriver.SetChannelValue(pwmChannel, PcaPwmDriver.FullyOff);
            }
        }

        private void ServoTest()
        {
            if (pwmDriver.IsInitialized)
            {
                const byte channel = 0;

                const int msSleepTime = 1000;

                const double minPulseDuration = 0.7;
                const double maxPulseDuration = 2.3;
                const double step = 0.1;

                for (var pulseDuration = maxPulseDuration; pulseDuration >= minPulseDuration - step; pulseDuration -= step)
                {
                    var registerValue = PcaPwmDriver.PulseDurationToRegisterValue(pulseDuration, hzFrequency);

                    pwmDriver.SetChannelValue(channel, registerValue);

                    Task.Delay(msSleepTime).Wait();
                }

                // PWM 채널 비활성화
                pwmDriver.SetChannelValue(pwmChannel, PcaPwmDriver.FullyOff);
            }
        }
    }
}
