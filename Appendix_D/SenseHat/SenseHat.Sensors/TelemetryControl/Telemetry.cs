using SenseHat.Portable.Helpers;
using SenseHat.UWP.Sensors;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SenseHat.Sensors.TelemetryControl
{
    public class Telemetry
    {
        // Event
        public event EventHandler<TelemetryEventArgs> DataReady = delegate { };

        // Telemetry task
        public bool IsActive { get; private set; } = false;

        private Task telemetryTask;
        private CancellationTokenSource telemetryCancellationTokenSource;

        // Sensors
        private TemperatureAndPressureSensor temperatureAndPressureSensor = TemperatureAndPressureSensor.Instance;
        private HumidityAndTemperatureSensor humidityAndTemperatureSensor = HumidityAndTemperatureSensor.Instance;
        private InertialSensor inertialSensor = InertialSensor.Instance;
        private MagneticFieldSensor magneticFieldSensor = MagneticFieldSensor.Instance;

        private TimeSpan readoutDelay;

        public void Start()
        {
            if (!IsActive)
            {
                InitializeTelemetryTask();

                telemetryTask.Start();

                IsActive = true;
            }
        }

        public void Stop()
        {
            if (IsActive)
            {
                telemetryCancellationTokenSource.Cancel();

                IsActive = false;
            }
        }

        public static async Task<Telemetry> CreateAsync(TimeSpan readoutDelay)
        {
            Check.IsNull(readoutDelay);

            var telemetry = new Telemetry(readoutDelay);

            await telemetry.InitializeSensors();

            return telemetry;
        }

        private Telemetry(TimeSpan readoutDelay)
        {
            this.readoutDelay = readoutDelay;
        }

        private async Task InitializeSensors()
        {
            await temperatureAndPressureSensor.Initialize();
            VerifyInitialization(temperatureAndPressureSensor, "Temperature and pressure sensor is unavailable");

            await humidityAndTemperatureSensor.Initialize();
            VerifyInitialization(humidityAndTemperatureSensor, "Humidity sensor is unavailable");

            await inertialSensor.Initialize();
            VerifyInitialization(inertialSensor, "Inertial sensor is unavailable");

            await magneticFieldSensor.Initialize();
            VerifyInitialization(magneticFieldSensor, "Magnetic field sensor is unavailable");
        }

        private void VerifyInitialization(SensorBase sensorBase, string exceptionMessage)
        {
            if (!sensorBase.IsInitialized)
            {
                throw new Exception(exceptionMessage);
            }
        }

        private void InitializeTelemetryTask()
        {
            telemetryCancellationTokenSource = new CancellationTokenSource();

            telemetryTask = new Task(() =>
            {
                while (!telemetryCancellationTokenSource.IsCancellationRequested)
                {
                    if (IsActive)
                    {
                        var telemetryEventArgs = GetSensorReadings();

                        DataReady(this, telemetryEventArgs);

                        Task.Delay(readoutDelay).Wait();
                    }
                }
            }, telemetryCancellationTokenSource.Token);
        }

        private TelemetryEventArgs GetSensorReadings()
        {
            var temperature = temperatureAndPressureSensor.GetTemperature();
            var humidity = humidityAndTemperatureSensor.GetHumidity();
            var pressure = temperatureAndPressureSensor.GetPressure();

            var linearAcc = inertialSensor.GetLinearAcceleration();
            var angularSpeed = inertialSensor.GetAngularSpeed();
            var magneticField = magneticFieldSensor.GetMagneticField();

            return new TelemetryEventArgs(temperature, humidity, pressure, linearAcc, angularSpeed, magneticField);
        }
    }
}
