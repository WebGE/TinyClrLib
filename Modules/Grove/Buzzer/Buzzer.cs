using System.Threading;
using GHIElectronics.TinyCLR.Devices.Gpio;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Bauland.Grove
{
    /// <summary>
    /// Wrapper for Grove Buzzer module
    /// </summary>
    public class Buzzer
    {
        private readonly GpioPin _pin;
        /// <summary>
        /// Construtcor of Buzzer object
        /// </summary>
        /// <param name="pinNumber">Gpio pin of module</param>
        public Buzzer(int pinNumber)
        {
            _pin = GpioController.GetDefault().OpenPin(pinNumber);
            _pin.Write(GpioPinValue.Low);
            _pin.SetDriveMode(GpioPinDriveMode.Output);
        }

        /// <summary>
        /// Turn buzzer on
        /// </summary>
        public void TurnOn()
        {
            _pin.Write(GpioPinValue.High);
        }

        /// <summary>
        /// Turn buzzer off
        /// </summary>
        public void TurnOff()
        {
            _pin.Write(GpioPinValue.Low);
        }

        /// <summary>
        /// Beep briefly with the buzzer
        /// </summary>
        public void Beep()
        {
            TurnOn();
            Thread.Sleep(10);
            TurnOff();
        }
    }
}
