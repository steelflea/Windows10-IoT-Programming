using HelloWorldIoTCS_2019.GpioControl;
using HelloWorldIoTCS_2019.ViewModels;
using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x412에 나와 있습니다.

namespace HelloWorldIoTCS_2019
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private BlinkingViewModel blinkingViewModel = new BlinkingViewModel();
        private LedBlinking ledBlinking;

        public MainPage()
        {
            InitializeComponent();
        }

        private void ButtonInitializeGpio_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ledBlinking = new LedBlinking(blinkingViewModel.GpioPinNumber, blinkingViewModel.MsShineDuration);
                blinkingViewModel.IsGpioPinAvailable = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void ButtonStartBlinking_Click(object sender, RoutedEventArgs e)
        {
            ledBlinking.Start();
            blinkingViewModel.IsBlinkingActive = ledBlinking.IsActive;
        }

        private void ButtonStopBlinking_Click(object sender, RoutedEventArgs e)
        {
            ledBlinking.Stop();
            blinkingViewModel.IsBlinkingActive = ledBlinking.IsActive;
        }
    }
}
