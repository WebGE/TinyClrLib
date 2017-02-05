using System;
using System.Diagnostics;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Pins;
using GroveModule;

namespace Example
{
    public class Program
    {
        public static void Main()
        {
            try
            {
                MyApp app = new MyApp();
                app.Run();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}