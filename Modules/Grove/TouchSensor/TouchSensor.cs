using GHIElectronics.TinyCLR.Devices.Gpio;
// ReSharper disable EventNeverSubscribedTo.Global

// ReSharper disable UnusedMember.Global

namespace Bauland.Grove
{
    /// <summary>
    /// Wrapper of Grove Touchsensor module
    /// </summary>
    public class TouchSensor
    {
        private readonly GpioPin _pin;
        /// <summary>
        /// Constructor of Grove TouchSensor
        /// </summary>
        /// <param name="pinNumber">Gpio pin of board</param>
        public TouchSensor(int pinNumber)
        {
            _pin = GpioController.GetDefault().OpenPin(pinNumber);
            _pin.Write(GpioPinValue.Low);
            _pin.SetDriveMode(GpioPinDriveMode.Input);
            _pin.ValueChanged += Pin_ValueChanged;
        }
        
        /// <summary>
        /// Test if sensor is touched
        /// </summary>
        /// <returns>true if it is touched, else false</returns>
        public bool IsTouched()
        {
            return _pin.Read() == GpioPinValue.High;
        }


        /// <summary>
        /// The signature of sensor events.
        /// </summary>
        public delegate void TouchEventHandler();

        /// <summary>
        /// The event raised when sensor is touched.
        /// </summary>
        public event TouchEventHandler Touched;

        /// <summary>
        /// The event raised when sensor is untouched.
        /// </summary>
        public event TouchEventHandler Untouched;

        private void Pin_ValueChanged(object sender, GpioPinValueChangedEventArgs e)
        {
            if (e.Edge == GpioPinEdge.RisingEdge)
                Touched?.Invoke();
            else
                Untouched?.Invoke();
        }
    }
}
