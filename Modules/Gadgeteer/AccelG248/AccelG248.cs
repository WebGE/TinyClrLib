using GHIElectronics.TinyCLR.Devices.I2c;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Bauland.Gadgeteer
{
    /// <summary>
    /// Wrapper class for AccelG248 Gadgeteer Module
    /// </summary>
    public class AccelG248
    {
        private readonly I2cDevice _i2C;
        private readonly byte[] _buffer1;
        private readonly byte[] _buffer2;
        private readonly byte[] _buffer6;

        /// <summary>Constructor of AccelG248</summary>
        /// <param name="i2CIdentifier">Identifer of I2C bus of I Socket.</param>
        public AccelG248(string i2CIdentifier)
        {
            _buffer1 = new byte[1];
            _buffer2 = new byte[2];
            _buffer6 = new byte[6];

            var settings = new I2cConnectionSettings(0x1C)
            {
                BusSpeed = I2cBusSpeed.FastMode
            };
            _i2C=I2cDevice.FromId(i2CIdentifier,settings);
            byte[] buffer = new byte[] {0x2A, 0x01};
            _i2C.Write(buffer);
        }

        /// <summary>Gets the current X acceleration value.</summary>
        /// <returns>The X acceleration between -2g and 2g.</returns>
        public double GetX()
        {
            ReadRegister(0x01, _buffer2);

            double value = (_buffer2[0] << 2) | (_buffer2[1] >> 6);

            if (value > 511.0)
                value = value - 1024.0;

            value /= 256.0;

            return value;
        }

        /// <summary>Gets the current Y acceleration value.</summary>
        /// <returns>The Y acceleration between -2g and 2g.</returns>
        public double GetY()
        {
            ReadRegister(0x03, _buffer2);

            double value = (_buffer2[0] << 2) | (_buffer2[1] >> 6);

            if (value > 511.0)
                value = value - 1024.0;

            value /= 256.0;

            return value;
        }

        /// <summary>Gets the current Z acceleration value.</summary>
        /// <returns>The Z acceleration between -2g and 2g.</returns>
        public double GetZ()
        {
            ReadRegister(0x05, _buffer2);

            double value = (_buffer2[0] << 2) | (_buffer2[1] >> 6);

            if (value > 511.0)
                value = value - 1024.0;

            value /= 256.0;

            return value;
        }

        /// <summary>Gets the current acceleration values.</summary>
        /// <returns>The acceleration.</returns>
        public Acceleration GetAcceleration()
        {
            GetAcceleration(out var x, out var y, out var z);

            return new Acceleration { X = x, Y = y, Z = z };
        }

        /// <summary>Gets the current acceleration values.</summary>
        /// <param name="x">The x acceleration between -2g and 2g.</param>
        /// <param name="y">The y acceleration between -2g and 2g.</param>
        /// <param name="z">The z acceleration between -2g and 2g.</param>
        public void GetAcceleration(out double x, out double y, out double z)
        {
            ReadRegister(0x01, _buffer6);

            x = (_buffer6[0] << 2) | (_buffer6[1] >> 6);
            y = (_buffer6[2] << 2) | (_buffer6[3] >> 6);
            z = (_buffer6[4] << 2) | (_buffer6[5] >> 6);

            if (x > 511.0)
                x -= 1024.0;

            if (y > 511.0)
                y -= 1024.0;

            if (z > 511.0)
                z -= 1024.0;

            x /= 256.0;
            y /= 256.0;
            z /= 256.0;
        }

        private void ReadRegister(byte register, byte[] read)
        {
            _buffer1[0] = register;

            _i2C.WriteRead(_buffer1, read);
        }

        /// <summary>Represents an acceleration.</summary>
        public struct Acceleration
        {
            /// <summary>The x acceleration between -2g and 2g.</summary>
            public double X { get; set; }

            /// <summary>The y acceleration between -2g and 2g.</summary>
            public double Y { get; set; }

            /// <summary>The z acceleration between -2g and 2g.</summary>
            public double Z { get; set; }

            /// <summary>Returns the string representation of this object.</summary>
            /// <returns>The string representation.</returns>
            public override string ToString()
            {
                return "(" + X.ToString("F2") + ", " + Y.ToString("F2") + ", " + Z.ToString("F2") + ")";
            }
        }
    }
}
