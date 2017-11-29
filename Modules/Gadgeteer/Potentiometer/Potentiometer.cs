using GHIElectronics.TinyCLR.Devices.Adc;
// ReSharper disable UnusedMember.Global

namespace Bauland.Gadgeteer
{
    /// <summary>
    /// Wrapper class for Potentiometer Gadgeteer Module
    /// </summary>
    public class Potentiometer
    {
        private readonly AdcChannel _input;

        /// <summary>Constructs a new instance.</summary>
        /// <param name="channelNumber">pin 3 of A socket</param>
        public Potentiometer(int channelNumber)
        {
            _input=AdcController.GetDefault().OpenChannel(channelNumber);
        }

        /// <summary>Gets the current voltage reading of the potentiometer.</summary>
        public double ReadVoltage()
        {
            return _input.ReadValue();
        }

        /// <summary>Gets the current position of the potentiometer between 0.0 and 1.0.</summary>
        public double ReadProportion()
        {
            return _input.ReadRatio();
        }
    }
}
