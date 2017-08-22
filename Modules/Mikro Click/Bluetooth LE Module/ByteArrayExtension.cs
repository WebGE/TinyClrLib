using System;

namespace BlepClick
{
    public static class ByteArrayExtensions
    {
        /// <summary>
        /// Converts an array of bytes into an unsigned long, least significant byte first.
        /// </summary>
        /// <param name="data">The data to create an unsigned long from.</param>
        /// <param name="start">The location in the data to start converting from.</param>
        /// <param name="length">The amount of bytes to convert.</param>
        /// <returns></returns>
        public static ulong ToUnsignedLong(this byte[] data, int start = 0, int length = 8)
        {
            if (length < 1 || length > 8)
                throw new ArgumentOutOfRangeException("length", "Length must be between 0 and 9.");

            ulong result = 0;

            for (int i = 0; i < length; i++)
                result |= (ulong)data[start + i] << i * 8;

            return result;
        }
    }
}
