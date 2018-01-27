using System;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.Gpio;
// ReSharper disable UnusedMember.Local
// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
// ReSharper disable UnusedMember.Global

namespace Bauland.Gadgeteer
{
    /// <summary>
    /// Wrapper class for RotaryH1 Gadgeteer Module
    /// </summary>
    public class RotaryH1
    {
        private readonly byte[] _write1;
        private readonly byte[] _write2;
        private readonly byte[] _read2;
        private readonly byte[] _read4;

        private readonly GpioPin _enable;
        private readonly GpioPin _miso;
        private readonly GpioPin _mosi;
        private readonly GpioPin _clock;
        private readonly GpioPin _cs;

        /// <summary>The direction the encoder is being turned.</summary>
        public enum Direction : byte
        {

            /// <summary>The encoder is moving in a counter-clockwise direction.</summary>
            CounterClockwise,

            /// <summary>The encoder is moving in a clockwise direction.</summary>
            Clockwise
        }

        [Flags]
        private enum Commands : byte
        {
            Ls7366Clear = 0x00,
            Ls7366Read = 0x40,
            Ls7366Write = 0x80,
            Ls7366Load = 0xC0,

            Ls7366Mdr0 = 0x08,
            Ls7366Mdr1 = 0x10,
            Ls7366Dtr = 0x18,
            Ls7366Cntr = 0x20,
            Ls7366Otr = 0x28,
            Ls7366Str = 0x30,
        }

        [Flags]
        private enum Mdr0Modes : byte
        {
            Ls7366Mdr0Quad0 = 0x00,
            Ls7366Mdr0Quad1 = 0x01,
            Ls7366Mdr0Quad2 = 0x02,
            Ls7366Mdr0Quad4 = 0x03,
            Ls7366Mdr0Freer = 0x00,
            Ls7366Mdr0Sicyc = 0x04,
            Ls7366Mdr0Range = 0x08,
            Ls7366Mdr0Modtr = 0x0C,
            Ls7366Mdr0Didx = 0x00,
            Ls7366Mdr0Ldcnt = 0x10,
            Ls7366Mdr0Recnt = 0x20,
            Ls7366Mdr0Ldotr = 0x30,
            Ls7366Mdr0Asidx = 0x00,
            Ls7366Mdr0Syinx = 0x40,
            Ls7366Mdr0Ffac1 = 0x00,
            Ls7366Mdr0Ffac2 = 0x80,
            Ls7366Mdr0Nofla = 0x00,
        }

        [Flags]
        private enum Mdr1Modes : byte
        {
            Ls7366Mdr14Byte = 0x00,
            Ls7366Mdr13Byte = 0x01,
            Ls7366Mdr12Byte = 0x02,
            Ls7366Mdr11Byte = 0x03,
            Ls7366Mdr1Encnt = 0x00,
            Ls7366Mdr1Dicnt = 0x04,
            Ls7366Mdr1Flidx = 0x10,
            Ls7366Mdr1Flcmp = 0x20,
            Ls7366Mdr1Flbw = 0x40,
            Ls7366Mdr1Flcy = 0x80,
        }

        /// <summary>
        /// Constructor of Button
        /// </summary>
        /// <param name="pinEnable">Enable pin, pin 5 of Y Socket</param>
        /// <param name="pinChipSelect">ChipSelect pin, pin 6 of Y Socket</param>
        /// <param name="pinMosi">Mosi pin, pin 7 of Y Socket</param>
        /// <param name="pinMiso">Miso pin, pin 8 of Y Socket</param>
        /// <param name="pinClock">Clock pin, pin 9 of Y Socket</param>
        public RotaryH1(int pinEnable, int pinChipSelect, int pinMosi, int pinMiso,int pinClock)
        {

            _write1 = new byte[1];
            _write2 = new byte[2];
            _read2 = new byte[2];
            _read4 = new byte[4];

            _enable = GpioController.GetDefault().OpenPin(pinEnable, GpioSharingMode.Exclusive);
            _enable.SetDriveMode(GpioPinDriveMode.Output);
            _enable.Write(GpioPinValue.High);

            _cs = GpioController.GetDefault().OpenPin(pinChipSelect, GpioSharingMode.Exclusive);
            _cs.SetDriveMode(GpioPinDriveMode.Output);
            _cs.Write(GpioPinValue.High);

            _mosi = GpioController.GetDefault().OpenPin(pinMosi, GpioSharingMode.Exclusive);
            _mosi.SetDriveMode(GpioPinDriveMode.Output);
            _mosi.Write(GpioPinValue.Low);

            _miso = GpioController.GetDefault().OpenPin(pinMiso, GpioSharingMode.Exclusive);
            _miso.SetDriveMode(GpioPinDriveMode.Input);

            _clock = GpioController.GetDefault().OpenPin(pinClock, GpioSharingMode.Exclusive);
            _clock.SetDriveMode(GpioPinDriveMode.Output);
            _clock.Write(GpioPinValue.Low);

            Initialize();
        }

        /// <summary>Gets the current count of the encoder.</summary>
        /// <returns>An integer representing the count.</returns>
        public int GetCount()
        {
            return Read2(Commands.Ls7366Read | Commands.Ls7366Cntr);
        }

        /// <summary>Gets the current direction that the encoder count is going.</summary>
        /// <returns>The direction the encoder count is going.</returns>
        public Direction GetDirection()
        {
            return ((GetStatus() & 0x02) >> 1) > 0 ? Direction.CounterClockwise : Direction.Clockwise;
        }

        /// <summary>Resets the count on the module to 0.</summary>
        public void ResetCount()
        {
            Write(Commands.Ls7366Clear | Commands.Ls7366Cntr);
        }

        private void Initialize()
        {
            Write(Commands.Ls7366Clear | Commands.Ls7366Mdr0);
            Write(Commands.Ls7366Clear | Commands.Ls7366Mdr1);
            Write(Commands.Ls7366Clear | Commands.Ls7366Str);
            Write(Commands.Ls7366Clear | Commands.Ls7366Cntr);
            Write(Commands.Ls7366Load | Commands.Ls7366Otr);

            Write(Commands.Ls7366Write | Commands.Ls7366Mdr0, Mdr0Modes.Ls7366Mdr0Quad1 | Mdr0Modes.Ls7366Mdr0Freer | Mdr0Modes.Ls7366Mdr0Didx | Mdr0Modes.Ls7366Mdr0Ffac2);
            Write(Commands.Ls7366Write | Commands.Ls7366Mdr1, Mdr1Modes.Ls7366Mdr12Byte | Mdr1Modes.Ls7366Mdr1Encnt);
        }

        private byte GetStatus()
        {
            return Read1(Commands.Ls7366Read | Commands.Ls7366Str);
        }

        private byte Read1(Commands register)
        {
            _write1[0] = (byte)register;

            WriteRead(_write1, _read2);

            return _read2[1];
        }

        private short Read2(Commands register)
        {
            _write1[0] = (byte)register;

            WriteRead(_write1, _read4);

            return (short)((_read4[1] << 8) | _read4[2]);
        }

        private void Write(Commands register)
        {
            _write1[0] = (byte)register;

            WriteRead(_write1, null);
        }

        private void Write(Commands register, Mdr0Modes command)
        {
            _write2[0] = (byte)register;
            _write2[1] = (byte)command;

            WriteRead(_write2, null);
        }

        private void Write(Commands register, Mdr1Modes command)
        {
            _write2[0] = (byte)register;
            _write2[1] = (byte)command;

            WriteRead(_write2, null);
        }

        private void WriteRead(byte[] writeBuffer, byte[] readBuffer)
        {
            int writeLength = writeBuffer.Length;
            int readLength = 0;

            if (readBuffer != null)
            {
                readLength = readBuffer.Length;

                for (int i = 0; i < readLength; i++)
                    readBuffer[i] = 0;
            }

            _cs.Write(GpioPinValue.Low);

            for (int i = 0; i < (writeLength < readLength ? readLength : writeLength); i++)
            {
                byte w = 0;

                if (i < writeLength)
                    w = writeBuffer[i];

                byte mask = 0x80;

                for (int j = 0; j < 8; j++)
                {
                    _clock.Write(GpioPinValue.Low);

                    _mosi.Write(((w & mask) != 0)?GpioPinValue.High:GpioPinValue.Low);

                    _clock.Write(GpioPinValue.High);

                    if (readBuffer != null)
                        readBuffer[i] |= ((_miso.Read()==GpioPinValue.High) ? mask : (byte)0x00);

                    mask >>= 1;
                }

                _mosi.Write(GpioPinValue.Low);
                _clock.Write(GpioPinValue.Low);
            }

            Thread.Sleep(20);

            _cs.Write(GpioPinValue.High);
        }
    }
}
