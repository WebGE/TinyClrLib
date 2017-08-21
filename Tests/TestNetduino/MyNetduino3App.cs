using Modules.Adafruit;
using System.Diagnostics;
using GHIElectronics.TinyCLR.Pins;
using TinyClrCore;

namespace TestNetduino
{
    class MyNetduino3App : Application
    {
        private ColorSensor1334 _colorsensor;
        public override void UpdateLoop()
        {
        }

        public override void ProgramStarted()
        {
            _colorsensor=new ColorSensor1334(FEZ.I2cBus.I2c1);
            Debug.WriteLine("ID: 0x"+_colorsensor.GetId().ToString("X"));
        }
    }
}
