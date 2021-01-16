using SenseHat.Portable.Helpers;
using System;
using Windows.UI.Xaml.Data;

namespace SenseHat.UWP.Converters
{
    public class HumidityToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string result = Constants.UnavailableValue;

            try
            {
                float humidity = System.Convert.ToSingle(value);

                result = string.Format("{0,4:F2} %", humidity);
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
