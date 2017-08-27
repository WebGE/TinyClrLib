using GHIElectronics.TinyCLR.Devices.Gpio;

namespace TinyClrCore
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
            _buttonPin.SetDriveMode(GpioPinDriveMode.Input);
            _buttonPin.ValueChanged += _buttonPin_ValueChanged;
        }

        private void _buttonPin_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e)
        {
            if (e.Edge == GpioPinEdge.FallingEdge)
            {
                OnPress?.Invoke(sender, e);
            }
            else
            {
                OnRelease?.Invoke(sender, e);
            }
        }

        public bool State => _buttonPin.Read() == GpioPinValue.High;

        //private void _buttonPin_ValueChanged(object sender, GpioPinValueChangedEventArgs e)
        //{
        //    if (e.Edge == GpioPinEdge.FallingEdge)
        //    {
        //        OnPress?.Invoke(sender, e);
        //    }
        //    else
        //    {
        //        OnRelease?.Invoke(sender, e);
        //    }
        //}
    }
}
