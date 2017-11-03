using System;
using System.Collections;
using System.Text;
using System.Threading;
using Bauland.Gadgeteer;
using GHIElectronics.TinyCLR.Pins;

namespace TestCerberus
{
    class Program
    {
        private static DisplayN18 display;
        static void Main()
        {
            display=new DisplayN18(FEZCerberus.GpioPin.Socket6.Pin3,FEZCerberus.GpioPin.Socket6.Pin4,FEZCerberus.GpioPin.Socket6.Pin5, FEZCerberus.SpiBus.Socket6,FEZCerberus.GpioPin.Socket6.Pin6);
            display.Clear();
            display.TurnOn();
            display.DrawText(10, 10, "OK !!!", Color.Blue);
            while (true)
            {
                Thread.Sleep(20);
            }
        }
    }
}
