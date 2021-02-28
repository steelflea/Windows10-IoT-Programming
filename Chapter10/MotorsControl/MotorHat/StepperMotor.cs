using MotorsControl.Enums;
using MotorsControl.Helpers;
using MotorsControl.PWM;
using SenseHat.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MotorsControl.MotorHat
{
    public class StepperMotor
    {
        // RPM (스테퍼 모터 속도)
        public byte MinRpm { get; } = 1;
        public byte MaxRpm { get; } = 60;
        public byte Rpm { get; private set; } = 30;

        // 적절한 초기화가 수행되었는지 여부를 나타내는 속성
        public bool IsInitialized
        {
            get { return pcaPwmDriver.IsInitialized; }
        }

        // 단계 및 단계 카운터
        public uint Steps { get; private set; }
        public int CurrentStep { get; private set; } = 0;

        // PWM 드라이버
        private PcaPwmDriver pcaPwmDriver;

        // 채널 및 제어 단계
        private StepperMotorPwmChannels channels;
        private List<StepperMotorPhase> fullStepControlPhases;

        // 마이크로 스테핑
        public uint MicroStepCount { get; } = 8;

        private List<StepperMotorPhase> microStepControlPhases;
        private List<PcaRegisterValue> microStepCurve;

        public StepperMotor(PcaPwmDriver pcaPwmDriver, uint steps = 200)
        {
            Check.IsNull(pcaPwmDriver);

            this.pcaPwmDriver = pcaPwmDriver;

            Steps = steps * MicroStepCount;

            SetSpeed(Rpm);

            fullStepControlPhases = ControlPhaseHelper.GetFullStepSequence();

            // 마이크로 스테핑 제어 단계 및 선형 램프
            microStepControlPhases = ControlPhaseHelper.GetMicroStepSequence();
            microStepCurve = MicrosteppingHelper.GetLinearRamp(MicroStepCount);
        }

        public void MakeStep(StepperMotorIndex motorIndex, MotorDirection direction, SteppingMode steppingMode = SteppingMode.FullSteps)
        {
            ConfigureChannels(motorIndex);

            UpdateCurrentStep(direction);

            UpdateChannels(steppingMode);
        }

        public void SetSpeed(byte rpm)
        {
            Check.IsLengthInValidRange(rpm, MinRpm, MaxRpm);

            Rpm = rpm;
        }

        public void Move(StepperMotorIndex motorIndex, MotorDirection direction, uint steps, SteppingMode steppingMode = SteppingMode.FullSteps)
        {
            var msDelay = RpmToMsDelay(Rpm, steppingMode);

            steps = GetTotalStepCount(steppingMode, steps);

            for (uint i = 0; i < steps; i++)
            {
                MakeStep(motorIndex, direction, steppingMode);

                Task.Delay(msDelay).Wait();
            }
        }

        public void MoveWithSpeedAdjustment(StepperMotorIndex motorIndex, MotorDirection direction, uint steps, SteppingMode steppingMode = SteppingMode.FullSteps)
        {
            steps = GetTotalStepCount(steppingMode, steps);

            var speedRamp = SpeedRampHelper.GenerateTrapezoidalRamp(steps);

            for (uint i = 0; i < steps; i++)
            {
                MakeStep(motorIndex, direction, steppingMode);

                var msAutoDelay = RpmToMsDelay(speedRamp[i], steppingMode);

                Task.Delay(msAutoDelay).Wait();
            }
        }

        private int RpmToMsDelay(byte rpm, SteppingMode steppingMode = SteppingMode.FullSteps)
        {
            const double minToMsScaler = 60000.0;

            var fullStepCount = Steps / MicroStepCount;

            var msDelay = minToMsScaler / (fullStepCount * rpm);

            if (steppingMode == SteppingMode.MicroSteps)
            {
                msDelay /= MicroStepCount;
            }

            return Convert.ToInt32(msDelay);
        }

        private void UpdateChannels(SteppingMode steppingMode = SteppingMode.FullSteps)
        {
            StepperMotorPhase currentPhase = null;

            switch (steppingMode)
            {
                case SteppingMode.MicroSteps:
                    currentPhase = GetMicroStepControlPhase();
                    break;

                default:
                    currentPhase = GetFullStepControlPhase();
                    break;
            }

            pcaPwmDriver.SetChannelValue(channels.PwmA, currentPhase.PwmA);
            pcaPwmDriver.SetChannelValue(channels.PwmB, currentPhase.PwmB);

            pcaPwmDriver.SetChannelValue(channels.AIn1, currentPhase.AIn1);
            pcaPwmDriver.SetChannelValue(channels.AIn2, currentPhase.AIn2);

            pcaPwmDriver.SetChannelValue(channels.BIn1, currentPhase.BIn1);
            pcaPwmDriver.SetChannelValue(channels.BIn2, currentPhase.BIn2);
        }

        private StepperMotorPhase GetMicroStepControlPhase()
        {
            // mu_i
            var microStepIndex = (int)(CurrentStep % MicroStepCount);

            // nu_i
            var microStepPhaseIndex = (int)(CurrentStep % (MicroStepCount * fullStepControlPhases.Count));

            // 풀 스텝 제어 단계 인덱스
            var mainPhaseIndex = MicrosteppingHelper.GetPhaseIndex(microStepPhaseIndex, MicroStepCount);

            // AIn1, AIn2, BIn1, BIn2 신호의 제어 단계
            var phase = microStepControlPhases[mainPhaseIndex];

            // PwmA, PwmB 신호
            MicrosteppingHelper.AdjustMicroStepPhase(phase, microStepCurve, MicroStepCount, microStepIndex, microStepPhaseIndex);

            return phase;
        }

        private StepperMotorPhase GetFullStepControlPhase()
        {
            var phaseIndex = CurrentStep % fullStepControlPhases.Count;

            return fullStepControlPhases[phaseIndex];
        }

        private void UpdateCurrentStep(MotorDirection direction)
        {
            if (direction == MotorDirection.Forward)
            {
                CurrentStep++;
            }
            else
            {
                CurrentStep--;
            }

            if (CurrentStep < 0)
            {
                CurrentStep = (int)Steps - 1;
            }

            if (CurrentStep >= Steps)
            {
                CurrentStep = 0;
            }
        }

        private void ConfigureChannels(StepperMotorIndex motorIndex)
        {
            switch (motorIndex)
            {
                case StepperMotorIndex.SM1:
                    channels.AIn1 = 10;
                    channels.AIn2 = 9;

                    channels.BIn1 = 11;
                    channels.BIn2 = 12;

                    channels.PwmA = 8;
                    channels.PwmB = 13;
                    break;

                case StepperMotorIndex.SM2:
                    channels.AIn1 = 4;
                    channels.AIn2 = 3;

                    channels.BIn1 = 5;
                    channels.BIn2 = 6;

                    channels.PwmA = 2;
                    channels.PwmB = 7;
                    break;
            }
        }

        private uint GetTotalStepCount(SteppingMode steppingMode, uint steps)
        {
            if (steppingMode == SteppingMode.MicroSteps)
            {
                steps *= MicroStepCount;
            }

            return steps;
        }
    }
}