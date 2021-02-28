#include "pch.h"
#include "StartupTask.h"

using namespace IoTBackgroundAppCpp;
using namespace Platform;
using namespace Windows::ApplicationModel::Background;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

void StartupTask::Run(IBackgroundTaskInstance^ taskInstance)
{
    // 
    // TODO: Insert code to perform background work
    //
    // If you start any asynchronous methods here, prevent the task
    // from closing prematurely by using BackgroundTaskDeferral as
    // described in http://aka.ms/backgroundtaskdeferral
    //

	BlinkLed(pinNumber, msShineDuration);
}

GpioPin^ StartupTask::ConfigureGpioPin(int pinNumber)
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

void StartupTask::BlinkLed(int ledPinNumber, int msShineDuration)
{
	GpioPin^ ledGpioPin = ConfigureGpioPin(ledPinNumber);

	if (ledGpioPin != nullptr)
	{
		while (true)
		{
			SwitchGpioPin(ledGpioPin);

			Sleep(msShineDuration);
		}
	}
}

void StartupTask::SwitchGpioPin(GpioPin^ gpioPin)
{
	auto currentPinValue = gpioPin->Read();

	GpioPinValue newPinValue = InvertGpioPinValue(currentPinValue);

	gpioPin->Write(newPinValue);
}

GpioPinValue StartupTask::InvertGpioPinValue(GpioPinValue currentPinValue)
{
	GpioPinValue invertedGpioPinValue;

	if (currentPinValue == GpioPinValue::High)
	{
		invertedGpioPinValue = GpioPinValue::Low;
	}
	else
	{
		invertedGpioPinValue = GpioPinValue::High;
	}

	return invertedGpioPinValue;
}
