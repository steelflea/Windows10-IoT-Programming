using Motors.PwmProvider.Exceptions;
using MotorsControl.Enums;
using MotorsControl.PWM;
using System;
using System.Threading.Tasks;
using Windows.Devices.Pwm.Provider;

namespace Motors.PwmProvider.ControllerProviders
{
    public class PcaPwmControllerProvider : IPwmControllerProvider
    {
        private PcaPwmDriver pcaPwmDriver = new PcaPwmDriver();

        public double SetDesiredFrequency(double frequency)
        {
            pcaPwmDriver.SetFrequency(Convert.ToInt32(frequency));

            return pcaPwmDriver.GetFrequency();
        }

        public void SetPulseParameters(int pin, double dutyCycle, bool invertPolarity)
        {
            var pcaRegisterValue = PcaPwmDriver.DutyCycleToRegisterValue(dutyCycle, invertPolarity);

            pcaPwmDriver.SetChannelValue(Convert.ToByte(pin), pcaRegisterValue);
        }

        public void AcquirePin(int pin) { }

        public void ReleasePin(int pin) { }

        public void EnablePin(int pin) { }

        public void DisablePin(int pin) { }

        public double ActualFrequency
        {
            get { return pcaPwmDriver.GetFrequency(); }
        }

        public double MaxFrequency
        {
            get { return PcaPwmDriver.HzMaxFrequency; }
        }

        public double MinFrequency
        {
            get { return PcaPwmDriver.HzMinFrequency; }
        }

        public int PinCount
        {
            get { return PcaPwmDriver.PinCount; }
        }

        public static PcaPwmControllerProvider GetDefault()
        {
            return new PcaPwmControllerProvider();
        }

        private PcaPwmControllerProvider(byte address = 0x60)
        {
            // UI가 블로킹되지 않도록 백그라운드 스레드에서 PcaPwmDriver를 초기화한다.
            Task.Run(async () =>
            {
                await pcaPwmDriver.Init(address);

                pcaPwmDriver.SetSleepMode(SleepMode.Normal);
            }).Wait();

            if (!pcaPwmDriver.IsInitialized)
            {
                throw DeviceInitializationException.Default(address);
            }
        }
    }
}
