using AllJoynCommunication.Consumer.Helpers;
using AllJoynCommunication.Producer.SenseHatLedArray;
using com.iot.SenseHatLedArray;
using SerialCommunication.Common.Helpers;
using System;
using System.Collections.ObjectModel;
using Windows.Devices.AllJoyn;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AllJoynCommunication.Consumer
{
    public sealed partial class MainPage : Page
    {
        private const string deviceUnavailable = "Device unavailable";

        private object shapes = Enum.GetValues(typeof(ShapeKind));
        private object selectedShape = ShapeKind.None;

        private ObservableCollection<string> diagnosticData = new ObservableCollection<string>();

        private SenseHatLedArrayConsumer senseHatLedArrayConsumer;
        private AllJoynBusAttachment allJoynBusAttachment;
        private SenseHatLedArrayWatcher senseHatLedArrayWatcher;

        private bool isSenseHatAvailable = false;        

        public MainPage()
        {
            InitializeComponent();

            InitializeWatcher();
        }

        private void InitializeWatcher()
        {
            allJoynBusAttachment = new AllJoynBusAttachment();

            senseHatLedArrayWatcher = new SenseHatLedArrayWatcher(allJoynBusAttachment);

            senseHatLedArrayWatcher.Added += SenseHatLedArrayWatcher_Added;
            senseHatLedArrayWatcher.Stopped += SenseHatLedArrayWatcher_Stopped;

            senseHatLedArrayWatcher.Start();
        }

        private async void SenseHatLedArrayWatcher_Added(SenseHatLedArrayWatcher sender, 
            AllJoynServiceInfo args)
        {
            var result = await SenseHatLedArrayConsumer.JoinSessionAsync(args, senseHatLedArrayWatcher);

            if (result.Status == AllJoynStatus.Ok)
            {
                isSenseHatAvailable = true;

                senseHatLedArrayConsumer = result.Consumer;

                DiagnosticInfo.Display(diagnosticData, 
                    "Successfully joined the AllJoyn session. Bus name: " + args.UniqueName);
            }
        }

        private void SenseHatLedArrayWatcher_Stopped(SenseHatLedArrayWatcher sender, 
            AllJoynProducerStoppedEventArgs args)
        {
            isSenseHatAvailable = false;

            senseHatLedArrayConsumer.Dispose();
            senseHatLedArrayConsumer = null;

            DiagnosticInfo.Display(diagnosticData, 
                "SenseHatLedArray AllJoyn device left the network");
        }

        private async void ButtonGetShape_Click(object sender, RoutedEventArgs e)
        {
            if (isSenseHatAvailable)
            {
                var getShapeResult = await senseHatLedArrayConsumer.GetShapeAsync();

                var allJoynStatus = AllJoynStatusHelper.GetStatusCodeName(getShapeResult.Status);

                var info = string.Format("Current shape: {0}, Status: {1}",
                    (ShapeKind)getShapeResult.Shape, allJoynStatus);

                DiagnosticInfo.Display(diagnosticData, info);
            }
            else
            {
                DiagnosticInfo.Display(diagnosticData, deviceUnavailable);
            }
        }

        private async void ButtonDrawShape_Click(object sender, RoutedEventArgs e)
        {
            if (isSenseHatAvailable)
            {
                var drawShapeResult = await senseHatLedArrayConsumer.DrawShapeAsync(
                    (int)selectedShape);

                var allJoynStatus = AllJoynStatusHelper.GetStatusCodeName(
                    drawShapeResult.Status);

                var info = string.Format("Shape drawn: {0}, Status: {1}", 
                    selectedShape, allJoynStatus);

                DiagnosticInfo.Display(diagnosticData, info);
            }
            else
            {
                DiagnosticInfo.Display(diagnosticData, deviceUnavailable);
            }
        }

        private async void ButtonTurnOff_Click(object sender, RoutedEventArgs e)
        {
            if (isSenseHatAvailable)
            {
                var turnOffResult = await senseHatLedArrayConsumer.TurnOffAsync();

                var allJoynStatus = AllJoynStatusHelper.GetStatusCodeName(turnOffResult.Status);

                DiagnosticInfo.Display(diagnosticData, "Turn off method result: " + allJoynStatus);
            }
            else
            {
                DiagnosticInfo.Display(diagnosticData, deviceUnavailable);
            }
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            diagnosticData.Clear();
        }
    }
}
