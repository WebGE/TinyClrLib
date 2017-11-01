using System;
using GHIElectronics.TinyCLR.Devices.Gpio;

namespace Bauland.Gadgeteer
{
    /// <summary>
    /// Manage a Gadgeteer LedStrip module from GHI
    /// </summary>
    public class LedStrip
    {
        private readonly GpioPin[] _pinArray;

        /// <summary>
        /// Ctor of module
        /// </summary>
        /// <param name="pinLed1">pin of gpio pin (usually pin3 on Y socket)</param>
        /// <param name="pinLed2">pin of gpio pin (usually pin4 on Y socket)</param>
        /// <param name="pinLed3">pin of gpio pin (usually pin5 on Y socket)</param>
        /// <param name="pinLed4">pin of gpio pin (usually pin6 on Y socket)</param>
        /// <param name="pinLed5">pin of gpio pin (usually pin7 on Y socket)</param>
        /// <param name="pinLed6">pin of gpio pin (usually pin8 on Y socket)</param>
        /// <param name="pinLed7">pin of gpio pin (usually pin9 on Y socket)</param>
        public LedStrip(int pinLed1, int pinLed2, int pinLed3, int pinLed4, int pinLed5, int pinLed6, int pinLed7)
        {
            GpioController gpio = GpioController.GetDefault();
            _pinArray = new GpioPin[7];
            _pinArray[0] = gpio.OpenPin(pinLed1);
            _pinArray[1] = gpio.OpenPin(pinLed2);
            _pinArray[2] = gpio.OpenPin(pinLed3);
            _pinArray[3] = gpio.OpenPin(pinLed4);
            _pinArray[4] = gpio.OpenPin(pinLed5);
            _pinArray[5] = gpio.OpenPin(pinLed6);
            _pinArray[6] = gpio.OpenPin(pinLed7);
            _pinArray[0].SetDriveMode(GpioPinDriveMode.Output);
            _pinArray[1].SetDriveMode(GpioPinDriveMode.Output);
            _pinArray[2].SetDriveMode(GpioPinDriveMode.Output);
            _pinArray[3].SetDriveMode(GpioPinDriveMode.Output);
            _pinArray[4].SetDriveMode(GpioPinDriveMode.Output);
            _pinArray[5].SetDriveMode(GpioPinDriveMode.Output);
            _pinArray[6].SetDriveMode(GpioPinDriveMode.Output);

        }

        /// <summary>
        /// Set the states of led at a given index
        /// </summary>
        /// <param name="idx">index of led. Must be between 0 and 6.</param>
        /// <param name="state">true if on, false if off.</param>
        public void SetLed(int idx, bool state)
        {
            if ((idx < 0) || (idx > 6))
                throw new ArgumentException("Index must be between 0 and 6 inclusive", nameof(idx));
            _pinArray[idx].Write(state ? GpioPinValue.High : GpioPinValue.Low);
        }

        /// <summary>
        /// Set all led on until index (excluding)
        /// </summary>
        /// <param name="idx">index of last led on</param>
        public void SetToLed(int idx)
        {
            if ((idx < 0) || (idx > 6))
                throw new ArgumentException("Index must be between 0 and 6 inclusive", nameof(idx));
            for (int i = 0; i < 7; i++)
                _pinArray[i].Write((idx < i) ? GpioPinValue.High : GpioPinValue.Low);
        }
    }
}
