using SenseHat.UWP.Sensors;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SenseHat.Sensors.ViewModels
{
    public class SensorsViewModel : INotifyPropertyChanged
    {
        #region Events

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #endregion

        #region Public properties

        public SensorReadings SensorReadings { get; set; } = new SensorReadings();

        // Delay time between consecutive sensor readings
        public TimeSpan ReadoutDelay { get; set; } 

        public bool IsConnected
        {
            get { return isConnected; }
            set
            {
                isConnected = value;
                OnPropertyChanged();

                ToggleStartStopButtons(value);
            }
        }

        public bool IsTelemetryActive
        {
            get { return isTelemetryActive; }
            set
            {
                isTelemetryActive = value;
                OnPropertyChanged();

                ToggleStartStopButtons(!value);                
            }
        }

        public bool IsStartSensorReadingButtonEnabled { get; private set; }
        public bool IsStopSensorReadingButtonEnabled { get; private set; }

        #endregion

        #region Fields        

        private bool isConnected;
        private bool isTelemetryActive;

        #endregion

        #region Private methods

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ToggleStartStopButtons(bool isStartEnabled)
        {
            IsStartSensorReadingButtonEnabled = isStartEnabled;
            OnPropertyChanged("IsStartSensorReadingButtonEnabled");

            IsStopSensorReadingButtonEnabled = !isStartEnabled;
            OnPropertyChanged("IsStopSensorReadingButtonEnabled");
        }

        #endregion
    }
}
