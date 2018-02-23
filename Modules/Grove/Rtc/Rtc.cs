using GHIElectronics.TinyCLR.Devices.I2c;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Bauland.Grove
{
    /// <summary>
    /// Days of the week which are represented in Rtc
    /// </summary>
    public enum DayOfWeek
    {
        /// <summary> Monday </summary>
        Monday,
        /// <summary> Tuesday </summary>
        Tuesday,
        /// <summary> Wednesday </summary>
        Wednesday,
        /// <summary> Thursday </summary>
        Thursday,
        /// <summary> Friday </summary>
        Friday,
        /// <summary> Saturday </summary>
        Saturday,
        /// <summary> Sunday </summary>
        Sunday
    }

    /// <summary>
    /// Wrapper for Grove Rtc module
    /// </summary>
    public class Rtc
    {
        private readonly I2cDevice _rtcDevice;
        /// <summary>
        /// Seconds 
        /// </summary>
        public byte Seconds;
        /// <summary>
        /// Minutes
        /// </summary>
        public byte Minutes;
        /// <summary>
        /// Hours
        /// </summary>
        public byte Hours;
        /// <summary>
        /// Day
        /// </summary>
        public byte Day;
        /// <summary>
        /// Day of the week
        /// </summary>
        public DayOfWeek DayOfWeek;
        /// <summary>
        /// Month
        /// </summary>
        public byte Month;
        /// <summary>
        /// Year
        /// </summary>
        public ushort Year;
        /// <summary>
        /// Set true to use AM/PM mode, false to use 24 Hrs mode
        /// </summary>
        public bool AmPmMode;

        /// <summary>
        /// Constructor of Rtc component
        /// </summary>
        /// <param name="i2C">String that represent i2c bus</param>
        public Rtc(string i2C)
        {
            var settings = new I2cConnectionSettings(0x68)
            {
                BusSpeed = I2cBusSpeed.FastMode
            };
            _rtcDevice = I2cDevice.FromId(i2C, settings);
        }

        private byte DecToBcd(byte b)
        {
            return (byte)((b / 10 * 16) + (b % 10));
        }

        /// <summary>
        /// Get string to display number to 2 digits
        /// </summary>
        /// <param name="number">Number to display</param>
        /// <returns>string that contains 2 digits</returns>
        public static string To2Digit(int number)
        {
            return number < 10 ? ("0" + number) : number.ToString();
        }

        private byte BcdToDec(byte b)
        {
            return (byte)((b / 16 * 10) + (b % 16));
        }

        /// <summary>
        /// Indicates if rtc has been reset (lost of power)
        /// </summary>
        /// <returns>true if rtc power has been lost, else false</returns>
        public bool HasBeenReset()
        {
            byte[] readBuffer = new byte[1];
            _rtcDevice.Write(new byte[] { 0x00 });
            _rtcDevice.Read(readBuffer);
            return (readBuffer[0] & 0x80) != 0;

        }

        /// <summary>
        /// Retrieve time store in rtc
        /// </summary>
        public void GetTime()
        {
            byte[] readBuffer = new byte[7];
            _rtcDevice.Write(new byte[] { 0x00 });
            _rtcDevice.Read(readBuffer);
            Seconds = BcdToDec((byte)(readBuffer[0] & 0x7f));
            Minutes = BcdToDec(readBuffer[1]);
            Hours = BcdToDec((byte)(readBuffer[2] & 0x3f));
            AmPmMode = (readBuffer[2] & 0x40) == 1;
            DayOfWeek = (DayOfWeek)readBuffer[3];
            Day = BcdToDec(readBuffer[4]);
            Month = BcdToDec(readBuffer[5]);
            Year = (ushort)(BcdToDec(readBuffer[6]) + 2000);
        }

        /// <summary>
        /// Set time in rtc using value of this object
        /// </summary>
        public void SetTime()
        {
            byte[] writedByte = new byte[8];
            writedByte[0] = 0x00;
            writedByte[1] = (byte)(DecToBcd(Seconds) & 0x7f);
            writedByte[2] = DecToBcd(Minutes);
            writedByte[3] = (byte)(DecToBcd(Hours) + (AmPmMode ? 0x40 : 0x00));
            writedByte[4] = (byte)DayOfWeek;
            writedByte[5] = DecToBcd(Day);
            writedByte[6] = DecToBcd(Month);
            writedByte[7] = DecToBcd((byte)(Year - 2000));
            _rtcDevice.Write(writedByte);
        }

        /// <summary>
        /// Return the string representing the day of the week
        /// </summary>
        /// <param name="d">Day of the week to represent by a string</param>
        /// <returns>String representing day of week</returns>
        public static string GetDayOfWeek(DayOfWeek d)
        {
            switch (d)
            {
                case DayOfWeek.Friday:
                    return "Vendredi";
                case DayOfWeek.Monday:
                    return "Lundi";
                case DayOfWeek.Saturday:
                    return "Samedi";
                case DayOfWeek.Sunday:
                    return "Dimanche";
                case DayOfWeek.Thursday:
                    return "Mardi";
                case DayOfWeek.Tuesday:
                    return "Jeudi";
                case DayOfWeek.Wednesday:
                    return "Mercredi";
                default:
                    return "Unknow day";
            }
        }
    }
}
