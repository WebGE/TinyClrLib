using System.Threading;
using GHIElectronics.TinyCLR.Devices.I2c;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable NotAccessedField.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
// ReSharper disable NotAccessedField.Local

namespace Bauland.Adafruit
{
    /// <summary>
    /// Represent color object
    /// </summary>
    public class Color
    {
        /// <summary>
        /// Clear component of color
        /// </summary>
        public ushort Clear;
        /// <summary>
        /// Red component of color
        /// </summary>
        public ushort Red;
        /// <summary>
        /// Green component of color
        /// </summary>
        public ushort Green;
        /// <summary>
        /// Blue component of color
        /// </summary>
        public ushort Blue;

        /// <summary>
        /// Constructor of Color object
        /// </summary>
        /// <param name="c">Clear component of color</param>
        /// <param name="r">Red component of color</param>
        /// <param name="g">Green component of color</param>
        /// <param name="b">Blue component of color</param>
        public Color(ushort c, ushort r, ushort g, ushort b)
        {
            Clear = c;
            Red = r;
            Green = g;
            Blue = b;
        }
    }

    /// <summary>
    /// Wrapper class for ColorSensor1334 Module
    /// </summary>
    public class ColorSensor1334
    {
        private IntegrationTime _it;

        #region Register
        private static class Register
        {
            public const byte RegisterEnable = 0x00;
            public const byte RegisterTimeIntegration = 0x01;
            public const byte RegisterId = 0x12;
            public const byte RegisterControl = 0x0f;
            public const byte RegisterClearDataLow = 0x14;
            public const byte RegisterRedDataLow = 0x16;
            public const byte RegisterGreenDataLow = 0x18;
            public const byte RegisterBlueDataLow = 0x1A;
        }
        #endregion

        #region Bit
        private static class Bit
        {
            public const byte BitPowerOn = 0x01;
            public const byte BitAen = 0x02;
            public const byte BitCommand = 0x80;
        }
        #endregion

        #region IntegrationTime
        /// <summary>
        /// Time for sensor to capture color
        /// </summary>
        public enum IntegrationTime : byte
        {
            /// <summary>
            /// 2.4 ms of Integration Time
            /// </summary>
            It_2_4ms = 0xFF,
            /// <summary>
            /// 24 ms of Integration Time
            /// </summary>
            It_24ms = 0xF6,
            /// <summary>
            /// 50 ms of Integration Time
            /// </summary>
            It_50ms = 0xEB,
            /// <summary>
            /// 101 ms of Integration Time
            /// </summary>
            It_101ms = 0xD5,
            /// <summary>
            /// 154 ms of Integration Time
            /// </summary>
            It_154ms = 0xC0,
            /// <summary>
            /// 700 ms of Integration Time
            /// </summary>
            It_700ms = 0x00
        }
        #endregion
        #region Gain
        /// <summary>
        /// Gain of sensor
        /// </summary>
        public enum Gain : byte
        {
            /// <summary>
            /// No gain
            /// </summary>
            Gain1X = 0x00,
            /// <summary>
            /// 4x gain
            /// </summary>
            Gain4X = 0x01,  
            /// <summary>
            /// 16x gain
            /// </summary>
            Gain16X = 0x02,
            /// <summary>
            /// 60x gain
            /// </summary>
            Gain60X = 0x03 
        }
        #endregion

        private const int Address = 0x29;

        private readonly I2cDevice _colorSensorDevice;
        private Gain _gain;

        /// <summary>
        /// Constructor of ColorSensor
        /// </summary>
        /// <param name="i2CBus">string which represent I2C bus</param>
        public ColorSensor1334(string i2CBus)
        {
            var settings = new I2cConnectionSettings(Address)
            {
                BusSpeed = I2cBusSpeed.FastMode
            };
            _colorSensorDevice = I2cDevice.FromId(i2CBus, settings);
            SetIntegrationTime(IntegrationTime.It_2_4ms);
            SetGain(Gain.Gain1X);
            Enable(true);
        }

        /// <summary>
        /// Set Integration Time for sensor
        /// </summary>
        /// <param name="it">Value of integration</param>
        public void SetIntegrationTime(IntegrationTime it)
        {
            WriteRegister(Register.RegisterTimeIntegration, (byte)it);
            _it = it;
        }

        /// <summary>
        /// Set gain of sensor
        /// </summary>
        /// <param name="gain">Value of gain</param>
        public void SetGain(Gain gain)
        {
            WriteRegister(Register.RegisterControl, (byte)gain);
            _gain = gain;
        }

        /// <summary>
        /// Get Id of sensor
        /// </summary>
        /// <returns>Id of sensor</returns>
        public byte GetId()
        {
            var readBuffer = ReadRegister(Bit.BitCommand | Register.RegisterId);
            return readBuffer;
        }

        private byte ReadRegister(byte registerAddress)
        {
            byte[] writeBuffer = new byte[] { registerAddress };
            byte[] readBuffer = new byte[1];
            _colorSensorDevice?.WriteRead(writeBuffer, readBuffer);

            return readBuffer[0];
        }

        private ushort ReadRegister16(byte registerAddress)
        {
            byte[] writeBuffer = new byte[] { (byte)(Bit.BitCommand | registerAddress) };
            byte[] readBuffer = new byte[2];
            _colorSensorDevice?.WriteRead(writeBuffer, readBuffer);

            return (ushort)(readBuffer[0] + (readBuffer[1] << 8));
        }

        private void WriteRegister(byte registerAddress, byte value)
        {
            byte[] writeBuffer = new byte[] { registerAddress, value };
            _colorSensorDevice.Write(writeBuffer);
        }

        /// <summary>
        /// Retrieve raw data of capture
        /// </summary>
        /// <returns>Raw color</returns>
        public Color GetRawData()
        {
            var c = ReadRegister16(Register.RegisterClearDataLow);
            var r = ReadRegister16(Register.RegisterRedDataLow);
            var g = ReadRegister16(Register.RegisterGreenDataLow);
            var b = ReadRegister16(Register.RegisterBlueDataLow);
            switch (_it)
            {
                case IntegrationTime.It_2_4ms:
                    Thread.Sleep(3);
                    break;
                case IntegrationTime.It_24ms:
                    Thread.Sleep(24);
                    break;
                case IntegrationTime.It_50ms:
                    Thread.Sleep(50);
                    break;
                case IntegrationTime.It_101ms:
                    Thread.Sleep(101);
                    break;
                case IntegrationTime.It_154ms:
                    Thread.Sleep(54);
                    break;
                case IntegrationTime.It_700ms:
                    Thread.Sleep(700);
                    break;
            }
            return new Color(c, r, g, b);
        }

        /// <summary>
        /// Enable sensor
        /// </summary>
        /// <param name="enable">true to enable, false to disable</param>
        public void Enable(bool enable)
        {
            if (enable)
            {
                WriteRegister(Register.RegisterEnable, Bit.BitPowerOn);
                Thread.Sleep(3);
                WriteRegister(Register.RegisterEnable, Bit.BitPowerOn | Bit.BitAen);
            }
            else
            {
                var reg = ReadRegister(Register.RegisterEnable);
                WriteRegister(Register.RegisterEnable, (byte)(reg & ~(Bit.BitAen | Bit.BitPowerOn)));
            }
        }
    }
}

