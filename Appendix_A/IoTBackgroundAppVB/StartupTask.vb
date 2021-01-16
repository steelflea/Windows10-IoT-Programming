Imports Windows.ApplicationModel.Background
Imports Windows.Devices.Gpio

Public NotInheritable Class StartupTask
    Implements IBackgroundTask

    Private Const gpioPinNumber = 5
    Private Const msShineDuration = 4000

    Public Sub Run(taskInstance As IBackgroundTaskInstance) Implements IBackgroundTask.Run
        BlinkLed(gpioPinNumber, msShineDuration)
    End Sub

    Private Function ConfigureGpioPin(pinNumber As Integer) As GpioPin
        Dim gpioControl = GpioController.GetDefault()
        Dim pin As GpioPin = Nothing

        If gpioControl IsNot Nothing Then
            pin = gpioControl.OpenPin(pinNumber)

            If pin IsNot Nothing Then
                pin.SetDriveMode(GpioPinDriveMode.Output)
            End If
        End If

        Return pin
    End Function

    Private Sub BlinkLed(gpioPinNumber As Integer, msShineDuration As Integer)
        Dim ledGpioPin = ConfigureGpioPin(gpioPinNumber)

        If ledGpioPin IsNot Nothing Then
            While True
                SwitchGpioPin(ledGpioPin)
                Task.Delay(msShineDuration).Wait()
            End While
        End If
    End Sub

    Private Sub SwitchGpioPin(gpioPin As GpioPin)
        Dim currentPinValue = gpioPin.Read()
        Dim newPinValue = InvertGpioPinValue(currentPinValue)

        gpioPin.Write(newPinValue)
    End Sub

    Private Function InvertGpioPinValue(currentPinValue As GpioPinValue) As GpioPinValue
        Dim invertedGpioPinValue As GpioPinValue

        If currentPinValue = GpioPinValue.High Then
            invertedGpioPinValue = GpioPinValue.Low
        Else
            invertedGpioPinValue = GpioPinValue.High
        End If

        Return invertedGpioPinValue
    End Function
End Class
