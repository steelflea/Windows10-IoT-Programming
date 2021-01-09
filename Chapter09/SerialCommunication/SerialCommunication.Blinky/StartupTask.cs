using SerialCommunication.Common.Enums;
using SerialCommunication.Common.Helpers;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.SerialCommunication;

namespace SerialCommunication.Blinky
{
    public sealed class StartupTask : IBackgroundTask
    {
        private BackgroundTaskDeferral taskDeferral;
        private LedControl ledControl;
        private SerialDevice serialDevice;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            taskDeferral = taskInstance.GetDeferral();

            ledControl = new LedControl();

            await SetupCommunication();
        }

        private async Task SetupCommunication()
        {
            serialDevice = await SerialCommunicationHelper.GetFirstDeviceAvailable();

            SerialCommunicationHelper.SetDefaultConfiguration(serialDevice);

            new Task(CommunicationListener).Start();
        }

        private async void CommunicationListener()
        {
            while (true)
            {
                var commandReceived = await SerialCommunicationHelper.ReadBytes(serialDevice);

                try
                {
                    ParseCommand(commandReceived);
                }
                catch (Exception ex)
                {
                    DiagnosticInfo.Display(null, ex.Message);
                }
            }
        }

        private void ParseCommand(byte[] command)
        {
            var errorCode = CommandHelper.VerifyCommand(command);

            if (errorCode == ErrorCode.OK)
            {
                var commandId = (CommandId)command[CommandHelper.CommandIdIndex];

                switch (commandId)
                {
                    case CommandId.BlinkingFrequency:
                        HandleBlinkingFrequencyCommand(command);
                        break;

                    case CommandId.BlinkingStatus:
                        HandleBlinkingStatusCommand(command);
                        break;
                }
            }
        }

        private void HandleBlinkingFrequencyCommand(byte[] command)
        {
            var frequency = BitConverter.ToDouble(command, CommandHelper.CommandDataBeginIndex);

            ledControl.SetFrequency(frequency);
        }

        private void HandleBlinkingStatusCommand(byte[] command)
        {
            var isLedBlinking = Convert.ToBoolean(command[CommandHelper.CommandDataBeginIndex]);

            ledControl.Update(isLedBlinking);
        }
    }
}
