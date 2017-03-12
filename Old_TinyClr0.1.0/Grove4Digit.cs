using System;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.Gpio;

namespace GroveModule
{
    public class Grove4Digit
    {
        private readonly GpioPin _clkPin;
        private readonly GpioPin _dataPin;

        public static byte[] Numbers = { 0x3f, 0x06, 0x5b, 0x4f, 0x66, 0x6d, 0x7d, 0x07, 0x7f, 0x6f };
        public static byte Dot = 0x80;
        private byte _brightness;
        private const byte CmdAutoAddr = 0x40;
        private const byte DisplayOn = 0x88;
        private const byte DefaultStartAddress = 0xc0;

        public Grove4Digit(int clkPin, int dataPin)
        {
            GpioController gpio=GpioController.GetDefault();
            _clkPin = gpio.OpenPin(clkPin);
            _clkPin.SetDriveMode(GpioPinDriveMode.Output);
            _dataPin = gpio.OpenPin(dataPin);
            _dataPin.SetDriveMode(GpioPinDriveMode.Output);
        }

        public int Brightness { get { return _brightness; } set { _brightness = (byte)(value & 0x07); } }

        public void Display4Digit(byte[] aBytes,byte startAddress=0)
        {
            SendStartSignal();
            WriteByte(CmdAutoAddr);
            SendStopSignal();

            SendStartSignal();
            WriteByte((startAddress==0)?DefaultStartAddress:startAddress);
            for (int i = 0; i < aBytes.Length; i++)
            {
                WriteByte(aBytes[i]);
            }
            SendStopSignal();
            SendStartSignal();
            WriteByte((byte)(DisplayOn | _brightness));
            SendStopSignal();

        }
        private void WriteByte(byte val)
        {
            try
            {
                for (int i = 0; i < 8; i++)
                {
                    _clkPin.Write(GpioPinValue.Low);
                    _dataPin.Write((val & 0x01) != 0 ? GpioPinValue.High : GpioPinValue.Low);
                    val >>= 1;
                    Thread.Sleep(1);
                    _clkPin.Write(GpioPinValue.High);
                    Thread.Sleep(1);
                }

                // End of 8th clock pulse, Start of ACK
                _clkPin.Write(GpioPinValue.Low);
                _dataPin.SetDriveMode(GpioPinDriveMode.Input);
                Thread.Sleep(1);
                var read = _dataPin.Read();
                if (read == GpioPinValue.High)
                    throw new Exception("No ack signal");
                _clkPin.Write(GpioPinValue.High);
                Thread.Sleep(1);
                _clkPin.Write(GpioPinValue.Low);
            }
            finally
            {
                _dataPin.SetDriveMode(GpioPinDriveMode.Output);
            }
        }

        private void SendStartSignal() // OK
        {
            _clkPin.Write(GpioPinValue.High);
            _dataPin.Write(GpioPinValue.High);
            Thread.Sleep(1);
            _dataPin.Write(GpioPinValue.Low);
            Thread.Sleep(1);
            _clkPin.Write(GpioPinValue.Low);
            Thread.Sleep(1);
        }

        private void SendStopSignal() // OK
        {
            _clkPin.Write(GpioPinValue.Low);
            _dataPin.Write(GpioPinValue.Low);
            Thread.Sleep(1);
            _clkPin.Write(GpioPinValue.High);
            Thread.Sleep(1);
            _dataPin.Write(GpioPinValue.High);
            Thread.Sleep(1);
        }
    }
}