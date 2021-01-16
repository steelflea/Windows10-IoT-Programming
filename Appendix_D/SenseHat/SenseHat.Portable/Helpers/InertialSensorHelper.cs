using SenseHat.Portable.Helpers;
using System.Collections;

namespace SenseHat.Portable.Helpers
{
    public class InertialSensorHelper
    {        
        private const int odrBeginIndex = 5;
        private const int odrEndIndex = 7;        

        public static byte ConfigureControlByte(InertialSensorOutputDataRate outputDataRate = InertialSensorOutputDataRate.Hz_238)
        {
            var bitArray = new BitArray(Constants.ByteBitLength);
            
            // ODR
            SetOdr(outputDataRate, bitArray);

            return ConversionHelper.GetByteValueFromBitArray(bitArray);
        }

        private static void SetOdr(InertialSensorOutputDataRate outputDataRate, BitArray bitArray)
        {
            bool[] odrBitValues;

            switch (outputDataRate)
            {
                case InertialSensorOutputDataRate.PowerDown:
                    odrBitValues = new bool[] { false, false, false };
                    break;

                case InertialSensorOutputDataRate.Hz_14_9:
                    odrBitValues = new bool[] { true, false, false };
                    break;

                case InertialSensorOutputDataRate.Hz_59_5:
                    odrBitValues = new bool[] { false, true, false };
                    break;

                case InertialSensorOutputDataRate.Hz_119:
                    odrBitValues = new bool[] { true, true, false };
                    break;

                case InertialSensorOutputDataRate.Hz_238:
                default:
                    odrBitValues = new bool[] { false, false, true };
                    break;

                case InertialSensorOutputDataRate.Hz_476:
                    odrBitValues = new bool[] { true, false, true };
                    break;

                case InertialSensorOutputDataRate.Hz_952:                
                    odrBitValues = new bool[] { true, false, true };
                    break;                
            }

            for (int i = odrBeginIndex, j = 0; i <= odrEndIndex; i++, j++)
            {
                bitArray[i] = odrBitValues[j];
            }
        }
    }

    public enum InertialSensorOutputDataRate
    {
        PowerDown, Hz_14_9, Hz_59_5, Hz_119, Hz_238, Hz_476, Hz_952
    }
}
