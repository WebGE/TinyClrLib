using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.I2c;
using GHIElectronics.TinyCLR.Pins;

namespace SeeedGrove
{
    public enum DayOfWeek { Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday }
    public class Rtc
    {
        private readonly I2cDevice _rtcDevice;
        public byte Seconds;
        public byte Minutes;
        public byte Hours;
        public byte Day;
        public DayOfWeek DayOfWeek;
        public byte Month;
        public ushort Year;
        public bool AMMode;


        public Rtc(string i2c)
        {
            var settings = new I2cConnectionSettings(0x68);
            settings.BusSpeed = I2cBusSpeed.FastMode;
            _rtcDevice = I2cDevice.FromId(i2c, settings);
        }

        private byte DecToBcd(byte b)
        {
            return (byte)((b / 10 * 16) + (b % 10));
        }

        private byte BcdToDec(byte b)
        {
            return (byte)((b / 16 * 10) + (b % 16));
        }

        private ushort DecToBcd(ushort b)
        {
            return (ushort)((b / 10 * 16) + (b % 10));
        }

        private ushort BcdToDec(ushort b)
        {
            return (ushort)((b / 16 * 10) + (b % 16));
        }

        public void StartClock()
        {
            GetTime();
            byte[] writedByte = new byte[8];
            writedByte[0] = 0x00;
            writedByte[1] = (byte)(DecToBcd(Seconds) & 0x7f);
            writedByte[2] = DecToBcd(Minutes);
            writedByte[3] = (byte)(DecToBcd(Hours) + (AMMode ? 0x40 : 0x00));
            writedByte[4] = (byte)DayOfWeek;
            writedByte[5] = DecToBcd(Day);
            writedByte[6] = DecToBcd(Month);
            writedByte[7] = DecToBcd((byte)(Year - 2000));
            _rtcDevice.Write(writedByte);
        }

        public void StopClock()
        {
            GetTime();
            byte[] writedByte = new byte[8];
            writedByte[0] = 0x00;
            writedByte[1] = (byte)(DecToBcd(Seconds) | 0x80);
            writedByte[2] = DecToBcd(Minutes);
            writedByte[3] = (byte)(DecToBcd(Hours) + (AMMode ? 0x40 : 0x00));
            writedByte[4] = (byte)DayOfWeek;
            writedByte[5] = DecToBcd(Day);
            writedByte[6] = DecToBcd(Month);
            writedByte[7] = DecToBcd((byte)(Year - 2000));
            _rtcDevice.Write(writedByte);
        }

        public void GetTime()
        {
            byte[] readBuffer = new byte[7];
            _rtcDevice.Write(new byte[] { 0x00 });
            _rtcDevice.Read(readBuffer);
            DisplayRead(readBuffer);
            Seconds = BcdToDec((byte)(readBuffer[0] & 0x7f));
            Minutes = BcdToDec(readBuffer[1]);
            Hours = BcdToDec((byte)(readBuffer[2] & 0x3f));
            AMMode = (readBuffer[2] & 0x40) == 1;
            DayOfWeek = (DayOfWeek)readBuffer[3];
            Day = BcdToDec(readBuffer[4]);
            Month = BcdToDec(readBuffer[5]);
            Year = (ushort) (BcdToDec(readBuffer[6]) + 2000);
        }

        public void SetTime()
        {
            //TODO: To complete
        }
        private void DisplayRead(byte[] readBuffer)
        {
            Debug.WriteLine("Read result:");
            foreach (byte b in readBuffer)
            {
                Debug.WriteLine("0x" + b.ToString("X"));
            }
        }
    }
}
