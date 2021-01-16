//
// MainPage.xaml.h
// MainPage 클래스의 선언입니다.
//

#pragma once

#include "MainPage.g.h"

using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Devices::Gpio;

namespace HelloWorldIoTCpp
{
	/// <summary>
	/// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
	/// </summary>
	public ref class MainPage sealed
	{
	public:
		MainPage();

	protected:
		void OnNavigatedTo(NavigationEventArgs^ e) override;

	private:
		const int pinNumber = 5;
		const int msShineDuration = 2000;

		GpioPin^ ConfigureGpioPin(int pinNumber);
		void BlinkLed(int ledPinNumber, int msShineDuration);
	};
}
