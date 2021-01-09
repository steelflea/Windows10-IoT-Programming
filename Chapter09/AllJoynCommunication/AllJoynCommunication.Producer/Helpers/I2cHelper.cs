using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;
using Windows.Foundation;

namespace AllJoynCommunication.Producer.Helpers
{
    public static class I2cHelper
    {
        public static IAsyncOperation<I2cDevice> GetI2cDevice(byte address)
        {
            return GetI2cDeviceHelper(address).AsAsyncOperation();
        }

        private static async Task<I2cDevice> GetI2cDeviceHelper(byte address)
        {
            I2cDevice device = null;

            var settings = new I2cConnectionSettings(address);

            string deviceSelectorString = I2cDevice.GetDeviceSelector();

            var matchedDevicesList = await DeviceInformation.FindAllAsync(deviceSelectorString);
            if (matchedDevicesList.Count > 0)
            {
                var deviceInformation = matchedDevicesList.First();

                device = await I2cDevice.FromIdAsync(deviceInformation.Id, settings);
            }

            return device;
        }
    }
}
