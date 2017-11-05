using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.Gpio;

namespace testUniversal
{
    static class Program
    {
        private static GpioPin  _led;
        static void Main()
        {
            switch (DeviceInformation.DeviceName)
            {
                case "FEZ":
                    SetupFez();
                    break;
                default:
                    Debug.WriteLine("Unkown board: " + DeviceInformation.DeviceName);
                    throw new Exception("Unkown board: " + DeviceInformation.DeviceName);
            }
            while (true)
            {
                _led.Write(GpioPinValue.High);
                Thread.Sleep(200);
                _led.Write(GpioPinValue.Low);
                Thread.Sleep(800);
            }
        }

        private static void SetupFez()
        {
            _led = GpioController.GetDefault().OpenPin(1);
            _led.SetDriveMode(GpioPinDriveMode.Output);
        }
    }
}
