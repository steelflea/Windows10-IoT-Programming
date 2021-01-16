using SenseHat.Portable.Helpers;
using System;
using Windows.UI.Xaml.Data;

namespace SenseHat.UWP.Converters
{
    public class LinearAccelerationToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {            
            string result = Constants.UnavailableValue;

            try
            {
                var linearAcceleration = ConversionHelper.GetVector3D(value);

                result = string.Format("X: {0,5:F2} g\nY: {1,5:F2} g \nZ: {2,5:F2} g",
                    linearAcceleration.X, linearAcceleration.Y, linearAcceleration.Z);
            }
            catch (Exception) { }

            return result;            
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
