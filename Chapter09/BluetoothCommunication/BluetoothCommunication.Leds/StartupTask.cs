using BluetoothCommunication.Leds.SenseHatLedArray;
using SerialCommunication.Common.Enums;
using SerialCommunication.Common.Helpers;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Networking.Sockets;
using Windows.UI;
using BluetoothCommunication.Leds.Helpers;

namespace BluetoothCommunication.Leds
{
    public sealed class StartupTask : IBackgroundTask
    {
        private BackgroundTaskDeferral taskDeferral;
        private StreamSocket streamSocket;

        private RfcommServiceProvider rfcommProvider;
        private StreamSocketListener streamSocketListener;

        private LedArray ledArray;

        private volatile bool isCommunicationListenerStarted = false;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            taskDeferral = taskInstance.GetDeferral();

            InitializeLedArray();

            await StartRfcommService();
        }

        private async void InitializeLedArray()
        {
            const byte address = 0x46;
            var device = await I2cHelper.GetI2cDevice(address);

            if (device != null)
            {
                ledArray = new LedArray(device);
                ledArray.Reset(Colors.Black);
            }
            else
            {
                DiagnosticInfo.Display(null, "Led array unavailable");
            }
        }

        private async Task StartRfcommService()
        {
            var serviceGuid = Guid.Parse("34B1CF4D-1069-4AD6-89B6-E161D79BE4D8");
            var serviceId = RfcommServiceId.FromUuid(serviceGuid);

            rfcommProvider = await RfcommServiceProvider.CreateAsync(serviceId);

            streamSocketListener = new StreamSocketListener();
            streamSocketListener.ConnectionReceived += StreamSocketListener_ConnectionReceived;

            try
            {
                await streamSocketListener.BindServiceNameAsync(rfcommProvider.ServiceId.AsString(), SocketProtectionLevel.BluetoothEncryptionAllowNullAuthentication);

                rfcommProvider.StartAdvertising(streamSocketListener);

                DiagnosticInfo.Display(null, "RFCOMM service started. Waiting for clients...");
            }
            catch (Exception ex)
            {
                DiagnosticInfo.Display(null, ex.Message);
            }
        }

        private void StreamSocketListener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            DiagnosticInfo.Display(null, "Client has been connected");

            streamSocket = args.Socket;

            StartCommunicationListener();
        }

        private void StartCommunicationListener()
        {
            if (!isCommunicationListenerStarted)
            {
                new Task(CommunicationListener).Start();

                isCommunicationListenerStarted = true;
            }
        }

        private async void CommunicationListener()
        {
            const int msSleepTime = 50;

            while (true)
            {
                var commandReceived = await SerialCommunicationHelper.ReadBytes(streamSocket.InputStream);

                try
                {
                    if (commandReceived.Length > 0)
                    {
                        ParseCommand(commandReceived);
                    }
                }
                catch (Exception ex)
                {
                    DiagnosticInfo.Display(null, ex.Message);
                }

                Task.Delay(msSleepTime).Wait();
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
                    case CommandId.LedColor:
                        HandleLedColorCommand(command);
                        break;
                }
            }
        }

        private void HandleLedColorCommand(byte[] command)
        {
            var redChannel = command[CommandHelper.CommandDataBeginIndex];
            var greenChannel = command[CommandHelper.CommandDataBeginIndex + 1];
            var blueChannel = command[CommandHelper.CommandDataBeginIndex + 2];

            var color = Color.FromArgb(0, redChannel, greenChannel, blueChannel);

            if (ledArray != null)
            {
                ledArray.Reset(color);
            }

            DiagnosticInfo.Display(null, color.ToString() + " " + redChannel 
                + " " + greenChannel + " " + blueChannel);
        }
    }
}
