using Motors.PwmProvider.ControllerProviders;
using Motors.PwmProvider.MotorsControl;
using MotorsControl.Enums;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Motors.PwmProvider
{
    public sealed partial class MainPage : Page
    {
        // 모터 방향
        private object directions;
        private MotorDirection motorDirection;

        // DC 모터
        private object dcMotors;
        private DcMotorIndex dcMotorIndex;

        private ushort speed;
        private DcMotor dcMotor;

        public MainPage()
        {
            InitializeComponent();

            ConfigureDataSources();
        }

        public double Speed
        {
            get { return speed; }
            set
            {
                speed = Convert.ToUInt16(value);

                if (dcMotor != null)
                {
                    dcMotor.SetSpeed(dcMotorIndex, speed);
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            InitializeDcMotor();
        }

        private void ConfigureDataSources()
        {
            directions = Enum.GetValues(typeof(MotorDirection));
            dcMotors = Enum.GetValues(typeof(DcMotorIndex));         
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            if (dcMotor != null)
            {
                dcMotor.Start(dcMotorIndex, motorDirection);
            }
        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            if (dcMotor != null)
            {
                dcMotor.Stop(dcMotorIndex);
            }
        }

        private void InitializeDcMotor()
        {
            var pcaPwmControllerProvider = PcaPwmControllerProvider.GetDefault();

            dcMotor = new DcMotor(pcaPwmControllerProvider);

            // 기본 속도 설정
            Speed = 1000;

            // 다음 줄의 주석을 해제하면, UI를 사용하지 않고 DC1 모터를 5초 동안 실행한다.
            // DcMotorTest(DcMotorIndex.DC1, MotorDirection.Backward, speed);
        }

        private void DcMotorTest(DcMotorIndex motorIndex, MotorDirection direction, ushort speed)
        {
            if (dcMotor != null)
            {
                const int msDelay = 5000;

                // 속도 설정 및 모터 구동
                dcMotor.SetSpeed(motorIndex, speed);
                dcMotor.Start(motorIndex, direction);

                // 지정된 지연 대기
                Task.Delay(msDelay).Wait();

                // 모터 정지
                dcMotor.Stop(motorIndex);
            }
        }

    }
}
