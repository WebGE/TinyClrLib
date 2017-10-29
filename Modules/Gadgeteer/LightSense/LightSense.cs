using GHIElectronics.TinyCLR.Devices.Adc;

namespace Bauland.Gadgeteer
{
    /// <summary>
    /// Manage a Gadgeteer LightSense module from GHI
    /// </summary>
    public class LightSense
    {
        private readonly AdcChannel _adc;

        /// <summary>
        /// Get int value of light
        /// </summary>
        public int Value => _adc.ReadValue();

        /// <summary>
        /// Get double value of light between 0 and 1.0
        /// </summary>
        public double Ratio => _adc.ReadRatio();

        /// <summary>
        /// Ctor of module
        /// </summary>
        /// <param name="adcSocket">pin of adc channel (usually pin3 on A socket)</param>
        public LightSense(int adcSocket)
        {
            _adc = AdcController.GetDefault().OpenChannel(adcSocket);
        }
    }
}
