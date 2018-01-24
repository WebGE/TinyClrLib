using System;
using System.Diagnostics;
using GHIElectronics.TinyCLR.Devices.Gpio;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable EventNeverSubscribedTo.Global

namespace Bauland.Gadgeteer
{
    /// <summary>
    /// Wrapper class for Button Gadgeteer Module
    /// </summary>
    public class Button
    {
        private readonly GpioPin _pinLed;
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
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
        /// Time that must be waited before LongPress event is fired (default is 2 seconds).
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
            _pinButton.DebounceTimeout=TimeSpan.FromMilliseconds(100);
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

    /// <summary>
    /// Delegate of event button
    /// </summary>
    /// <param name="sender">Reference of component which raises the event</param>
    /// <param name="args">Args of event</param>
    public delegate void ButtonEventHandler(object sender, ButtonEventHandlerArgs args);

    /// <summary>
    /// Contains arguments of event
    /// </summary>
    public class ButtonEventHandlerArgs
    {
        /// <summary>
        /// Contains time at which event occurs
        /// </summary>
        public DateTime Time { get; set; }
    }
}
