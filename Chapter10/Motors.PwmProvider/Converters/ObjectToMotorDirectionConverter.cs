using MotorsControl.Enums;
using System;
using Windows.UI.Xaml.Data;

namespace Motors.PwmProvider.Converters
{
    public class ObjectToMotorDirectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // 기본값
            var direction = MotorDirection.Forward;

            if (Enum.IsDefined(typeof(MotorDirection), value))
            {
                direction = (MotorDirection)value;
            }

            return direction;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
