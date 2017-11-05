using System;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.I2c;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
// ReSharper disable UnusedMember.Global
// ReSharper disable EventNeverSubscribedTo.Global

namespace Bauland.Gadgeteer
{
    /// <summary>
    /// Wrapper class for TempHumid Gadgeteer Module
    /// </summary>
	public class TempHumidity
    {
        private Thread _timer;
        private readonly GpioPin _data;
        private readonly GpioPin _sck;
        private bool _running;
        private int _interval;

        private MeasurementCompleteEventHandler _onMeasurementComplete;

        /// <summary>Represents the delegate used for the MeasurementComplete event.</summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        public delegate void MeasurementCompleteEventHandler(TempHumidity sender, MeasurementCompleteEventArgs e);

        /// <summary>Raised when a measurement reading is complete.</summary>
        public event MeasurementCompleteEventHandler MeasurementComplete;

        /// <summary>The interval in milliseconds at which measurements are taken.</summary>
        public int MeasurementInterval
        {
            get => _interval;

            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value), "value must be positive.");

                _interval = value;
            }
        }


        /// <summary>Whether or not the driver is currently taking measurements.</summary>
        public bool IsTakingMeasurements => _running || (_timer != null && _timer.IsAlive);

        /// <summary>
        /// Constructor of TempHumidity
        /// </summary>
        /// <param name="sckPin">Usually pin 4 of X or Y Socket</param>
        /// <param name="dataPin">Usually pin 5 of X or Y Socket</param>
        public TempHumidity(int sckPin, int dataPin)
        {
            // Software I2C
            var softwareProvider = new I2cSoftwareProvider(dataPin, sckPin);
            var controllers = I2cController.GetControllers(softwareProvider);
            var controller = controllers[0];
            var device = controller.GetDevice(new I2cConnectionSettings( /* TODO */0x00)
            {
                BusSpeed = I2cBusSpeed.StandardMode,
                SharingMode = I2cSharingMode.Exclusive
            });
            _sck = GpioController.GetDefault().OpenPin(sckPin);
            _sck.SetDriveMode(GpioPinDriveMode.Output);
            // GTI.DigitalOutputFactory.Create(socket, Socket.Pin.Four, false, this);
            _data = GpioController.GetDefault().OpenPin(dataPin);
            _data.SetDriveMode(GpioPinDriveMode.Output);
            // GTI.DigitalIOFactory.Create(socket, Socket.Pin.Three, true, GTI.GlitchFilterMode.Off, GTI.ResistorMode.Disabled, this);
            _running = false;
        }

        /// <summary>Obtains a single measurement and raises the event when complete.</summary>
        public void RequestSingleMeasurement()
        {
            if (IsTakingMeasurements) throw new InvalidOperationException("You cannot request a single measurement while continuous measurements are being taken.");

            _running = false;
            _timer = new Thread(TakeMeasurement);
            _timer.Start();
        }

        /// <summary>Starts taking measurements and fires MeasurementComplete when a new measurement is available.</summary>
        public void StartTakingMeasurements()
        {
            _running = true;
            _timer = new Thread(TakeMeasurement);
            _timer.Start();
        }

        /// <summary>Stops taking measurements.</summary>
        public void StopTakingMeasurements()
        {
            _running = false;
            _timer.Join();
        }

        private void TakeMeasurement()
        {
            do
            {
                ResetCommuncation();

                TransmissionStart();

                double temperature = -39.65 + 0.01 * MeasureTemperature();

                TransmissionStart();

                int rawHumidity = MeasureHumidity();
                double humidity = -2.0468 + 0.0367 * rawHumidity - 1.5955E-6 * rawHumidity * rawHumidity;
                humidity = (temperature - 25) * (0.01 + 0.00008 * rawHumidity) + humidity;

                temperature = Math.Round(100.0 * temperature) / 100.0;
                humidity = Math.Round(100.0 * humidity) / 100.0;

                OnMeasurementComplete(this, new MeasurementCompleteEventArgs(temperature, humidity));

                Thread.Sleep(_interval);
            } while (_running);
        }

        private void TransmissionStart()
        {
            _data.Write(GpioPinValue.High);
            _sck.Write(GpioPinValue.High);
            _data.Write(GpioPinValue.Low);
            _sck.Write(GpioPinValue.Low);
            _sck.Write(GpioPinValue.High);
            _data.Write(GpioPinValue.High);
            _sck.Write(GpioPinValue.Low);
        }

        private int MeasureTemperature()
        {
            _data.Write(GpioPinValue.Low);

            _sck.Write(GpioPinValue.High);
            _sck.Write(GpioPinValue.Low);

            _sck.Write(GpioPinValue.High);
            _sck.Write(GpioPinValue.Low);

            _sck.Write(GpioPinValue.High);
            _sck.Write(GpioPinValue.Low);

            _sck.Write(GpioPinValue.High);
            _sck.Write(GpioPinValue.Low);

            _sck.Write(GpioPinValue.High);
            _sck.Write(GpioPinValue.Low);

            _sck.Write(GpioPinValue.High);
            _sck.Write(GpioPinValue.Low);

            _data.Write(GpioPinValue.High);

            _sck.Write(GpioPinValue.High);
            _sck.Write(GpioPinValue.Low);

            _data.Write(GpioPinValue.Low);

            _data.Write(GpioPinValue.High);

            _sck.Write(GpioPinValue.High);
            _sck.Write(GpioPinValue.Low);

            _sck.Write(GpioPinValue.High);

            _data.Read();

            _sck.Write(GpioPinValue.Low);

            //while (_data.Read() == GpioPinValue.High)
            //    Thread.Sleep(1);

            int reading = 0;

            for (int i = 0; i < 8; i++)
            {
                reading |= _data.Read() == GpioPinValue.High ? (1 << (15 - i)) : 0;
                _sck.Write(GpioPinValue.High);
                _sck.Write(GpioPinValue.Low);
            }

            _data.Write(GpioPinValue.Low);

            _sck.Write(GpioPinValue.High);
            _sck.Write(GpioPinValue.Low);

            for (int i = 8; i < 16; i++)
            {
                reading |= _data.Read() == GpioPinValue.High ? (1 << (15 - i)) : 0;
                _sck.Write(GpioPinValue.High);
                _sck.Write(GpioPinValue.Low);
            }

            _data.Write(GpioPinValue.High);

            return reading;
        }

        private int MeasureHumidity()
        {
            _data.Write(GpioPinValue.Low);

            _sck.Write(GpioPinValue.High);
            _sck.Write(GpioPinValue.Low);

            _sck.Write(GpioPinValue.High);
            _sck.Write(GpioPinValue.Low);

            _sck.Write(GpioPinValue.High);
            _sck.Write(GpioPinValue.Low);

            _sck.Write(GpioPinValue.High);
            _sck.Write(GpioPinValue.Low);

            _sck.Write(GpioPinValue.High);
            _sck.Write(GpioPinValue.Low);

            _data.Write(GpioPinValue.High);

            _sck.Write(GpioPinValue.High);
            _sck.Write(GpioPinValue.Low);

            _data.Write(GpioPinValue.Low);

            _sck.Write(GpioPinValue.High);
            _sck.Write(GpioPinValue.Low);

            _data.Write(GpioPinValue.High);

            _sck.Write(GpioPinValue.High);
            _sck.Write(GpioPinValue.Low);

            _sck.Write(GpioPinValue.High);

            _data.Read();

            _sck.Write(GpioPinValue.Low);

            //while (_data.Read() == GpioPinValue.High)
            //    Thread.Sleep(1);

            int reading = 0;
            for (int i = 0; i < 8; i++)
            {
                reading |= _data.Read() == GpioPinValue.High ? (1 << (15 - i)) : 0;
                _sck.Write(GpioPinValue.High);
                _sck.Write(GpioPinValue.Low);
            }

            _data.Write(GpioPinValue.Low);

            _sck.Write(GpioPinValue.High);
            _sck.Write(GpioPinValue.Low);

            for (int i = 8; i < 16; i++)
            {
                reading |= _data.Read() == GpioPinValue.High ? (1 << (15 - i)) : 0;
                _sck.Write(GpioPinValue.High);
                _sck.Write(GpioPinValue.Low);
            }

            _data.Write(GpioPinValue.High);

            return reading;
        }

        private void ResetCommuncation()
        {
            _data.Write(GpioPinValue.High);

            for (int i = 0; i < 9; i++)
            {
                _sck.Write(GpioPinValue.High);
                _sck.Write(GpioPinValue.Low);
            }
        }

        private void OnMeasurementComplete(TempHumidity sender, MeasurementCompleteEventArgs e)
        {
            if (_onMeasurementComplete == null)
                _onMeasurementComplete = OnMeasurementComplete;

            MeasurementComplete?.Invoke(sender, e);
        }
        /// <summary>Event arguments for the MeasurementComplete event.</summary>
        public class MeasurementCompleteEventArgs : EventArgs
        {

            /// <summary>The measured temperature in degrees Celsius.</summary>
            public double Temperature { get; private set; }

            /// <summary>The measured relative humidity.</summary>
            public double RelativeHumidity { get; private set; }

            internal MeasurementCompleteEventArgs(double temperature, double relativeHumidity)
            {
                Temperature = temperature;
                RelativeHumidity = relativeHumidity;
            }

            /// <summary>Provides a string representation of the instance.</summary>
            /// <returns>A string describing the values contained in the object.</returns>
            public override string ToString()
            {
                return "Temperature: " + Temperature.ToString("f2") + " degrees Celsius. Relative humidity: " + RelativeHumidity.ToString("f2") + ".";
            }
        }
    }
}
