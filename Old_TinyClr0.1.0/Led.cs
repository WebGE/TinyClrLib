using System;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.Gpio;

namespace GroveModule
{
    public class Led : IDisposable
    {
        readonly GpioPin _ledPin;
        private Thread _thread;
        private bool _bStopThread;
        private int _timeOn, _timeOff;

        public Led(int pin)
        {
            GpioController gpio = GpioController.GetDefault();
            _ledPin = gpio.OpenPin(pin);
            _ledPin.SetDriveMode(GpioPinDriveMode.Output);
        }

        ~Led()
        {
            Dispose(false);
        }
        public GpioPinValue State { get { return _ledPin.Read(); } set { _ledPin.Write(value); } }

        public void Blink(int timeOn = 200, int timeOff = 200)
        {
            _timeOff = timeOff;
            _timeOn = timeOn;
            if (_thread == null)
            {
                _thread = new Thread(BlinkThread);
            }
            _thread.Start();
        }

        private void BlinkThread()
        {
            while (!_bStopThread)
            {
                var ret = _ledPin.Read();
                if (ret == GpioPinValue.High)
                {
                    _ledPin.Write(GpioPinValue.Low);
                    Thread.Sleep(_timeOff);
                }
                else
                {
                    _ledPin.Write(GpioPinValue.High);
                    Thread.Sleep(_timeOn);
                }
            }
        }

        public void StopBlinking()
        {
            _bStopThread = true;
            while (_thread.IsAlive)
            {
                Thread.Sleep(5);
            }
            _ledPin.Write(GpioPinValue.Low);
        }

        private void ReleaseUnmanagedResources()
        {
            StopBlinking();
        }

        private void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
            {
                //_ledPin?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

