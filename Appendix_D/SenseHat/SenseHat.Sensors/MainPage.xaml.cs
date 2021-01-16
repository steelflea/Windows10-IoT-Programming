using SenseHat.Sensors.TelemetryControl;
using SenseHat.Sensors.ViewModels;
using System;
using System.Diagnostics;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SenseHat.Sensors
{
    public sealed partial class MainPage : Page
    {
        private Telemetry telemetry;
        private SensorsViewModel sensorsViewModel = new SensorsViewModel()
        {
            ReadoutDelay = TimeSpan.FromSeconds(1)
        };

        public MainPage()
        {
            InitializeComponent();
        }

        private async void ButtonConnect_Click(object sender, RoutedEventArgs e)
        {
            if (!sensorsViewModel.IsConnected)
            {
                try
                {
                    telemetry = await Telemetry.CreateAsync(sensorsViewModel.ReadoutDelay);
                    telemetry.DataReady += Telemetry_DataReady;

                    sensorsViewModel.IsConnected = true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        private void Telemetry_DataReady(object sender, TelemetryEventArgs e)
        {
            DisplaySensorReadings(e);
        }

        private async void DisplaySensorReadings(TelemetryEventArgs telemetryEventArgs)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                sensorsViewModel.SensorReadings.Temperature = telemetryEventArgs.Temperature;
                sensorsViewModel.SensorReadings.Humidity = telemetryEventArgs.Humidity;
                sensorsViewModel.SensorReadings.Pressure = telemetryEventArgs.Pressure;

                sensorsViewModel.SensorReadings.Accelerometer = telemetryEventArgs.LinearAcceleration;
                sensorsViewModel.SensorReadings.Gyroscope = telemetryEventArgs.AngularSpeed;
                sensorsViewModel.SensorReadings.Magnetometer = telemetryEventArgs.MagneticField;
            });
        }

        private void ButtonStartSensorReading_Click(object sender, RoutedEventArgs e)
        {
            telemetry.Start();
            sensorsViewModel.IsTelemetryActive = telemetry.IsActive;
        }

        private void ButtonStopSensorReading_Click(object sender, RoutedEventArgs e)
        {
            telemetry.Stop();
            sensorsViewModel.IsTelemetryActive = telemetry.IsActive;
        }
    }
}
