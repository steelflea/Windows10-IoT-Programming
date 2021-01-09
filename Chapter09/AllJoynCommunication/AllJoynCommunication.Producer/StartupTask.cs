using System;
using AllJoynCommunication.Producer.Helpers;
using AllJoynCommunication.Producer.SenseHatLedArray;
using com.iot.SenseHatLedArray;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.AllJoyn;
using Windows.UI;
using SerialCommunication.Common.Helpers;

namespace AllJoynCommunication.Producer
{
    public sealed class StartupTask : IBackgroundTask
    {
        private BackgroundTaskDeferral taskDeferral;

        private AllJoynBusAttachment allJoynBusAttachment;

        private LedArray ledArray;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            taskDeferral = taskInstance.GetDeferral();

            await InitializeLedArray();

            StartAllJoynService();
        }

        private void StartAllJoynService()
        {
            allJoynBusAttachment = new AllJoynBusAttachment();

            SenseHatLedArrayProducer senseHatAllJoynProducer = 
                new SenseHatLedArrayProducer(allJoynBusAttachment);
            senseHatAllJoynProducer.Service = new AllJoynLedArray(ledArray);
            senseHatAllJoynProducer.Start();
        }

        private async Task InitializeLedArray()
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
    }
}
