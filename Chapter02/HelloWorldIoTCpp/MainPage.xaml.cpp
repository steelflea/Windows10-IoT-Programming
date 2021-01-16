//
// MainPage.xaml.cpp
// MainPage 클래스의 구현입니다.
//

#include "pch.h"
#include "MainPage.xaml.h"

using namespace HelloWorldIoTCpp;
using namespace Platform;


// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x412에 나와 있습니다.

MainPage::MainPage()
{
	InitializeComponent();
}

void MainPage::OnNavigatedTo(NavigationEventArgs^ e)
{
	__super::OnNavigatedTo(e);

	BlinkLed(pinNumber, msShineDuration);
}

GpioPin^ MainPage::ConfigureGpioPin(int pinNumber)
{
	auto gpioController = GpioController::GetDefault();

	GpioPin^ pin = nullptr;

	if (gpioController != nullptr)
	{
		pin = gpioController->OpenPin(pinNumber);

		if (pin != nullptr)
		{
			pin->SetDriveMode(GpioPinDriveMode::Output);
		}
	}

	return pin;
}

void MainPage::BlinkLed(int ledPinNumber, int msShineDuration)
{
	GpioPin^ ledGpioPin = ConfigureGpioPin(ledPinNumber);

	if (ledGpioPin != nullptr)
	{
		ledGpioPin->Write(GpioPinValue::Low);

		Sleep(msShineDuration);

		ledGpioPin->Write(GpioPinValue::High);
	}
}