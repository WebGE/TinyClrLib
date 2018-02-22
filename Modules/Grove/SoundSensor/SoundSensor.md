# Grove SoundSensor modules
Version: __0.8.0__

## Connections ##
Grove SoundSensor is connected as followed on [Netduino3](http://developer.wildernesslabs.co/Netduino/About/):

![Schematic](SoundSensor-Netduino3-with-base-shield.jpg)

Grove SoundSensor | Mainboard with base shield
---------------- | ----------
 Yellow wire | Socket A0

## Example of code:
```CSharp
using System.Diagnostics;
using System.Threading;
using Bauland.Grove;
using Bauland.Pins;

namespace TestSoundSensor
{
    static class Program
    {
        static void Main()
        {
            // Grove SoundSensor module is connected on pin A0 of Netduino3 with base shield
            SoundSensor soundSensor=new SoundSensor(Netduino3.AdcChannel.A0);

            while (true)
            {
                Debug.WriteLine("Level: "+soundSensor.ReadLevel().ToString("F"));
                Thread.Sleep(1000);
            }
        }
    }
}
```
