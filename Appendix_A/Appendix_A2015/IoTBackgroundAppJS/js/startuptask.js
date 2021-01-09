// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409
(function () {
    "use strict";

    var gpio = Windows.Devices.Gpio;
    var gpioPinNumber = 5;
    var msShineDuration = 1000;

    function configureGpioPin(pinNumber) {
        var gpioController = gpio.GpioController.getDefault();

        var gpioPin = null;

        if (gpioController) {
            gpioPin = gpioController.openPin(pinNumber);

            if (gpioPin) {
                gpioPin.setDriveMode(gpio.GpioPinDriveMode.output);
            }
        }

        return gpioPin;
    }

    function switchGpioPin(gpioPin) {
        var currentPinValue = gpioPin.read();
        var newPinValue = !currentPinValue;

        gpioPin.write(newPinValue);
    }

    function blinkLED(pinNumber, msShineDuration) {
        var ledGpioPin = configureGpioPin(pinNumber);

        if (ledGpioPin) {
            setInterval(function () {
                switchGpioPin(ledGpioPin);
            }, msShineDuration);
        }
    }

    blinkLED(gpioPinNumber, msShineDuration);
})();
