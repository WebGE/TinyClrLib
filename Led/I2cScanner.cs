using System;
using System.Collections;

namespace GroveModule
{
    public static class I2cScanner
    {
        public static Array GetAddress()
        {
            ArrayList list=new ArrayList();
            return list.ToArray(typeof(byte));
        }
    }

}