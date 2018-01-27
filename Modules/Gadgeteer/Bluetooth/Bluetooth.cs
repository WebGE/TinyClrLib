using System;
using System.Diagnostics;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.SerialCommunication;
using GHIElectronics.TinyCLR.Storage.Streams;
// ReSharper disable StringIndexOfIsCultureSpecific.1
// ReSharper disable StringIndexOfIsCultureSpecific.2
// ReSharper disable UnusedMember.Global
// ReSharper disable EventNeverSubscribedTo.Global

namespace Bauland.Gadgeteer
{
    /// <summary>
    /// Wrapper class for Bluetooth Gadgeteer Module
    /// </summary>
    public class Bluetooth
    {
        private readonly GpioPin _reset;
        private readonly GpioPin _statusInt;
        private readonly DataReader _dataReader;
        private readonly DataWriter _dataWriter;

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

        /// <summary>Constructs a new instance.</summary>
        /// <param name="pinStatus">pin to get status of module (usually pin 3 of U Socket)</param>
        /// <param name="pinReset">pin to reset module (usually pin 6 of U Socket)</param>
        /// <param name="serialId">string which represent Id of serial port of socket (usually pin 4 and 5 of U Socket)</param>
        public Bluetooth(int pinStatus, int pinReset, string serialId)
        {
            _reset = GpioController.GetDefault().OpenPin(pinReset, GpioSharingMode.Exclusive);
            _reset.SetDriveMode(GpioPinDriveMode.Output);
            _reset.Write(GpioPinValue.Low);

            _statusInt = GpioController.GetDefault().OpenPin(pinStatus, GpioSharingMode.Exclusive);
            _statusInt.SetDriveMode(GpioPinDriveMode.Input);

            var serialPort = SerialDevice.FromId(serialId);
            serialPort.BaudRate = 38400;
            serialPort.Parity = SerialParity.None;
            serialPort.StopBits = SerialStopBitCount.One;
            serialPort.DataBits = 8;
            serialPort.ReadTimeout = TimeSpan.Zero;
            _dataReader=new DataReader(serialPort.InputStream);
            _dataWriter=new DataWriter(serialPort.OutputStream);

            //this.statusInt.Interrupt += GTI.InterruptInputFactory.Create.InterruptEventHandler(statusInt_Interrupt);

            Thread.Sleep(5);
            _reset.Write(GpioPinValue.High);

            var readerThread = new Thread(RunReaderThread);
            readerThread.Start();
            Thread.Sleep(500);
        }

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
        /// <param name="baud">value of new speed</param>
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
                _dataWriter.WriteString("\r\n+STBD=" + cmd + "\r\n");
            Thread.Sleep(500);
        }

        /// <summary>Sets the PIN code for the Bluetooth module</summary>
        /// <param name="pinCode"></param>
        public void SetPinCode(string pinCode)
        {
            _dataWriter.WriteString("\r\n+STPIN=" + pinCode + "\r\n");

        }

        /// <summary>Thread that continuously reads incoming messages from the module, parses them and triggers the corresponding events.</summary>
        private void RunReaderThread()
        {
            while (true)
            {
                String response = "";
                var i = _dataReader.Load(1);
                while (i > 0)
                {
                    response = response + (char)_dataReader.ReadByte();
                    i = _dataReader.Load(1);
                }
                if (response.Length > 0)
                {
                    Debug.WriteLine(response);

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
                            var j = _dataReader.Load(1);
                            while (j > 0)
                            {
                                response = response + (char)_dataReader.ReadByte();
                                j = _dataReader.Load(1);
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
            // ReSharper disable once FunctionNeverReturns
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
                Debug.WriteLine("Client Mode");
                _bluetooth = bluetooth;
                bluetooth._dataWriter.WriteString("\r\n+STWMOD=0\r\n");
            }

            /// <summary>Enters pairing mode</summary>
            public void EnterPairingMode()
            {
                Debug.WriteLine("Enter Pairing Mode");
                _bluetooth._dataWriter.WriteString("\r\n+INQ=1\r\n");
            }

            /// <summary>Inputs pin code</summary>
            /// <param name="pinCode">Module's pin code. Default: 0000</param>
            public void InputPinCode(string pinCode)
            {
                Debug.WriteLine("Inputting pin: " + pinCode);
                _bluetooth._dataWriter.WriteString("\r\n+RTPIN=" + pinCode + "\r\n");
            }

            /// <summary>Closes current connection. Doesn't work yet.</summary>
            public void Disconnect()
            {
                Debug.WriteLine("Disconnection is not working...");
                //NOT WORKING
                // Documentation states that in order to disconnect, we pull PIO0 HIGH,
                // but this pin is not available in the socket... (see schematics)
            }

            /// <summary>Sends data through the connection.</summary>
            /// <param name="message">String containing the data to be sent</param>
            public void Send(string message)
            {
                Debug.WriteLine("Sending: " + message);
                _bluetooth._dataWriter.WriteString(message);
            }

            /// <summary>Sends data through the connection.</summary>
            /// <param name="message">String containing the data to be sent</param>
            public void SendLine(string message)
            {
                Debug.WriteLine("Sending: " + message);
                _bluetooth._dataWriter.WriteString(message);
            }
        }

        /// <summary>Implements the host functionality for the Bluetooth module</summary>
        public class Host
        {
            private readonly Bluetooth _bluetooth;

            internal Host(Bluetooth bluetooth)
            {
                Debug.WriteLine("Host mode");
                _bluetooth = bluetooth;
                bluetooth._dataWriter.WriteString("\r\n+STWMOD=1\r\n");
            }

            /// <summary>Starts inquiring for devices</summary>
            public void InquireDevice()
            {
                Debug.WriteLine("Inquiring device");
                _bluetooth._dataWriter.WriteString("\r\n+INQ=1\r\n");
            }

            /// <summary>Makes a connection with a device using its MAC address.</summary>
            /// <param name="macAddress">MAC address of the device</param>
            public void Connect(string macAddress)
            {
                Debug.WriteLine("Connecting to: " + macAddress);
                _bluetooth._dataWriter.WriteString("\r\n+CONN=" + macAddress + "\r\n");
            }

            /// <summary>Inputs the PIN code.</summary>
            /// <param name="pinCode">PIN code. Default 0000</param>
            public void InputPinCode(string pinCode)
            {
                Debug.WriteLine("Inputting pin: " + pinCode);
                _bluetooth._dataWriter.WriteString("\r\n+RTPIN=" + pinCode + "\r\n");
            }

            /// <summary>Closes the current connection. Doesn't work yet.</summary>
            public void Disconnect()
            {
                Debug.WriteLine("Disconnection is not working...");
                //NOT WORKING
                // Documentation states that in order to disconnect, we pull PIO0 HIGH,
                // but this pin is not available in the socket... (see schematics)
            }
        }
    }
}
