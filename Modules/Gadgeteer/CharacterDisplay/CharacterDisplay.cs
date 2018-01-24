using System.Threading;
using GHIElectronics.TinyCLR.Devices.Gpio;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Bauland.Gadgeteer
{
    /// <summary>
    /// Wrapper class for CharacterDisplay Gadgeteer Module
    /// </summary>
    public class CharacterDisplay
    {
        private const byte DispOn = 0x0C;
        private const byte ClrDisp = 1;
        private const byte CurHome = 2;
        private const byte SetCursor = 0x80;
        private static readonly byte[] RowOffsets = new byte[] { 0x00, 0x40, 0x14, 0x54 };
        private readonly GpioPin _lcdRs;
        private readonly GpioPin _lcdE;

        private readonly GpioPin _lcdD4;
        private readonly GpioPin _lcdD5;
        private readonly GpioPin _lcdD6;
        private readonly GpioPin _lcdD7;

        private readonly GpioPin _backlight;

        private int _currentRow;

        /// <summary>Whether or not the backlight is enabled.</summary>
        public bool BacklightEnabled
        {
            get => _backlight.Read() == GpioPinValue.High;

            set => _backlight.Write(value ? GpioPinValue.High : GpioPinValue.Low);
        }

        /// <summary>
        /// Constructor of CharacterDisplay
        /// </summary>
        /// <param name="pinLcdEnable">Enable pin, pin 3 of Y Socket</param>
        /// <param name="pinLcdReset">Reset pin, pin 4 of Y Socket</param>
        /// <param name="pinLcdD4">D4 pin, pin 5 of Y Socket</param>
        /// <param name="pinLcdD7">D7 pin, pin 6 of Y Socket</param>
        /// <param name="pinLcdD5">D5 pin, pin 7 of Y Socket</param>
        /// <param name="pinBacklight">Backlight pin, pin 8 of Y Socket</param>
        /// <param name="pinLcdD6">D6 pin, pin 9 of Y Socket</param>
        public CharacterDisplay(int pinLcdEnable, int pinLcdReset, int pinLcdD4, int pinLcdD7, int pinLcdD5, int pinBacklight, int pinLcdD6)
        {

            _lcdE = GpioController.GetDefault().OpenPin(pinLcdEnable, GpioSharingMode.Exclusive);
            _lcdE.SetDriveMode(GpioPinDriveMode.Output);

            _lcdRs = GpioController.GetDefault().OpenPin(pinLcdReset, GpioSharingMode.Exclusive);
            _lcdRs.SetDriveMode(GpioPinDriveMode.Output);

            _lcdD4 = GpioController.GetDefault().OpenPin(pinLcdD4, GpioSharingMode.Exclusive);
            _lcdD4.SetDriveMode(GpioPinDriveMode.Output);

            _lcdD7 = GpioController.GetDefault().OpenPin(pinLcdD7, GpioSharingMode.Exclusive);
            _lcdD7.SetDriveMode(GpioPinDriveMode.Output);

            _lcdD5 = GpioController.GetDefault().OpenPin(pinLcdD5, GpioSharingMode.Exclusive);
            _lcdD5.SetDriveMode(GpioPinDriveMode.Output);

            _backlight = GpioController.GetDefault().OpenPin(pinBacklight, GpioSharingMode.Exclusive);
            _backlight.SetDriveMode(GpioPinDriveMode.Output);

            _lcdD6 = GpioController.GetDefault().OpenPin(pinLcdD6, GpioSharingMode.Exclusive);
            _lcdD6.SetDriveMode(GpioPinDriveMode.Output);

            _currentRow = 0;

            SendCommand(0x33);
            SendCommand(0x32);
            SendCommand(DispOn);
            SendCommand(ClrDisp);

            Thread.Sleep(3);
        }

        /// <summary>Prints the passed in string to the screen at the current cursor position. A newline character (\n) will move the cursor to the start of the next row.</summary>
        /// <param name="value">The string to print.</param>
        public void Print(string value)
        {
            for (int i = 0; i < value.Length; i++)
                Print(value[i]);
        }

        /// <summary>Prints a character to the screen at the current cursor position. A newline character (\n) will move the cursor to the start of the next row.</summary>
        /// <param name="value">The character to display.</param>
        public void Print(char value)
        {
            if (value != '\n')
            {
                WriteNibble((byte)(value >> 4));
                WriteNibble((byte)value);
            }
            else
            {
                SetCursorPosition((_currentRow + 1) % 2, 0);
            }
        }

        /// <summary>Clears the screen.</summary>
        public void Clear()
        {
            SendCommand(ClrDisp);

            _currentRow = 0;

            Thread.Sleep(2);
        }

        /// <summary>Places the cursor at the top left of the screen.</summary>
        public void CursorHome()
        {
            SendCommand(CurHome);

            _currentRow = 0;

            Thread.Sleep(2);
        }

        /// <summary>Moves the cursor to given position.</summary>
        /// <param name="row">The new row.</param>
        /// <param name="column">The new column.</param>
        public void SetCursorPosition(int row, int column)
        {
            if (column > 15 || column < 0) throw new System.ArgumentOutOfRangeException(nameof(column), "column must be between 0 and 15.");
            if (row > 1 || row < 0) throw new System.ArgumentOutOfRangeException(nameof(row), "row must be between 0 and 1.");

            _currentRow = row;

            SendCommand((byte)(SetCursor | RowOffsets[row] | column));
        }

        private void WriteNibble(byte b)
        {
            _lcdD7.Write(((b & 0x8) != 0) ? GpioPinValue.High : GpioPinValue.Low);
            _lcdD6.Write(((b & 0x4) != 0) ? GpioPinValue.High : GpioPinValue.Low);
            _lcdD5.Write(((b & 0x2) != 0) ? GpioPinValue.High : GpioPinValue.Low);
            _lcdD4.Write(((b & 0x1) != 0) ? GpioPinValue.High : GpioPinValue.Low);

            _lcdE.Write(GpioPinValue.High);
            _lcdE.Write(GpioPinValue.Low);

            Thread.Sleep(1);
        }

        private void SendCommand(byte command)
        {
            _lcdRs.Write(GpioPinValue.Low);

            WriteNibble((byte)(command >> 4));
            WriteNibble(command);

            Thread.Sleep(2);

            _lcdRs.Write(GpioPinValue.High);
        }
    }
}
