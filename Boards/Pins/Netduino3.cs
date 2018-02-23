using GHIElectronics.TinyCLR.Pins;
// ReSharper disable InconsistentNaming

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberHidesStaticFromOuterClass

namespace Bauland.Pins
{
    /// <summary>Pins definition for Netduino3</summary>
    public static class Netduino3
    {
        /// <summary>GPIO pin definitions</summary>
        public static class GpioPin
        {
            /// <summary>Id of GpioProvider</summary>
            public const string Id = "GHIElectronics.TinyCLR.NativeApis.STM32F4.GpioProvider\\0";

            /// <summary>Debug LED definition</summary>
            public const int Led = STM32F4.GpioPin.PA10;
            /// <summary>PowerLed definition</summary>
            public const int PowerLed = STM32F4.GpioPin.PC13;

            /// <summary>SD Card Dectect definition</summary>
            public const int SdCardDetect = STM32F4.GpioPin.PB2;
            /// <summary>SD Card Power Control definition</summary>
            public const int SdCardPwrCtrl = STM32F4.GpioPin.PB1;

            /// <summary>GPIO pin.</summary>
            public const int D0 = STM32F4.GpioPin.PC7;
            /// <summary>GPIO pin.</summary>
            public const int D1 = STM32F4.GpioPin.PC6;
            /// <summary>GPIO pin.</summary>
            public const int D2 = STM32F4.GpioPin.PA3;
            /// <summary>GPIO pin.</summary>
            public const int D3 = STM32F4.GpioPin.PA2;
            /// <summary>GPIO pin.</summary>
            public const int D4 = STM32F4.GpioPin.PB12;
            /// <summary>GPIO pin.</summary>
            public const int D5 = STM32F4.GpioPin.PB8;
            /// <summary>GPIO pin.</summary>
            public const int D6 = STM32F4.GpioPin.PB9;
            /// <summary>GPIO pin.</summary>
            public const int D7 = STM32F4.GpioPin.PA1;
            /// <summary>GPIO pin.</summary>
            public const int D8 = STM32F4.GpioPin.PA0;
            /// <summary>GPIO pin.</summary>
            public const int D9 = STM32F4.GpioPin.PE5;
            /// <summary>GPIO pin.</summary>
            public const int D10 = STM32F4.GpioPin.PB10;
            /// <summary>GPIO pin.</summary>
            public const int D11 = STM32F4.GpioPin.PB15;
            /// <summary>GPIO pin.</summary>
            public const int D12 = STM32F4.GpioPin.PB14;
            /// <summary>GPIO pin.</summary>
            public const int D13 = STM32F4.GpioPin.PB13;
            /// <summary>GPIO pin.</summary>
            public const int A0 = STM32F4.GpioPin.PC0;
            /// <summary>GPIO pin.</summary>
            public const int A1 = STM32F4.GpioPin.PC1;
            /// <summary>GPIO pin.</summary>
            public const int A2 = STM32F4.GpioPin.PC2;
            /// <summary>GPIO pin.</summary>
            public const int A3 = STM32F4.GpioPin.PC3;
            /// <summary>GPIO pin.</summary>
            public const int A4 = STM32F4.GpioPin.PC4;
            /// <summary>GPIO pin.</summary>
            public const int A5 = STM32F4.GpioPin.PC5;

            /// <summary>Socket definition.</summary>
            public static class GoPort1
            {
                /// <summary>Pin definition.</summary>
                public const int Pin3 = STM32F4.GpioPin.PD13;
                /// <summary>Pin definition.</summary>
                public const int Pin4 = STM32F4.GpioPin.PD8;
                /// <summary>Pin definition.</summary>
                public const int Pin5 = STM32F4.GpioPin.PD9;
                /// <summary>Pin definition.</summary>
                public const int Pin6 = STM32F4.GpioPin.PD0;
                /// <summary>LED definition.</summary>
                public const int Led = STM32F4.GpioPin.PE9;
                /// <summary>Power On definition.</summary>
                public const int PwrOn = STM32F4.GpioPin.PD7;
            }

            /// <summary>Socket definition.</summary>
            public static class GoPort2
            {
                /// <summary>Pin definition.</summary>
                public const int Pin3 = STM32F4.GpioPin.PD14;
                /// <summary>Pin definition.</summary>
                public const int Pin4 = STM32F4.GpioPin.PE8;
                /// <summary>Pin definition.</summary>
                public const int Pin5 = STM32F4.GpioPin.PE7;
                /// <summary>Pin definition.</summary>
                public const int Pin6 = STM32F4.GpioPin.PD1;
                /// <summary>LED definition.</summary>
                public const int Led = STM32F4.GpioPin.PE11;
                /// <summary>Power On definition.</summary>
                public const int PwrOn = STM32F4.GpioPin.PD10;
            }

            /// <summary>Socket definition.</summary>
            public static class GoPort3
            {
                /// <summary>Pin definition.</summary>
                public const int Pin3 = STM32F4.GpioPin.PD15;
                /// <summary>Pin definition.</summary>
                public const int Pin4 = STM32F4.GpioPin.PE1;
                /// <summary>Pin definition.</summary>
                public const int Pin5 = STM32F4.GpioPin.PE0;
                /// <summary>Pin definition.</summary>
                public const int Pin6 = STM32F4.GpioPin.PD2;
                /// <summary>LED definition.</summary>
                public const int Led = STM32F4.GpioPin.PB0;
                /// <summary>Power On definition.</summary>
                public const int PwrOn = STM32F4.GpioPin.PE14;
            }
        }

        /// <summary>Analog channel definition.</summary>
        public static class AdcChannel
        {
            /// <summary> Id of Adc Provider</summary>
            public const string Id = "GHIElectronics.TinyCLR.NativeApis.STM32F4.AdcProvider\\0";
            /// <summary>Pin definition.</summary>
            public const int A0 = STM32F4.AdcChannel.Channel10;
            /// <summary>Pin definition.</summary>
            public const int A1 = STM32F4.AdcChannel.Channel11;
            /// <summary>Pin definition.</summary>
            public const int A2 = STM32F4.AdcChannel.Channel12;
            /// <summary>Pin definition.</summary>
            public const int A3 = STM32F4.AdcChannel.Channel13;
            /// <summary>Pin definition.</summary>
            public const int A4 = STM32F4.AdcChannel.Channel14;
            /// <summary>Pin definition.</summary>
            public const int A5 = STM32F4.AdcChannel.Channel15;
        }

        /// <summary>Uart port definition.</summary>
        public static class UartPort
        {

            /// <summary>Socket definition.</summary>
            public const string GoPort1 = STM32F4.UartPort.Usart3;
            /// <summary>Socket definition.</summary>
            public const string GoPort2 = STM32F4.UartPort.Uart7;
            /// <summary>Socket definition.</summary>
            public const string GoPort3 = STM32F4.UartPort.Uart8;

            /// <summary>UART D0 (RX) and D1 (TX).</summary>
            public const string Uart6 = STM32F4.UartPort.Usart6;
        }

        /// <summary>SPI Bus definition.</summary>
        public static class SpiBus
        {
            /// <summary>Socket GoPort 1 Spi definition</summary>
            public const string GoPort1 = STM32F4.SpiBus.Spi4;
            /// <summary>Socket GoPort 2 Spi definition</summary>
            public const string GoPort2 = STM32F4.SpiBus.Spi4;
            /// <summary>Socket GoPort 3 Spi definition</summary>
            public const string GoPort3 = STM32F4.SpiBus.Spi4;
            /// <summary>Default Spi definition</summary>
            public const string Spi2 = STM32F4.SpiBus.Spi2;
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
                public const int Led_Goport1 = 0;
                /// <summary>Pwm1 definition</summary>
                public const int Led_Goport2 = 1;
                /// <summary>Pwm2 definition</summary>
                public const int Led = 2;
                /// <summary>Pwm3 definition</summary>
                public const int Led_Goport3 = 3;
            }

            /// <summary>Pwm Controller2 definition</summary>
            public static class Controller2
            {
                /// <summary>Id of PwnProvider</summary>
                public const string Id = "GHIElectronics.TinyCLR.NativeApis.STM32F4.PwmProvider\\1";
                /// <summary>Pwm2 definition</summary>
                public const int D10 = 2;
            }

            /// <summary>Pwm Controller5 definition</summary>
            public static class Controller5
            {
                /// <summary>Id of PwnProvider</summary>
                public const string Id = "GHIElectronics.TinyCLR.NativeApis.STM32F4.PwmProvider\\4";
                /// <summary>Pwm0 definition</summary>
                public const int D8 = 0;
                /// <summary>Pwm1 definition</summary>
                public const int D7 = 1;
                /// <summary>Pwm2 definition</summary>
                public const int D3 = 2;
                /// <summary>Pwm3 definition</summary>
                public const int D2 = 3;
            }

            /// <summary>Pwm Controller8 definition</summary>
            public static class Controller8
            {
                /// <summary>Id of PwnProvider</summary>
                public const string Id = "GHIElectronics.TinyCLR.NativeApis.STM32F4.PwmProvider\\7";
                /// <summary>Pwm0 definition</summary>
                public const int D1 = 0;
                /// <summary>Pwm1 definition</summary>
                public const int D0 = 1;
            }

            /// <summary>Pwm Controller9 definition</summary>
            public static class Controller9
            {
                /// <summary>Id of PwnProvider</summary>
                public const string Id = "GHIElectronics.TinyCLR.NativeApis.STM32F4.PwmProvider\\8";
                /// <summary>Pwm0 definition</summary>
                public const int D9 = 0;
            }

            /// <summary>Pwm Controller10 definition</summary>
            public static class Controller10
            {
                /// <summary>Id of PwnProvider</summary>
                public const string Id = "GHIElectronics.TinyCLR.NativeApis.STM32F4.PwmProvider\\9";
                /// <summary>Pwm0 definition</summary>
                public const int D5 = 0;
            }

            /// <summary>Pwm Controller11 definition</summary>
            public static class Controller11
            {
                /// <summary>Id of PwnProvider</summary>
                public const string Id = "GHIElectronics.TinyCLR.NativeApis.STM32F4.PwmProvider\\10";
                /// <summary>Pwm0 definition</summary>
                public const int D6 = 0;
            }
        }
    }
}
