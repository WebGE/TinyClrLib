using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Threading;
using GHIElectronics.TinyCLR.Pins;
using Module;
using TinyClrCore;

namespace TestG400S
{
    public class MyG400SApp:TinyClrCore.Application
    {
        public override void UpdateLoop()
        {
        }

        public override void ProgramStarted()
        {
            Led led=new Led(G400S.GpioPin.PD3);
            led.Blink();
            Thread.Sleep(1000);
            led.StopBlinking();
            //Tunes tunes=new Tunes(500);
            //Melody melody=new Melody(new MusicNote[] { new MusicNote(Tone.A3, 200), new MusicNote(Tone.B4, 200) });
            //tunes.Play(melody);
            //foreach (int val in I2CScanner.Ping(G400S.I2cBus.I2c1))
            //{
            //    Debug.WriteLine("Found: "+val.ToString("X"));
            //}

        }
    }
}
