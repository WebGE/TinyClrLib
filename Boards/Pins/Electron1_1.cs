using GHIElectronics.TinyCLR.Pins;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberHidesStaticFromOuterClass
// ReSharper disable InconsistentNaming

namespace Bauland.Pins
{
            /// <summary>Pins definition for Electron11</summary>
    public static class Electron11
    {
        /// <summary>GPIO pin definitions</summary>
        public static class GpioPin
        {
            /// <summary>Id of GpioProvider</summary>
            public const string Id = "GHIElectronics.TinyCLR.NativeApis.STM32F4.GpioProvider\\0";

            /// <summary>Led1 definition</summary>
            public const int Led1 = STM32F4.GpioPin.PA1;
            /// <summary>Led2 definition</summary>
            public const int Led = STM32F4.GpioPin.PA8;
    
            /// <summary>GPIO pin.</summary>
            public const int PA1 = STM32F4.GpioPin.PA1;
            /// <summary>GPIO pin.</summary>
            public const int PA2 = STM32F4.GpioPin.PA2;
            /// <summary>GPIO pin.</summary>
            public const int PA3 = STM32F4.GpioPin.PA3;
            /// <summary>GPIO pin.</summary>
            public const int PA4 = STM32F4.GpioPin.PA4;
            /// <summary>GPIO pin.</summary>
            public const int PA5 = STM32F4.GpioPin.PA5;
            /// <summary>GPIO pin.</summary>
            public const int PA8 = STM32F4.GpioPin.PA8;
            /// <summary>GPIO pin.</summary>
            public const int PB3 = STM32F4.GpioPin.PB3;
            /// <summary>GPIO pin.</summary>
            public const int PB4 = STM32F4.GpioPin.PB4;
            /// <summary>GPIO pin.</summary>
            public const int PB5 = STM32F4.GpioPin.PB5;
            /// <summary>GPIO pin.</summary>
            public const int PB6 = STM32F4.GpioPin.PB6;
            /// <summary>GPIO pin.</summary>
            public const int PB7 = STM32F4.GpioPin.PB7;
        }

        /// <summary>Analog channel definition.</summary>
        public static class AdcChannel
        {
            /// <summary> Id of Adc Provider</summary>
            public const string Id = "GHIElectronics.TinyCLR.NativeApis.STM32F4.AdcProvider\\0";
            /// <summary>Pin definition.</summary>
            public const int ADC2 = STM32F4.AdcChannel.Channel12;
            /// <summary>Pin definition.</summary>
            public const int ADC3 = STM32F4.AdcChannel.Channel13;
            /// <summary>Pin definition.</summary>
            public const int ADC4 = STM32F4.AdcChannel.Channel14;
            /// <summary>Pin definition.</summary>
            public const int ADC5 = STM32F4.AdcChannel.Channel15;
        }

        /// <summary>Uart port definition.</summary>
        public static class UartPort
        {
            /// <summary>UART PA3 (RX) and PA2(TX).</summary>
            public const string Uart2 = STM32F4.UartPort.Usart2;
        }

        /// <summary>SPI Bus definition.</summary>
        public static class SpiBus
        {
            /// <summary>Default Spi definition</summary>
            public const string Spi1 = STM32F4.SpiBus.Spi1;
        }

        /// <summary>I2c Bus definition.</summary>
        public static class I2cBus
        {
            /// <summary>Default I2c definition</summary>
            public const string I2c = STM32F4.I2cBus.I2c1;
        }

        /// <summary>Pwm definition.</summary>
        public static class PwmPin
        {
            /// <summary>Pwm Controller1 definition</summary>
            public static class Controller1
            {
                /// <summary>Id of PwnProvider</summary>
                public const string Id = "GHIElectronics.TinyCLR.NativeApis.STM32F4.PwmProvider\\0";
                /// <summary>Pwm0 definition</summary>
                public const int Pwm0 = 0;
            }

            /// <summary>Pwm Controller2 definition</summary>
            public static class Controller2
            {
                /// <summary>Id of PwnProvider</summary>
                public const string Id = "GHIElectronics.TinyCLR.NativeApis.STM32F4.PwmProvider\\1";
                /// <summary>Pwm4 definition</summary>
                public const int Pwm4 = 1;
                /// <summary>Pwm5 definition</summary>
                public const int Pwm5 = 2;
                /// <summary>Pwm6 definition</summary>
                public const int Pwm6 = 3;
            }

            /// <summary>Pwm Controller4 definition</summary>
            public static class Controller4
            {
                /// <summary>Id of PwnProvider</summary>
                public const string Id = "GHIElectronics.TinyCLR.NativeApis.STM32F4.PwmProvider\\3";
                /// <summary>Pwm11 definition</summary>
                public const int Pwm11 = 0;
                /// <summary>Pwm12 definition</summary>
                public const int Pwm12 = 1;
            }
        }
    }
}
