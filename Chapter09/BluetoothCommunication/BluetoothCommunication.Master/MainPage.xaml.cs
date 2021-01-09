using System;
using BluetoothCommunication.Common.Helpers;
using Windows.Networking.Sockets;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SerialCommunication.Common.Helpers;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace BluetoothCommunication.Master
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
                var device = await BluetoothCommunicationHelper.GetFirstPairedDeviceAvailable();

                await CloseConnection();

                streamSocket = await BluetoothCommunicationHelper.Connect(device);

                DiagnosticInfo.Display(diagnosticData, "Connected to: " + device.HostName);
            }
            catch (Exception ex)
            {
                DiagnosticInfo.Display(diagnosticData, ex.Message);
            }
        }

        private async Task CloseConnection()
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
