using System;
using Windows.UI.Xaml.Data;

namespace Servo.Converters
{
    public class ObjectToByteConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value;            
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return System.Convert.ToByte(value);
        }
    }
}
