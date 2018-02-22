using GHIElectronics.TinyCLR.Devices.Adc;
// ReSharper disable UnusedMember.Global

namespace Bauland.Grove
{
    /// <summary>
    /// Wrapper for Grove SoundSensor module
    /// </summary>
    public class SoundSensor
    {
        private readonly AdcChannel _channel;
        /// <summary>
        /// Constructor of Grove SoundSensor module
        /// </summary>
        /// <param name="adcPinNumber">Adc pin of board</param>
        public SoundSensor(int adcPinNumber)
        {
            _channel = AdcController.GetDefault().OpenChannel(adcPinNumber);
        }

        /// <summary>
        /// Get level of sound
        /// </summary>
        /// <returns>Percentage of sound</returns>
        public double ReadLevel()
        {
            double lastRead = 0;
            for (int i = 0; i < 10; i++)
            {
                double d = _channel.ReadRatio();
                if (d > lastRead)
                    lastRead = d;
            }
            return lastRead * 100;
        }
    }
}
