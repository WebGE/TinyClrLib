using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using BlepClick.Commands;
using BlepClick.Events;
using BLEPBrainpad;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Spi;

namespace BlepClick
{
    //public delegate void DeviceStartedEvent(int length, byte eventCode, byte operatingMode, byte hwError, byte dataCreditAvailable);
    //public delegate void CommandResponseEvent(int length, byte eventCode, byte commandOpCode, byte status, byte[] data);

    public delegate void AciEventReceivedEventHandler(AciEvent aciEvent);

    public delegate void DataReceivedEventHandler(DataReceivedEvent dataReceivedEvent);

    public class BlepModule
    {
        #region NewVersion

        private static readonly byte[] ReadEventLengthBuffer = new byte[2];
        private Queue _eventQueue;
        private byte[][] _setupData;
        private int _setupIndex;
        public event AciEventReceivedEventHandler AciEventReceived;
        public event DataReceivedEventHandler DataReceived;

        public bool Bonded { get; protected set; }
        public Nrf8001State State { get; protected set; }
        public byte DataCreditsAvailable { get; protected set; }
        protected ulong OpenPipesBitmap { get; set; }
        protected ulong ClosedPipesBitmap { get; set; }

        #endregion // NewVersion
        private readonly GpioPin _reqn;
        private readonly GpioPin _rdy;
        private readonly GpioPin _reset;
        private readonly GpioPin _activate;
        private readonly SpiDevice _spi;

        public GpioPinValue Activate
        {
            get => _activate.Read();
        }

        //public event DeviceStartedEvent OnDeviceStartedEvent;
        //public event CommandResponseEvent OnCommandResponseEvent;

        public BlepModule(int resetPin, int reqnPin, int rdyPin, int activatePin, int misoPin, int mosiPin, int sckPin, int dummyPin)
        {
            _setupIndex = 0;
            GpioController controller = GpioController.GetDefault();
            _reset = controller.OpenPin(resetPin);
            _reset.SetDriveMode(GpioPinDriveMode.Output);
            _reset.Write(GpioPinValue.High);

            _reqn = controller.OpenPin(reqnPin);
            _reqn.SetDriveMode(GpioPinDriveMode.Output);
            _reqn.Write(GpioPinValue.High);

            _rdy = controller.OpenPin(rdyPin);
            _rdy.SetDriveMode(GpioPinDriveMode.InputPullUp);

            _activate = controller.OpenPin(activatePin);
            _activate.SetDriveMode(GpioPinDriveMode.InputPullUp);

            SpiSoftwareProvider spp = new SpiSoftwareProvider(misoPin, mosiPin, sckPin);
            var controllers = SpiController.GetControllers(spp);
            _spi = controllers[0].GetDevice(new SpiConnectionSettings(dummyPin)
            {
                ClockFrequency = 100000,
                DataBitLength = 8,
                Mode = SpiMode.Mode0,
                SharingMode = SpiSharingMode.Exclusive
            });

            // Reset device
            State = Nrf8001State.Unknown;
            Reset();
            _rdy.ValueChanged += _rdy_ValueChanged;
            Debug.WriteLine("RDY value: " + (_rdy.Read() == GpioPinValue.High ? "1" : "0"));

        }

        private void Reset()
        {
            Bonded = false;
            State = Nrf8001State.Resetting;
            _reset.Write(GpioPinValue.Low);
            Thread.Sleep(10);
            _eventQueue = new Queue();
            _reset.Write(GpioPinValue.High);
        }

        public void ProcessEvents()
        {
            if (_eventQueue.Count == 0)
                return;
            ProcessEvent((byte[])_eventQueue.Dequeue());
        }

        private void ProcessEvent(byte[] content)
        {
            var eventType = (AciEventType)content[0];
            AciEvent aciEvent = null;

            switch (eventType)
            {
                case AciEventType.BondStatus:
                    aciEvent = new BondStatusEvent(content);
                    HandleBondStatusEvent((BondStatusEvent)aciEvent);
                    break;

                case AciEventType.CommandResponse:
                    aciEvent = new CommandResponseEvent(content);
                    HandleCommandResponseEvent((CommandResponseEvent)aciEvent);
                    break;

                case AciEventType.Connected:
                    aciEvent = new AciEvent(content);
                    State = Nrf8001State.Connected;
                    break;

                case AciEventType.DataCredit:
                    aciEvent = new DataCreditEvent(content);
                    HandleDataCreditEvent((DataCreditEvent)aciEvent);
                    break;

                case AciEventType.DataReceived:
                    aciEvent = new DataReceivedEvent(content);
                    HandleDataReceivedEvent((DataReceivedEvent)aciEvent);
                    break;

                case AciEventType.DeviceStarted:
                    aciEvent = new DeviceStartedEvent(content);
                    HandleDeviceStartedEvent((DeviceStartedEvent)aciEvent);
                    break;

                case AciEventType.Disconnected:
                    aciEvent = new AciEvent(content);
                    State = Nrf8001State.Standby;
                    break;

                case AciEventType.PipeStatus:
                    aciEvent = new AciEvent(content);
                    OpenPipesBitmap = aciEvent.Content.ToUnsignedLong(1);
                    ClosedPipesBitmap = aciEvent.Content.ToUnsignedLong(9);
                    break;

                default:
                    aciEvent = new AciEvent(content);
                    break;
            }

            Debug.WriteLine("Event: " + eventType.GetName());

            if (AciEventReceived != null)
                AciEventReceived(aciEvent);
        }

        private void HandleCommandResponseEvent(CommandResponseEvent aciEvent)
        {
            if (aciEvent.Command == AciOpCode.Setup)
            {
                if (aciEvent.StatusCode == AciStatusCode.TransactionContinue)
                    Setup(_setupData[_setupIndex++]);
                else if (aciEvent.StatusCode != AciStatusCode.TransactionComplete)
                    throw new Nrf8001Exception("Setup data invalid.");
            }
        }

        private void HandleBondStatusEvent(BondStatusEvent aciEvent)
        {
            if (aciEvent.StatusCode == BondStatusCode.Success)
                Bonded = true;
        }



        public void Setup(byte[][] setupData)
        {
            _setupData = setupData;
            _setupIndex = 0;
            while (State != Nrf8001State.Setup)
                ProcessEvents();
            Setup(_setupData[_setupIndex++]);
            while (State != Nrf8001State.Standby)
                ProcessEvents();
        }

        private void Setup(byte[] data)
        {
            AciSend(AciOpCode.Setup, data);
        }

        public void AwaitConnection(ushort timeout, ushort interval)
        {
            Connect(timeout, interval);
            while (true)
            {
                ProcessEvents();
                if (State == Nrf8001State.Connected)
                    return;
                else if (State == Nrf8001State.Standby)
                    Connect(timeout, interval);
            }
        }

        private void Connect(ushort timeout, ushort interval)
        {
            if (State != Nrf8001State.Standby)
                throw new InvalidOperationException("BlepClick is not in Standby mode.");
            if (timeout < 0x0001 || timeout > 0x3fff)
                throw new ArgumentOutOfRangeException("timeout", "Timeout must be between 0x0001 and 0X3fff inclusive.");
            if (interval < 0x0020 || interval > 0x4000)
                throw new ArgumentOutOfRangeException("interval", "Interval must be between 0x0020 and 0x4000 inclusive.");
            AciSend(AciOpCode.Connect, (byte)(timeout), (byte)(timeout >> 8), (byte)(interval), (byte)(interval >> 8));
            State = Nrf8001State.Connecting;
        }

        private void WaitForNrfReady(GpioPinValue pinValue)
        {
            while (_rdy.Read() != pinValue) { }
        }

        private byte[] AciReceived()
        {
            var readBuffer = new byte[2];
            _reqn.Write(GpioPinValue.Low);
            _spi.WriteReadLsb(ReadEventLengthBuffer, readBuffer);
            if (readBuffer[1] > 0)
            {
                readBuffer = new byte[readBuffer[1]];
                _spi.WriteReadLsb(new byte[readBuffer.Length], readBuffer);
            }
            _reqn.Write(GpioPinValue.High);

            return readBuffer;
        }

        private void _rdy_ValueChanged(object sender, GpioPinValueChangedEventArgs e)
        {
            if (e.Edge == GpioPinEdge.FallingEdge)
            {
                // if (_reqn.Read() == GpioPinValue.High)
                _eventQueue.Enqueue(AciReceived());
                // else Debug.WriteLine("#Beginnig of command transmission#");
            }
            // else Debug.WriteLine("#End of transmission#");
        }

        public void AwaitBond(ushort timeout, ushort interval)
        {
            Bond(timeout, interval);

            while (true)
            {
                ProcessEvents();

                if (Bonded)
                    return;
                else if (State == Nrf8001State.Standby)
                    Bond(timeout, interval);
            }
        }

        private void Bond(ushort timeout, ushort interval)
        {
            if (State != Nrf8001State.Standby)
                throw new InvalidOperationException("nRF8001 is not in Standby mode.");

            if (timeout < 0x0001 || timeout > 0x00B4)
                throw new ArgumentOutOfRangeException("timeout", "Timeout must be between 0x0001 and 0x00B4, inclusive.");

            if (interval < 0x0020 || interval > 0x4000)
                throw new ArgumentOutOfRangeException("interval", "Interval must be between 0x0020 and 0x4000, inclusive.");

            AciSend(AciOpCode.Bond, (byte)(timeout), (byte)(timeout >> 8), // Timeout
                (byte)(interval), (byte)(interval >> 8)); // Interval

            State = Nrf8001State.Bonding;
        }



        private void AciEventPending()
        {
            Debug.WriteLine("#Beginnig of event transmission#");
            byte[] debug = new byte[1];
            byte[] length = new byte[1];
            byte[] buff;
            _reqn.Write(GpioPinValue.Low);
            // read debug byte and discard
            _spi.ReadLsb(debug);
            _spi.ReadLsb(length);
            //Debug.WriteLine("Must be read: length:" + length[0]);
            buff = new byte[length[0]];
            _spi.ReadLsb(buff);
            //Debug.WriteLine("Event: 0x" + buff[0].ToString("X"));
            _reqn.Write(GpioPinValue.High);
            ProcessEvent(buff);
        }

        public void SendGetDeviceVersionCommand()
        {
            byte[] send = new byte[2];
            send[0] = 0x01;
            send[1] = 0x09;
            _reqn.Write(GpioPinValue.Low);
            WaitForNrfReady(GpioPinValue.Low);
            _spi.WriteLsb(send);
            _reqn.Write(GpioPinValue.High);
            WaitForNrfReady(GpioPinValue.High);
        }

        //private void ProcessEvent(byte[] buff)
        //{
        //    // first byte is event code
        //    byte eventCode = buff[0];
        //    switch (eventCode)
        //    {
        //        case 0x81:
        //            OnDeviceStartedEvent?.Invoke(buff.Length, eventCode, buff[1], buff[2], buff[3]);
        //            break;
        //        case 0x84:
        //            OnCommandResponseEvent?.Invoke(buff.Length, eventCode, buff[1], buff[2], buff);
        //            break;
        //        default:
        //            Debug.WriteLine("Unimplemented event: 0x" + eventCode.ToString("X"));
        //            break;
        //    }
        //}
        #region ACI Interface

        protected void AciSend(AciOpCode opCode, params byte[] data)
        {
            if (data.Length > 30)
                throw new ArgumentOutOfRangeException("data", "The maximum amount of data bytes is 30.");

            //Create ACI packet
            var packet = new byte[data.Length + 2];
            packet[0] = (byte)(data.Length + 1);
            packet[1] = (byte)opCode;
            Array.Copy(data, 0, packet, 2, data.Length);

            // Request transfer
            _rdy.SetDriveMode(GpioPinDriveMode.OutputOpenDrainPullUp);
            _reqn.Write(GpioPinValue.Low);
            while (_rdy.Read() == GpioPinValue.High) { }
            _spi.WriteLsb(packet);
            _reqn.Write(GpioPinValue.High);
            while (_rdy.Read() == GpioPinValue.Low) { }
            _rdy.SetDriveMode(GpioPinDriveMode.InputPullUp);
        }

        protected void AciSend(AciOpCode opCode, byte arg0, params byte[] data)
        {
            var buffer = new byte[data.Length + 1];
            buffer[0] = arg0;
            Array.Copy(data, 0, buffer, 1, data.Length);
            AciSend(opCode, buffer);
        }

        #endregion

        public void SendData(byte servicePipeId, params byte[] data)
        {
            if (servicePipeId < 1 || servicePipeId > 62)
                throw new ArgumentOutOfRangeException("servicePipeId", "Service pipe ID must be between 1 and 62 inclusive.");
            if (data.Length < 1 || data.Length > 20)
                throw new ArgumentOutOfRangeException("data", "data length must be between 1 and 20 inclusive.");
            if (State != Nrf8001State.Connected)
                throw new InvalidOperationException("BlepClick is not connected.");
            if (DataCreditsAvailable < 1)
                throw new InvalidOperationException("There are no data credits available.");
            if ((OpenPipesBitmap >> servicePipeId & 0x01) == 0)
                throw new InvalidOperationException("Service pipe is not open.");

            AciSend(AciOpCode.SendData, servicePipeId, data);
        }
    }
}
