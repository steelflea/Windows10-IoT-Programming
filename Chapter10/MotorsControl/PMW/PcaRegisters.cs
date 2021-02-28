using MotorsControl.Enums;
using System;

namespace MotorsControl.PWM
{
    public static class PcaRegisters
    {
        public static byte Mode1 { get; } = 0x00;
        public static byte Prescale { get; } = 0xFE;

        public static byte SleepModeBitIndex { get; } = 4;        

        private const byte ledAddressBeginIndex = 0x06;
        private const byte channelOffset = 4;
        private const byte registerLength = 2;
        private const byte maxChannelIndex = 15;             

        public static byte[] GetRegisterAddressList(byte channelIndex, RegisterType registerType)
        {
            // 채널 인덱스 확인
            if(channelIndex > maxChannelIndex)
            {
                throw new ArgumentException("Channel index cannot be larger than " 
                    + maxChannelIndex);
            }
            
            // LED 시작 색인 6에서 4 * channelIndex를 건너뛰어 시작 주소 가져오기
            var registerStartAddress = Convert.ToByte(ledAddressBeginIndex 
                + channelIndex * channelOffset);

            // 레지스터 유형이 off인 경우, 오프셋 2를 추가한다
            if(registerType == RegisterType.Off)
            {
                registerStartAddress += Convert.ToByte((byte)registerType * registerLength);
            }

            // 주소 목록 구성
            var addressList = new byte[registerLength];

            for(byte i = 0; i < registerLength; i++)
            {
                addressList[i] = Convert.ToByte(registerStartAddress + i);
            }

            return addressList;
        }
    }
}
