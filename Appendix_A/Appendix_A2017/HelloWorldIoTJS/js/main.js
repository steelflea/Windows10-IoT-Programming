(function () {
    "use strict";

    var app = WinJS.Application;
    var activation = Windows.ApplicationModel.Activation;

    var gpio = Windows.Devices.Gpio;
    var gpioPinNumber = 5;
    var msShineDuration = 1000;

    app.onactivated = function (args) {
        if (args.detail.kind === activation.ActivationKind.launch) {
            if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {

            } else {

            }
            args.setPromise(WinJS.UI.processAll());

            blinkLed(gpioPinNumber, msShineDuration);
        }
    };

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

    function blinkLed(pinNumber, msShineDuration) {
        var ledGpioPin = configureGpioPin(pinNumber);

        if (ledGpioPin) {
            ledGpioPin.write(gpio.GpioPinValue.low);

            setTimeout(function () {
                ledGpioPin.write(gpio.GpioPinValue.high);
            }, msShineDuration);
        }
    }

    app.oncheckpoint = function (args) {

    };

    app.start();
})();
