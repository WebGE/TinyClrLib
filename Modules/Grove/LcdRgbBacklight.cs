using System.Threading;
using GHIElectronics.TinyCLR.Devices.I2c;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Modules.Grove
{
    public class LcdRgbBacklight
    {
        private readonly I2cDevice _displayDevice;
        private readonly I2cDevice _backlightDevice;

        private readonly byte _displayfunction;
        private byte _displaycontrol;

        private const byte LcdClearDisplay = 0x01;
        private const byte LcdReturnHome = 0x02;
        private const byte LcdEntryModeSet = 0x04;
        private const byte LcdDisplaycontrol = 0x08;
        private const byte LcdCursorshift = 0x10;
        private const byte LcdFunctionset = 0x20;
        private const byte LcdSetcgramaddr = 0x40;
        private const byte LcdSetddramaddr = 0x80;

        private const byte LcdDisplayon = 0x04;
        private const byte LcdDisplayoff = 0x00;
        private const byte LcdCursoron = 0x02;
        private const byte LcdCursoroff = 0x00;
        private const byte LcdBlinkon = 0x01;
        private const byte LcdBlinkoff = 0x00;
        private const byte LcdEntryright = 0x00;
        private const byte LcdEntryleft = 0x02;
        private const byte LcdEntryshiftincrement = 0x01;
        private const byte LcdEntryshiftdecrement = 0x00;

        private const byte RegMode1 = 0x00;
        private const byte RegMode2 = 0x01;
        private const byte RegOutput = 0x08;

        public LcdRgbBacklight(string i2CBus)
        {
            var settings = new I2cConnectionSettings(0x7c >> 1) { BusSpeed = I2cBusSpeed.FastMode };

            //string aqs = I2cDevice.GetDeviceSelector("I2C1");
            _displayDevice = I2cDevice.FromId(i2CBus, settings);

            settings = new I2cConnectionSettings(0xc4 >> 1)
            {
                BusSpeed = I2cBusSpeed.FastMode
            };

            _backlightDevice = I2cDevice.FromId(i2CBus, settings);

            ////////////////////////////////////
            // get the display going

            //byte cols = 6;
            byte lines = 2;
            byte dotsize = 0;


            if (lines > 1)
            {
                _displayfunction |= 0x08;// LCD_2LINE;
            }

            // for some 1 line displays you can select a 10 pixel high font
            if ((dotsize != 0) && (lines == 1))
            {
                _displayfunction |= 0x04;// LCD_5x10DOTS;
            }

            // SEE PAGE 45/46 FOR INITIALIZATION SPECIFICATION!
            // according to datasheet, we need at least 40ms after power rises above 2.7V
            // before sending commands. Arduino can turn on way befer 4.5V so we'll wait 50
            //delayMicroseconds(50000);
            Thread.Sleep(50);

            // this is according to the hitachi HD44780 datasheet
            // page 45 figure 23

            // Send function set command sequence
            Command((byte)(LcdFunctionset | _displayfunction));
            //delayMicroseconds(4500);  // wait more than 4.1ms
            Thread.Sleep(5);

            // second try
            Command((byte)(LcdFunctionset | _displayfunction));
            //delayMicroseconds(150);
            Thread.Sleep(1);

            // third go
            Command((byte)(LcdFunctionset | _displayfunction));


            // finally, set # lines, font size, etc.
            Command((byte)(LcdFunctionset | _displayfunction));

            // turn the display on with no cursor or blinking default
            _displaycontrol = LcdDisplayon | LcdCursoroff | LcdBlinkoff;
            EnableDisplay(true);

            // clear it off
            Clear();

            // Initialize to default text direction (for romance languages)
            byte displaymode = LcdEntryleft | LcdEntryshiftdecrement;
            // set the entry mode
            Command((byte)(LcdEntryModeSet | displaymode));


            // backlight init
            WriteBacklightReg(RegMode1, 0);
            // set LEDs controllable by both PWM and GRPPWM registers
            WriteBacklightReg(RegOutput, 0xFF);
            // set MODE2 values
            // 0010 0000 -> 0x20  (DMBLNK to 1, ie blinky mode)
            WriteBacklightReg(RegMode2, 0x20);

            //setColorWhite();
            SetBacklightRgb(255, 0, 100);

        }

        /*********** mid level commands, for sending data/cmds */
        //private byte LCD_ADDRESS = (0x7c >> 1);
        /*private void i2c_send_byte(byte dta)
        {
            //int written, read;
            byte[] wb = new byte[2];
            //byte[] rb = new byte[2];
            wb[0] = dta;
            DisplayDevice.Write(wb);
            //I2C.WriteRead(LCD_ADDRESS, wb, 0, 1, rb, 0, 0, out written, out read);

        }*/

        /*private void i2c_send_byteS(byte[] dta, byte len)
        {
            int written, read;
            //byte[] wb = new byte[2];
            byte[] rb = new byte[2];
            //wb[0] = dta;

            //I2C.WriteRead(LCD_ADDRESS, dta, 0, len, rb, 0, 0, out written, out read);


        }*/

        // send command
        private void Command(byte value)
        {
            byte[] data = { 0x80, value };
            _displayDevice.Write(data);
            //i2c_send_byteS(dta, 2);
        }

        // send data
        private void Write(byte value)
        {

            byte[] data = { 0x40, value };
            //i2c_send_byteS(dta, 2);
            _displayDevice.Write(data);
            //return 1; // assume sucess
        }

        /********** high level commands, for the user! */

        public void Write(string s)
        {

            for (int i = 0; i < s.Length; i++)
                Write((byte)s[i]);
        }

        public void Clear()
        {
            Command(LcdClearDisplay);        // clear display, set cursor position to zero
            //delayMicroseconds(2000);          // this command takes a long time!
            Thread.Sleep(2);
        }

        public void GoHome()
        {
            Command(LcdReturnHome);        // set cursor position to zero
            //delayMicroseconds(2000);        // this command takes a long time!
            Thread.Sleep(2);
        }

        public void SetCursor(byte col, byte row)
        {

            col = (byte)(row == 0 ? (col | 0x80) : (col | 0xc0));
            Command(col);
            //byte[] dta = new byte[2] { 0x80, col };
            //DisplayDevice.Write(dta);

            //i2c_send_byteS(dta, 2);

        }

        // Turn the display on/off (quickly)
        public void EnableDisplay(bool on)
        {
            if (on)
                _displaycontrol |= LcdDisplayon;
            else
                _displaycontrol = (byte)(~LcdDisplayon & _displaycontrol);

            Command((byte)(LcdDisplaycontrol | _displaycontrol));
        }

        // =============================================================================

        // Control the backlight LED blinking
        private void WriteBacklightReg(byte addr, byte data)
        {
            byte[] wb = new byte[2];
            wb[0] = addr;
            wb[1] = data;

            _backlightDevice.Write(wb);

            //I2C.WriteRead((0xc4 >> 1), wb, 0, 2, rb, 0, 0, out written, out read);

        }

        public void BlinkBacklight(bool on)
        {
            // blink period in seconds = (<reg 7> + 1) / 24
            // on/off ratio = <reg 6> / 256
            if (on)
            {
                WriteBacklightReg(0x07, 0x17);  // blink every second
                WriteBacklightReg(0x06, 0x7f);  // half on, half off
            }
            else
            {
                WriteBacklightReg(0x07, 0x00);
                WriteBacklightReg(0x06, 0xff);
            }
        }
        public void SetBacklightRgb(byte r, byte g, byte b)
        {

            byte regRed = 0x04;    // pwm2 
            byte regGreen = 0x03;      // pwm1 
            byte regBlue = 0x02;      // pwm0 

            WriteBacklightReg(regRed, r);
            WriteBacklightReg(regGreen, g);
            WriteBacklightReg(regBlue, b);
        }
    }
}
