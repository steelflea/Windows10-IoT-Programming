using SenseHat.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Networking.Sockets;

namespace BluetoothCommunication.Common.Helpers
{
    public static class BluetoothCommunicationHelper
    {
        public static async Task<DeviceInformationCollection> FindPairedDevices()
        {
            var defaultSelector = BluetoothDevice.GetDeviceSelector();

            return await DeviceInformation.FindAllAsync(defaultSelector);
        }

        public static async Task<BluetoothDevice> GetFirstPairedDeviceAvailable()
        {
            var serialDeviceCollection = await FindPairedDevices();

            var serialDeviceInformation = serialDeviceCollection.FirstOrDefault();

            if (serialDeviceInformation != null)
            {
                return await BluetoothDevice.FromIdAsync(serialDeviceInformation.Id);
            }
            else
            {
                return null;
            }
        }

        public static async Task<StreamSocket> Connect(BluetoothDevice bluetoothDevice)
        {
            Check.IsNull(bluetoothDevice);
            var serviceGuid = Guid.Parse("34B1CF4D-1069-4AD6-89B6-E161D79BE4D8");
            var rfcommServices = await bluetoothDevice.GetRfcommServicesForIdAsync(RfcommServiceId.FromUuid(serviceGuid), BluetoothCacheMode.Uncached);
            var rfcommService = bluetoothDevice.RfcommServices.FirstOrDefault();

            for (int i = 0; i < bluetoothDevice.RfcommServices.Count; i++)
            {
                rfcommService = bluetoothDevice.RfcommServices.ElementAt(i);

                if(rfcommService.ServiceId.Uuid.Equals(serviceGuid))
                {
                    break;
                }
            }

            if (rfcommService != null)
            {
                return await ConnectToStreamSocket(bluetoothDevice, 
                    rfcommService.ConnectionServiceName);
            }
            else
            {
                throw new Exception(
                    "Selected bluetooth device does not advertise any RFCOMM service");
            }
        }

        private async static Task<StreamSocket> ConnectToStreamSocket(
            BluetoothDevice bluetoothDevice, string connectionServiceName)
        {
            try
            {
                var streamSocket = new StreamSocket();

                await streamSocket.ConnectAsync(bluetoothDevice.HostName, connectionServiceName);
                
                return streamSocket;
            }
            catch (Exception)
            {
                throw new Exception(
                    "Connection cannot be established. Verify that device is paired");
            }
        }   
    }
}
