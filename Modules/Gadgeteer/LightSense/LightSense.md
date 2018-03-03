# LightSense modules
Version: __0.9.0__

## Connections ##
LightSense is connected as followed on [Cerberus](http://docs.ghielectronics.com/hardware/legacy_products/gadgeteer/fez_cerberus.html):

![Schematic](Gadgeteer-LightSense-Cerberus.jpg)

LightSense    | Mainboard
------------- | ----------
Socket Type A | Socket 4

## Example of code:
```CSharp
using System.Diagnostics;
using System.Threading;
using Bauland.Gadgeteer;
using GHIElectronics.TinyCLR.Pins;

namespace TestLightSense
{
    static class Program
    {
        static void Main()
        {
            // LightSense connected on Socket 4 (Type A) of FEZ Cerberus mainboard.
            LightSense lightSense = new LightSense(FEZCerberus.AdcChannel.Socket4.Pin3);

            while (true)
            {
                Debug.WriteLine("Light value: " + lightSense.Value + ", ratio: " + lightSense.Ratio.ToString("F1"));
                Thread.Sleep(1000);
            }
        }
    }
}
```
