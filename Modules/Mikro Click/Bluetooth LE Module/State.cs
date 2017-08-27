using System;

namespace BlepClick
{
    public class Nrf8001Exception : Exception
    {
        public Nrf8001Exception(string message)
            : base(message)
        {
        }
    }

    public enum Nrf8001State : byte
    {
        // Section 26.1.3
        Test = 0x01,
        Setup = 0x02,
        Standby = 0x03,

        // Custom
        Unknown = 0x00,
        Resetting = 0xFF,

        Sleep = 0x11,
        Connected = 0x12,
        Connecting = 0x13,
        Bonding = 0x15
    }
}
