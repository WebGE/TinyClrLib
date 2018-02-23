using System;
using GHIElectronics.TinyCLR.Devices.Pwm;
// ReSharper disable UnusedMember.Global

namespace Bauland.Grove
{
    /// <summary>
    /// Wrapper for Grove ServoMotor module
    /// </summary>
    public class ServoMotor
    {
        private readonly PwmPin _servo;
        /// <summary>
        /// Constructor of Grove ServoMotor module
        /// </summary>
        /// <param name="controller">Id of pwm controller</param>
        /// <param name="pwmPinNumber">Pwm pin number of board</param>
        public ServoMotor(string controller, int pwmPinNumber)
        {
            PwmController pwm = PwmController.FromId(controller);

            _servo = pwm.OpenPin(pwmPinNumber);
            pwm.SetDesiredFrequency(1 / 0.020);
        }

        /// <summary>
        /// Get or set minimum value for pulse
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Raised if value is not between 0.1 and 1.5ms</exception>
        public double MinPulseCalibration
        {
            set
            {
                if (value > 1.5 || value < 0.1)
                    throw new ArgumentOutOfRangeException(nameof(value),"Must be between 0.1 and 1.5ms");
                _minPulseCalibration = value;
            }
        }

        /// <summary>
        /// Get or set maximum value for pulse
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Raised if value is not between 1.6 and 3ms</exception>
        public double MaxPulseCalibration
        {
            set
            {
                if (value > 3 || value < 1.6)
                    throw new ArgumentOutOfRangeException(nameof(value),"Must be between 1.6 and 3ms");
                _maxPulseCalibration = value;
            }
        }
        // min and max pulse width in milliseconds
        private double _minPulseCalibration = 1.0;
        private double _maxPulseCalibration = 2.0;

        /// <summary>
        /// Sets the position of the Servo Motor.
        /// </summary>
        /// <param name="position">The position of the servo between 0 and 180 degrees.</param>
        public void SetPosition(double position)
        {
            if (position < 0 || position > 180) throw new ArgumentOutOfRangeException(nameof(position), "degrees must be between 0 and 180.");

            // Typically, with 50 hz, 0 degree is 0.05 and 180 degrees is 0.10
            //double duty = ((position / 180.0) * (0.10 - 0.05)) + 0.05;
            double duty = ((position / 180.0) * (_maxPulseCalibration / 20 - _minPulseCalibration / 20)) + _minPulseCalibration / 20;


            _servo.SetActiveDutyCyclePercentage(duty);
            _servo.Start();
        }
    }
}
