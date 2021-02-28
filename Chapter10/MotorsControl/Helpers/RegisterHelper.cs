using System;
using System.Collections;
using System.Linq;
using Windows.Devices.I2c;

namespace SenseHat.Helpers
{
    public class RegisterHelper
    {
        private const int int16Length = 2;        

        public static byte ReadByte(I2cDevice device, byte address)
        {
            Check.IsNull(device);

            // 쓰기 버퍼는 레지스터 주소를 포함한다
            var writeBuffer = new byte[] { address };

            // 읽기 버퍼는 단일 요소 바이트 배열이다
            var readBuffer = new byte[1];

            device.WriteRead(writeBuffer, readBuffer);

            return readBuffer.First();
        }

        public static void WriteByte(I2cDevice device, byte address, byte value)
        {
            Check.IsNull(device);

            var writeBuffer = new byte[] { address, value };

            device.Write(writeBuffer);
        }

        public static short GetShort(I2cDevice device, byte[] addressList)
        {
            var bytes = GetBytesInt16(device, addressList);

            return BitConverter.ToInt16(bytes.ToArray(), 0);
        }

        public static ushort GetUShort(I2cDevice device, byte[] addressList)
        {
            var bytes = GetBytesInt16(device, addressList);

            return BitConverter.ToUInt16(bytes.ToArray(), 0);
        }

        public static void WriteUShort(I2cDevice device, byte[] addressList, ushort value)
        {
            Check.IsNull(device);

            var bytes = BitConverter.GetBytes(value);

            AdjustEndianness(bytes);

            for (int i = 0; i < int16Length; i++)
            {
                var writeBuffer = new byte[] { addressList[i], bytes[i] };
                device.Write(writeBuffer);
            }
        }

        public static int GetInt(I2cDevice device, byte[] addressList)
        {
            const int minLength = 3;
            const int maxLength = 4;

            Check.IsLengthInValidRange(addressList.Length, minLength, maxLength);

            var bytes = GetBytes(device, addressList, maxLength);

            return BitConverter.ToInt32(bytes.ToArray(), 0);
        }

        public static byte GetByteValueFromBitArray(BitArray bitArray)
        {
            Check.IsNull(bitArray);

            var buffer = new byte[1];

            // 변환 수행
            ((ICollection)bitArray).CopyTo(buffer, 0);

            return buffer[0];
        }

        private static byte[] GetBytes(I2cDevice device, byte[] addressList, int totalLength)
        {
            var bytes = new byte[totalLength];

            for (int i = 0; i < addressList.Length; i++)
            {
                bytes[i] = ReadByte(device, addressList[i]);
            }

            AdjustEndianness(bytes);

            return bytes;
        }

        private static byte[] GetBytesInt16(I2cDevice device, byte[] addressList)
        {            
            Check.IsLengthEqualTo(addressList.Length, int16Length);

            return GetBytes(device, addressList, int16Length);
        }

        private static void AdjustEndianness(byte[] bytes)
        {
            if (!BitConverter.IsLittleEndian)
            {
                bytes = bytes.Reverse().ToArray();
            }
        }
    }
}
