using System;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Pwm;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable UnusedMember.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Bauland.Gadgeteer
{
    /// <summary>
    /// Wrapper class for MotorDriverL298 Gadgeteer Module
    /// </summary>
    public class MotorDriverL298
    {
        private const int StepFactor = 250;

        private readonly GpioPin[] _directions;
        private readonly PwmPin[] _pwms;
        private readonly PwmController[] _controllers;

        private readonly double[] _lastSpeeds;

        /// <summary>Used to set the PWM frequency for the motors because some motors require a certain frequency in order to operate properly. It defaults to 25KHz (25000).</summary>
        public int Frequency { get; set; }

        /// <summary>The possible motors.</summary>
        public enum Motor
        {

            /// <summary>The motor marked M1.</summary>
            Motor1 = 0,

            /// <summary>The motor marked M2.</summary>
            Motor2 = 1,
        }

        /// <summary>
        /// Constructor of MotorDriverL298
        /// </summary>
        /// <param name="pinDir1">pin of direction for Motor 1, pin 6 of P Socket</param>
        /// <param name="pinDir2">pin of direction for Motor 2, pin 7 of P Socket</param>
        /// <param name="pinPwm1">pin of Pwm for Motor 1, pin 8 of P Socket</param>
        /// <param name="pinPwm2">pin of Pwm for Motor 2, pin 9 of P Socket</param>
        /// <param name="idPwmController1">id of Pwm for Motor 1</param>
        /// <param name="idPwmController2">id of Pwm for Motor 2</param>
        public MotorDriverL298(int pinDir1, int pinDir2, int pinPwm1, int pinPwm2, string idPwmController1, string idPwmController2)
        {
            _lastSpeeds = new double[] { 0, 0 };

            Frequency = 25000;

            _controllers = new[]
             {
                PwmController.FromId(idPwmController1),
                PwmController.FromId(idPwmController2),
            };
            foreach (var controller in _controllers)
                controller.SetDesiredFrequency(Frequency);

            _pwms = new[]
            {
                _controllers[0].OpenPin(pinPwm1),
                _controllers[1].OpenPin(pinPwm2),
            };
            foreach (var pwmPin in _pwms)
                pwmPin.Start();

            _directions = new[]
            {
                GpioController.GetDefault().OpenPin(pinDir1,GpioSharingMode.Exclusive),
                GpioController.GetDefault().OpenPin(pinDir2,GpioSharingMode.Exclusive),
            };
            foreach (var dir in _directions)
            {
                dir.SetDriveMode(GpioPinDriveMode.Output);
                dir.Write(GpioPinValue.Low);
            }

            StopAll();
        }

        /// <summary>Stops all motors.</summary>
        public void StopAll()
        {
            SetSpeed(Motor.Motor1, 0);
            SetSpeed(Motor.Motor2, 0);
        }

        /// <summary>Sets the given motor's speed.</summary>
        /// <param name="motor">The motor to set the speed for.</param>
        /// <param name="speed">The desired speed of the motor between -1 and 1.</param>
        public void SetSpeed(Motor motor, double speed)
        {
            if (speed > 1 || speed < -1) throw new ArgumentOutOfRangeException(nameof(speed), "speed must be between -1 and 1.");
            if (motor != Motor.Motor1 && motor != Motor.Motor2) throw new ArgumentException("You must specify a valid motor.", nameof(motor));

            if (speed == 1.0)
                speed = 0.99;

            if (speed == -1.0)
                speed = -0.99;

            _directions[(int)motor].Write((speed < 0) ? GpioPinValue.High : GpioPinValue.Low);
            _controllers[(int)motor].SetDesiredFrequency(Frequency);
            _pwms[(int)motor].SetActiveDutyCyclePercentage(speed < 0 ? 1 + speed : speed);
            _lastSpeeds[(int)motor] = speed;
        }

        /// <summary>Sets the given motor's speed.</summary>
        /// <param name="motor">The motor to set the speed for.</param>
        /// <param name="speed">The desired speed of the motor between -1 and 1.</param>
        /// <param name="time">How many milliseconds the motor should take to reach the specified speed.</param>
        public void SetSpeed(Motor motor, double speed, int time)
        {
            if (speed > 1 || speed < -1) throw new ArgumentOutOfRangeException(nameof(speed), "speed must be between -1  and 1.");
            if (motor != Motor.Motor1 && motor != Motor.Motor2) throw new ArgumentException("You must specify a valid motor.", nameof(motor));

            double currentSpeed = _lastSpeeds[(int)motor];

            if (currentSpeed == speed)
                return;

            int sleep = (int)(time / (Math.Abs(speed - currentSpeed) * StepFactor));
            double step = 1.0 / StepFactor;

            if (sleep < 1)
                throw new ArgumentOutOfRangeException(nameof(time), "You cannot move to a speed this close to the existing speed in so little time.");

            if (speed < currentSpeed)
                step *= -1;

            while (Math.Abs(speed - currentSpeed) >= 0.01)
            {
                currentSpeed += step;

                SetSpeed(motor, currentSpeed);

                Thread.Sleep(sleep);
            }
        }
    }
}
