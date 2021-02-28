using System;

namespace MotorsControl.Helpers
{
    public static class SpeedRampHelper
    {
        private const float rampSlope = 0.15f;        

        private const byte minRampSlopeLength = 5;

        private const byte minRpm = 10;
        private const byte maxRpm = 60;

        public static byte[] GenerateTrapezoidalRamp(uint steps)
        {
            byte[] speedRamp;

            var rampSlopeLength = Convert.ToInt32(rampSlope * steps);

            if (rampSlopeLength >= minRampSlopeLength)
            {
                speedRamp = TrapezoidalRamp(steps, rampSlopeLength);
            }
            else
            {
                speedRamp = FlatRamp(steps, minRpm);
            }

            return speedRamp;
        }

        private static byte[] TrapezoidalRamp(uint steps, int rampSlopeLength)
        {
            var speedRamp = new byte[steps];

            // 속도 단계 결정(선형 단계)
            var speedStep = Math.Ceiling(1.0 * (maxRpm - minRpm) / rampSlopeLength);

            // 가속(ACC)
            var acceleration = LinearSlope(rampSlopeLength, minRpm, speedStep);
            acceleration.CopyTo(speedRamp, 0);

            // 정속 부분
            var flatPartLength = (uint)(steps - 2 * rampSlopeLength);
            var flatPart = FlatRamp(flatPartLength, maxRpm);
            flatPart.CopyTo(speedRamp, rampSlopeLength);

            // 감속(DEC)
            var deacceleration = LinearSlope(rampSlopeLength, maxRpm, -speedStep);
            deacceleration.CopyTo(speedRamp, (int)(steps - rampSlopeLength));

            return speedRamp;
        }

        private static byte[] LinearSlope(int rampSlopeLength, byte startRpm, double speedStep)
        {
            var slope = new byte[rampSlopeLength];

            for (var i = 0; i < rampSlopeLength; i++)
            {
                var speed = startRpm + i * speedStep;

                // 속도가 최소 및 최대 RPM 사이인지 확인
                speed = Math.Min(speed, maxRpm);
                speed = Math.Max(speed, minRpm);

                slope[i] = (byte)speed;
            }

            return slope;
        }

        private static byte[] FlatRamp(uint steps, byte rpm)
        {
            var speedRamp = new byte[steps];

            for (int i = 0; i < steps; i++)
            {
                speedRamp[i] = rpm;
            }

            return speedRamp;
        }
    }
}