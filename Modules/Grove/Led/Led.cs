using System.Threading;
using GHIElectronics.TinyCLR.Devices.Gpio;
// ReSharper disable UnusedMember.Global

namespace Bauland.Grove
{
    /// <summary>
    /// Wrapper for Led Grove module
    /// </summary>
    public class Led
    {

        private readonly GpioPin _pin;
        private readonly Thread _blinkerThread;
        private int _blinkDelay = 300;
        private void Blinker()
        {
            while (true)
            {
                _pin.Write(GpioPinValue.Low);
                Thread.Sleep(_blinkDelay);
                _pin.Write(GpioPinValue.High);
                Thread.Sleep(_blinkDelay);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        /// <summary>
        /// Constructor of Led
        /// </summary>
        /// <param name="gpioPinNumber">Pin number on which led is connected.</param>
        public Led(int gpioPinNumber)
        {
            _pin = GpioController.GetDefault().OpenPin(gpioPinNumber);
            _pin.Write(GpioPinValue.Low);
            _pin.SetDriveMode(GpioPinDriveMode.Output);
            _blinkerThread = new Thread(Blinker);
            _blinkerThread.Start();
            _blinkerThread.Suspend();
        }

        /// <summary>
        /// Turn the Led on
        /// </summary>
        public void TurnOn()
        {
            lock (_blinkerThread)
            {
                _blinkerThread.Suspend();
                _pin.Write(GpioPinValue.High);
            }
        }

        /// <summary>
        /// Turn the Led off
        /// </summary>
        public void TurnOff()
        {
            lock (_blinkerThread)
            {
                _blinkerThread.Suspend();
                _pin.Write(GpioPinValue.Low);
            }
        }
        /// <summary>
        /// Make the Led blinking
        /// </summary>
        /// <param name="blinkRateSec">Rate of blinking in second.</param>
        public void Blink(double blinkRateSec = 3)
        {
            _blinkDelay = (int)(((1 / blinkRateSec) * 1000) / 2);
            lock (_blinkerThread)
            {
                _blinkerThread.Resume();
            }
        }
    }
}
