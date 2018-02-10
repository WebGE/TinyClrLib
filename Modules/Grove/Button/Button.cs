using GHIElectronics.TinyCLR.Devices.Gpio;
// ReSharper disable EventNeverSubscribedTo.Global
// ReSharper disable UnusedMember.Global

namespace Bauland.Grove
{
    /// <summary>
    /// Wrapper for Grove Button
    /// </summary>
    public class Button
    {
        private readonly GpioPin _pin;

        /// <summary>
        /// Constructor of Button object
        /// </summary>
        /// <param name="gpioPinNumber">pin number on which button is connected</param>
        public Button(int gpioPinNumber)
        {
            _pin = GpioController.GetDefault().OpenPin(gpioPinNumber);
            _pin.Write(GpioPinValue.Low);
            _pin.SetDriveMode(GpioPinDriveMode.Input);
            _pin.ValueChanged += Pin_ValueChanged;
        }

        /// <summary>
        /// Return state of button
        /// </summary>
        /// <returns>True if button is pressed, else False</returns>
        public bool IsPressed()
        {
            return _pin.Read() == GpioPinValue.High;
        }
        /// <summary>
        /// The signature of button events.
        /// </summary>
        public delegate void ButtonEventHandler();

        /// <summary>
        /// The event raised when a button is released.
        /// </summary>
        public event ButtonEventHandler ButtonReleased;
        /// <summary>
        /// The event raised when a button is pressed.
        /// </summary>
        public event ButtonEventHandler ButtonPressed;
        private void Pin_ValueChanged(object sender, GpioPinValueChangedEventArgs e)
        {
            if (e.Edge == GpioPinEdge.RisingEdge)
                ButtonPressed?.Invoke();
            else
                ButtonReleased?.Invoke();
        }

    }
}
