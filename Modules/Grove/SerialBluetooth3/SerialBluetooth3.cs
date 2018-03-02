using System;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.SerialCommunication;
using GHIElectronics.TinyCLR.Storage.Streams;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMethodReturnValue.Local

namespace Bauland.Grove
{

    /// <summary>
    /// Define accessible rates for communication speed
    /// </summary>
    public enum BaudRate
    {
        /// <summary> </summary>
        BaudRate9600 = 4,
        /// <summary> </summary>
        BaudRate19200 = 5,
        /// <summary> </summary>
        BaudRate38400 = 6,
        /// <summary> </summary>
        BaudRate57600 = 7,
        /// <summary> </summary>
        BaudRate115200 = 8,
        /// <summary> </summary>
        BaudRate230400 = 9,
    }

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
        private readonly Thread _readerThread;
        private const int Delay = 100;

        private readonly object _lock = new Object();

        private Client _client;
        /// <summary>Sets Bluetooth module to work in Client mode.</summary>
        public Client ClientMode
        {
            get
            {
                lock (_lock)
                {
                    return _client ?? (_client = new Client(this));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class Client
        {
            private readonly SerialBluetooth3 _bluetooth;

            internal Client(SerialBluetooth3 bluetooth)
            {
                _bluetooth = bluetooth;
                _bluetooth.QueryResponse("AT+ROLES");
                _bluetooth._readerThread.Start();
            }

            /// <summary>Sends data through the connection.</summary>
            /// <param name="message">String containing the data to be sent</param>
            public void SendString(string message)
            {
                _bluetooth._writer.WriteString(message);
                _bluetooth._writer.Store();
            }

            /// <summary>Sends data through the connection.</summary>
            /// <param name="b">Byte containing the data to be sent</param>
            public void SendByte(byte b)
            {
                _bluetooth._writer.WriteByte(b);
                _bluetooth._writer.Store();
            }

            /// <summary>Sends data through the connection.</summary>
            /// <param name="bytesArray">Bytes containing the data to be sent</param>
            public void SendBytes(byte[] bytesArray)
            {
                _bluetooth._writer.WriteBytes(bytesArray);
                _bluetooth._writer.Store();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public delegate void DataReceivedHandler(SerialBluetooth3 sender, string data);

        /// <summary>Event raised when the bluetooth module changes its state.</summary>
        public event DataReceivedHandler OnDataReceived;

        /// <summary>
        /// Constructor of Grove SerialBluetooth3 module
        /// </summary>
        /// <param name="idUart"></param>
        /// <param name="baudRate">Set initial rate of module (default is 9600)</param>
        public SerialBluetooth3(string idUart, BaudRate baudRate = BaudRate.BaudRate9600)
        {
            _uart = idUart;
            OpenSerial(idUart, baudRate);
            _readerThread = new Thread(RunReaderThread);
        }

        /// <summary>
        /// If false, led is always off.
        /// </summary>
        public bool LedMode
        {
            get => GetLedMode().Response == "1";
            set => SetLedMode(value);
        }

        public bool Key
        {
            get => GetKey().Response == "1";
        }

        private QueryResponse GetKey()
        {
            return QueryResponse("AT+KEY?");
        }

        /// <summary>
        /// Get notification when module is connected/disconnected
        /// </summary>
        public bool Notification
        {
            get => GetNotification().Response == "1";
            set => SetNotification(value);
        }

        private QueryResponse SetNotification(bool value)
        {
            return QueryResponse("AT+NOTI" + (value ? "1" : "0"));
        }

        private QueryResponse GetNotification()
        {
            return QueryResponse("AT+NOTI?");
        }


        /// <summary>
        /// Name of bluetooth module
        /// </summary>
        public string Name
        {
            get => GetName().Response;
            set => SetName(value);
        }

        /// <summary>
        /// Pin code of bluetooth module
        /// </summary>
        public string Pin
        {
            get => GetName().Response;
            set => SetName(value);
        }

        /// <summary>
        /// BaudRate of bluetooth module
        /// </summary>
        public BaudRate BaudRate
        {
            get => GetBaudRateFromIndex(int.Parse(GetBaudRate().Response));
            set => SetBaudRate(value);
        }

        /// <summary>
        /// Get Address of module
        /// </summary>
        public string Address => GetAddress().Response;

        /// <summary>
        /// Get Version of module
        /// </summary>
        public string Version => GetVersion().Response;

        /// <summary>
        /// Check if device is present(if it is not connected on master)
        /// </summary>
        /// <returns>true if present, else false</returns>
        public bool CheckDevice()
        {
            return QueryResponse("AT").Response == "OK";
        }

        private QueryResponse GetAddress()
        {
            return QueryResponse("AT+ADDR?");
        }

        private QueryResponse GetLedMode()
        {
            return QueryResponse("AT+LED?");
        }

        private QueryResponse SetLedMode(bool blink)
        {
            var command = "AT+LED" + (blink ? "0" : "1");
            return QueryResponse(command);
        }

        private QueryResponse GetName()
        {
            return QueryResponse("AT+NAME?");
        }

        private QueryResponse SetName(string name)
        {
            if (name.Length > 12)
                throw new ArgumentOutOfRangeException(nameof(name), "Max length of name is 12.");

            return QueryResponse("AT+NAME" + name);
        }

        private QueryResponse GetPin()
        {
            return QueryResponse("AT+PIN?");
        }

        private QueryResponse SetPin(string pinCode)
        {
            if (pinCode.Length > 12)
                throw new ArgumentOutOfRangeException(nameof(pinCode), "Max length of name is 12.");
            return QueryResponse("AT+PIN" + pinCode);
        }

        private QueryResponse GetBaudRate()
        {
            return QueryResponse("AT+BAUD?");
        }

        private static BaudRate GetBaudRateFromIndex(int parse)
        {
            return (BaudRate)parse;
        }

        private QueryResponse SetBaudRate(BaudRate baudRate)
        {
            var qr = QueryResponse("AT+BAUD" + baudRate);
            ChangeSerial(baudRate);
            return qr;
        }

        private QueryResponse GetVersion()
        {
            return QueryResponse("AT+VERSION?");
        }

        /// <summary>
        /// Restore default values of module (long task ~12sec)
        /// </summary>
        /// <returns>Result of query is in Response member</returns>
        public QueryResponse RestoreDefault()
        {
            _writer.WriteString("AT+DEFAULT");
            _writer.Store();
            ChangeSerial(BaudRate.BaudRate9600);
            Thread.Sleep(12000);
            var read = _reader.Load(30);
            var rep = _reader.ReadString(read);
            var indexPlus = rep.IndexOf('+');
            var response = new QueryResponse();

            if (indexPlus >= 0)
            {
                response.Result = rep.Substring(0, indexPlus);
                response.Type = rep.Substring(indexPlus + 1, rep.Length - indexPlus - 1);
                response.Response = "";
            }
            else
            {
                response.Result = "FAILED";
                response.Type = "DEFAULT";
                response.Response = "";
            }
            return response;
        }

        private void RunReaderThread()
        {
            while (true)
            {
                String response = "";
                while (_reader.Load(1) > 0)
                {
                    response = response + (char)_reader.ReadByte();
                    Thread.Sleep(1);
                }

                if (response.Length > 0)
                {
                    OnDataReceived?.Invoke(this, response);
                }
                Thread.Sleep(1);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private static uint GetBaudRateToValue(BaudRate baudRate)
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

        private QueryResponse QueryResponse(string command)
        {
            _writer.WriteString(command);
            _writer.Store();
            var read = _reader.Load(30);
            var rep = _reader.ReadString(read);
            var response = TreatResponse(rep);
            return response;
        }

        private static QueryResponse TreatResponse(string rep)
        {
            var response = new QueryResponse();
            var plusIndex = rep.IndexOf('+');
            if (plusIndex == -1)
            {
                response.Response = rep;
                response.Result = "";
                response.Type = "";
            }
            else
            {
                response.Result = rep.Substring(0, plusIndex);
                response.Type = rep.Substring(plusIndex + 1, rep.IndexOf(':') - plusIndex - 1);
                response.Response = rep.Substring(rep.IndexOf(':') + 1, rep.Length - rep.IndexOf(':') - 1);
            }
            return response;
        }
        private void OpenSerial(string idUart, BaudRate baudRate)
        {
            _serial = SerialDevice.FromId(idUart);
            _baudRate = baudRate;
            _serial.BaudRate = GetBaudRateToValue(baudRate);
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
        private void ChangeSerial(BaudRate baudRate)
        {
            if (baudRate == _baudRate)
                // Nothing to do
                return;
            CloseSerial();
            Thread.Sleep(1000);
            OpenSerial(_uart, baudRate);
        }
    }
}
