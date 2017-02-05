using System;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Pins;
using GHIElectronics.TinyCLR.Storage.Streams;
using GroveModule;

namespace Example
{
    public class MyApp : GroveModule.Application
    {
        private static Button _button;
        private static Led _ledOrange;
        private Grove4Digit _digit;
        private bool _state;
        private Timer timer;

        public override void ProgramStarted()
        {
            Led ledBlue = new Led(FEZPandaIII.Gpio.Led1);
            ledBlue.State = GpioPinValue.High;

            _ledOrange = new Led(FEZPandaIII.Gpio.Led3);

            _button = new Button(FEZPandaIII.Gpio.D8);
            _button.OnRelease += _button_OnRelease;
            _button.OnPress += _button_OnPress;

            _digit = new Grove4Digit(FEZPandaIII.Gpio.D4, FEZPandaIII.Gpio.D5);
            _digit.Brightness = 0x00;
            _digit.Display4Digit(new byte[] { 0x01, 0x02, 0x04, 0x08 });

            Led led = new Led(FEZPandaIII.Gpio.D6);
            led.Blink(500, 200);
            Thread.Sleep(5000);
            led.StopBlinking();

            timer = new Timer(MyTimerCallback, _digit, 0, 1000);
            ledBlue.State = GpioPinValue.Low;
        }

        private void MyTimerCallback(object state)
        {
            _state = !_state;
            DisplayTime(_state);
        }

        private void DisplayTime(bool dot = false)
        {
            int[] buffer = new int[4];
            int hour = DateTime.Now.Hour;
            int min = DateTime.Now.Minute;
            buffer[0] = hour / 10;
            buffer[1] = hour % 10;
            buffer[2] = min / 10;
            buffer[3] = min % 10;
            if (dot)
            {
                _digit.Display4Digit(new byte[]
                {
                    Grove4Digit.Numbers[buffer[0]], (byte) (Grove4Digit.Numbers[buffer[1]] | 0x80),
                    Grove4Digit.Numbers[buffer[2]],
                    Grove4Digit.Numbers[buffer[3]]
                });
            }
            else
            {
                _digit.Display4Digit(new byte[]
                {
                    Grove4Digit.Numbers[buffer[0]], (byte) (Grove4Digit.Numbers[buffer[1]]),
                    Grove4Digit.Numbers[buffer[2]],
                    Grove4Digit.Numbers[buffer[3]]
                });
            }
        }

        private static void _button_OnPress(object sender, GpioPinValueChangedEventArgs e)
        {
            _ledOrange.State = GpioPinValue.High;
        }

        private static void _button_OnRelease(object sender, GpioPinValueChangedEventArgs e)
        {
            _ledOrange.State = GpioPinValue.Low;
        }

    }
}
