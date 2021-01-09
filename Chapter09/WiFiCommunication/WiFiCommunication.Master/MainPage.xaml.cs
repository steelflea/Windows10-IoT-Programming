using SerialCommunication.Common.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WiFiCommunication.Common;
using Windows.Devices.WiFi;
using Windows.Networking.Sockets;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WiFiCommunication.Master
{
    public sealed partial class MainPage : Page
    {
        private ObservableCollection<string> diagnosticData = new ObservableCollection<string>();

        private SenseHatColor senseHatColor = new SenseHatColor();

        private StreamSocket streamSocket;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void ButtonConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await CloseStreamSocket();

                var connectionStatus = await WiFiCommunicationHelper.ConnectToWiFiNetwork();

                if (connectionStatus == WiFiConnectionStatus.Success)
                {
                    streamSocket = await WiFiCommunicationHelper.ConnectToHost();

                    DiagnosticInfo.Display(diagnosticData, "Connected to: " + WiFiCommunicationHelper.Rpi2HostName);
                }
            }
            catch (Exception ex)
            {
                DiagnosticInfo.Display(diagnosticData, ex.Message);
            }
        }

        private async Task CloseStreamSocket()
        {
            if (streamSocket != null)
            {
                await streamSocket.CancelIOAsync();

                streamSocket.Dispose();
                streamSocket = null;
            }
        }

        private async void ButtonSendColor_Click(object sender, RoutedEventArgs e)
        {
            if (streamSocket != null)
            {
                var commandData = CommandHelper.PrepareLedColorCommand(senseHatColor.Brush.Color);

                await SerialCommunicationHelper.WriteBytes(streamSocket.OutputStream, commandData);

                DiagnosticInfo.Display(diagnosticData, CommandHelper.CommandToString(commandData));
            }
            else
            {
                DiagnosticInfo.Display(diagnosticData, "No active connection");
            }
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            diagnosticData.Clear();
        }
    }
}
