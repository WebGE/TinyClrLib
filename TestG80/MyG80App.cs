using System.Diagnostics;
using System.Drawing;
using GHIElectronics.TinyCLR.Pins;
using TinyClrCore;

namespace TestG80
{
    internal class MyG80App : Application
    {
        public override void UpdateLoop()
        {
        }

        public override void ProgramStarted()
        {
            Debug.WriteLine("Scanning ...");
            foreach (var value in I2CScanner.Ping(FEZPandaIII.I2cBus.I2c1))
            {
                Debug.WriteLine(((int)value).ToString("X"));
            }
            Debug.WriteLine("End scanning.");

            I2cGroveColor.I2cGroveColor module=new I2cGroveColor.I2cGroveColor(FEZPandaIII.I2cBus.I2c1);
            Color color =module.GetRgbColor();
            Debug.WriteLine("Module initialized.");

        }
    }
}