using MotorsControl.Enums;
using System;
using Windows.UI.Xaml.Data;

namespace Motors.Converters
{
    public class BooleanToSteppingModeConverter : IValueConverter
    {        
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var isChecked = false;

            if (value != null)
            {
                if (Enum.IsDefined(typeof(SteppingMode), value))
                {
                    var steppingMode = (SteppingMode)value;

                    isChecked = steppingMode == SteppingMode.MicroSteps;
                }
            }

            return isChecked;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            // 기본값
            var steppingMode = SteppingMode.FullSteps;

            try
            {
                var isChecked = System.Convert.ToBoolean(value);

                steppingMode = isChecked ? SteppingMode.MicroSteps : SteppingMode.FullSteps;
            }
            catch (Exception) { };

            return steppingMode;
        }
    }
}
