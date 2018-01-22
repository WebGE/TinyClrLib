using GHIElectronics.TinyCLR.Devices.Gpio;
// ReSharper disable UnusedMember.Global

namespace Bauland.Gadgeteer
{
    /// <summary>
    /// 
    /// </summary>
    public class Led7C
    {
        private readonly GpioPin _mRedPin;
        private readonly GpioPin _mBluePin;
        private readonly GpioPin _mGreenPin;

        /// <summary>
        /// Constructor of Led7C
        /// </summary>
        /// <param name="pinB">pin connected to blue led (usually pin 3 of Socket)</param>
        /// <param name="pinR">pin connected on red led (usually pin 4 of Socket)</param>
        /// <param name="pinG">pin connected to green led (usually pin 5 of Socket)</param>
        public Led7C(int pinB, int pinR, int pinG)
        {
            _mRedPin = GpioController.GetDefault().OpenPin(pinR);
            _mGreenPin = GpioController.GetDefault().OpenPin(pinG);
            _mBluePin = GpioController.GetDefault().OpenPin(pinB);

            _mRedPin.SetDriveMode(GpioPinDriveMode.Output);
            _mGreenPin.SetDriveMode(GpioPinDriveMode.Output);
            _mBluePin.SetDriveMode(GpioPinDriveMode.Output);
        }

        /// <summary>
        /// Represents possible display colors.
        /// </summary>
        public enum LedColor
        {
            /// <summary>
            /// Red
            /// </summary>
            Red = (1 << 2) | (0 << 1) | 0,

            /// <summary>
            /// Green
            /// </summary>
            Green = (0 << 2) | (1 << 1) | 0,

            /// <summary>
            /// Blue
            /// </summary>
            Blue = (0 << 2) | (0 << 1) | 1,

            /// <summary>
            /// Yellow
            /// </summary>
            Yellow = (1 << 2) | (1 << 1) | 0,

            /// <summary>
            /// Cyan
            /// </summary>
            Cyan = (0 << 2) | (1 << 1) | 1,

            /// <summary>
            /// Magenta
            /// </summary>
            Magenta = (1 << 2) | (0 << 1) | 1,

            /// <summary>
            /// White
            /// </summary>
            White = (1 << 2) | (1 << 1) | 1,

            /// <summary>
            /// Off
            /// </summary>
            Off = 0,

        }

        /// <summary>
        /// Sets the color of the module's LED to the passed in LEDColor enum.
        /// </summary>
        /// <param name="color"></param>
        public void SetColor(LedColor color)
        {
            _mRedPin.Write((((int) color & 4) != 0 ? GpioPinValue.High : GpioPinValue.Low));
            _mGreenPin.Write((((int) color & 2) != 0 ? GpioPinValue.High : GpioPinValue.Low));
            _mBluePin.Write((((int) color & 1) != 0 ? GpioPinValue.High : GpioPinValue.Low));
        }
    }
}
