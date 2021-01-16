using SenseHat.Portable.Helpers;
using System;
using Windows.UI.Xaml.Data;

namespace SenseHat.UWP.Converters
{
    public class MagneticFieldToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string result = Constants.UnavailableValue;

            try
            {
                var magneticField = ConversionHelper.GetVector3D(value);

                result = string.Format("X: {0,5:F2} G\nY: {1,5:F2} G \nZ: {2,5:F2} G",
                    magneticField.X, magneticField.Y, magneticField.Z);
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
