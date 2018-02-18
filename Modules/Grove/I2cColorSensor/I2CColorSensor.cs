using System;
using GHIElectronics.TinyCLR.Devices.I2c;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global

namespace Bauland.Grove
{
    /// <summary>
    /// Wrapper for I2CColorSensor
    /// </summary>
    public class I2CColorSensor
    {
        private const byte DeviceAddress = 0x39;
        private int _red, _green, _blue, _clear;
        private readonly I2cDevice _i2C;

        /// <summary>
        /// Get or set white led status: true if led is on, false if led if off
        /// </summary>
        public bool LedStatus { get; set; }

        private enum Transaction
        {
            Byte = 0x00 << 5,
            Word = 0x01 << 5,
            Block = 0x02 << 5,
            InterruptClear = 0x03 << 5
        }

        private enum Register
        {
            Control = 0x80,
            Timing = 0x81,
            Interrupt = 0x82,
            InterruptSource = 0x83,
            Id = 0x84,
            Gain = 0x87,
            LowThresholdLowByte = 0x88,
            LowThresholdHighByte = 0x89,
            HighThresholdLowByte = 0x8A,
            HighThresholdHighByte = 0x8B,
            BlockRead = 0xD0,
            GreebLow = 0xD0,
            GreenHigh = 0xD1,
            RedLow = 0xD2,
            RedHigh = 0xD3,
            BlueLow = 0xD4,
            BlueHigh = 0xD5,
            ClearLow = 0xD6,
            ClearHigh = 0xD7,
            ClearInterrupt = 0xe0
        }

        private enum ControlRegister
        {
            ControlDatInitiate = 0x03
        }

        [Flags]
        private enum TimingRegister
        {
            SyncEdge = 0x40,
            IntegModeFree = 0x00,
            IntegModeManual = 0x10,
            IntegModeSynSingle = 0x20,
            IntegModeSynMulti = 0x30,
            IntegParamPulseCount1 = 0x00,
            IntegParamPulseCount2 = 0x01,
            IntegParamPulseCount4 = 0x02,
            IntegParamPulseCount8 = 0x03
        }

        [Flags]
        private enum InterruptControlRegister
        {
            InterruptStop = 0x40,
            InterruptDisable = 0x00,
            InterruptLevel = 0x10,
            InterruptPersistEvery = 0x00,
            InterruptPersistSingle = 0x01
        }

        private enum InterruptSourceRegister
        {
            InterruptSourceGreen = 0x00,
            InterruptSourceRed = 0x01,
            InterruptSourceBlue = 0x10,
            InterruptSourceClear = 0x03
        }

        [Flags]
        private enum GainRegister
        {
            Gain1 = 0x00,
            Gain4 = 0x10,
            Gain16 = 0x20,
            Gain64 = 0x30,
            Prescaler1 = 0x00,
            Prescaler2 = 0x01,
            Prescaler4 = 0x02,
            Prescaler8 = 0x03,
            Prescaler16 = 0x04,
            Prescaler32 = 0x05,
            Prescaler64 = 0x06,
        }

        /// <summary>
        /// Constructor of I2CColorSensor
        /// </summary>
        /// <param name="i2CId">string of I2C identifier</param>
        public I2CColorSensor(string i2CId)
        {
            I2cConnectionSettings ics = new I2cConnectionSettings(DeviceAddress) { BusSpeed = I2cBusSpeed.FastMode, SharingMode = I2cSharingMode.Shared };
            _i2C = I2cDevice.FromId(i2CId, ics);
            Init();
            // WriteReg(Register.Control, 0x03);
        }

        private void Init()
        {
            // Set TimingReg
            WriteReg(Register.Timing, (byte)(TimingRegister.IntegModeFree | TimingRegister.IntegParamPulseCount1));
            // Set Interrupt source
            WriteReg(Register.InterruptSource, (byte)InterruptSourceRegister.InterruptSourceClear);
            // Set Interrupt control
            WriteReg(Register.Interrupt, (byte)(InterruptControlRegister.InterruptLevel | InterruptControlRegister.InterruptPersistEvery));
            // Set Gain
            WriteReg(Register.Gain, (byte)(GainRegister.Gain1 | GainRegister.Prescaler1));
            // Set Enable ADC
            WriteReg(Register.Control, (byte)ControlRegister.ControlDatInitiate);
        }

        /// <summary>
        /// Get raw data from sensor
        /// </summary>
        /// <returns>Return an array of values: R,G,B,C</returns>
        public int[] GetRawData()
        {
            byte[] data = ReadBlock(Register.BlockRead);
            _green = data[1] * 256 + data[0];
            _red = data[3] * 256 + data[2];
            _blue = data[5] * 256 + data[4];
            _clear = data[7] * 256 + data[6];

            return new[] { _red, _green, _blue, _clear };
        }

        /// <summary>
        /// Get ARGB Component of measure
        /// </summary>
        /// <returns> Array of 4 bytes indicates values for ARGB components</returns>
        public int[] GetArgb()
        {
            byte[] data = ReadBlock(Register.BlockRead);
            _green = data[1] * 256 + data[0];
            _red = data[3] * 256 + data[2];
            _blue = data[5] * 256 + data[4];
            _clear = data[7] * 256 + data[6];

            double tmp;
            int maxColor;

            if (!LedStatus)
            {
                maxColor = Math.Max(_red, _green);
                maxColor = Math.Max(maxColor, _blue);
                if (maxColor > 0)
                {
                    tmp = 250.0 / maxColor;
                }
                else
                {
                    tmp = 0;
                }

                _green = (int)(_green * tmp);
                _red *= (int)(_red * tmp);
                _blue *= (int)(_blue * tmp);
            }
            if (LedStatus)
            {
                _red = (int)(_red * 1.70);
                _blue = (int)(_blue * 1.35);

                maxColor = Math.Max(_red, _green);
                maxColor = Math.Max(maxColor, _blue);
                if (maxColor > 255)
                {
                    tmp = 250.0 / maxColor;
                    _green = (int)(_green * tmp);
                    _red *= (int)(_red * tmp);
                    _blue *= (int)(_blue * tmp);
                }


                maxColor = Math.Max(_red, _green);
                maxColor = Math.Max(maxColor, _blue);

                int greenTmp = _green;
                int redTmp = _red;
                int blueTmp = _blue;

                // When LED is turn ON, it is almost white color, so adjust RGB data
                if (_red < 0.8 * maxColor && _red >= 0.6 * maxColor)
                {
                    _red = (int)(_red * 0.4);
                }
                else if (_red < 0.6 * maxColor)
                {
                    _red = (int)(_red * 0.2);

                }
                if (_green < 0.8 * maxColor && _green >= 0.6 * maxColor)
                {
                    _green = (int)(_green * 0.4);
                }
                else if (_green < 0.6 * maxColor)
                {
                    if (maxColor == redTmp && greenTmp >= 2 * blueTmp && greenTmp >= 0.2 * redTmp)
                    {
                        _green *= 5;
                    }
                    _green = (int)(_green * 0.2);
                }
                if (_blue < 0.8 * maxColor && _blue >= 0.6 * maxColor)
                {
                    _blue = (int)(_blue * 0.4);
                }
                else if (_blue < 0.6 * maxColor)
                {
                    if (maxColor == redTmp && greenTmp >= 2 * blueTmp && greenTmp >= 0.2 * redTmp)
                    {
                        _blue = (int)(_blue * 0.5);
                    }
                    if (maxColor == redTmp && greenTmp <= blueTmp && blueTmp >= 0.2 * redTmp)
                    {
                        _blue *= 5;
                    }
                    _blue = (int)(_blue * 0.2);
                }

                var minColor = Math.Min(_red, _green);
                minColor = Math.Min(minColor, _blue);
                if (maxColor == _green && _red >= 0.85 * maxColor && minColor == _blue)
                {
                    _red = maxColor;
                    _blue = (int)(_blue * 0.4);
                }
            }
            return new[] { _red, _green, _blue, _clear };
        }

        private byte ReadReg(Register reg)
        {
            byte[] writeBuffer = new byte[1];
            writeBuffer[0] = (byte)reg;
            byte[] readBuffer = new byte[1];
            _i2C.Write(writeBuffer);
            _i2C.Read(readBuffer);
            return readBuffer[0];
        }

        private byte[] ReadBlock(Register reg)
        {
            byte[] writeBuffer = new byte[1];
            writeBuffer[0] = (byte)reg;
            byte[] readBuffer = new byte[9];
            _i2C.Write(writeBuffer);
            _i2C.Read(readBuffer);
            return readBuffer;
        }

        private void WriteReg(Register reg, byte data)
        {
            byte[] writeBuffer = new byte[] { (byte)reg, data };
            _i2C.Write(writeBuffer);
        }

        /// <summary>
        /// Clear interrupt
        /// </summary>
        public void ClearInterrupt()
        {
            byte[] buffer ={
                (byte)Register.ClearInterrupt
            };
            _i2C.WritePartial(buffer);
        }
    }
}
