using System;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.SerialCommunication;
using GHIElectronics.TinyCLR.Storage.Streams;
// ReSharper disable StringIndexOfIsCultureSpecific.1
// ReSharper disable StringLastIndexOfIsCultureSpecific.1
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable EventNeverSubscribedTo.Global
// ReSharper disable TooWideLocalVariableScope

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
        private const int Delay = 100;
        private SerialDevice _serial;
        private DataReader _reader;
        private DataWriter _writer;
        private readonly string _uart;
        private BaudRate _baudRate;
        private readonly Thread _readerThread;
        private readonly Object _scanLock = new Object();
        private readonly Object _connectedLock = new Object();
        private readonly object _lockClientHost = new Object();

        private bool _isScanning;
        private bool _isConnected;

        private Slave _client;
        private Master _host;

        /// <summary>
        /// Constructor of Grove SerialBluetooth3 module
        /// </summary>
        /// <param name="idUart"></param>
        /// <param name="baudRate">Set initial rate of module (default is 9600)</param>
        public SerialBluetooth3(string idUart, BaudRate baudRate = BaudRate.BaudRate9600)
        {
            IsScanning = false;
            IsConnected = false;
            _uart = idUart;
            OpenSerial(idUart, baudRate);
            _readerThread = new Thread(RunReaderThread);
            if (!IsPresent) throw new InvalidOperationException("Module is already connected or missing. Check hardware");
        }

        /// <summary>
        /// Get Address of module
        /// </summary>
        public string Address => QueryResponse("AT+LADD?").Response;

        /// <summary>
        /// BaudRate of bluetooth module
        /// </summary>
        public BaudRate BaudRate
        {
            get => (BaudRate)(int.Parse(QueryResponse("AT+BAUD?").Response));
            set
            {
                QueryResponse("AT+BAUD" + value);
                ChangeSerial(value);
            }
        }

        /// <summary>
        /// Get Last Connected Address with module
        /// </summary>
        public string LastConnectedAddress => QueryResponse("AT+RADD?").Response;

        /// <summary>
        /// 
        /// </summary>
        public bool IsConnected
        {
            get
            {
                lock (_connectedLock)
                {
                    return _isConnected;
                }
            }
            private set
            {
                lock (_connectedLock)
                {
                    _isConnected = value;
                }
            }
        }

        /// <summary>
        /// Check if device is present(if it is not connected on master)
        /// </summary>
        public bool IsPresent => QueryResponse("AT").Response == "OK";

        /// <summary>
        /// True when module performs scanning of other bluetooth module
        /// </summary>
        public bool IsScanning
        {
            get
            {
                lock (_scanLock)
                {
                    return _isScanning;
                }
            }
            private set
            {
                lock (_scanLock)
                {
                    _isScanning = value;
                }
            }
        }

        /// <summary>
        /// If false, led is always off.
        /// </summary>
        public bool LedMode
        {
            get => QueryResponse("AT+LED?").Response == "0";
            set => QueryResponse("AT+LED" + (value ? "0" : "1"));
        }

        /// <summary>
        /// Name of bluetooth module
        /// </summary>
        public string Name
        {
            get => QueryResponse("AT+NAME?").Response;
            set => SetName(value);
        }

        /// <summary>
        /// Get notification when module is connected/disconnected
        /// </summary>
        public bool Notification
        {
            get => QueryResponse("AT+NOTI?").Response == "1";
            set => QueryResponse("AT+NOTI" + (value ? "1" : "0"));
        }

        /// <summary>
        /// Pin code of bluetooth module
        /// </summary>
        public string Pin
        {
            get => QueryResponse("AT+PIN?").Response;
            set => SetPin(value);
        }

        /// <summary>
        /// Get Version of module
        /// </summary>
        public string Version => QueryResponse("AT+VERSION?").Response;

        /// <summary>
        /// Restore default values of module (long task ~12sec)
        /// </summary>
        /// <returns>Result of query is in Response member</returns>
        public QueryResponse RestoreDefault()
        {
            _writer.WriteString("AT+DEFAULT");
            _writer.Store();
            ChangeSerial(BaudRate.BaudRate9600);
            Thread.Sleep(3000);
            var read = _reader.Load(30);
            var rep = _reader.ReadString(read);
            return TreatResponse(rep);
        }

        /// <summary>Sends data through the connection.</summary>
        /// <param name="message">String containing the data to be sent</param>
        public void SendString(string message)
        {
            if (!IsConnected) throw new InvalidOperationException("Module must be connected first");
            _writer.WriteString(message);
            _writer.Store();
        }

        /// <summary>Sends data through the connection.</summary>
        /// <param name="b">Byte containing the data to be sent</param>
        public void SendByte(byte b)
        {
            if (!IsConnected) throw new InvalidOperationException("Module must be connected first");
            _writer.WriteByte(b);
            _writer.Store();
        }

        /// <summary>Sends data through the connection.</summary>
        /// <param name="bytesArray">Bytes containing the data to be sent</param>
        public void SendBytes(byte[] bytesArray)
        {
            if (!IsConnected) throw new InvalidOperationException("Module must be connected first");
            _writer.WriteBytes(bytesArray);
            _writer.Store();
        }

        #region Master
        /// <summary>
        /// Set master mode of module
        /// </summary>
        public class Master
        {
            private readonly SerialBluetooth3 _bluetooth;

            internal Master(SerialBluetooth3 bluetooth)
            {
                _bluetooth = bluetooth;
                _bluetooth.QueryResponse("AT+IMME0");
                _bluetooth.QueryResponse("AT+AUTH1");
                _bluetooth.QueryResponse("AT+ROLEM");
                _bluetooth.QueryResponse("AT+RESTART");
                Thread.Sleep(1000);
                _bluetooth._readerThread.Start();
            }

            /// <summary>
            /// Launch performing scan of other bluetooth modules
            /// </summary>
            public void Scan()
            {
                _bluetooth.QueryResponse("AT+SCAN?");
            }

            /// <summary>
            /// 
            /// </summary>
            //public void Work()
            //{
            //    _bluetooth.QueryResponse("AT+WORK");
            //}

            public void Connect(string macAddress)
            {
                _bluetooth.QueryResponse("AT+LNK" + macAddress);
            }
        }

        /// <summary>Sets Bluetooth module to work in Master mode.</summary>
        public Master MasterMode
        {
            get
            {
                lock (_lockClientHost)
                {
                    if (_client != null) throw new InvalidOperationException("Cannot use both Client and Host modes for Bluetooth module");
                    return _host ?? (_host = new Master(this));
                }
            }
        }
        #endregion

        #region Slave
        /// <summary>
        /// Set slave mode of module
        /// </summary>
        public class Slave
        {
            internal Slave(SerialBluetooth3 bluetooth)
            {
                bluetooth.QueryResponse("AT+ROLES");
                bluetooth.QueryResponse("AT+RESTART");
                Thread.Sleep(1000);
                bluetooth._readerThread.Start();
            }

        }

        /// <summary>Sets Bluetooth module to work in Slave mode.</summary>
        public Slave SlavetMode
        {
            get
            {
                lock (_lockClientHost)
                {
                    if (_host != null) throw new InvalidOperationException("Cannot use both Client and Host modes for Bluetooth module");
                    return _client ?? (_client = new Slave(this));
                }
            }
        }

        #endregion

        #region Events
        #region DataReceived
        private DataReceivedHandler _onDataReceived;

        /// <summary>Represents the delegate used for the <see cref="DataReceived" /> event.</summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="data">Data received from the Bluetooth module</param>
        public delegate void DataReceivedHandler(SerialBluetooth3 sender, string data);

        /// <summary>Event raised when the bluetooth module changes its state.</summary>
        public event DataReceivedHandler DataReceived;

        /// <summary>Raises the <see cref="DataReceived" /> event.</summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="data">Data string received by the Bluetooth module</param>
        protected virtual void OnDataReceived(SerialBluetooth3 sender, string data)
        {
            if (_onDataReceived == null) _onDataReceived = OnDataReceived;
            DataReceived?.Invoke(sender, data);
        }
        #endregion DataReceived

        #region Scan Events
        /// <summary>Represents the delegate used for the <see cref="ScanStarted" /> and <see cref="ScanEnded" /> events.</summary>
        /// <param name="sender">The object that raised the event.</param>
        public delegate void ScanHandler(SerialBluetooth3 sender);

        /// <summary>Represents the delegate used for the <see cref="ScanDevice" /> event.</summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="macAddress">String which is mac Address of remote device</param>
        public delegate void ScanDeviceHandler(SerialBluetooth3 sender, string macAddress);

        private ScanHandler _onScanStarted;
        private ScanDeviceHandler _onScanDevice;
        private ScanHandler _onScanEnded;

        /// <summary>Event raised when scan by bluetooth module is started.</summary>
        public event ScanHandler ScanStarted;
        /// <summary>Event raised when scan by the bluetooth module found a new device.</summary>
        public event ScanDeviceHandler ScanDevice;
        /// <summary>Event raised when scan by bluetooth module is ended.</summary>
        public event ScanHandler ScanEnded;

        /// <summary>Raises the <see cref="ScanStarted" /> event.</summary>
        /// <param name="sender">The object that raised the event.</param>
        protected virtual void OnScanStarted(SerialBluetooth3 sender)
        {
            if (_onScanStarted == null) _onScanStarted = OnScanStarted;
            ScanStarted?.Invoke(sender);
        }

        /// <summary>Raises the <see cref="ScanDevice" /> event.</summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="macAddress">The mac address of remote device.</param>
        protected virtual void OnScanDevice(SerialBluetooth3 sender, string macAddress)
        {
            if (_onScanDevice == null) _onScanDevice = OnScanDevice;
            ScanDevice?.Invoke(sender, macAddress);
        }

        /// <summary>Raises the <see cref="ScanEnded" /> event.</summary>
        /// <param name="sender">The object that raised the event.</param>
        protected virtual void OnScanEnded(SerialBluetooth3 sender)
        {
            if (_onScanEnded == null) _onScanEnded = OnScanEnded;
            ScanEnded?.Invoke(sender);
        }


        #endregion // Scan Events
        #region Connection Events

        /// <summary>Represents the delegate used for the <see cref="Connected" />, the <see cref="ConnectionFailed" />, the <see cref="ConnectionStarted" /> and <see cref="Disconnected" /> events.</summary>
        /// <param name="sender">The object that raised the event.</param>
        public delegate void ConnectionHandler(SerialBluetooth3 sender);

        private ConnectionHandler _onConnected;
        private ConnectionHandler _onConnectionFailed;
        private ConnectionHandler _onConnectionStarted;
        private ConnectionHandler _onDisconnected;

        /// <summary>Event raised when the bluetooth module is connected.</summary>
        public event ConnectionHandler Connected;
        /// <summary>Event raised when the bluetooth module connection has failed.</summary>
        public event ConnectionHandler ConnectionFailed;
        /// <summary>Event raised when the bluetooth module connection is started.</summary>
        public event ConnectionHandler ConnectionStarted;
        /// <summary>Event raised when the bluetooth module is disconnected.</summary>
        public event ConnectionHandler Disconnected;

        /// <summary>Raises the <see cref="Connected" /> event.</summary>
        /// <param name="sender">The object that raised the event.</param>
        protected virtual void OnConnected(SerialBluetooth3 sender)
        {
            if (_onConnected == null) _onConnected = OnConnected;
            Connected?.Invoke(sender);
        }

        /// <summary>Raises the <see cref="ConnectionFailed" /> event.</summary>
        /// <param name="sender">The object that raised the event.</param>
        protected virtual void OnConnectionFailed(SerialBluetooth3 sender)
        {
            if (_onConnectionFailed == null) _onConnectionFailed = OnConnectionFailed;
            ConnectionFailed?.Invoke(sender);
        }

        /// <summary>Raises the <see cref="ConnectionStarted" /> event.</summary>
        /// <param name="sender">The object that raised the event.</param>
        protected virtual void OnConnectionStarted(SerialBluetooth3 sender)
        {
            if (_onConnectionStarted == null) _onConnectionStarted = OnConnectionStarted;
            ConnectionStarted?.Invoke(sender);
        }

        /// <summary>Raises the <see cref="Disconnected" /> event.</summary>
        /// <param name="sender">The object that raised the event.</param>
        protected virtual void OnDisconnected(SerialBluetooth3 sender)
        {
            if (_onDisconnected == null) _onDisconnected = OnDisconnected;
            Disconnected?.Invoke(sender);
        }
        #endregion // Connection Events

        #endregion // Events

        private void ChangeSerial(BaudRate baudRate)
        {
            if (baudRate == _baudRate)
                // Nothing to do
                return;
            CloseSerial();
            Thread.Sleep(1000);
            OpenSerial(_uart, baudRate);
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

        private static uint GetValue(BaudRate baudRate)
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

        private void OpenSerial(string idUart, BaudRate baudRate)
        {
            _serial = SerialDevice.FromId(idUart);
            _baudRate = baudRate;
            _serial.BaudRate = GetValue(baudRate);
            _serial.ReadTimeout = TimeSpan.FromMilliseconds(200);
            _reader = new DataReader(_serial.InputStream);
            _writer = new DataWriter(_serial.OutputStream);
        }
        private QueryResponse QueryResponse(string command)
        {
            if (IsConnected) throw new InvalidOperationException("Module is connected. It can't be configured.");
            _writer.WriteString(command);
            _writer.Store();
            Thread.Sleep(Delay);
            var read = _reader.Load(30);
            var rep = _reader.ReadString(read);
            var response = TreatResponse(rep);
            return response;
        }

        private void RunReaderThread()
        {
            string response;
            string delim = "OK+SCAN";
            string mac;
            int beginResponse, first;

            while (true)
            {
                response = "";
                while (_reader.Load(1) > 0)
                {
                    response = response + (char)_reader.ReadByte();
                    Thread.Sleep(1);
                }
                if (response.Length > 0)
                {
                    if (response == "OK+CONN")
                    {
                        IsConnected = true;
                        OnConnected(this);
                    }
                    else if (response == "OK+SCANE")
                    {
                        IsScanning = false;
                        OnScanEnded(this);
                    }
                    else if (response == "OK+SCANS")
                    {
                        IsScanning = true;
                        OnScanStarted(this);
                    }
                    else if (response.IndexOf("OK+SCAN") > -1)
                    {
                        while (response.Length > 18)
                        {
                            beginResponse = response.LastIndexOf(delim);
                            first = beginResponse + delim.Length;
                            mac = response.Substring(first + 1);
                            OnScanDevice(this, mac);
                            response = first > 0 ? response.Substring(0, beginResponse) : "";
                        }
                    }
                    else if (response == "OK+CONNS")
                    {
                        OnConnectionStarted(this);
                    }
                    else if (response == "OK+CONNF")
                    {
                        IsConnected = false;
                        OnConnectionFailed(this);
                    }
                    else if (response == "OK+LOST")
                    {
                        IsConnected = false;
                        OnDisconnected(this);
                    }
                    else
                    {
                        OnDataReceived(this, response);
                    }
                }

                Thread.Sleep(1);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private void SetName(string name)
        {
            if (name.Length > 12)
                throw new ArgumentOutOfRangeException(nameof(name), "Max length of name is 12.");

            QueryResponse("AT+NAME" + name);
        }

        private void SetPin(string pinCode)
        {
            if (pinCode.Length > 12)
                throw new ArgumentOutOfRangeException(nameof(pinCode), "Max length of name is 12.");
            QueryResponse("AT+PIN" + pinCode);
        }

        private static QueryResponse TreatResponse(string rep)
        {
            var response = new QueryResponse();
            if (rep.Length > 0)
            {
                var plusIndex = rep.IndexOf('+');
                var sepIndex = rep.IndexOf(':');
                if (plusIndex < sepIndex && plusIndex != -1)
                {
                    // Three params
                    response.Result = rep.Substring(0, plusIndex);
                    response.Type = rep.Substring(plusIndex + 1, sepIndex - plusIndex - 1);
                    response.Response = rep.Substring(sepIndex + 1, rep.Length - sepIndex - 1);
                }
                else if (sepIndex == -1 && plusIndex > -1)
                {
                    // Two params
                    response.Result = rep.Substring(0, plusIndex);
                    response.Type = rep.Substring(plusIndex + 1);
                    response.Response = "";
                }
                else if (rep == "OK")
                {
                    // One params
                    response.Response = rep;
                    response.Result = "";
                    response.Type = "";
                }
            }
            return response;
        }
    }
}
