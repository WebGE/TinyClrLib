using GHIElectronics.TinyCLR.Devices.Adc;
// ReSharper disable UnusedMember.Global

namespace Modules.Grove
{
    public class LightSensor
    {
        private readonly AdcChannel _channel;
        public LightSensor(int adcPinNumber)
        {
            _channel = AdcController.GetDefault().OpenChannel(adcPinNumber);
        }
        // between 0 and 100
        public double ReadLightLevel()
        {
            return _channel.ReadRatio() * 100;
        }
    }
}
