﻿namespace GHIElectronics.TinyCLR.Pins
{
    /// <summary>Board definition for the Netduino3.</summary>
    public static class Netduino3
    {
        /// <summary>GPIO pin definitions.</summary>
        public static class GpioPin
        {

            /// <summary>Debug LED definition</summary>
            public const int Led = STM32F4.GpioPin.PA10;
            public const int PowerLed = STM32F4.GpioPin.PC13;

            /// <summary>SD Card Dectect definition</summary>
            public const int SdCardDetect = STM32F4.GpioPin.PB2;
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
            public const string Usart6 = STM32F4.UartPort.Usart6;
        }

        /// <summary>SPI Bus definition.</summary>
        public static class SpiBus
        {

            /// <summary>Socket definition.</summary>
            public const string GoPort1 = STM32F4.SpiBus.Spi4;
            public const string GoPort2 = STM32F4.SpiBus.Spi4;
            public const string GoPort3 = STM32F4.SpiBus.Spi4;
            public const string Spi2 = STM32F4.SpiBus.Spi2;
        }
    }
}