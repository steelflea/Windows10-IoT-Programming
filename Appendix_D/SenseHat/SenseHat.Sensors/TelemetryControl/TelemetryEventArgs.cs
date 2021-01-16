using SenseHat.Portable.Helpers;

namespace SenseHat.Sensors.TelemetryControl
{
    public class TelemetryEventArgs
    {
        public float Temperature { get; private set; }

        public float Humidity { get; private set; }

        public float Pressure { get; private set; }

        public Vector3D<float> LinearAcceleration { get; private set; }

        public Vector3D<float> AngularSpeed { get; private set; }

        public Vector3D<float> MagneticField { get; private set; }

        public TelemetryEventArgs(float temperature, float humidity, float pressure,
            Vector3D<float> linearAcc, Vector3D<float> angularSpeed, Vector3D<float> magneticField)
        {
            Temperature = temperature;
            Humidity = humidity;
            Pressure = pressure;

            LinearAcceleration = linearAcc;
            AngularSpeed = angularSpeed;
            MagneticField = magneticField;
        }
    }
}
