using System;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.SerialCommunication;
using GHIElectronics.TinyCLR.Storage.Streams;
// ReSharper disable StringIndexOfIsCultureSpecific.1
// ReSharper disable StringIndexOfIsCultureSpecific.2
// ReSharper disable FunctionNeverReturns
// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
// ReSharper disable UnusedMember.Global
// ReSharper disable EventNeverSubscribedTo.Global

// Driver contributed by TinyCLR communitity members Eduardo Velloso (@eduardo.velloso) and Brett Pound (@Brett).
namespace Bauland.Gadgeteer
{
    /// <summary>A Bluetooth module for Microsoft .NET Gadgeteer</summary>
    public class Bluetooth
    {

        /// <summary>
        /// Direct access to Serial Port.
        /// </summary>

        private readonly SerialDevice _serialPort;

        private readonly DataReader _dataReader;
        private readonly DataWriter _dataWriter;

        private readonly GpioPin _reset;
        private readonly GpioPin _statusInt;

        private readonly Thread _readerThread;

        private readonly object _lock = new Object();

        private Client _client;

        private Host _host;

        /// <summary>Gets a value that indicates whether the bluetooth connection is connected.</summary>
        public bool IsConnected => _statusInt.Read() == GpioPinValue.High;

        /// <summary>Sets Bluetooth module to work in Client mode.</summary>
        public Client ClientMode
        {
            get
            {
                lock (_lock)
                {
                    if (_host != null) throw new InvalidOperationException("Cannot use both Client and Host modes for Bluetooth module");
                    return _client ?? (_client = new Client(this));
                }
            }
        }

        /// <summary>Sets Bluetooth module to work in Host mode.</summary>
        public Host HostMode
        {
            get
            {
                lock (_lock)
                {
                    if (_client != null) throw new InvalidOperationException("Cannot use both Client and Host modes for Bluetooth module");
                    return _host ?? (_host = new Host(this));
                }
            }
        }

        /// <summary>Possible states of the Bluetooth module</summary>
        public enum BluetoothState
        {

            /// <summary>Module is initializing</summary>
            Initializing = 0,

            /// <summary>Module is ready</summary>
            Ready = 1,

            /// <summary>Module is in pairing mode</summary>
            Inquiring = 2,

            /// <summary>Module is making a connection attempt</summary>
            Connecting = 3,

            /// <summary>Module is connected</summary>
            Connected = 4,

            /// <summary>Module is diconnected</summary>
            Disconnected = 5
        }

        /// <summary>
        /// Construct Bluetooth module
        /// </summary>
        /// <param name="pinStatus">GpioPin Pin 3 of U Socket</param>
        /// <param name="pinReset">GpioPin Pin 6 of U Socket</param>
        /// <param name="uartId">Uart Id of U Socket</param>
        public Bluetooth(int pinStatus, int pinReset, string uartId)
        {
            // This finds the Socket instance from the user-specified socket number. This will generate user-friendly error messages if the socket is invalid. If there is more than one socket on this
            // module, then instead of "null" for the last parameter, put text that identifies the socket to the user (e.g. "S" if there is a socket type S)
            // Socket socket = Socket.GetSocket(socketNumber, true, this, null);

            //this.reset = GTI.DigitalOutputFactory.Create(socket, Socket.Pin.Six, false, this);
            _reset = GpioController.GetDefault().OpenPin(pinReset);
            _reset.SetDriveMode(GpioPinDriveMode.Output);
            _reset.Write(GpioPinValue.Low);

            // this.statusInt = GTI.InterruptInputFactory.Create(socket, Socket.Pin.Three, GTI.GlitchFilterMode.Off, GTI.ResistorMode.Disabled, GTI.InterruptMode.RisingAndFallingEdge, this);
            _statusInt = GpioController.GetDefault().OpenPin(pinStatus);
            _statusInt.SetDriveMode(GpioPinDriveMode.Input);
            _statusInt.DebounceTimeout = TimeSpan.FromMilliseconds(1);

            // this.serialPort = GTI.SerialFactory.Create(socket, 38400, GTI.SerialParity.None, GTI.SerialStopBits.One, 8, GTI.HardwareFlowControl.NotRequired, this);
            _serialPort = SerialDevice.FromId(uartId);
            _serialPort.BaudRate = 38400;
            _serialPort.Parity = SerialParity.None;
            _serialPort.StopBits = SerialStopBitCount.One;
            _serialPort.Handshake = SerialHandshake.None;
            _serialPort.ReadTimeout = TimeSpan.MaxValue;

            //this.statusInt.Interrupt += GTI.InterruptInputFactory.Create.InterruptEventHandler(statusInt_Interrupt);
            //           this.serialPort.ReadTimeout = Timeout.Infinite;
            //           this.serialPort.Open();

            _dataReader = new DataReader(_serialPort.InputStream);
            _dataWriter = new DataWriter(_serialPort.OutputStream);

            Thread.Sleep(5);
            _reset.Write(GpioPinValue.High);

            _readerThread = new Thread(RunReaderThread);
            _readerThread.Start();
            Thread.Sleep(500);
        }

        // Note: A constructor summary is auto-generated by the doc builder.
        ///// <summary></summary>
        ///// <param name="socketNumber">The socket that this module is plugged in to.</param>
        ///// <param name="baud">The baud rate to communicate with the module with.</param>
        //public Bluetooth(int socketNumber, long baud)
        //{
        //    // This finds the Socket instance from the user-specified socket number. This will generate user-friendly error messages if the socket is invalid. If there is more than one socket on this
        //    // module, then instead of "null" for the last parameter, put text that identifies the socket to the user (e.g. "S" if there is a socket type S)
        //    Socket socket = Socket.GetSocket(socketNumber, true, this, null);

        //    this.reset = GTI.DigitalOutputFactory.Create(socket, Socket.Pin.Six, false, this);
        //    this.statusInt = GTI.InterruptInputFactory.Create(socket, Socket.Pin.Three, GTI.GlitchFilterMode.Off, GTI.ResistorMode.Disabled, GTI.InterruptMode.RisingAndFallingEdge, this);
        //    this.serialPort = GTI.SerialFactory.Create(socket, 38400, GTI.SerialParity.None, GTI.SerialStopBits.One, 8, GTI.HardwareFlowControl.NotRequired, this);

        //    //this.statusInt.Interrupt += GTI.InterruptInputFactory.Create.InterruptEventHandler(statusInt_Interrupt);
        //    this.serialPort.ReadTimeout = Timeout.Infinite;
        //    this.serialPort.Open();

        //    Thread.Sleep(5);
        //    this.reset.Write(true);

        //    // Poundy added:
        //    Thread.Sleep(5);
        //    this.SetDeviceBaud(baud);
        //    this.serialPort.Flush();
        //    this.serialPort.Close();
        //    this.serialPort.BaudRate = (int)baud;
        //    this.serialPort.Open();
        //    // Poundy

        //    readerThread = new Thread(new ThreadStart(runReaderThread));
        //    readerThread.Start();
        //    Thread.Sleep(500);
        //}

        /// <summary>Hard Reset Bluetooth module</summary>
        public void Reset()
        {
            _reset.Write(GpioPinValue.Low);
            Thread.Sleep(5);
            _reset.Write(GpioPinValue.High);
        }

        /// <summary>Sets the device name as seen by other devices</summary>
        /// <param name="name">Name of the device</param>
        public void SetDeviceName(string name)
        {
            _dataWriter.WriteString("\r\n+STNA=" + name + "\r\n");
            _dataWriter.Store();
        }

        /// <summary>Switch the device to the directed speed</summary>
        /// <param name="baud">Speed of device</param>
        public void SetDeviceBaud(long baud)
        {
            string cmd;
            switch (baud)
            {
                case 9600:
                    cmd = "9600";
                    break;

                case 19200:
                    cmd = "19200";
                    break;

                case 38400:
                    cmd = "38400";
                    break;

                case 57600:
                    cmd = "57600";
                    break;

                case 115200:
                    cmd = "115200";
                    break;

                case 230400:
                    cmd = "230400";
                    break;

                case 460800:
                    cmd = "460800";
                    break;

                default:
                    cmd = "";
                    break;
            }

            if (cmd != "")
            {
                _dataWriter.WriteString("\r\n+STBD=" + cmd + "\r\n");
                _dataWriter.Store();
            }
            Thread.Sleep(500);
        }

        /// <summary>Sets the PIN code for the Bluetooth module</summary>
        /// <param name="pinCode"></param>
        public void SetPinCode(string pinCode)
        {
            _dataWriter.WriteString("\r\n+STPIN=" + pinCode + "\r\n");
            _dataWriter.Store();
        }

        /// <summary>Thread that continuously reads incoming messages from the module, parses them and triggers the corresponding events.</summary>
        private void RunReaderThread()
        {
            while (true)
            {
                String response = "";
                while (_dataReader.Load(1) > 0)
                {
                    response = response + (char)_dataReader.ReadByte();
                    Thread.Sleep(1);
                }
                if (response.Length > 0)
                {
                    //Check Bluetooth State Changed
                    if (response.IndexOf("+BTSTATE:") > -1)
                    {
                        string atCommand = "+BTSTATE:";

                        //String parsing
                        // Return format: +COPS:<mode>[,<format>,<oper>]
                        int first = response.IndexOf(atCommand) + atCommand.Length;
                        int last = response.IndexOf("\n", first);
                        int state = int.Parse(((response.Substring(first, last - first)).Trim()));

                        OnBluetoothStateChanged(this, (BluetoothState)state);
                    }
                    //Check Pin Requested
                    if (response.IndexOf("+INPIN") > -1)
                    {
                        // EDUARDO : Needs testing
                        OnPinRequested(this);
                    }
                    if (response.IndexOf("+RTINQ") > -1)
                    {
                        //EDUARDO: Needs testing

                        string atCommand = "+RTINQ=";
                        //String parsing
                        int first = response.IndexOf(atCommand) + atCommand.Length;
                        int mid = response.IndexOf(";", first);
                        int last = response.IndexOf("\r", first);

                        // Keep reading until the end of the message
                        while (last < 0)
                        {
                            while (_dataReader.Load(1) > 0)
                            {
                                response = response + (char)_dataReader.ReadByte();
                            }
                            last = response.IndexOf("\r", first);
                        }

                        string address = ((response.Substring(first, mid - first)).Trim());

                        string name = (response.Substring(mid + 1, last - mid));

                        OnDeviceInquired(this, address, name);
                        //Debug.Print("Add: " + address + ", Name: " + name );
                    }
                    else
                    {
                        OnDataReceived(this, response);
                    }
                }
                Thread.Sleep(1);  //poundy changed from thread.sleep(10)
            }
        }

        #region DELEGATES AND EVENTS

        #region Bluetooth State Changed
        private BluetoothStateChangedHandler _onBluetoothStateChanged;

        /// <summary>Represents the delegate used for the <see cref="BluetoothStateChanged" /> event.</summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="btState">Current state of the Bluetooth module</param>
        public delegate void BluetoothStateChangedHandler(Bluetooth sender, BluetoothState btState);

        /// <summary>Event raised when the bluetooth module changes its state.</summary>
        public event BluetoothStateChangedHandler BluetoothStateChanged;

        /// <summary>Raises the <see cref="BluetoothStateChanged" /> event.</summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="btState">Current state of the Bluetooth module</param>
        protected virtual void OnBluetoothStateChanged(Bluetooth sender, BluetoothState btState)
        {
            if (_onBluetoothStateChanged == null) _onBluetoothStateChanged = OnBluetoothStateChanged;
            BluetoothStateChanged?.Invoke(sender, btState);
        }

        #endregion Bluetooth State Changed

        #region DataReceived
        private DataReceivedHandler _onDataReceived;

        /// <summary>Represents the delegate used for the <see cref="DataReceived" /> event.</summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="data">Data received from the Bluetooth module</param>
        public delegate void DataReceivedHandler(Bluetooth sender, string data);

        /// <summary>Event raised when the bluetooth module changes its state.</summary>
        public event DataReceivedHandler DataReceived;

        /// <summary>Raises the <see cref="DataReceived" /> event.</summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="data">Data string received by the Bluetooth module</param>
        protected virtual void OnDataReceived(Bluetooth sender, string data)
        {
            if (_onDataReceived == null) _onDataReceived = OnDataReceived;
            DataReceived?.Invoke(sender, data);
        }

        #endregion DataReceived

        #region PinRequested
        private PinRequestedHandler _onPinRequested;

        /// <summary>Represents the delegate used for the <see cref="PinRequested" /> event.</summary>
        /// <param name="sender">The object that raised the event.</param>
        public delegate void PinRequestedHandler(Bluetooth sender);

        /// <summary>Event raised when the bluetooth module changes its state.</summary>
        public event PinRequestedHandler PinRequested;

        /// <summary>Raises the <see cref="PinRequested" /> event.</summary>
        /// <param name="sender">The object that raised the event.</param>
        protected virtual void OnPinRequested(Bluetooth sender)
        {
            if (_onPinRequested == null) _onPinRequested = OnPinRequested;
            PinRequested?.Invoke(sender);
        }

        #endregion PinRequested

        #endregion DELEGATES AND EVENTS

        #region DeviceInquired
        private DeviceInquiredHandler _onDeviceInquired;

        /// <summary>Represents the delegate used for the <see cref="DeviceInquired" /> event.</summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="macAddress">MAC Address of the inquired device</param>
        /// <param name="name">Name of the inquired device</param>
        public delegate void DeviceInquiredHandler(Bluetooth sender, string macAddress, string name);

        /// <summary>Event raised when the bluetooth module changes its state.</summary>
        public event DeviceInquiredHandler DeviceInquired;

        /// <summary>Raises the <see cref="PinRequested" /> event.</summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="macAddress">MAC Address of the inquired device</param>
        /// <param name="name">Name of the inquired device</param>
        protected virtual void OnDeviceInquired(Bluetooth sender, string macAddress, string name)
        {
            if (_onDeviceInquired == null) _onDeviceInquired = OnDeviceInquired;
            DeviceInquired?.Invoke(sender, macAddress, name);
        }

        #endregion DeviceInquired
        /// <summary>Client functionality for the Bluetooth module</summary>
        public class Client
        {
            private readonly Bluetooth _bluetooth;

            internal Client(Bluetooth bluetooth)
            {
                _bluetooth = bluetooth;
                _bluetooth._dataWriter.WriteString("\r\n+STWMOD=0\r\n");
                _bluetooth._dataWriter.Store();
            }

            /// <summary>Enters pairing mode</summary>
            public void EnterPairingMode()
            {
                _bluetooth._dataWriter.WriteString("\r\n+INQ=1\r\n");
                _bluetooth._dataWriter.Store();
            }

            /// <summary>Inputs pin code</summary>
            /// <param name="pinCode">Module's pin code. Default: 0000</param>
            public void InputPinCode(string pinCode)
            {
                _bluetooth._dataWriter.WriteString("\r\n+RTPIN=" + pinCode + "\r\n");
                _bluetooth._dataWriter.Store();
            }

            /// <summary>Closes current connection. Doesn't work yet.</summary>
            public void Disconnect()
            {
                //NOT WORKING
                // Documentation states that in order to disconnect, we pull PIO0 HIGH,
                // but this pin is not available in the socket... (see schematics)
            }

            /// <summary>Sends data through the connection.</summary>
            /// <param name="message">String containing the data to be sent</param>
            public void Send(string message)
            {
                _bluetooth._dataWriter.WriteString(message);
                _bluetooth._dataWriter.Store();
            }

            /// <summary>Sends data through the connection.</summary>
            /// <param name="message">String containing the data to be sent</param>
            public void SendLine(string message)
            {
                _bluetooth._dataWriter.WriteString(message);
                _bluetooth._dataWriter.Store();
            }
        }

        /// <summary>Implements the host functionality for the Bluetooth module</summary>
        public class Host
        {
            private readonly Bluetooth _bluetooth;

            internal Host(Bluetooth bluetooth)
            {
                _bluetooth = bluetooth;
                bluetooth._dataWriter.WriteString("\r\n+STWMOD=1\r\n");
                bluetooth._dataWriter.Store();
            }

            /// <summary>Starts inquiring for devices</summary>
            public void InquireDevice()
            {
                _bluetooth._dataWriter.WriteString("\r\n+INQ=1\r\n");
                _bluetooth._dataWriter.Store();
            }

            /// <summary>Makes a connection with a device using its MAC address.</summary>
            /// <param name="macAddress">MAC address of the device</param>
            public void Connect(string macAddress)
            {
                _bluetooth._dataWriter.WriteString("\r\n+CONN=" + macAddress + "\r\n");
                _bluetooth._dataWriter.Store();
            }

            /// <summary>Inputs the PIN code.</summary>
            /// <param name="pinCode">PIN code. Default 0000</param>
            public void InputPinCode(string pinCode)
            {
                _bluetooth._dataWriter.WriteString("\r\n+RTPIN=" + pinCode + "\r\n");
                _bluetooth._dataWriter.Store();
            }

            /// <summary>Closes the current connection. Doesn't work yet.</summary>
            public void Disconnect()
            {
                //NOT WORKING
                // Documentation states that in order to disconnect, we pull PIO0 HIGH,
                // but this pin is not available in the socket... (see schematics)
            }
        }
    }
}