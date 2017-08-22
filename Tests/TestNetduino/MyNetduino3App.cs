using Modules.Adafruit;
using System.Diagnostics;
using System.Threading;
using GHIElectronics.TinyCLR.Pins;
using TinyClrCore;

namespace TestNetduino
{
    class MyNetduino3App : Application
    {
        private ColorSensor1334 _colorsensor;
        public override void UpdateLoop()
        {
            var color = _colorsensor.GetRawData();
            Debug.WriteLine("C:" + color.Clear + " ,R:" + color.Red + " ,G:" + color.Green + " ,B:" + color.Blue);
            Thread.Sleep(500);
        }

        public override void ProgramStarted()
        {
            _colorsensor = new ColorSensor1334(FEZ.I2cBus.I2c1);
            _colorsensor.SetGain(ColorSensor1334.Gain.Gain1X);
            _colorsensor.SetIntegrationTime(ColorSensor1334.IntegrationTime.It_700ms);
            Debug.WriteLine("ID: 0x" + _colorsensor.GetId().ToString("X"));
        }
    }
}
