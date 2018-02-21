using GHIElectronics.TinyCLR.Devices.Adc;
// ReSharper disable UnusedMember.Global

namespace Bauland.Grove
{
    /// <summary>
    /// Wrapper for Grove LightSensor Module
    /// </summary>
    public class LightSensor
    {
        private readonly AdcChannel _channel;
  
        /// <summary>
        /// Constructor of LightSensor
        /// </summary>
        /// <param name="adcPinNumber"> Adc pin of board</param>
        public LightSensor(int adcPinNumber)
        {
            _channel = AdcController.GetDefault().OpenChannel(adcPinNumber);
        }

        /// <summary>
        /// Read the light value of sensor
        /// </summary>
        /// <returns>Percentage of light value between 0 and 100.</returns>
        public double ReadPercentageLightLevel()
        {
            return _channel.ReadRatio() * 100;
        }
    }
}