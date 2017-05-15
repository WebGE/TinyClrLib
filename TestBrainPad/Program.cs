using System.Diagnostics;
using System.Threading;
using BLEPBrainpad;

partial class Program
{
    public void BrainPadSetup()
    {
        //Put your setup code here. It runs once when the BrainPad starts up.
        BlepModule blepModule=new BlepModule(
            GHIElectronics.TinyCLR.Pins.BrainPad.GpioPin.Rst,
            GHIElectronics.TinyCLR.Pins.BrainPad.GpioPin.Cs,
            GHIElectronics.TinyCLR.Pins.BrainPad.GpioPin.Int,
            GHIElectronics.TinyCLR.Pins.BrainPad.GpioPin.An,
            GHIElectronics.TinyCLR.Pins.BrainPad.GpioPin.Miso,
            GHIElectronics.TinyCLR.Pins.BrainPad.GpioPin.Mosi,
            GHIElectronics.TinyCLR.Pins.BrainPad.GpioPin.Sck,
            GHIElectronics.TinyCLR.Pins.BrainPad.GpioPin.Pwm
            );

        blepModule.OnCommandResponseEvent += BlepModule_OnCommandResponseEvent;
        blepModule.OnDeviceStartedEvent += BlepModuleOnOnDeviceStartedEvent;
        //byte[] buff = new byte[] { 0x02, 0x02, 0xa7 };
        //_reqn.Write(GpioPinValue.Low);
        //WaitForNrfReady(GpioPinValue.Low);
        //spi.Write(buff);
        //_reqn.Write(GpioPinValue.High);
        //WaitForNrfReady(GpioPinValue.High);
        Thread.Sleep(1000);
        blepModule.SendGetDeviceVersionCommand();

    }

    private void BlepModule_OnCommandResponseEvent(int length, byte eventCode, byte commandOpCode, byte status, byte[] data)
    {
        Debug.WriteLine("New event _OnCommandResponseEvent: 0x" + eventCode.ToString("X") + " , OP code: 0x" + commandOpCode.ToString("X") + " , Status: 0x" + status);
        for (int i = 0; i < data.Length; i++)
        {
            Debug.WriteLine(i.ToString("X") + ": 0x" + data[i].ToString("X"));
        }
    }

    private void BlepModuleOnOnDeviceStartedEvent(int length, byte eventCode, byte operatingMode, byte hwError, byte dataCreditAvailable)
    {
        Debug.WriteLine("New event OnDeviceStartedEvent: " + eventCode.ToString("X"));
        //        throw new NotImplementedException();
    }



    public void BrainPadLoop()
    {
        //Put your program code here. It runs repeatedly after the BrainPad starts up.

    }
}
