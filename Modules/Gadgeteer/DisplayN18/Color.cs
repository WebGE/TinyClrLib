/* 
 * Based on GHI code of Brainpad
 */
 
 namespace Bauland.Gadgeteer
{
    /// <summary>
    /// Represents a color made up of red, green, and blue.
    /// </summary>
    public class Color
    {
        /// <summary>
        /// The amount of red.
        /// </summary>
        public byte R { get; set; }

        /// <summary>
        /// The amount of green.
        /// </summary>
        public byte G { get; set; }

        /// <summary>
        /// The amount of blue.
        /// </summary>
        public byte B { get; set; }

        /// <summary>
        /// The color in 565 format.
        /// </summary>
        public ushort As565
        {
            get
            {
                return (ushort)(((R & 0x1F) << 11) | ((G & 0x3F) << 5) | (B & 0x1F));
            }
        }

        /// <summary>
        /// Constructs a new instance with the given levels.
        /// </summary>
        public Color()
            : this(0, 0, 0)
        {

        }

        /// <summary>
        /// Constructs a new instance with the given levels.
        /// </summary>
        /// <param name="red">The amount of red.</param>
        /// <param name="green">The amount of green.</param>
        /// <param name="blue">The amount of blue.</param>
        public Color(byte red, byte green, byte blue)
        {
            R = red;
            G = green;
            B = blue;
        }

        /// <summary>
        /// A predefined color for black.
        /// </summary>
        public static Color Black = new Color(0, 0, 0);
        /// <summary>
        /// A predefined color for white.
        /// </summary>
        public static Color White = new Color(255, 255, 255);
        /// <summary>
        /// A predefined color for red.
        /// </summary>
        public static Color Red = new Color(255, 0, 0);
        /// <summary>
        /// A predefined color for green.
        /// </summary>
        public static Color Green = new Color(0, 255, 0);
        /// <summary>
        /// A predefined color for blue.
        /// </summary>
        public static Color Blue = new Color(0, 0, 255);
        /// <summary>
        /// A predefined color for yellow.
        /// </summary>
        public static Color Yellow = new Color(255, 255, 0);
        /// <summary>
        /// A predefined color for cyan.
        /// </summary>
        public static Color Cyan = new Color(0, 255, 255);
        /// <summary>
        /// A predefined color for magenta.
        /// </summary>
        public static Color Magenta = new Color(255, 0, 255);
    }
}
