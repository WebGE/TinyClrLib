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

        public override void ProgramStarted()
        {
            Led led = new Led(G120E.GpioPin.P1_31);
            led.Blink(500, 200);
            Thread.Sleep(5000);
            led.StopBlinking();

            btn=new Button(G120E.GpioPin.P2_31);
            btn.OnPress += Btn_OnPress;
            btn.OnRelease += Btn_OnRelease;

            btnLed=new Led(G120E.GpioPin.P2_0);
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
