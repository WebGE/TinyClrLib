using System.Diagnostics;
using System.Drawing;
using System.Threading;
using GHIElectronics.TinyCLR.Pins;
using TinyClrCore;

namespace TestG80
{
    internal class MyG80App : Application
    {
        private I2cGroveColor.I2cGroveColor module;

        public override void UpdateLoop()
        {
            byte[] data = module.GetARGB();
            Debug.WriteLine("Color R:" + data[0] + ", G:" + data[1] + ", B:" + data[2]);
            Thread.Sleep(1000);
        }

        public override void ProgramStarted()
        {
            //Debug.WriteLine("Scanning ...");
            //foreach (var value in I2CScanner.Ping(FEZPandaIII.I2cBus.I2c1))
            //{
            //    Debug.WriteLine(((int)value).ToString("X"));
            //}
            //Debug.WriteLine("End scanning.");

            module = new I2cGroveColor.I2cGroveColor(FEZPandaIII.I2cBus.I2c1);
            Debug.WriteLine("Module initialized.");
            module.LedStatus = false;
            byte[] data = module.GetARGB();
            Debug.WriteLine("Color R:" + data[0] + ", G:" + data[1] + ", B:" + data[2]);
        }
    }
}