using System;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Pins;
using GroveModule;

namespace Example
{
    public class MyApp : Application.Application
    {
        private static Led _ledOrange;
        private bool _state;
        private Timer timer;

        public override void ProgramStarted()
        {
            Led led = new Led(G120E.GpioPin.P1_31);
            led.Blink(500, 200);
            Thread.Sleep(5000);
            led.StopBlinking();
        }
    }
}
