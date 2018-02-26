using System;
using System.Diagnostics;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.SerialCommunication;
using GHIElectronics.TinyCLR.Storage.Streams;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Local
#pragma warning disable 1591

namespace Bauland.Grove
{
    /// <summary>
    /// Wrapper for Grove Serail Bluetooth 3 module
    /// </summary>
    public class SerialBluetooth3
    {
        private SerialDevice _serial;
        private DataReader _reader;
        private DataWriter _writer;
        private readonly string _uart;
        private BaudRate _baudRate;

        public enum BaudRate
        {
            BaudRate9600 = 4,
            BaudRate19200=5,
            BaudRate38400 = 6,
            BaudRate57600=7,
            BaudRate115200=8,
            BaudRate230400=9,
        }

        /// <summary>
        /// Constructor of Grove SerialBluetooth3 module
        /// </summary>
        /// <param name="idUart"></param>
        /// <param name="baudRate">Set initial rate of module (default is 9600)</param>
        public SerialBluetooth3(string idUart,BaudRate baudRate=BaudRate.BaudRate9600)
        {
            _uart = idUart;
            OpenSerial(idUart, baudRate);
        }

        private void OpenSerial(string idUart, BaudRate baudRate)
        {
            _serial = SerialDevice.FromId(idUart);
            _baudRate = baudRate;
            _serial.BaudRate = GetBaudRate(baudRate);
            _serial.ReadTimeout = TimeSpan.FromMilliseconds(200);
            _reader = new DataReader(_serial.InputStream);
            _writer = new DataWriter(_serial.OutputStream);
        }
        private void CloseSerial()
        {
            _writer.Dispose();
            _writer = null;
            _reader.Dispose();
            _reader = null;
            _serial.Dispose();
            _serial = null;
        }

        /// <summary>
        /// Check if device is present(if it is not connected on master)
        /// </summary>
        /// <returns>true if present, else false</returns>
        public bool CheckDevice()
        {
            string expected = "OK";
            _writer.WriteString("AT");
            _writer.Store();
            var read = _reader.Load((uint)expected.Length);
            if (read == (uint)expected.Length)
            {
                var str = _reader.ReadString((uint)expected.Length);
                if (str == expected)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Change Baud Rate to communicate with module
        /// </summary>
        /// <param name="baudRate">Value of baud rate</param>
        /// <returns>true if change has been made, false if an error occurs</returns>
        public bool ChangeBaudRate(BaudRate baudRate)
        {
            string str = "AT+BAUD" + baudRate;
            _writer.WriteString(str);
            _writer.Store();
            var read = _reader.Load(2);
            if (read == 2)
            {
                var rep = _reader.ReadString(read);
                if (rep == "OK")
                {
                    read = _reader.Load(15);
                    rep = _reader.ReadString(read);
                    Debug.WriteLine("ChangeBaudRate:"+rep);
                    SetSerial(baudRate);
                    while (!CheckDevice())
                    {
                        Thread.Sleep(20);
                    }
                    return true;
                }
            }
            return false;
        }

        public static uint GetBaudRate(BaudRate baudRate)
        {
            switch (baudRate)
            {
                case BaudRate.BaudRate9600:
                    return 9600;
                case BaudRate.BaudRate19200:
                    return 19200;
                case BaudRate.BaudRate38400:
                    return 38400;
                case BaudRate.BaudRate57600:
                    return 57600;
                case BaudRate.BaudRate115200:
                    return 115200;
                case BaudRate.BaudRate230400:
                    return 230400;
                default:
                    throw new InvalidOperationException();
            }
        }

        private void SetSerial(BaudRate baudRate)
        {
            if (baudRate == _baudRate)
                // Nothing to do
                return;
            CloseSerial();
            Thread.Sleep(1000);
            OpenSerial(_uart,baudRate);
        }
    }
}
