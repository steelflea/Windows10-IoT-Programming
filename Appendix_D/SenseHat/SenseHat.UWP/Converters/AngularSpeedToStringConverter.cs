using SenseHat.Portable.Helpers;
using System;
using Windows.UI.Xaml.Data;

namespace SenseHat.UWP.Converters
{
    public class AngularSpeedToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string result = Constants.UnavailableValue;

            try
            {
                var angularSpeed = ConversionHelper.GetVector3D(value);

                result = string.Format("R: {0,7:F2} dps\nP: {1,7:F2} dps \nY: {2,7:F2} dps",
                    angularSpeed.X, angularSpeed.Y, angularSpeed.Z);
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
