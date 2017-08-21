using System;
using System.Collections;
using System.Text;
using System.Threading;

namespace I2cGroveColor
{
    public enum Transaction
    {
        Byte = 0x00 << 5,
        Word = 0x01 << 5,
        Block = 0x02 << 5,
        InterruptClear = 0x03 << 5
    }
}
