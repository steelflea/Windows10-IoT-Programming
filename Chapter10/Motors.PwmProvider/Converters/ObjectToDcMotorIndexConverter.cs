using MotorsControl.Enums;
using System;
using Windows.UI.Xaml.Data;

namespace Motors.PwmProvider.Converters
{
    public class ObjectToDcMotorIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // 기본값
            var motorIndex = DcMotorIndex.DC1;

            if(Enum.IsDefined(typeof(DcMotorIndex), value))
            {
                motorIndex = (DcMotorIndex)value;
            }

            return motorIndex;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
