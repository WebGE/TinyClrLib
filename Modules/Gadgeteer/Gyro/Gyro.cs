using System;
using GHIElectronics.TinyCLR.Devices.I2c;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable EventNeverSubscribedTo.Global

namespace Bauland.Gadgeteer
{
    /// <summary>
    /// Wrapper class for Gyro Gadgeteer Module
    /// </summary>
    public class Gyro
    {
        private readonly Timer _timer;
        private readonly I2cDevice _i2CDevice;
        private readonly byte[] _readBuffer1;
        private readonly byte[] _writeBuffer1;
        private readonly byte[] _writeBuffer2;
        private readonly byte[] _readBuffer8;
        private double _offsetX;
        private double _offsetY;
        private double _offsetZ;

        private MeasurementCompleteEventHandler _onMeasurementComplete;

        /// <summary>Represents the delegate used for the MeasurementComplete event.</summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        public delegate void MeasurementCompleteEventHandler(Gyro sender, MeasurementCompleteEventArgs e);

        /// <summary>Raised when a measurement reading is complete.</summary>
        public event MeasurementCompleteEventHandler MeasurementComplete;

        /// <summary>
        /// The low pass filter configuration. Note that setting the low pass filter to 256Hz results in a maximum internal sample rate of 8kHz. Any other setting results in a maximum sample rate of
        /// 1kHz. The sample rate can be further divided by using SampleRateDivider.
        /// </summary>
        public Bandwidth LowPassFilter
        {
            get => (Bandwidth)(Read(Register.Scale) & 0x7);
            set => Write(Register.Scale, (byte)((byte)value | 0x18));
        }

        /// <summary>
        /// the internal sample rate divider. The gyro outputs are sampled internally at either 8kHz (if the LowPassFilter is set to 256Hz) or 1kHz for any other LowPassFilter settings. This setting
        /// can be used to further divide the sample rate.
        /// </summary>
        public byte SampleRateDivider
        {
            get => Read(Register.SampleRateDivider);
            set => Write(Register.SampleRateDivider, value);
        }

        /// <summary>The interval at which measurements are taken.</summary>
        public TimeSpan MeasurementInterval
        {
            get => _timer.Interval;
            set
            {
                var wasRunning = _timer.IsRunning;

                _timer.Stop();
                _timer.Interval = value;

                if (wasRunning)
                    _timer.Start();
            }
        }

        /// <summary>Whether or not the driver is currently taking measurements.</summary>
        public bool IsTakingMeasurements => _timer.IsRunning;

        /// <summary>Available low pass filter bandwidth settings.</summary>
        public enum Bandwidth
        {
            /// <summary>256Hz</summary>
            Hertz256 = 0,
            /// <summary>188Hz</summary>
            Hertz188 = 1,
            /// <summary>98Hz</summary>
            Hertz98 = 2,
            /// <summary>42Hz</summary>
            Hertz42 = 3,
            /// <summary>20Hz</summary>
            Hertz20 = 4,
            /// <summary>10Hz</summary>
            Hertz10 = 5,
            /// <summary>5Hz</summary>
            Hertz5 = 6
        }

        private enum Register : byte
        {
            SampleRateDivider = 0x15,
            Scale = 0x16,
            TemperatureOutHigh = 0x1B,
        }

        /// <summary>
        /// Constructor of Gyro
        /// </summary>
        /// <param name="deviceId">string of i2c bus of I Socket</param>
        public Gyro(string deviceId)
        {
            //Socket socket = Socket.GetSocket(socketNumber, true, this, null);
            //socket.EnsureTypeIsSupported('I', this);

            _readBuffer1 = new byte[1];
            _writeBuffer1 = new byte[1];
            _writeBuffer2 = new byte[2];
            _readBuffer8 = new byte[8];

            _offsetX = 0;
            _offsetY = 0;
            _offsetZ = 0;

            _timer = new Timer(TimeSpan.FromMilliseconds(200));
            _timer.Tick += TakeMeasurement;

            // _i2CDevice = GTI.I2CBusFactory.Create(socket, 0x68, 100, this);
            _i2CDevice=I2cDevice.FromId(deviceId,new I2cConnectionSettings(0x68){BusSpeed = I2cBusSpeed.StandardMode});

            SetFullScaleRange();
        }

        /// <summary>Calibrates the gyro values. Ensure that the sensor is not moving when calling this method.</summary>
        public void Calibrate()
        {
            Read(Register.TemperatureOutHigh, _readBuffer8);
            int rawX = (_readBuffer8[2] << 8) | _readBuffer8[3];
            int rawY = (_readBuffer8[4] << 8) | _readBuffer8[5];
            int rawZ = (_readBuffer8[6] << 8) | _readBuffer8[7];

            rawX = (rawX >> 15 == 1 ? -32767 : 0) + (rawX & 0x7FFF);
            rawY = (rawY >> 15 == 1 ? -32767 : 0) + (rawY & 0x7FFF);
            rawZ = (rawZ >> 15 == 1 ? -32767 : 0) + (rawZ & 0x7FFF);

            _offsetX = rawX / -14.375;
            _offsetY = rawY / -14.375;
            _offsetZ = rawZ / -14.375;
        }

        /// <summary>Obtains a single measurement and raises the event when complete.</summary>
        public void RequestSingleMeasurement()
        {
            if (IsTakingMeasurements) throw new InvalidOperationException("You cannot request a single measurement while continuous measurements are being taken.");

            _timer.Behavior = Timer.BehaviorType.RunOnce;
            _timer.Start();
        }

        /// <summary>Starts taking measurements and fires MeasurementComplete when a new measurement is available.</summary>
        public void StartTakingMeasurements()
        {
            _timer.Behavior = Timer.BehaviorType.RunContinuously;
            _timer.Start();
        }

        /// <summary>Stops taking measurements.</summary>
        public void StopTakingMeasurements()
        {
            _timer.Stop();
        }

        private void SetFullScaleRange()
        {
            Write(Register.Scale, (byte)(Read(Register.Scale) | 0x18));
        }

        private byte Read(Register register)
        {
            _writeBuffer1[0] = (byte)register;
            _i2CDevice.WriteRead(_writeBuffer1, _readBuffer1);
            return _readBuffer1[0];
        }

        private void Read(Register register, byte[] readBuffer)
        {
            _writeBuffer1[0] = (byte)register;
            _i2CDevice.WriteRead(_writeBuffer1, readBuffer);
        }

        private void Write(Register register, byte value)
        {
            _writeBuffer2[0] = (byte)register;
            _writeBuffer2[1] = value;
            _i2CDevice.Write(_writeBuffer2);
        }

        private void TakeMeasurement(object sender, EventHandlerArgs args)
        {
            Read(Register.TemperatureOutHigh, _readBuffer8);

            int rawT = (_readBuffer8[0] << 8) | _readBuffer8[1];
            int rawX = (_readBuffer8[2] << 8) | _readBuffer8[3];
            int rawY = (_readBuffer8[4] << 8) | _readBuffer8[5];
            int rawZ = (_readBuffer8[6] << 8) | _readBuffer8[7];

            rawT = (rawT >> 15 == 1 ? -32767 : 0) + (rawT & 0x7FFF);
            rawX = (rawX >> 15 == 1 ? -32767 : 0) + (rawX & 0x7FFF);
            rawY = (rawY >> 15 == 1 ? -32767 : 0) + (rawY & 0x7FFF);
            rawZ = (rawZ >> 15 == 1 ? -32767 : 0) + (rawZ & 0x7FFF);

            double x = rawX / 14.375 + _offsetX;
            double y = rawY / 14.375 + _offsetY;
            double z = rawZ / 14.375 + _offsetZ;
            double t = (rawT + 13200) / 280.0 + 35;

            OnMeasurementComplete(this, new MeasurementCompleteEventArgs(x, y, z, t));
        }

        private void OnMeasurementComplete(Gyro sender, MeasurementCompleteEventArgs e)
        {
            if (_onMeasurementComplete == null)
                _onMeasurementComplete = OnMeasurementComplete;

            MeasurementComplete?.Invoke(sender, e);
        }

        /// <summary>Event arguments for the MeasurementComplete event.</summary>
        public class MeasurementCompleteEventArgs : EventArgs
        {
            /// <summary>Angular rate around the X axis (roll) in degree per second.</summary>
            public double X { get; }

            /// <summary>Angular rate around the Y axis (pitch) in degree per second.</summary>
            public double Y { get; }

            /// <summary>Angular rate around the Z axis (yaw) in degree per second.</summary>
            public double Z { get; }

            /// <summary>Temperature in degree celsius.</summary>
            public double Temperature { get; }

            internal MeasurementCompleteEventArgs(double x, double y, double z, double temperature)
            {
                X = x;
                Y = y;
                Z = z;
                Temperature = temperature;
            }

            /// <summary>Provides a string representation of the instance.</summary>
            /// <returns>A string describing the values contained in the object.</returns>
            public override string ToString()
            {
                return "X: " + X.ToString("f2") + " Y: " + Y.ToString("f2") + " Z: " + Z.ToString("f2") + " Temperature: " + Temperature.ToString("f2");
            }
        }
    }
}
