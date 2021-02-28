using MotorsControl.MotorHat;
using MotorsControl.PWM;
using SenseHat.Helpers;
using System;
using System.Collections.Generic;

namespace MotorsControl.Helpers
{
    public static class MicrosteppingHelper
    {
        public static List<PcaRegisterValue> GetLinearRamp(uint microstepCount)
        {
            Check.IsPositive(microstepCount);

            var ramp = new List<PcaRegisterValue>();

            var increment = PcaPwmDriver.Range / microstepCount;

            for (var i = 0; i <= microstepCount; i++)
            {
                ramp.Add(new PcaRegisterValue()
                {
                    On = 0,
                    Off = Convert.ToUInt16(i * increment)
                });
            }

            return ramp;
        }

        public static int GetPhaseIndex(int microStepPhaseIndex, uint microstepCount)
        {
            Check.IsPositive(microStepPhaseIndex);            

            int phaseIndex;

            if (microStepPhaseIndex >= 0 && microStepPhaseIndex <= microstepCount)
            {
                phaseIndex = 0;
            }
            else if (microStepPhaseIndex >= microstepCount && microStepPhaseIndex <= 2 * microstepCount)
            {
                phaseIndex = 1;
            }
            else if (microStepPhaseIndex >= 2 * microstepCount && microStepPhaseIndex <= 3 * microstepCount)
            {
                phaseIndex = 2;
            }
            else
            {
                phaseIndex = 3;
            }

            return phaseIndex;
        }

        public static void AdjustMicroStepPhase(StepperMotorPhase phase, List<PcaRegisterValue> microStepCurve, uint microstepCount, int microStepIndex, int microStepPhaseIndex)
        {
            Check.IsNull(phase);
            Check.IsNull(microStepCurve);

            Check.IsPositive(microStepIndex);
            Check.IsPositive(microStepPhaseIndex);

            Check.LengthNotLessThan(microStepCurve.Count, (int)(microstepCount + 1));

            var microStepPhase1 = microStepCurve[(int)(microstepCount - microStepIndex)];
            var microStepPhase2 = microStepCurve[microStepIndex];

            if (microStepPhaseIndex >= 0 && microStepPhaseIndex < microstepCount
                || microStepPhaseIndex >= 2 * microstepCount && microStepPhaseIndex < 3 * microstepCount)
            {
                phase.PwmA = microStepPhase1;
                phase.PwmB = microStepPhase2;
            }
            else
            {
                phase.PwmA = microStepPhase2;
                phase.PwmB = microStepPhase1;
            }
        }
    }
}
