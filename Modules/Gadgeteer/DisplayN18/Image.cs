using System;

/* 
 * Based on GHI code of Brainpad
 */

namespace Bauland.Gadgeteer
{
    /// <summary>
    /// Represents an image that can be reused.
    /// </summary>
    public class Image
    {
        /// <summary>
        /// Width of image
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Height of image
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Data of each pixel of image
        /// </summary>
        public byte[] Pixels { get; private set; }

        /// <summary>
        /// Constructs a new image with the given dimensions.
        /// </summary>
        /// <param name="width">The image width.</param>
        /// <param name="height">The image height.</param>
        public Image(int width, int height)
        {
            if (width < 0) throw new ArgumentOutOfRangeException(nameof(width), "width must not be negative.");
            if (height < 0) throw new ArgumentOutOfRangeException(nameof(height), "height must not be negative.");

            Width = width;
            Height = height;
            Pixels = new byte[width * height * 2];
        }

        /// <summary>
        /// Sets the given pixel to the given color.
        /// </summary>
        /// <param name="x">The x coordinate of the pixel to set.</param>
        /// <param name="y">The y coordinate of the pixel to set.</param>
        /// <param name="color">The color to set the pixel to.</param>
        public void SetPixel(int x, int y, Color color)
        {
            if (x < 0) throw new ArgumentOutOfRangeException(nameof(x), "x must not be negative.");
            if (y < 0) throw new ArgumentOutOfRangeException(nameof(y), "y must not be negative.");
            if (x > Width) throw new ArgumentOutOfRangeException(nameof(x), "x must not exceed Width.");
            if (y > Height) throw new ArgumentOutOfRangeException(nameof(y), "y must not exceed Height.");

            Pixels[(x * Width + y) * 2 + 0] = (byte)(color.As565 >> 8);
            Pixels[(x * Width + y) * 2 + 1] = (byte)(color.As565 >> 0);
        }
    }
}
