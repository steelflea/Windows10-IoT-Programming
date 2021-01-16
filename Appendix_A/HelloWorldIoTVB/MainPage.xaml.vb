Imports Windows.Devices.Gpio

Public NotInheritable Class MainPage
    Inherits Page

    Private Const gpioPinNumber = 5
    Private Const msShineDuration = 4000

    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        MyBase.OnNavigatedTo(e)

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
        Dim pin = ConfigureGpioPin(gpioPinNumber)

        If pin IsNot Nothing Then
            pin.Write(GpioPinValue.Low)

            Task.Delay(msShineDuration).Wait()

            pin.Write(GpioPinValue.High)
        End If
    End Sub
End Class
