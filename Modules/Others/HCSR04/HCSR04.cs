using GHIElectronics.TinyCLR.Devices.Gpio;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Bauland.Other
{
    /// <summary>
    /// Wrapper class for HC-SR04 Module
    /// </summary>
    public class Hcsr04
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
        /// <param name="triggerPin">pin connected to trigger pin</param>
        /// <param name="echoPin">pin connected to echo pin</param>
        public Hcsr04(int triggerPin, int echoPin)
        {
            _pulse = new GpioPulseReaderWriter(GpioPulseReaderWriter.Mode.EchoDuration, true, 10, triggerPin, true, echoPin);
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
