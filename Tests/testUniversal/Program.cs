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
        private static Bluetooth _bluetooth;
        private static Bluetooth.Client client;

        static void Main()
        {
            // Config system
            Setup(DeviceInformation.DeviceName);

            // Run loop
            while (true)
            {
                Thread.Sleep(20);
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
            _bluetooth=new Bluetooth(FEZSpiderII.GpioPin.Socket9.Pin3,FEZSpiderII.GpioPin.Socket9.Pin6,FEZSpiderII.UartPort.Socket8);
            _bluetooth.SetDeviceName("Test");
            _bluetooth.SetPinCode("1234");
            client = _bluetooth.ClientMode;
            
            _bluetooth.BluetoothStateChanged += _bluetooth_BluetoothStateChanged;
            _bluetooth.DataReceived += _bluetooth_DataReceived;
            _bluetooth.DeviceInquired += _bluetooth_DeviceInquired;
            _bluetooth.PinRequested += _bluetooth_PinRequested;
        }

        private static void _bluetooth_PinRequested(Bluetooth sender)
        {
            Debug.WriteLine("PinRequest");
        }

        private static void _bluetooth_DeviceInquired(Bluetooth sender, string macAddress, string name)
        {
            Debug.WriteLine("DeviceInquired: "+name+", "+macAddress);
        }

        private static void _bluetooth_DataReceived(Bluetooth sender, string data)
        {
            Debug.WriteLine("DataReceived: "+data);
        }

        private static void _bluetooth_BluetoothStateChanged(Bluetooth sender, Bluetooth.BluetoothState btState)
        {
            Debug.WriteLine("BluetoothStateChanged:"+btState);
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
            _bluetooth = new Bluetooth(FEZCerberus.GpioPin.Socket2.Pin3, FEZCerberus.GpioPin.Socket2.Pin6, FEZCerberus.UartPort.Socket2);
            client = _bluetooth.ClientMode;
            _bluetooth.SetDeviceName("Test");
            _bluetooth.SetPinCode("1234");
            _bluetooth.
            _bluetooth.BluetoothStateChanged += _bluetooth_BluetoothStateChanged;
            _bluetooth.DataReceived += _bluetooth_DataReceived;
            _bluetooth.DeviceInquired += _bluetooth_DeviceInquired;
            _bluetooth.PinRequested += _bluetooth_PinRequested;
            Thread.Sleep(1000);
           Debug.WriteLine("### Entering pairing Mode ###");
             client.EnterPairingMode();
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
