using System;
using GHIElectronics.TinyCLR.Devices.Gpio;

namespace Bauland.Gadgeteer
{
    /// <summary>
    /// 
    /// </summary>
    public class Button
    {
        private readonly GpioPin _pinLed;
        private readonly GpioPin _pinButton;
        private DateTime _time;
        private TimeSpan _longPressTimeout;

        /// <summary>
        /// Event fired when button is pressed
        /// </summary>
        public event ButtonEventHandler Pressed;

        /// <summary>
        /// Event fired when button is released
        /// </summary>
        public event ButtonEventHandler Released;

        /// <summary>
        /// Event fired when button is pressed more than LongPressTimeout
        /// </summary>
        public event ButtonEventHandler LongPressed;

        /// <summary>
        /// Time that must be waited before LongPress event is fired
        /// </summary>
        public TimeSpan LongPressTimeout
        {
            get => _longPressTimeout;
            set
            {
               if(value<TimeSpan.FromMilliseconds(20)) throw new ArgumentOutOfRangeException(nameof(value),"LongPressTimeout can't be less than 20ms.");
                _longPressTimeout = value;
            }
        }

        /// <summary>
        /// Constructor of Button
        /// </summary>
        /// <param name="pinButton">pin connected on button</param>
        /// <param name="pinLed">pin connected to led</param>
        public Button(int pinButton, int pinLed)
        {
            LongPressTimeout=TimeSpan.FromSeconds(2);
            _pinLed = GpioController.GetDefault().OpenPin(pinLed);
            _pinLed.SetDriveMode(GpioPinDriveMode.Output);
            _pinLed.Write(GpioPinValue.Low);

            _pinButton = GpioController.GetDefault().OpenPin(pinButton);
            _pinButton.SetDriveMode(GpioPinDriveMode.InputPullUp);
            _pinButton.ValueChanged += _pinButton_ValueChanged;

        }

        /// <summary>
        /// Use to set or read led state
        /// </summary>
        public bool Led
        {
            get => _pinLed.Read() == GpioPinValue.High;
            set => _pinLed.Write(value?GpioPinValue.High:GpioPinValue.Low);
        }

        private void _pinButton_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e)
        {
            if (e.Edge == GpioPinEdge.FallingEdge)
            {
                Pressed?.Invoke(this, new ButtonEventHandlerArgs() {Time = DateTime.Now});
                _time = DateTime.Now;
            }
            else
            {
                Released?.Invoke(this, new ButtonEventHandlerArgs() {Time = DateTime.Now});
                if(DateTime.Now-_time>_longPressTimeout)
                    LongPressed?.Invoke(this,new ButtonEventHandlerArgs(){Time = DateTime.Now});
            }
        }
    }

    public delegate void ButtonEventHandler(object sender, ButtonEventHandlerArgs args);

    public class ButtonEventHandlerArgs
    {
        public DateTime Time { get; set; }
    }
}
