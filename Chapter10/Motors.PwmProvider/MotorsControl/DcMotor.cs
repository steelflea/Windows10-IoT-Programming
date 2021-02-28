using MotorsControl.Enums;
using MotorsControl.MotorHat;
using MotorsControl.PWM;
using SenseHat.Helpers;
using System;
using Windows.Devices.Pwm.Provider;

namespace Motors.PwmProvider.MotorsControl
{
    public class DcMotor
    {
        private IPwmControllerProvider pwmControllerProvider;
        private DcMotorPwmChannels channels;

        private const double dutyCycleFullyOn = 100.0;
        private const double dutyCycleFullyOff = 0.0;

        public DcMotor(IPwmControllerProvider pwmControllerProvider)
        {
            Check.IsNull(pwmControllerProvider);

            this.pwmControllerProvider = pwmControllerProvider;
        }

        public void Start(DcMotorIndex motorIndex, MotorDirection direction)
        {
            ConfigureChannels(motorIndex);

            if (direction == MotorDirection.Forward)
            {
                pwmControllerProvider.SetPulseParameters(channels.In1, dutyCycleFullyOn, false);
                pwmControllerProvider.SetPulseParameters(channels.In2, dutyCycleFullyOff, false);
            }
            else
            {
                pwmControllerProvider.SetPulseParameters(channels.In1, dutyCycleFullyOff, false);
                pwmControllerProvider.SetPulseParameters(channels.In2, dutyCycleFullyOn, false);
            }
        }

        public void Stop(DcMotorIndex motorIndex)
        {
            ConfigureChannels(motorIndex);

            pwmControllerProvider.SetPulseParameters(channels.In1, dutyCycleFullyOff, true);
            pwmControllerProvider.SetPulseParameters(channels.In2, dutyCycleFullyOff, true);
        }

        public void SetSpeed(DcMotorIndex motorIndex, ushort speed)
        {
            ConfigureChannels(motorIndex);

            var dutyCycle = PcaPwmDriver.PercentageScaler * speed / PcaPwmDriver.Range;

            dutyCycle = Math.Min(dutyCycle, PcaPwmDriver.PercentageScaler);

            pwmControllerProvider.SetPulseParameters(channels.Speed, dutyCycle, false);
        }

        private void ConfigureChannels(DcMotorIndex motorIndex)
        {
            switch (motorIndex)
            {
                case DcMotorIndex.DC1:
                    channels.In1 = 10;
                    channels.In2 = 9;
                    channels.Speed = 8;
                    break;

                case DcMotorIndex.DC2:
                    channels.In1 = 11;
                    channels.In2 = 12;
                    channels.Speed = 13;
                    break;

                case DcMotorIndex.DC3:
                    channels.In1 = 4;
                    channels.In2 = 3;
                    channels.Speed = 2;
                    break;

                case DcMotorIndex.DC4:
                    channels.In1 = 5;
                    channels.In2 = 6;
                    channels.Speed = 7;
                    break;
            }
        }
    }
}
