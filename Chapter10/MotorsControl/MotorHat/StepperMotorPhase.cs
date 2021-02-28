using MotorsControl.PWM;

namespace MotorsControl.MotorHat
{
    public class StepperMotorPhase
    {
        public PcaRegisterValue AIn1 { get; set; }
        public PcaRegisterValue AIn2 { get; set; }

        public PcaRegisterValue BIn1 { get; set; }
        public PcaRegisterValue BIn2 { get; set; }

        public PcaRegisterValue PwmA { get; set; } = PcaPwmDriver.FullyOn;
        public PcaRegisterValue PwmB { get; set; } = PcaPwmDriver.FullyOn;
    }
}
