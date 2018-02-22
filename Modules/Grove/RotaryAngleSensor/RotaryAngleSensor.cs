using GHIElectronics.TinyCLR.Devices.Adc;
// ReSharper disable UnusedMember.Global

namespace Bauland.Grove
{
    /// <summary>
    /// Wrapper for Grove RotaryAngleSensor module
    /// </summary>
    public class RotaryAngleSensor
    {
        private readonly AdcChannel _channel;
        /// <summary>
        /// Constructor of Grove RotaryAngleSensor module
        /// </summary>
        /// <param name="adcPinNumber">Adc pin of board</param>
        public RotaryAngleSensor(int adcPinNumber)
        {
            _channel = AdcController.GetDefault().OpenChannel(adcPinNumber);
        }
        // between 0 and 100
        /// <summary>
        /// Get the percentage of value of position
        /// </summary>
        /// <returns>Get percentage of value</returns>
        public double GetPercentage()
        {
            return _channel.ReadRatio() * 100;
        }
    }
}
