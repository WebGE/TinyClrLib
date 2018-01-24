using System;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.Gpio;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Bauland.Gadgeteer
{
    /// <summary>
    /// Wrapper class for Led7R Gadgeteer Module
    /// </summary>
    public class Led7R
    {
        private readonly GpioPin[] _leds;

        /// <summary>The number of LEDs on the module.</summary>
        public int LedCount => 7;

        /// <summary>The state of the given LED.</summary>
        /// <param name="index">The LED whose state to get or set.</param>
        /// <returns>Whether or not the LED is on or off.</returns>
        public bool this[int index]
        {
            get
            {
                if (index >= LedCount || index < 0) throw new ArgumentOutOfRangeException(nameof(index), "index must be between 0 and LedCount.");

                return _leds[index].Read() == GpioPinValue.High;
            }

            set
            {
                if (index >= LedCount || index < 0) throw new ArgumentOutOfRangeException(nameof(index), "index must be between 0 and LedCount.");

                SetLed(index, value);
            }
        }

        /// <summary>Constructor</summary>
        /// <param name="pin3">Pin 3 of Y socket</param>
        /// <param name="pin4">Pin 4 of Y socket</param>
        /// <param name="pin5">Pin 5 of Y socket</param>
        /// <param name="pin6">Pin 6 of Y socket</param>
        /// <param name="pin7">Pin 7 of Y socket</param>
        /// <param name="pin8">Pin 8 of Y socket</param>
        /// <param name="pin9">Pin 9 of Y socket</param>
        public Led7R(int pin3, int pin4, int pin5, int pin6, int pin7, int pin8, int pin9)
        {
            _leds=new GpioPin[LedCount];
            _leds[0] = GpioController.GetDefault().OpenPin(pin3);
            _leds[0].SetDriveMode(GpioPinDriveMode.Output);
            _leds[1] = GpioController.GetDefault().OpenPin(pin4);
            _leds[1].SetDriveMode(GpioPinDriveMode.Output);
            _leds[2] = GpioController.GetDefault().OpenPin(pin5);
            _leds[2].SetDriveMode(GpioPinDriveMode.Output);
            _leds[3] = GpioController.GetDefault().OpenPin(pin6);
            _leds[3].SetDriveMode(GpioPinDriveMode.Output);
            _leds[4] = GpioController.GetDefault().OpenPin(pin7);
            _leds[4].SetDriveMode(GpioPinDriveMode.Output);
            _leds[5] = GpioController.GetDefault().OpenPin(pin8);
            _leds[5].SetDriveMode(GpioPinDriveMode.Output);
            _leds[6] = GpioController.GetDefault().OpenPin(pin9);
            _leds[6].SetDriveMode(GpioPinDriveMode.Output);
        }

        /// <summary>Turns the specified LED on.</summary>
        /// <param name="led">The LED to turn on.</param>
        public void TurnLedOn(int led)
        {
            if (led >= LedCount || led < 0) throw new ArgumentOutOfRangeException(nameof(led), "led must be between 0 and LedCount.");

            SetLed(led, true);
        }

        /// <summary>Turns the specified LED off.</summary>
        /// <param name="led">The LED to turn off.</param>
        public void TurnLedOff(int led)
        {
            if (led >= LedCount || led < 0) throw new ArgumentOutOfRangeException(nameof(led), "led must be between 0 and LedCount.");

            SetLed(led, false);
        }

        /// <summary>Sets the given LED to the given state.</summary>
        /// <param name="led">The LED to change to set.</param>
        /// <param name="state">The new LED state.</param>
        public void SetLed(int led, bool state)
        {
            if (led >= LedCount || led < 0) throw new ArgumentOutOfRangeException(nameof(led), "led must be between 0 and LedCount.");

            _leds[led].Write(state?GpioPinValue.High:GpioPinValue.Low);
        }

        /// <summary>Sets the LEDs on the module to the corresponding bit in the mask.</summary>
        /// <param name="mask">The bit mask to set the LEDs to.</param>
        public void SetBitmask(uint mask)
        {
            uint value = 1;

            for (int i = 0; i < LedCount; i++)
            {
                if ((mask & value) != 0)
                    TurnLedOn(i);
                else
                    TurnLedOff(i);

                value <<= 1;
            }
        }

        /// <summary>Turns on all of the LEDs.</summary>
        public void TurnAllLedsOn()
        {
            for (int i = 0; i < 7; i++)
                TurnLedOn(i);
        }

        /// <summary>Turns off all of the LEDs.</summary>
        public void TurnAllLedsOff()
        {
            for (int i = 0; i < 7; i++)
                TurnLedOff(i);
        }

        /// <summary>Turns all of the LEDs on up until, but not including, the LED numbered by endIndex. The rest are turned off.</summary>
        /// <param name="endIndex">The LED to stop before.</param>
        public void SetLeds(int endIndex)
        {
            if (endIndex > LedCount || endIndex < 0) throw new ArgumentOutOfRangeException(nameof(endIndex), "endIndex must be between 0 and LedCount.");

            int led = 0;

            for (; led < endIndex; led++)
                TurnLedOn(led);

            for (; led < LedCount; led++)
                TurnLedOff(led);
        }

        /// <summary>Turns on the LedCount * percentage LEDs starting at LED 0.</summary>
        /// <param name="percentage">The amount of LEDs to turn on.</param>
        public void SetPercentage(double percentage)
        {
            if (percentage > 1 || percentage < 0) throw new ArgumentOutOfRangeException(nameof(percentage), "percentage must be between 0 and 1.");

            SetLeds((int)(percentage * LedCount));
        }

        /// <summary>Animates the lights on the board, according to the passed in values.</summary>
        /// <param name="switchTime">Time between each operation in milliseconds</param>
        /// <param name="clockwise">Whether or not the animation should play in a clockwise motion.</param>
        /// <param name="on">Whether or not the animation should turn the lights on, false if the lights should be turned off.</param>
        /// <param name="remainOn">Whether or not a light should remain on when another one is lit, false if only one light should be lit at a time.</param>
        public void Animate(int switchTime, bool clockwise, bool on, bool remainOn)
        {
            int length = LedCount;
            int i;
            int terminate;
            int dir;

            if (clockwise)
            {
                i = 0;
                terminate = length;
                dir = 1;
            }
            else
            {
                i = length-1;
                terminate = -1;
                dir = -1;
            }

            for (; i != terminate; i += dir)
            {
                if (on)
                {
                    if (!remainOn)
                        TurnAllLedsOff();

                    TurnLedOn(i);
                }
                else
                {
                    TurnLedOff(i);
                }

                Thread.Sleep(switchTime);
            }
        }
    }
}
