using System;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Pins;
using Module;

namespace Example
{
    public class MyApp : Application.Application
    {
        private static Led _ledOrange;
        private bool _state;
        private Timer timer;
        private Button btn;
        private Led btnLed;
        private LedStrip _ledStrip;
        private Tunes _tunes;

        public override void ProgramStarted()
        {
            Melody mel=new Melody(new MusicNote[]{new MusicNote(Tone.A3, 200),new MusicNote(Tone.B4, 200)  });
            _tunes=new Tunes(G120E.PwmPin.P3_26);
            _tunes.Play(mel);

            Led led = new Led(G120E.GpioPin.P1_31);
            btn=new Button(G120E.GpioPin.P2_31);
            btn.OnPress += Btn_OnPress;
            btn.OnRelease += Btn_OnRelease;

            _ledStrip=new LedStrip(G120E.GpioPin.P0_13,G120E.GpioPin.P0_25,G120E.GpioPin.P0_26,G120E.GpioPin.P3_25,G120E.GpioPin.P0_18,G120E.GpioPin.P0_17,G120E.GpioPin.P0_15);
            _ledStrip.SetLed(0,false);
            led.Blink(500, 200);
            Thread.Sleep(5000);
            led.StopBlinking();
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


            btnLed = new Led(G120E.GpioPin.P2_0);
            btnLed.State=GpioPinValue.High;
        }

        private void Btn_OnRelease(object sender, GpioPinValueChangedEventArgs e)
        {
            btnLed.State=GpioPinValue.Low;
        }

        private void Btn_OnPress(object sender, GpioPinValueChangedEventArgs e)
        {
            btnLed.State=GpioPinValue.High;
        }
    }
}
