using MotorsControl.MotorHat;
using MotorsControl.PWM;
using System.Collections.Generic;

namespace MotorsControl.Helpers
{
    public static class ControlPhaseHelper
    {
        public static List<StepperMotorPhase> GetFullStepSequence()
        {
            var controlPhases = new List<StepperMotorPhase>();

            controlPhases.Add(new StepperMotorPhase()
            {
                AIn2 = PcaPwmDriver.FullyOn,
                BIn1 = PcaPwmDriver.FullyOff,
                AIn1 = PcaPwmDriver.FullyOff,
                BIn2 = PcaPwmDriver.FullyOff
            });

            controlPhases.Add(new StepperMotorPhase()
            {
                AIn2 = PcaPwmDriver.FullyOff,
                BIn1 = PcaPwmDriver.FullyOn,
                AIn1 = PcaPwmDriver.FullyOff,
                BIn2 = PcaPwmDriver.FullyOff
            });

            controlPhases.Add(new StepperMotorPhase()
            {
                AIn2 = PcaPwmDriver.FullyOff,
                BIn1 = PcaPwmDriver.FullyOff,
                AIn1 = PcaPwmDriver.FullyOn,
                BIn2 = PcaPwmDriver.FullyOff
            });

            controlPhases.Add(new StepperMotorPhase()
            {
                AIn2 = PcaPwmDriver.FullyOff,
                BIn1 = PcaPwmDriver.FullyOff,
                AIn1 = PcaPwmDriver.FullyOff,
                BIn2 = PcaPwmDriver.FullyOn
            });

            return controlPhases;
        }

        public static List<StepperMotorPhase> GetMicroStepSequence()
        {
            var controlPhases = new List<StepperMotorPhase>();

            controlPhases.Add(new StepperMotorPhase()
            {
                AIn2 = PcaPwmDriver.FullyOn,
                BIn1 = PcaPwmDriver.FullyOn,
                AIn1 = PcaPwmDriver.FullyOff,
                BIn2 = PcaPwmDriver.FullyOff
            });

            controlPhases.Add(new StepperMotorPhase()
            {
                AIn2 = PcaPwmDriver.FullyOff,
                BIn1 = PcaPwmDriver.FullyOn,
                AIn1 = PcaPwmDriver.FullyOn,
                BIn2 = PcaPwmDriver.FullyOff
            });

            controlPhases.Add(new StepperMotorPhase()
            {
                AIn2 = PcaPwmDriver.FullyOff,
                BIn1 = PcaPwmDriver.FullyOff,
                AIn1 = PcaPwmDriver.FullyOn,
                BIn2 = PcaPwmDriver.FullyOn
            });

            controlPhases.Add(new StepperMotorPhase()
            {
                AIn2 = PcaPwmDriver.FullyOn,
                BIn1 = PcaPwmDriver.FullyOff,
                AIn1 = PcaPwmDriver.FullyOff,
                BIn2 = PcaPwmDriver.FullyOn
            });

            return controlPhases;
        }
    }
}
