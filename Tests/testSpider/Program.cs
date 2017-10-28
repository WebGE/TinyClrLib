using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Bauland.Gadgeteer;
using GHIElectronics.TinyCLR.Pins;

namespace testSpider
{
    class Program
    {
        private static Button btn;

        static void Main()
        {
            try
            {
                btn = new Button(FEZCerberus.GpioPin.Socket5.Pin3, FEZCerberus.GpioPin.Socket5.Pin4);
                btn.Led = true;
                btn.Pressed += Btn_Pressed;
                btn.Released += Btn_Released;
                btn.LongPressed += Btn_LongPressed;
                while (true)
                {
                    Thread.Sleep(20);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private static void Btn_LongPressed(object sender, ButtonEventHandlerArgs args)
        {
            for (int i = 0; i < 4; i++)
            {
                btn.Led = true;
                Thread.Sleep(100);
                btn.Led = false;
                Thread.Sleep(100);
            }
        }

        private static void Btn_Released(object sender, ButtonEventHandlerArgs args)
        {
            Debug.WriteLine(args.Time + " - Released");
            btn.Led = false;
        }

        private static void Btn_Pressed(object sender, ButtonEventHandlerArgs args)
        {
            Debug.WriteLine(args.Time+" - Pressed");
            btn.Led = true;
        }
    }
}
