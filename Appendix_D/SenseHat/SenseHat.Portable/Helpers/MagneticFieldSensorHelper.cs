using System;
using System.Collections;

namespace SenseHat.Portable.Helpers
{
    public class MagneticFieldSensorHelper
    {
        // CTRL_REG1_M (Output Data Rate, Performance, Self-test, Temperature compensation)
        private const int selfTestIndex = 0;
        private const int fastOdrIndex = 1;
        private const int odrBeginIndex = 2;
        private const int odrEndIndex = 4;
        private const int performanceModeBeginIndex = 5;
        private const int performanceModeEndIndex = 6;
        private const int temperatureCompensationIndex = 7;

        // CTRL_REG3_M (operating mode)
        private const int operatingModeBeginIndex = 0;
        private const int operatingModeEndIndex = 1;

        public static byte ConfigureSensingParameters(MagnetometerOutputDataRate outputDataRate = MagnetometerOutputDataRate.Hz_80,
            MagnetometerPerformanceMode performanceMode = MagnetometerPerformanceMode.High,
            bool selfTest = false,
            bool fastOdr = false,
            bool temperatureCompensation = true)
        {
            var bitArray = new BitArray(Constants.ByteBitLength);

            // Self-test
            bitArray.Set(selfTestIndex, selfTest);

            // Fast ODR
            bitArray.Set(fastOdrIndex, fastOdr);

            // Temperature compensation
            bitArray.Set(temperatureCompensationIndex, temperatureCompensation);

            // ODR
            SetOdr(outputDataRate, bitArray);

            // Performance mode                        
            SetPerformance(performanceMode, bitArray);

            return ConversionHelper.GetByteValueFromBitArray(bitArray);
        }

        public static byte ConfigureOperatingMode(MagnetometerOperatingMode operatingMode = MagnetometerOperatingMode.ContinuousConversion)
        {
            var bitArray = new BitArray(Constants.ByteBitLength);

            SetOperatingMode(operatingMode, bitArray);

            return ConversionHelper.GetByteValueFromBitArray(bitArray);
        }

        public static double GetDirectionAngle(Vector3D<float> sensorReading)
        {
            Check.IsNull(sensorReading);

            double directionAngle;

            if (sensorReading.Y != Constants.NorthAngle)
            {
                var radAngle = Math.Atan2(sensorReading.Y, sensorReading.X);
                var degAngle = RadToDeg(radAngle);

                directionAngle = degAngle < Constants.NorthAngle ? degAngle + Constants.MaxAngle : degAngle;
            }
            else
            {
                directionAngle = sensorReading.X > Constants.NorthAngle ? Constants.NorthAngle : Constants.SouthAngle;
            }

            return directionAngle;
        }

        private static void SetOdr(MagnetometerOutputDataRate outputDataRate, BitArray bitArray)
        {
            bool[] odrBitValues;

            switch (outputDataRate)
            {
                case MagnetometerOutputDataRate.Hz_0_625:
                    odrBitValues = new bool[] { false, false, false };
                    break;

                case MagnetometerOutputDataRate.Hz_1_25:
                    odrBitValues = new bool[] { true, false, false };
                    break;

                case MagnetometerOutputDataRate.Hz_2_5:
                    odrBitValues = new bool[] { false, true, false };
                    break;

                case MagnetometerOutputDataRate.Hz_5:
                    odrBitValues = new bool[] { true, true, false };
                    break;

                case MagnetometerOutputDataRate.Hz_10:
                    odrBitValues = new bool[] { false, false, true };
                    break;

                case MagnetometerOutputDataRate.Hz_20:
                    odrBitValues = new bool[] { true, false, true };
                    break;

                case MagnetometerOutputDataRate.Hz_40:
                    odrBitValues = new bool[] { false, true, true };
                    break;

                case MagnetometerOutputDataRate.Hz_80:
                default:
                    odrBitValues = new bool[] { true, true, true };
                    break;
            }

            ConversionHelper.SetBitArrayValues(bitArray, odrBitValues, odrBeginIndex, odrEndIndex);
        }

        private static void SetPerformance(MagnetometerPerformanceMode performanceMode, BitArray bitArray)
        {
            bool[] performanceBitValues;

            switch (performanceMode)
            {
                case MagnetometerPerformanceMode.Low:
                    performanceBitValues = new bool[] { false, false };
                    break;

                case MagnetometerPerformanceMode.Medium:
                    performanceBitValues = new bool[] { true, false };
                    break;

                case MagnetometerPerformanceMode.High:
                default:
                    performanceBitValues = new bool[] { false, true };
                    break;

                case MagnetometerPerformanceMode.UltraHigh:
                    performanceBitValues = new bool[] { true, true };
                    break;
            }

            ConversionHelper.SetBitArrayValues(bitArray, performanceBitValues,
                performanceModeBeginIndex, performanceModeEndIndex);
        }

        private static void SetOperatingMode(MagnetometerOperatingMode operatingMode, BitArray bitArray)
        {
            bool[] operatingModeBitValues;

            switch (operatingMode)
            {
                case MagnetometerOperatingMode.ContinuousConversion:
                default:
                    operatingModeBitValues = new bool[] { false, false };
                    break;

                case MagnetometerOperatingMode.SingleConversion:
                    operatingModeBitValues = new bool[] { true, false };
                    break;

                case MagnetometerOperatingMode.PowerDown:
                    operatingModeBitValues = new bool[] { true, true };
                    break;
            }

            ConversionHelper.SetBitArrayValues(bitArray, operatingModeBitValues,
                operatingModeBeginIndex, operatingModeEndIndex);
        }

        private static double RadToDeg(double radAngle)
        {
            return radAngle * 180.0 / Math.PI;
        }
    }

    public enum MagnetometerOutputDataRate
    {
        Hz_0_625, Hz_1_25, Hz_2_5, Hz_5, Hz_10, Hz_20, Hz_40, Hz_80
    }

    public enum MagnetometerPerformanceMode
    {
        Low, Medium, High, UltraHigh
    }

    public enum MagnetometerOperatingMode
    {
        ContinuousConversion, SingleConversion, PowerDown
    }
}

