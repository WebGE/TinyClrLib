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
        private static Led7R _led7R;
        private static Potentiometer _potentiometer;

        static void Main()
        {
            // Config system
            Setup(DeviceInformation.DeviceName);

            // Run loop
            while (true)
            {
                _led7R.SetPercentage(_potentiometer.ReadProportion());
                Thread.Sleep(20);
            }
            // ReSharper disable once FunctionNeverReturns
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
            _led7R = new Led7R(FEZSpiderII.GpioPin.Socket4.Pin3, FEZSpiderII.GpioPin.Socket4.Pin4, FEZSpiderII.GpioPin.Socket4.Pin5, FEZSpiderII.GpioPin.Socket4.Pin6, FEZSpiderII.GpioPin.Socket4.Pin7, FEZSpiderII.GpioPin.Socket4.Pin8, FEZSpiderII.GpioPin.Socket4.Pin9);
            _potentiometer = new Potentiometer(FEZSpiderII.AdcChannel.Socket9.Pin3);

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
            _led7R = new Led7R(FEZCerberus.GpioPin.Socket7.Pin3, FEZCerberus.GpioPin.Socket7.Pin4, FEZCerberus.GpioPin.Socket7.Pin5, FEZCerberus.GpioPin.Socket7.Pin6, FEZCerberus.GpioPin.Socket7.Pin7, FEZCerberus.GpioPin.Socket7.Pin8, FEZCerberus.GpioPin.Socket7.Pin9);
            _potentiometer = new Potentiometer(FEZCerberus.AdcChannel.Socket4.Pin3);
        }

        private static void LedBlink()
        {
            while (true)
            {
                _led.Write(GpioPinValue.High);
                Thread.Sleep(200);
                _led.Write(GpioPinValue.Low);
                Thread.Sleep(800);
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}
