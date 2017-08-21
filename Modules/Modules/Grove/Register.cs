namespace I2cGroveColor
{
    public enum Register
    {
        Control=0x00,
        Timing=0x01,
        Interrupt=0x02,
        InterruuptSource=0x03,
        // ReSharper disable once InconsistentNaming
        ID=0x04,
        Gain=0x07,
        LowThresholdLowByte = 0x08,
        LowThresholdHighByte = 0x09,
        HighThresholdLowByte = 0x0A,
        HighThresholdHighByte = 0x0B,
        BlockRead=0x0F,
        Data1Low = 0x10,
        Data1High = 0x11,
        Data2Low = 0x12,
        Data2High = 0x13,
        Data3Low = 0x14,
        Data3High = 0x15,
        Data4Low = 0x16,
        Data4High = 0x17
    }
}
