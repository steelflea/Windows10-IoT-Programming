using System;

namespace Motors.PwmProvider.Exceptions
{
    public class DeviceInitializationException : Exception
    {        
        public static DeviceInitializationException Default(byte address)
        {
            var message = $"I2cDevice of address: {address} could not be initialized";

            return new DeviceInitializationException(message);
        }

        public DeviceInitializationException(string message) : base(message)
        {
        }
    }
}
