using System;
using GHIElectronics.TinyCLR.Devices.I2c;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Local
// ReSharper disable UnusedParameter.Global
// ReSharper disable PossibleLossOfFraction
// ReSharper disable MemberCanBePrivate.Global

namespace Bauland.Grove
{
    /// <summary>
    /// Wrapper for Grove BarometerSensorBME280 module
    /// </summary>
    public class BarometerSensorBme280
    {
        private readonly I2cDevice _sensor;
        private const byte Address = 0x76;
        private readonly Coefficient _coefficient;
        private long _tFine;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i2CId"></param>
        public BarometerSensorBme280(string i2CId)
        {
            _coefficient = new Coefficient();
            _sensor = I2cDevice.FromId(i2CId, new I2cConnectionSettings(Address)
            {
                BusSpeed = I2cBusSpeed.FastMode,
                SharingMode = I2cSharingMode.Shared
            });

            CheckCommunication();
            RetrieveCoefficients();
            WriteRegister(Registry.Bme280RegControl, 0x3F); //Choose 16X oversampling
            WriteRegister(Registry.Bme280RegControlhumid, 0x03); //Choose 4X oversampling
        }

        private void RetrieveCoefficients()
        {
            _coefficient.DigT1 = Read16LittleEndian(Registry.Bme280RegDigT1);
            _coefficient.DigT2 = ReadSigned16LittleEndian(Registry.Bme280RegDigT2);
            _coefficient.DigT3 = ReadSigned16LittleEndian(Registry.Bme280RegDigT3);

            _coefficient.DigP1 = Read16LittleEndian(Registry.Bme280RegDigP1);
            _coefficient.DigP2 = ReadSigned16LittleEndian(Registry.Bme280RegDigP2);
            _coefficient.DigP3 = ReadSigned16LittleEndian(Registry.Bme280RegDigP3);
            _coefficient.DigP4 = ReadSigned16LittleEndian(Registry.Bme280RegDigP4);
            _coefficient.DigP5 = ReadSigned16LittleEndian(Registry.Bme280RegDigP5);
            _coefficient.DigP6 = ReadSigned16LittleEndian(Registry.Bme280RegDigP6);
            _coefficient.DigP7 = ReadSigned16LittleEndian(Registry.Bme280RegDigP7);
            _coefficient.DigP8 = ReadSigned16LittleEndian(Registry.Bme280RegDigP8);
            _coefficient.DigP9 = ReadSigned16LittleEndian(Registry.Bme280RegDigP9);

            _coefficient.DigH1 = Read8(Registry.Bme280RegDigH1);
            _coefficient.DigH2 = ReadSigned16LittleEndian(Registry.Bme280RegDigH2);
            _coefficient.DigH3 = Read8(Registry.Bme280RegDigH3);
            _coefficient.DigH4 = (Int16)((Read8(Registry.Bme280RegDigH4) << 4) | (Read8(Registry.Bme280RegDigH4 + 1) & 0xf));
            _coefficient.DigH5 = (Int16)((Read8(Registry.Bme280RegDigH5 + 1) << 4) | (Read8(Registry.Bme280RegDigH5) >> 4));
            _coefficient.DigH6 = (sbyte)Read8(Registry.Bme280RegDigH6);
        }

        private void CheckCommunication()
        {
            if (Read8(Registry.Bme280RegChipid) != 0x60)
                throw new SensorNotFoundException("No device found at address 0x60");
        }

        /// <summary>
        /// Retrieve temperature from sensor
        /// </summary>
        /// <returns>Temperature in Celsius</returns>
        // ReSharper disable once UnusedMethodReturnValue.Global
        public double GetTemperature()
        {
            var adcT = Read20(Registry.Bme280RegTempdata);
            var var1 = (adcT / 16384.0 - _coefficient.DigT1 / 1024.0) *
                          _coefficient.DigT2;
            var var2 = (adcT / 131072.0 - _coefficient.DigT1 / 8192.0) * (adcT / 131072.0 - _coefficient.DigT1 / 8192.0) * _coefficient.DigT3;
            _tFine = (long)(var1 + var2);
            return(var1 + var2) / 5120.0;
        }

        /// <summary>
        /// Retrieve pressure from sensor
        /// </summary>
        /// <returns>Pressure in Pa</returns>
        public double GetPressure()
        {
            // Call GetTemperature to get _tFine
            GetTemperature();
            var adcP = Read20(Registry.Bme280RegPressuredata);
            var var1 = _tFine / 2.0 - 64000.0;
            var var2 = var1 * var1 * _coefficient.DigP6 / 32768.0;
            var2 = var2 + var1 * _coefficient.DigP5 / 2.0;
            var2 = var2 / 4.0 + _coefficient.DigP4 * 65536.0;
            var1 = (_coefficient.DigP3 * var1 * var1 / 524288.0 + _coefficient.DigP2 * var1) / 524288.0;
            var1 = (1.0 + var1 / 32768.0) * _coefficient.DigP1;

            if (Math.Abs(var1) < 0.0001)
            {
                return 0; // avoid exception caused by division by zero
            }

            var p = 1048576.0 - adcP;
            p = (p - var2 / 4096.0) * 6250.0 / var1;
            var1 = _coefficient.DigP9 * p * p / 2147483648.0;
            var2 = p * _coefficient.DigP8 / 32768.0;
            p = p + (var1 + var2 + _coefficient.DigP7) / 16.0;
            return p;
        }

        /// <summary>
        /// Retrieve humidity from sensor
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public double GetHumidity()
        {
            var adcH = Read16(Registry.Bme280RegHumiditydata);
            var varH = _tFine - 76800.0;
            varH = (adcH - (_coefficient.DigH4 * 64.0 + _coefficient.DigH5 / 16384.0 * varH)) *
                (_coefficient.DigH2 / 65536.0 * (1.0 + _coefficient.DigH6 / 67108864.0 * varH *
                                               (1.0 + _coefficient.DigH3 / 67108864.0 * varH)));
            varH = varH * (1.0 - _coefficient.DigH1 * varH / 524288.0);
            if (varH > 100.0)
                varH = 100.0;
            else if (varH < 0.0)
                varH = 0.0;
            return varH;
        }

        /// <summary>
        /// Calculate altitude from pressure
        /// </summary>
        /// <param name="pressure"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public double CalculateAltitude(double pressure)
        {
            double a = pressure / 101325;
            double b = 1.0 / 5.25588;
            double c = Math.Pow(a, b);
            c = 1 - c;
            c = c * 44330;
            return c;
        }

        private byte Read8(byte reg)
        {
            byte[] buffer = new byte[1];
            _sensor.Write(new[] { reg });
            _sensor.Read(buffer);
            return buffer[0];
        }

        private ushort Read16LittleEndian(byte reg)
        {
            var data = Read16(reg);
            return (ushort)((data >> 8) | (data << 8));
        }

        private ushort Read16(byte reg)
        {
            byte[] buffer = new byte[2];
            _sensor.Write(new[] { reg });
            _sensor.Read(buffer);
            return (ushort)((buffer[0] << 8) | buffer[1]);
        }

        private short ReadSigned16(byte reg)
        {
            byte[] buffer = new byte[2];
            _sensor.Write(new[] { reg });
            _sensor.Read(buffer);
            return (short)((buffer[0] << 8) | buffer[1]);
        }

        private short ReadSigned16LittleEndian(byte reg)
        {
            return (short)Read16LittleEndian(reg);

        }

        private int Read20(byte reg)
        {
            _sensor.Write(new[] { reg });
            byte[] buffer = new byte[3];
            _sensor.Read(buffer);
            var data = (buffer[0] << 12) + (buffer[1] << 4) + (buffer[2] >> 4);
            return data;


        }
        private void WriteRegister(byte reg, byte value)
        {
            _sensor.Write(new[] { reg, value });
        }

    }
}
