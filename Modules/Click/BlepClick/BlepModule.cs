using System.Diagnostics;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Spi;

namespace BLEPBrainpad
{
    public delegate void DeviceStartedEvent(int length, byte eventCode, byte operatingMode, byte hwError, byte dataCreditAvailable);
    public delegate void CommandResponseEvent(int length, byte eventCode, byte commandOpCode, byte Status, byte[] data);

    public class BlepModule
    {
        private readonly GpioPin _reqn;
        private readonly GpioPin _rdy;
        private readonly GpioPin _reset;
        private readonly GpioPin _activate;
        private readonly SpiDevice _spi;

        public GpioPinValue Activate
        {
            get => _activate.Read();
        }

        public event DeviceStartedEvent OnDeviceStartedEvent;
        public event CommandResponseEvent OnCommandResponseEvent;

        public BlepModule(int resetPin, int reqnPin, int rdyPin, int activatePin, int misoPin, int mosiPin, int sckPin, int dummyPin)
        {
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
            Reset();
            _rdy.ValueChanged += _rdy_ValueChanged;
            Debug.WriteLine("RDY value: " + (_rdy.Read() == GpioPinValue.High ? "1" : "0"));

        }

        private void Reset()
        {
            _reset.Write(GpioPinValue.Low);
            Thread.Sleep(10);
            _reset.Write(GpioPinValue.High);
        }

        private void WaitForNrfReady(GpioPinValue pinValue)
        {
            while (_rdy.Read() != pinValue) { }
        }

        private void _rdy_ValueChanged(object sender, GpioPinValueChangedEventArgs e)
        {
            if (e.Edge == GpioPinEdge.FallingEdge)
                if (_reqn.Read() == GpioPinValue.High)
                    AciEventPending();
                else Debug.WriteLine("#Beginnig of command transmission#");
            else Debug.WriteLine("#End of transmission#");
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
            byte[] send=new byte[2];
            send[0] = 0x01;
            send[1] = 0x09;
            _reqn.Write(GpioPinValue.Low);
            WaitForNrfReady(GpioPinValue.Low);
            _spi.WriteLsb(send);
            _reqn.Write(GpioPinValue.High);
            WaitForNrfReady(GpioPinValue.High);
        }

        private void ProcessEvent(byte[] buff)
        {
            // first byte is event code
            byte eventCode = buff[0];
            switch (eventCode)
            {
                case 0x81:
                    OnDeviceStartedEvent?.Invoke(buff.Length,eventCode, buff[1], buff[2], buff[3]);
                    break;
                case 0x84:
                    OnCommandResponseEvent?.Invoke(buff.Length,eventCode,buff[1],buff[2],buff);
                    break;
                default:
                    Debug.WriteLine("Unimplemented event: 0x"+eventCode.ToString("X"));
                    break;
            }
        }
    }
}
