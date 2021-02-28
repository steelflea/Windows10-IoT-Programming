using MotorsControl.Enums;
using MotorsControl.MotorHat;
using MotorsControl.PWM;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Motors
{
    public sealed partial class MainPage : Page
    {
        // PWM Driver
        private PcaPwmDriver pwmDriver;

        // Motor direction
        private object directions;
        private MotorDirection motorDirection;

        // DC motor
        private object dcMotors;
        private DcMotorIndex dcMotorIndex;

        private ushort speed;
        private DcMotor dcMotor;

        // Stepper motor
        private object stepperMotors;
        private StepperMotorIndex stepperMotorIndex;

        private StepperMotor stepperMotor;

        private uint stepperMotorSteps;
        private byte stepperRpm;

        // Stepping mode
        private SteppingMode steppingMode = SteppingMode.FullSteps;

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

                if (dcMotor.IsInitialized)
                {
                    dcMotor.SetSpeed(dcMotorIndex, speed);
                }
            }
        }

        public double StepperRpm
        {
            get { return stepperRpm; }
            set
            {
                stepperRpm = Convert.ToByte(value);

                if (stepperMotor.IsInitialized)
                {
                    stepperMotor.SetSpeed(stepperRpm);
                }
            }
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            await InitializePwmDriver();

            InitializeDcMotor();

            InitializeStepperMotor();                        
        }

        private async Task InitializePwmDriver()
        {
            pwmDriver = new PcaPwmDriver();

            await pwmDriver.Init();

            if (pwmDriver.IsInitialized)
            {
                // Enable oscillator
                pwmDriver.SetSleepMode(SleepMode.Normal);
            }
        }

        private void InitializeDcMotor()
        {
            dcMotor = new DcMotor(pwmDriver);

            // Set default speed
            Speed = 1000;

            // DC motor test. Uncomment the following line to run DC1 motor for 5 seconds without using UI.            
            // DcMotorTest(DcMotorIndex.DC1, MotorDirection.Backward, speed);            
        }

        private void ConfigureDataSources()
        {
            directions = Enum.GetValues(typeof(MotorDirection));
            dcMotors = Enum.GetValues(typeof(DcMotorIndex));
            stepperMotors = Enum.GetValues(typeof(StepperMotorIndex));
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            if (dcMotor.IsInitialized)
            {
                dcMotor.Start(dcMotorIndex, motorDirection);
            }
        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            if (dcMotor.IsInitialized)
            {
                dcMotor.Stop(dcMotorIndex);
            }
        }

        private void DcMotorTest(DcMotorIndex motorIndex, MotorDirection direction, ushort speed)
        {
            if (dcMotor.IsInitialized)
            {
                const int msDelay = 5000;

                // Set speed, and run motor
                dcMotor.SetSpeed(motorIndex, speed);
                dcMotor.Start(motorIndex, direction);

                // Wait a specified delay
                Task.Delay(msDelay).Wait();

                // Stop motor
                dcMotor.Stop(motorIndex);
            }
        }

        private void InitializeStepperMotor()
        {
            stepperMotor = new StepperMotor(pwmDriver);

            StepperRpm = stepperMotor.Rpm;

            // Stepper motor test. Uncomment the following line to move stepper motor 
            // by 200 steps forward, and then backwards.            
            // StepperMotorTest(StepperMotorIndex.SM2, SteppingMode.FullSteps);
        }

        private void ButtonStepperMove_Click(object sender, RoutedEventArgs e)
        {
            if (stepperMotor.IsInitialized)
            {                
                stepperMotor.Move(stepperMotorIndex, motorDirection, stepperMotorSteps, steppingMode);
            }
        }

        private void StepperMotorTest(StepperMotorIndex motorIndex, SteppingMode steppingMode = SteppingMode.FullSteps)
        {
            if (stepperMotor.IsInitialized)
            {
                // Configure steps, and RPM                
                const uint steps = 200;
                const byte rpm = 20;

                // Set speed
                stepperMotor.SetSpeed(rpm);

                // Move motor forward
                stepperMotor.Move(motorIndex, MotorDirection.Forward, steps, steppingMode);
                //stepperMotor.MoveWithSpeedAdjustment(motorIndex, MotorDirection.Forward, steps, steppingMode);

                // ... and go back to initial position
                stepperMotor.Move(motorIndex, MotorDirection.Backward, steps, steppingMode);
                //stepperMotor.MoveWithSpeedAdjustment(motorIndex, MotorDirection.Backward, steps, steppingMode);
            }
        }

        private void ButtonStepperMoveAutoSpeedAdjustment_Click(object sender, RoutedEventArgs e)
        {
            if (stepperMotor.IsInitialized)
            {
                stepperMotor.MoveWithSpeedAdjustment(stepperMotorIndex, motorDirection, stepperMotorSteps, steppingMode);
            }
        }        
    }
}