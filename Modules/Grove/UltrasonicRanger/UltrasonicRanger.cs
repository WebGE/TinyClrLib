using GHIElectronics.TinyCLR.Devices.Gpio;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Bauland.Grove
{
    /// <summary>
    /// Wrapper class for UltrasonicRanger Grove Module
    /// </summary>
    public class UltrasonicRanger
    {
        private readonly GpioPulseReaderWriter _pulse;

        /// <summary>
        /// Use to correct measurement in linear way:  result = value * A + B (default: 0.81f)
        /// </summary>
        public float A = 0.81f;

        /// <summary>
        /// Use to correct measurement in linear way:  result = value * A + B (default: 2.11f)
        /// </summary>
        public float B = 2.11f;

        /// <summary>
        /// Constructor of HC-SR04
        /// </summary>
        /// <param name="signalPin">pin connected to trigger and echo pin</param>
        public UltrasonicRanger(int signalPin)
        {
            _pulse = new GpioPulseReaderWriter(GpioPulseReaderWriter.Mode.EchoDuration, true, 10, signalPin);
        }

        /// <summary>
        /// Get distance in centimeters
        /// </summary>
        /// <returns>Value return is in centimeters</returns>
        public float ReadCentimeters()
        {
            return (float)(_pulse.Read() * 17 / 1000.0) * A + B;
        }

        /// <summary>
        /// Get distance in inches
        /// </summary>
        /// <returns>Value return is in inches</returns>
        public double ReadInches()
        {
            return ReadCentimeters() / 2.54;
        }

    }
}
