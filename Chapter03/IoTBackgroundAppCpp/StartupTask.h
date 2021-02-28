#pragma once
#include "pch.h"

using namespace Windows::Devices::Gpio;

namespace IoTBackgroundAppCpp
{
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class StartupTask sealed : public Windows::ApplicationModel::Background::IBackgroundTask
    {
    public:
        virtual void Run(Windows::ApplicationModel::Background::IBackgroundTaskInstance^ taskInstance);

    private:
        const int pinNumber = 5;
        const int msShineDuration = 2000;

        GpioPin^ ConfigureGpioPin(int pinNumber);
        void BlinkLed(int ledPinNumber, int msShineDuration);
        void SwitchGpioPin(GpioPin^ gpioPin);
        GpioPinValue InvertGpioPinValue(GpioPinValue currentPinValue);
    };
}
