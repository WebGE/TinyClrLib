using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.Enumeration;
using GHIElectronics.TinyCLR.Devices.I2c;

namespace I2cGroveColor
{
    // ReSharper disable once InconsistentNaming
    public class I2cGroveColor
    {
        private const byte DeviceAddress = 0x39;
        private int _red, _green, _blue;
        private int _clear;

        public bool LedStatus { get; set; }


        private I2cDevice _i2c;

        private byte CommandRegister(Register reg, Transaction tr)
        {
            int result = 0x80;

            result += (int)reg;
            result += (int)tr;
            return (byte)result;

        }

        // ReSharper disable once InconsistentNaming
        public I2cGroveColor(string i2c)
        {
            var devices = DeviceInformation.FindAll(i2c);
            I2cConnectionSettings ics = new I2cConnectionSettings(DeviceAddress) { BusSpeed = I2cBusSpeed.FastMode, SharingMode = I2cSharingMode.Shared };
            _i2c = I2cDevice.FromId(devices[0].Id, ics);
            WriteReg(Register.Control, 0x03);
        }

        public Color Test()
        {
            Color color = new Color();

            Debug.WriteLine("## ID: 0x" + ReadReg(Register.ID).ToString("X"));
            Debug.WriteLine("## CTL: 0x" + ReadReg(Register.Control).ToString("X"));
            Thread.Sleep(15);
            Debug.WriteLine("## CTL: 0x" + ReadReg(Register.Control).ToString("X"));
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

        public byte[] GetARGB()
        {
            byte[] data = ReadBlock(Register.Data1High);
            _green = data[1] * 256 + data[0];
            _red = data[3] * 256 + data[2];
            _blue = data[5] * 256 + data[4];
            _clear = data[7] * 256 + data[6];
            _red = Normalize(_red);
            _red = (int)((_red - 7.0) * 255.0 / 87.0);
            _green = Normalize(_green);
            _green = (int)((_green - 7.0) * 255.0 / 89.0);
            _blue = Normalize(_blue);
            _blue = (int)((_blue - 30.0) * 255.0 / 64.0);
            // Normalize
            return new byte[] { (byte)(_red), (byte)(_green), (byte)(_blue), Normalize(_clear) };
        }

        private byte Normalize(int valueToNormalize)
        {
            return (byte)(valueToNormalize * 255.0 / 65535.0);
        }

        private byte ReadReg(Register reg)
        {
            Debug.WriteLine("## Read register: " + reg);
            byte[] writeBuffer = new byte[1];
            writeBuffer[0] = CommandRegister(reg, Transaction.Byte);
            byte[] readBuffer = new byte[1];
            var tr = _i2c.WritePartial(writeBuffer);
            Debug.WriteLine("Write #: " + tr.BytesTransferred + ", :" + (tr.Status == I2cTransferStatus.FullTransfer ? "OK" : "Erreur") + " , :0x" + writeBuffer[0].ToString("X"));
            tr = _i2c.ReadPartial(readBuffer);
            Debug.WriteLine("Read #: " + tr.BytesTransferred + ", :" + (tr.Status == I2cTransferStatus.FullTransfer ? "OK" : "Erreur") + " , :0x" + readBuffer[0].ToString("X"));
            return readBuffer[0];
        }

        private byte[] ReadBlock(Register reg)
        {
            Debug.WriteLine("## Read register: " + reg);
            byte[] writeBuffer = new byte[1];
            writeBuffer[0] = CommandRegister(reg, Transaction.Block);
            byte[] readBuffer = new byte[9];
            var tr = _i2c.WritePartial(writeBuffer);
            Debug.WriteLine("Write #: " + tr.BytesTransferred + ", :" + (tr.Status == I2cTransferStatus.FullTransfer ? "OK" : "Erreur") + " , :0x" + writeBuffer[0].ToString("X"));
            tr = _i2c.ReadPartial(readBuffer);
            Debug.WriteLine("Read #: " + tr.BytesTransferred + ", :" + (tr.Status == I2cTransferStatus.FullTransfer ? "OK" : "Erreur") + " , :0x" + readBuffer[0].ToString("X"));
            return readBuffer;
        }

        private void WriteReg(Register reg, byte data)
        {
            Debug.WriteLine("## write 0x" + data.ToString("X") + " to register: " + reg);
            byte[] writeBuffer = new byte[] { CommandRegister(reg, Transaction.Byte), data };
            var tr = _i2c.WritePartial(writeBuffer);
            Debug.WriteLine("Write #: " + tr.BytesTransferred + ", :" + (tr.Status == I2cTransferStatus.FullTransfer ? "OK" : "Erreur") + " , :0x" + writeBuffer[0].ToString("X"));
        }
    }
}
