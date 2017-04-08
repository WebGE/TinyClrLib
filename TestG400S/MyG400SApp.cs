using System;
using System.Collections;
using System.Text;
using System.Threading;
using GHIElectronics.TinyCLR.Pins;
using Module;

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
            Tunes tunes=new Tunes(500);
            Melody melody=new Melody(new MusicNote[] { new MusicNote(Tone.A3, 200), new MusicNote(Tone.B4, 200) });
            tunes.Play(melody);

        }
    }
}
