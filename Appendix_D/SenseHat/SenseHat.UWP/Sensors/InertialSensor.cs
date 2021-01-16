using SenseHat.Portable.Helpers;
using SenseHat.UWP.Helpers;

namespace SenseHat.UWP.Sensors
{
    public class InertialSensor : SensorBase
    {
        public static InertialSensor Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new InertialSensor();
                        }
                    }
                }

                return instance;
            }
        }

        private static volatile InertialSensor instance;

        private float linearAccelerationScaler = short.MaxValue / 2.0f;
        private float angularSpeedScaler = short.MaxValue / 245.0f;
        
        public Vector3D<float> GetLinearAcceleration()
        {
            var xLinearAccelerationRegisterAddresses = new byte[] { 0x28, 0x29 };
            var yLinearAccelerationRegisterAddresses = new byte[] { 0x2A, 0x2B };
            var zLinearAccelerationRegisterAddresses = new byte[] { 0x2C, 0x2D };

            var xLinearAcceleration = RegisterHelper.GetShort(device, xLinearAccelerationRegisterAddresses);
            var yLinearAcceleration = RegisterHelper.GetShort(device, yLinearAccelerationRegisterAddresses);
            var zLinearAcceleration = RegisterHelper.GetShort(device, zLinearAccelerationRegisterAddresses);

            return new Vector3D<float>()
            {
                X = xLinearAcceleration / linearAccelerationScaler,
                Y = yLinearAcceleration / linearAccelerationScaler,
                Z = zLinearAcceleration / linearAccelerationScaler
            };
        }

        public Vector3D<float> GetAngularSpeed()
        {
            var xAngularSpeedRegisterAddresses = new byte[] { 0x18, 0x19 };
            var yAngularSpeedRegisterAddresses = new byte[] { 0x1A, 0x1B };
            var zAngularSpeedRegisterAddresses = new byte[] { 0x1C, 0x1D };

            var xAngularSpeed = RegisterHelper.GetShort(device, xAngularSpeedRegisterAddresses);
            var yAngularSpeed = RegisterHelper.GetShort(device, yAngularSpeedRegisterAddresses);
            var zAngularSpeed = RegisterHelper.GetShort(device, zAngularSpeedRegisterAddresses);

            return new Vector3D<float>()
            {
                X = xAngularSpeed / angularSpeedScaler,
                Y = yAngularSpeed / angularSpeedScaler,
                Z = zAngularSpeed / angularSpeedScaler
            };
        }

        protected override void Configure()
        {
            CheckInitialization();

            // Write power-down to 6XL register
            const byte controlRegister6XlAddress = 0x20;
            const byte controlRegister6XlByteValue = 0x00;

            RegisterHelper.WriteByte(device, controlRegister6XlAddress, controlRegister6XlByteValue);

            // Enable gyroscope, and accelerometer
            const byte controlRegister1GAddress = 0x10;
            var controlRegister1GByteValue = InertialSensorHelper.ConfigureControlByte();

            RegisterHelper.WriteByte(device, controlRegister1GAddress, controlRegister1GByteValue);
        }

        private InertialSensor()
        {
            sensorAddress = 0x6A;

            whoAmIRegisterAddress = 0x0F;
            whoAmIDefaultValue = 0x68;
        }
    }
}
