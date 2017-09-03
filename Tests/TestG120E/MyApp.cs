using System.Diagnostics;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Pwm;
using GHIElectronics.TinyCLR.Pins;
using Module;
using TinyClrCore;

namespace Example
{
    public class MyApp : Application
    {
        private Button _btn;
        private Led _btnLed;
        private LedStrip _ledStrip;
        private Tunes _tunes;

        public override void ProgramStarted()
        {
            var list = I2CScanner.Ping(G120E.I2cBus.I2c0);
            foreach (var value in list)
            {
                Debug.WriteLine("Device: " + ((int)value).ToString("X"));
            }

            TestLed();
            TestLedStrip();
            TestTunes();
            TestButton();
            CheckDevices();
        }


        private void TestLed()
        {
            Led led = new Led(G120E.GpioPin.P1_31);
            led.Blink(500);
            Thread.Sleep(5000);
            led.StopBlinking();
            _btnLed = new Led(G120E.GpioPin.P2_0) { State = GpioPinValue.High };
        }

        private void TestButton()
        {
            _btn = new Button(G120E.GpioPin.P2_31);
            _btn.OnPress += Btn_OnPress;
            _btn.OnRelease += Btn_OnRelease;
        }

        private void TestTunes()
        {
            var controller=PwmController.FromId(G120E.PwmPin.Controller1.Id);
            PwmPin pin=controller.OpenPin(G120E.PwmPin.Controller1.P3_26);
            Melody mel = new Melody(new MusicNote(Tone.A3, 200), new MusicNote(Tone.B4, 200));
            _tunes = new Tunes(pin);
            _tunes.Play(mel);
        }

        private void TestLedStrip()
        {
            _ledStrip = new LedStrip(G120E.GpioPin.P0_13, G120E.GpioPin.P0_25, G120E.GpioPin.P0_26, G120E.GpioPin.P3_25,
                G120E.GpioPin.P0_18, G120E.GpioPin.P0_17, G120E.GpioPin.P0_15);
            _ledStrip.SetLed(0, false);
            _ledStrip.SetLed(1, true);
            _ledStrip.SetLed(3, true);
            _ledStrip.SetLed(5, true);
            Thread.Sleep(2000);
            _ledStrip.SetToLed(5);
            Thread.Sleep(2000);
            _ledStrip.SetToLed(1);
            Thread.Sleep(2000);
            _ledStrip.SetToLed(6);
            Thread.Sleep(2000);
        }

        private void Btn_OnRelease(object sender, GpioPinValueChangedEventArgs e)
        {
            _btnLed.State = GpioPinValue.Low;
        }

        private void Btn_OnPress(object sender, GpioPinValueChangedEventArgs e)
        {
            _btnLed.State = GpioPinValue.High;
        }

        public override void UpdateLoop()
        {
            //if (btn.State)
            //{
            //    btnLed.State = GpioPinValue.Low;
            //}
            //else
            //{
            //    btnLed.State=GpioPinValue.High;
            //    CheckDevices();
            //}
        }

        private void CheckDevices()
        {
            Debug.WriteLine("Checking ...");
            foreach (var value in I2CScanner.Ping(G120E.I2cBus.I2c0))
            {
                Debug.WriteLine(((int)value).ToString("X"));
            }
            Debug.WriteLine("End checking.");

        }
    }
}
