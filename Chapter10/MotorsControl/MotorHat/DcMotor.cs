using MotorsControl.Enums;
using MotorsControl.PWM;
using SenseHat.Helpers;

namespace MotorsControl.MotorHat
{
    public class DcMotor
    {
        private PcaPwmDriver pcaPwmDriver;
        private DcMotorPwmChannels channels;

        public bool IsInitialized
        {
            get { return pcaPwmDriver.IsInitialized; }
        }

        public DcMotor(PcaPwmDriver pcaPwmDriver)
        {
            Check.IsNull(pcaPwmDriver);

            this.pcaPwmDriver = pcaPwmDriver;
        }

        public void Start(DcMotorIndex motorIndex, MotorDirection direction)
        {
            ConfigureChannels(motorIndex);

            if (direction == MotorDirection.Forward)
            {
                pcaPwmDriver.SetChannelValue(channels.In1, PcaPwmDriver.FullyOn);
                pcaPwmDriver.SetChannelValue(channels.In2, PcaPwmDriver.FullyOff);
            }
            else
            {
                pcaPwmDriver.SetChannelValue(channels.In1, PcaPwmDriver.FullyOff);
                pcaPwmDriver.SetChannelValue(channels.In2, PcaPwmDriver.FullyOn);
            }            
        }

        public void Stop(DcMotorIndex motorIndex)
        {
            ConfigureChannels(motorIndex);

            pcaPwmDriver.SetChannelValue(channels.In1, PcaPwmDriver.FullyOff);
            pcaPwmDriver.SetChannelValue(channels.In2, PcaPwmDriver.FullyOff);
        }

        public void SetSpeed(DcMotorIndex motorIndex, ushort speed)
        {
            Check.IsLengthInValidRange(speed, 0, PcaPwmDriver.Range - 1);

            ConfigureChannels(motorIndex);

            var speedRegisterValue = new PcaRegisterValue()
            {
                On = speed
            };

            pcaPwmDriver.SetChannelValue(channels.Speed, speedRegisterValue);
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
