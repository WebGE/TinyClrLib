using GHIElectronics.TinyCLR.Devices.Gpio;
// ReSharper disable UnusedMember.Global

namespace Bauland.Grove
{
    /// <summary>
    /// Wrapper of Grove Relay module
    /// </summary>
    public class Relay
    {
        private readonly GpioPin _pin;
        /// <summary>
        /// Constructor of Grove Relay module
        /// </summary>
        /// <param name="gpioPinNumber">Gpio pin of board</param>
        public Relay(int gpioPinNumber)
        {
            _pin = GpioController.GetDefault().OpenPin(gpioPinNumber);
            _pin.Write(GpioPinValue.Low);
            _pin.SetDriveMode(GpioPinDriveMode.Output);
        }

        /// <summary>
        /// Activate Relay
        /// </summary>
        public void TurnOn()
        {
            _pin.Write(GpioPinValue.High);
        }

        /// <summary>
        /// Release Relay
        /// </summary>
        public void TurnOff()
        {
            _pin.Write(GpioPinValue.Low);
        }
    }
}
