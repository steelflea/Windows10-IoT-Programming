using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HelloWorldIoTCS_2019.ViewModels
{
    public class BlinkingViewModel : INotifyPropertyChanged
    {
        #region Public properties    
            
        public int GpioPinNumber { get; set; } = 47;
        public int MsShineDuration { get; set; } = 100;

        public bool IsBlinkingActive
        {
            get { return isBlinkingActive; }
            set
            {
                isBlinkingActive = value;
                OnPropertyChanged();

                ToggleStartStopButtons(!value);
            }
        }
        
        public bool IsGpioPinAvailable
        {
            get { return isGpioPinAvailable; }
            set
            {
                isGpioPinAvailable = value;
                OnPropertyChanged();

                ToggleStartStopButtons(value);
            }
        }

        public bool IsStartBlinkingButtonEnabled { get; private set; }
        public bool IsStopBlinkingButtonEnabled { get; private set; }
        
        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #endregion

        #region Fields

        private bool isBlinkingActive;
        private bool isGpioPinAvailable;

        #endregion

        #region Private methods

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ToggleStartStopButtons(bool isStartEnabled)
        {
            IsStartBlinkingButtonEnabled = isStartEnabled;
            OnPropertyChanged("IsStartBlinkingButtonEnabled");

            IsStopBlinkingButtonEnabled = !isStartEnabled;
            OnPropertyChanged("IsStopBlinkingButtonEnabled");
        }

        #endregion
    }
}
