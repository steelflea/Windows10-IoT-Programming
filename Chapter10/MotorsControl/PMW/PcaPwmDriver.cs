using MotorsControl.Enums;
using SenseHat.Helpers;
using SenseHatDisplay.Helpers;
using System;
using System.Collections;
using System.Threading.Tasks;
using Windows.Devices.I2c;

namespace MotorsControl.PWM
{
    public class PcaPwmDriver
    {
        public bool IsInitialized { get; private set; } = false;

        public static ushort Range { get; } = 4096;

        public static PcaRegisterValue FullyOn { get; } = new PcaRegisterValue()
        {
            On = Range,
            Off = 0
        };

        public static PcaRegisterValue FullyOff { get; } = new PcaRegisterValue()
        {
            On = 0,
            Off = Range
        };

        private I2cDevice device;

        private const byte defaultAddress = 0x60;

        public static int HzMinFrequency { get; } = 24;
        public static int HzMaxFrequency { get; } = 1526;

        public static int PinCount { get; } = 16;

        public static double PercentageScaler { get; } = 100.0;

        private const int hzOscillatorFrequency = (int)25e+6; // 25 MHz

        public async Task Init(byte address = defaultAddress)
        {
            device = await I2cHelper.GetI2cDevice(address);

            IsInitialized = device != null;
        }

        public PcaRegisterValue GetChannelValue(byte index)
        {
            CheckInitialization();

            var onRegisterAddressList = PcaRegisters.GetRegisterAddressList(index, RegisterType.On);
            var offRegisterAddressList = PcaRegisters.GetRegisterAddressList(index, RegisterType.Off);

            return new PcaRegisterValue()
            {
                On = RegisterHelper.GetUShort(device, onRegisterAddressList),
                Off = RegisterHelper.GetUShort(device, offRegisterAddressList)
            };
        }

        public void SetChannelValue(byte index, PcaRegisterValue pcaRegisterValue)
        {
            CheckInitialization();

            var onRegisterAddressList = PcaRegisters.GetRegisterAddressList(index, RegisterType.On);
            var offRegisterAddressList = PcaRegisters.GetRegisterAddressList(index, RegisterType.Off);

            RegisterHelper.WriteUShort(device, onRegisterAddressList, pcaRegisterValue.On);
            RegisterHelper.WriteUShort(device, offRegisterAddressList, pcaRegisterValue.Off);
        }

        public void SetFrequency(int hzFrequency)
        {
            // 인수 유효성 검사
            Check.IsLengthInValidRange(hzFrequency, HzMinFrequency, HzMaxFrequency);

            // 장치 초기화 확인
            CheckInitialization();

            // 저전력 모드 설정
            SetSleepMode(SleepMode.LowPower);

            // 주파수 업데이트
            UdpateFrequency(hzFrequency);

            // 정상 전원 모드 다시 설정
            SetSleepMode(SleepMode.Normal);
        }

        public int GetFrequency()
        {
            CheckInitialization();

            var prescale = RegisterHelper.ReadByte(device, PcaRegisters.Prescale);

            return (int)Math.Round(1.0 * hzOscillatorFrequency / (Range * (prescale + 1)), 0);
        }

        public void SetSleepMode(SleepMode mode)
        {
            CheckInitialization();

            // 현재 모드 읽기
            var currentMode = RegisterHelper.ReadByte(device, PcaRegisters.Mode1);

            // Pwm Mode == Low Power 인 경우, 절전 모드 비트를 true로 업데이트
            var currentModeBits = new BitArray(new byte[] { currentMode });
            currentModeBits[PcaRegisters.SleepModeBitIndex] = mode == SleepMode.LowPower;

            // Mode1 레지스터에 업데이트된 바이트 값 쓰기
            RegisterHelper.WriteByte(device, PcaRegisters.Mode1,
                RegisterHelper.GetByteValueFromBitArray(currentModeBits));

            // 내부 오실레이터에 필요한 지연
            Task.Delay(1).Wait();
        }

        public static PcaRegisterValue PulseDurationToRegisterValue(double msPulseDuration, int hzFrequency)
        {
            Check.IsLengthInValidRange(hzFrequency, HzMinFrequency, HzMaxFrequency);
            Check.IsPositive(msPulseDuration);

            var msCycleDuration = 1000.0 / hzFrequency;

            var msTimePerOscillatorTick = msCycleDuration / Range;

            return new PcaRegisterValue()
            {
                On = 0,
                Off = Convert.ToUInt16(msPulseDuration / msTimePerOscillatorTick)
            };
        }

        public static PcaRegisterValue DutyCycleToRegisterValue(double dutyCycle, bool invertPolarity)
        {
            var registerValue = dutyCycle * Range / PercentageScaler;
            registerValue = Math.Min(registerValue, Range);

            ushort offValue = 0;
            ushort onValue = Convert.ToUInt16(registerValue);

            return new PcaRegisterValue()
            {
                On = !invertPolarity ? onValue : offValue,
                Off = !invertPolarity ? offValue : onValue
            };
        }

        private void UdpateFrequency(int hzFrequency)
        {
            var prescale = Math.Round(1.0 * hzOscillatorFrequency / (hzFrequency * Range), 0) - 1;

            RegisterHelper.WriteByte(device, PcaRegisters.Prescale, Convert.ToByte(prescale));
        }

        private void CheckInitialization()
        {
            if (!IsInitialized)
            {
                throw new Exception("Device is not initialized");
            }
        }
    }
}