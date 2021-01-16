using SenseHat.Portable.Helpers;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SenseHat.UWP.Sensors
{
    public class SensorReadings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public float Temperature
        {
            get { return temperature; }
            set
            {
                temperature = value;
                OnPropertyChanged();
            }
        }

        public float Pressure
        {
            get { return pressure; }
            set
            {
                pressure = value;
                OnPropertyChanged();
            }
        }

        public float Humidity
        {
            get { return humidity; }
            set
            {
                humidity = value;
                OnPropertyChanged();
            }
        }

        public Vector3D<float> Accelerometer
        {
            get { return accelerometer; }
            set
            {
                accelerometer = value;
                OnPropertyChanged();
            }
        }

        public Vector3D<float> Gyroscope
        {
            get { return gyroscope; }
            set
            {
                gyroscope = value;
                OnPropertyChanged();
            }
        }

        public Vector3D<float> Magnetometer
        {
            get { return magnetometer; }
            set
            {
                magnetometer = value;
                OnPropertyChanged();
            }
        }

        private float temperature;
        private float pressure;
        private float humidity;
        private Vector3D<float> accelerometer;
        private Vector3D<float> gyroscope;
        private Vector3D<float> magnetometer;
    }
}
