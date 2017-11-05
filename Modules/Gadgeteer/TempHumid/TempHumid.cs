using System;
using System.Threading;
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
        private bool _running;
        private int _interval;
        private readonly I2cDevice _device;

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
            _device = controller.GetDevice(new I2cConnectionSettings(0x40)
            {
                BusSpeed = I2cBusSpeed.StandardMode,
                SharingMode = I2cSharingMode.Exclusive
            });

            // Reset device
            Thread.Sleep(80);
            _device.Write(new byte[] { 0xFE });

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
                byte[] readBuffer = new byte[2];
                // Read humidity
                _device.Write(new byte[] { 0xF5 });
                Thread.Sleep(20);
                _device.Read(readBuffer);
                int rawHumidity = readBuffer[0] << 8 | readBuffer[1];
                // Read Temperature
                _device.Write(new byte[] { 0xE0 });
                Thread.Sleep(20);
                _device.Read(readBuffer);
                int rawTemp = readBuffer[0] << 8 | readBuffer[1];

                double temperature = 175.72 * rawTemp / 65536.0 - 46.85;
                double humidity = 125.0 * rawHumidity / 65536.0 - 6.0;

                temperature = Math.Round(100.0 * temperature) / 100.0;
                humidity = Math.Round(100.0 * humidity) / 100.0;

                OnMeasurementComplete(this, new MeasurementCompleteEventArgs(temperature, humidity));

                Thread.Sleep(_interval);
            } while (_running);
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
