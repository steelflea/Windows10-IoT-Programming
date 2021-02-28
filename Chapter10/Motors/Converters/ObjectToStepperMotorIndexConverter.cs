using MotorsControl.Enums;
using System;
using Windows.UI.Xaml.Data;

namespace Motors.Converters
{
    public class ObjectToStepperMotorIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // 기본값
            var motorIndex = StepperMotorIndex.SM1;

            if(Enum.IsDefined(typeof(StepperMotorIndex), value))
            {
                motorIndex = (StepperMotorIndex)value;
            }

            return motorIndex;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
