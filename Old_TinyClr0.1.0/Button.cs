using GHIElectronics.TinyCLR.Devices.Gpio;

namespace GroveModule
{
    public class Button
    {
        private readonly GpioPin _buttonPin;
        public event GpioPinValueChangedEventHandler OnPress;
        public event GpioPinValueChangedEventHandler OnRelease;

        public Button(int pinNumber)
        {
            GpioController gpio = GpioController.GetDefault();
            _buttonPin = gpio.OpenPin(pinNumber, GpioSharingMode.Exclusive);
            _buttonPin.SetDriveMode(GpioPinDriveMode.InputPullDown);
            _buttonPin.ValueChanged += _buttonPin_ValueChanged;

        }

        private void _buttonPin_ValueChanged(object sender, GpioPinValueChangedEventArgs e)
        {
            if (e.Edge == GpioPinEdge.FallingEdge)
            {
                OnRelease?.Invoke(sender, e);
            }
            else
            {
                OnPress?.Invoke(sender, e);
            }
        }
    }
}
