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
        private static GpioPin  _led;
        private static TempHumidity _tempHumidity;

        static void Main()
        {
            switch (DeviceInformation.DeviceName)
            {
                case "FEZ":
                    SetupFez();
                    break;

                case "Cerb":
                    SetupCerb();
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

        private static void SetupCerb()
        {
            _led = GpioController.GetDefault().OpenPin(FEZCerberus.GpioPin.DebugLed);
            _led.SetDriveMode(GpioPinDriveMode.Output);

            _tempHumidity=new TempHumidity(FEZCerberus.GpioPin.Socket2.Pin4,FEZCerberus.GpioPin.Socket2.Pin5);
            _tempHumidity.MeasurementComplete += _tempHumidity_MeasurementComplete;
            _tempHumidity.MeasurementInterval = 1000;
            _tempHumidity.StartTakingMeasurements();
        }

        private static void _tempHumidity_MeasurementComplete(TempHumidity sender, TempHumidity.MeasurementCompleteEventArgs e)
        {
            Debug.WriteLine("Temp: "+e.Temperature.ToString("F1")+"°C ; Humdity: "+e.RelativeHumidity.ToString("F1")+"%");
        }

        private static void SetupFez()
        {
            _led = GpioController.GetDefault().OpenPin(1);
            _led.SetDriveMode(GpioPinDriveMode.Output);
        }
    }
}
