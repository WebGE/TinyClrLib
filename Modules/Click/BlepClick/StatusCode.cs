using System;
using System.Collections;
using System.Text;
using System.Threading;

namespace BLEPBrainpad
{
    public enum AciStatusCode
    {
        Success=0x00,
        TransactionContinue = 0x01,
        TransactionComplete = 0x02
    }
}
