// ReSharper disable UnusedMember.Global
#pragma warning disable 1591

namespace Bauland.Grove
{
    /// <summary>
    /// All Bme280 Registry address
    /// </summary>
    public static class Registry
    {
        public const byte Bme280RegDigT1 = 0x88;
        public const byte Bme280RegDigT2 = 0x8A;
        public const byte Bme280RegDigT3 = 0x8C;

        public const byte Bme280RegDigP1 = 0x8E;
        public const byte Bme280RegDigP2 = 0x90;
        public const byte Bme280RegDigP3 = 0x92;
        public const byte Bme280RegDigP4 = 0x94;
        public const byte Bme280RegDigP5 = 0x96;
        public const byte Bme280RegDigP6 = 0x98;
        public const byte Bme280RegDigP7 = 0x9A;
        public const byte Bme280RegDigP8 = 0x9C;
        public const byte Bme280RegDigP9 = 0x9E;

        public const byte Bme280RegDigH1 = 0xA1;
        public const byte Bme280RegDigH2 = 0xE1;
        public const byte Bme280RegDigH3 = 0xE3;
        public const byte Bme280RegDigH4 = 0xE4;
        public const byte Bme280RegDigH5 = 0xE5;
        public const byte Bme280RegDigH6 = 0xE7;

        public const byte Bme280RegChipid = 0xD0;
        public const byte Bme280RegVersion = 0xD1;
        public const byte Bme280RegSoftreset = 0xE0;

        public const byte Bme280RegCal26 = 0xE1;

        public const byte Bme280RegControlhumid = 0xF2;
        public const byte Bme280RegControl = 0xF4;
        public const byte Bme280RegConfig = 0xF5;
        public const byte Bme280RegPressuredata = 0xF7;
        public const byte Bme280RegTempdata = 0xFA;
        public const byte Bme280RegHumiditydata = 0xFD;
    }

}
