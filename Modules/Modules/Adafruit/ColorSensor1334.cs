using System.Threading;
using GHIElectronics.TinyCLR.Devices.I2c;

namespace Modules.Adafruit
{
    public class ColorSensor1334
    {
        private IntegrationTime _it;

        #region Register
        private static class Register
        {
            public const byte RegisterEnable = 0x00;
            public const byte RegisterTimeIntegration = 0x01;
            public const byte RegisterId = 0x12;
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

        private const int Address = 0x29;

        private readonly I2cDevice _colorSensorDevice;

        public ColorSensor1334(string i2CBus)
        {
            var settings = new I2cConnectionSettings(Address)
            {
                BusSpeed = I2cBusSpeed.FastMode
            };
            _colorSensorDevice = I2cDevice.FromId(i2CBus, settings);
            SetIntegrationTime(IntegrationTime.It_2_4ms);
            SetGain();
            Enable(true);
        }

        private void SetIntegrationTime(IntegrationTime it)
        {
            WriteRegister(Register.RegisterTimeIntegration, (byte)it);
            _it = it;
        }

        private void SetGain()
        {
            throw new System.NotImplementedException();
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

        private void WriteRegister(byte registerAddress, byte value)
        {
            byte[] writeBuffer = new byte[] { registerAddress, value };
            _colorSensorDevice.Write(writeBuffer);
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
