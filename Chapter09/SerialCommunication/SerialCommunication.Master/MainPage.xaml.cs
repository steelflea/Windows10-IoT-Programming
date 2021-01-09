using System;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SerialCommunication.Common.Helpers;
using SerialCommunication.Common.Enums;

namespace SerialCommunication.Master
{
    public sealed partial class MainPage : Page
    {
        private SerialDevice serialDevice;

        private string serialDeviceId = string.Empty;

        private bool? isLedBlinking;
        private double hzBlinkingFrequency = 10.0f;

        private ObservableCollection<string> diagnosticData = new ObservableCollection<string>();
        private ObservableCollection<DeviceInformation> devicesList = new ObservableCollection<DeviceInformation>();

        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            await InitializeUI();
        }

        private async Task InitializeUI()
        {
            await GetDeviceList();

            var devicesCount = devicesList.Count;

            isLedBlinking = false;

            DiagnosticInfo.Display(diagnosticData, "Initialized. Number of available devices: " + devicesCount);
        }

        private async Task GetDeviceList()
        {
            var serialDevices = await SerialCommunicationHelper.FindSerialDevices();            
            
            foreach (var deviceInformation in serialDevices)
            {
                devicesList.Add(deviceInformation);
            }
        }

        private async void ButtonConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await SetupConnection();
            }
            catch (Exception ex)
            {
                DiagnosticInfo.Display(diagnosticData, ex.Message);
            }
        }

        private async Task SetupConnection()
        {
            // 새로운 연결을 시도하기 전에 이전 연결을 모두 닫는다
            CloseConnection();

            serialDevice = await SerialDevice.FromIdAsync(serialDeviceId);

            if (serialDevice != null)
            {
                // 연결 구성
                SerialCommunicationHelper.SetDefaultConfiguration(serialDevice);
            }
        }

        private void CloseConnection()
        {
            if (serialDevice != null)
            {
                serialDevice.Dispose();
                serialDevice = null;
            }
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            diagnosticData.Clear();
        }

        private async void ButtonSendData_Click(object sender, RoutedEventArgs e)
        {
            await SendCommand(CommandId.BlinkingFrequency);
            await SendCommand(CommandId.BlinkingStatus);
        }

        private async Task SendCommand(CommandId commandId)
        {
            if (serialDevice != null)
            {
                byte[] command = null;

                switch (commandId)
                {
                    case CommandId.BlinkingFrequency:
                        command = CommandHelper.PrepareSetFrequencyCommand(hzBlinkingFrequency);
                        break;

                    case CommandId.BlinkingStatus:
                        command = CommandHelper.PrepareBlinkingStatusCommand(isLedBlinking);
                        break;
                }

                await SerialCommunicationHelper.WriteBytes(serialDevice, command);
                DiagnosticInfo.Display(diagnosticData, "Data written: " + CommandHelper.CommandToString(command));
            }
            else
            {
                DiagnosticInfo.Display(diagnosticData, "No active connection");
            }
        }
    }
}
