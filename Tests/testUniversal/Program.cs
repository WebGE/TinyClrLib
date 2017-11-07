using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Bauland.Gadgeteer;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Pins;

namespace testUniversal
{
    static class Program
    {
        private static GpioPin _led;
        private static Led7C _led7C;

        static void Main()
        {
            // Config system
            Setup(DeviceInformation.DeviceName);

            // Run loop
            while (true)
            {
                _led7C.SetColor(Led7C.LedColor.Blue);
                Thread.Sleep(2000);
                _led7C.SetColor(Led7C.LedColor.White);
                Thread.Sleep(2000);
                _led7C.SetColor(Led7C.LedColor.Red);
                Thread.Sleep(2000);
                _led7C.SetColor(Led7C.LedColor.Cyan);
                Thread.Sleep(2000);
                _led7C.SetColor(Led7C.LedColor.Off);
                Thread.Sleep(2000);
            }
        }

        private static void Setup(string deviceName)
        {
            // Specific part
            switch (deviceName)
            {
                case "FEZ":
                    SetupFez();
                    break;
                case "Cerb":
                    SetupCerb();
                    break;
                case "G120":
                    SetupG120();
                    break;
                case "ELECTRON":
                    SetupElectron();
                    break;
                default:
                    Debug.WriteLine("Unkown board: " + DeviceInformation.DeviceName);
                    throw new Exception("Unkown board: " + DeviceInformation.DeviceName);
            }
            // Common part
            _led.SetDriveMode(GpioPinDriveMode.Output);
            Thread t = new Thread(LedBlink);
            t.Start();
        }

        private static void SetupG120()
        {
            _led = GpioController.GetDefault().OpenPin(FEZSpiderII.GpioPin.DebugLed);
            _led7C=new Led7C(FEZSpiderII.GpioPin.Socket10.Pin3,FEZSpiderII.GpioPin.Socket10.Pin4,FEZSpiderII.GpioPin.Socket10.Pin5);
        }

        private static void SetupElectron()
        {
            _led = GpioController.GetDefault().OpenPin(8);
        }

        private static void SetupFez()
        {
            _led = GpioController.GetDefault().OpenPin(1);
        }

        private static void SetupCerb()
        {
            _led = GpioController.GetDefault().OpenPin(FEZCerberus.GpioPin.DebugLed);
        }

        private static void LedBlink()
        {
            while (true)
            {
                _led.Write(GpioPinValue.High);
                Thread.Sleep(2);
                _led.Write(GpioPinValue.Low);
                Thread.Sleep(8);
            }
        }
    }
}
