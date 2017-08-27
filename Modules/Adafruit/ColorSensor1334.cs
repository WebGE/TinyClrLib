using System.Threading;
using GHIElectronics.TinyCLR.Devices.I2c;

namespace Modules.Adafruit
{
    public class Color
    {
        public ushort Clear;
        public ushort Red;
        public ushort Green;
        public ushort Blue;

        public Color(ushort c, ushort r, ushort g, ushort b)
        {
            Clear = c;
            Red = r;
            Green = g;
            Blue = b;
        }
    }

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
        public enum IntegrationTime : byte
        {
            It_2_4ms = 0xFF,
            It_24ms = 0xF6,
            It_50ms = 0xEB,
            It_101ms = 0xD5,
            It_154ms = 0xC0,
            It_700ms = 0x00
        }
        #endregion
        #region Gain
        public enum Gain : byte
        {
            Gain1X = 0x00,   //  No gain
            Gain4X = 0x01,   //  4x gain
            Gain16X = 0x02,   //  16x gain
            Gain60X = 0x03    //  60x gain
        }
        #endregion

        private const int Address = 0x29;

        private readonly I2cDevice _colorSensorDevice;
        private Gain _gain;
        private Color _color;

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

        public void SetIntegrationTime(IntegrationTime it)
        {
            WriteRegister(Register.RegisterTimeIntegration, (byte)it);
            _it = it;
        }

        public void SetGain(Gain gain)
        {
            WriteRegister(Register.RegisterControl, (byte)gain);
            _gain = gain;
        }

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
