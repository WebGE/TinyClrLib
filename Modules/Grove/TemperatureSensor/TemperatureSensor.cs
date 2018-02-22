using GHIElectronics.TinyCLR.Devices.Adc;
// ReSharper disable UnusedMember.Global

namespace Bauland.Grove
{
    /// <summary>
    /// Wrapper for Grove TemperatureSensor module
    /// </summary>
    public class TemperatureSensor
    {
        private readonly AdcChannel _channel;
        /// <summary>
        /// Constructor of Grove TemperatureSensor module
        /// </summary>
        /// <param name="adcPinNumber">Adc pin of board</param>
        public TemperatureSensor(int adcPinNumber)
        {
            _channel = AdcController.GetDefault().OpenChannel(adcPinNumber);
        }

        /// <summary>
        /// Read temperature value of sensor
        /// </summary>
        /// <returns>Get Temperature in Celsius</returns>
        public double ReadTemperature()
        {
            // Per code example from seeed http://wiki.seeed.cc/Grove-Temperature_Sensor_V1.2/
            // Seemes to work fine with 3.3V
            double d = _channel.ReadRatio() * 1023;
            double r = 1023.0 / d - 1.0;
            double temperature = 1.0 / (System.Math.Log(r ) / 4275 + 1 / 298.15) - 273.15;
            
            return temperature;
        }
    }
}
