namespace MotorsControl.MotorHat
{
    public struct StepperMotorPwmChannels
    {
        public byte AIn1 { get; set; }
        public byte AIn2 { get; set; }

        public byte BIn1 { get; set; }
        public byte BIn2 { get; set; }

        public byte PwmA { get; set; }
        public byte PwmB { get; set; }
    }
}
