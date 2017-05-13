using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.Enumeration;
using GHIElectronics.TinyCLR.Devices.I2c;

namespace I2cGroveColor
{
    public class I2cGroveColor
    {
        private const byte DeviceAddress = 0x39;
        private const byte RegistryBlockRead = 0xd0;

        private I2cDevice _i2c;

        public I2cGroveColor(string i2c)
        {
            var devices = DeviceInformation.FindAll(i2c);
            I2cConnectionSettings ics = new I2cConnectionSettings(DeviceAddress) { BusSpeed = I2cBusSpeed.FastMode, SharingMode = I2cSharingMode.Shared };
            _i2c = I2cDevice.FromId(devices[0].Id, ics);
            WriteReg(0x80, 0x03);
        }

        public Color GetRgbColor()
        {
            Color color = new Color();

            Debug.WriteLine("## ID: 0x" + ReadReg(0x84).ToString("X"));
            Debug.WriteLine("## CTL: 0x" + ReadReg(0x80).ToString("X"));
            Thread.Sleep(15);
            Debug.WriteLine("## CTL: 0x" + ReadReg(0x80).ToString("X"));
            //ReadByte(Address, Command, DataLow) //Result returned in DataLow
            //Command = 0x91 //Address the Ch1 upper data register
            //ReadByte(Address, Command, DataHigh) //Result returned in DataHigh
            //Channel1 = 256 * DataHigh + DataLow //Shift DataHigh to upper byte
            //Command = 0x92 //Address the Ch2 lower data register
            //ReadByte(Address, Command, DataLow) //Result returned in DataLow
            //Command = 0x93 //Address the Ch2 upper data register
            //ReadByte(Address, Command, DataHigh) //Result returned in DataHigh
            //Channel2 = 256 * DataHigh + DataLow //Shift DataHigh to upper byte
            //Command = 0x94 //Address the Ch3 lower data register
            //ReadByte(Address, Command, DataLow) //Result returned in DataLow
            //Command = 0x95 //Address the Ch3 upper data register
            //ReadByte(Address, Command, DataHigh) //Result returned in DataHigh
            //Channel3 = 256 * DataHigh + DataLow //Shift DataHigh to upper byte
            //Command = 0x96 //Address the Ch4 lower data register
            //ReadByte(Address, Command, DataLow) //Result returned in DataLow
            //Command = 0x97 //Address the Ch4 upper data register
            //ReadByte(Address, Command, DataHigh) //Result returned in DataHigh
            //Channel4 = 256 * DataHigh + DataLow //Shift DataHigh to upper byte
            return color;
        }

        private byte ReadReg(byte reg)
        {
            byte[] writeBuffer = new byte[1];
            writeBuffer[0] = reg;
            byte[] readBuffer = new byte[1];
            var tr = _i2c.WritePartial(writeBuffer);
            Debug.WriteLine("Write #: " + tr.BytesTransferred + ", :" + (tr.Status == I2cTransferStatus.FullTransfer ? "OK" : "Erreur") + " , :0x" + writeBuffer[0].ToString("X"));
            tr = _i2c.ReadPartial(readBuffer);
            Debug.WriteLine("Read #: " + tr.BytesTransferred + ", :" + (tr.Status == I2cTransferStatus.FullTransfer ? "OK" : "Erreur") + " , :0x" + readBuffer[0].ToString("X"));
            return readBuffer[0];
        }
        private void WriteReg(byte reg, byte data)
        {
            byte[] writeBuffer = new byte[] { reg, data };
            var tr = _i2c.WritePartial(writeBuffer);
            Debug.WriteLine("Write #: " + tr.BytesTransferred + ", :" + (tr.Status == I2cTransferStatus.FullTransfer ? "OK" : "Erreur") + " , :0x" + writeBuffer[0].ToString("X"));
        }
    }
}
