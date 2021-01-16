using SenseHat.Portable.Helpers;
using SenseHat.UWP.Helpers;

namespace SenseHat.UWP.Sensors
{
    public sealed class MagneticFieldSensor : SensorBase
    {
        public static MagneticFieldSensor Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new MagneticFieldSensor();
                        }
                    }
                }

                return instance;
            }
        }

        private static volatile MagneticFieldSensor instance;

        private float scaler = short.MaxValue / 4.0f;

        // Sensor-specific calibration offset
        private Vector3D<float> calibrationOffset = new Vector3D<float>()
        {
            X = 0.17646f,
            Y = 0.08201f,
            Z = -0.03026f
        };

        private MagneticFieldSensor()
        {
            sensorAddress = 0x1C;

            whoAmIRegisterAddress = 0x0F;
            whoAmIDefaultValue = 0x3D;
        }

        public Vector3D<float> GetMagneticField()
        {
            var xMagneticFieldRegisterAddresses = new byte[] { 0x28, 0x29 };
            var yMagneticFieldRegisterAddresses = new byte[] { 0x2A, 0x2B };
            var zMagneticFieldRegisterAddresses = new byte[] { 0x2C, 0x2D };

            var xMagneticField = RegisterHelper.GetShort(device, xMagneticFieldRegisterAddresses);
            var yMagneticField = RegisterHelper.GetShort(device, yMagneticFieldRegisterAddresses);
            var zMagneticField = RegisterHelper.GetShort(device, zMagneticFieldRegisterAddresses);

            return new Vector3D<float>()
            {
                X = xMagneticField / scaler - calibrationOffset.X,
                Y = yMagneticField / scaler - calibrationOffset.Y,
                Z = zMagneticField / scaler - calibrationOffset.Z
            };
        }

        protected override void Configure()
        {
            CheckInitialization();

            // Enable magnetometer
            const byte operatingModeControlRegisterAddress = 0x22;
            var operatingModeResiterValue = MagneticFieldSensorHelper.ConfigureOperatingMode();

            RegisterHelper.WriteByte(device, operatingModeControlRegisterAddress, operatingModeResiterValue);

            // Configure sensing parameters (ODR, performance)
            const byte controlRegisterAddress = 0x20;
            var controlRegisterByteValue = MagneticFieldSensorHelper.ConfigureSensingParameters();

            RegisterHelper.WriteByte(device, controlRegisterAddress, controlRegisterByteValue);
        }
    }
}
