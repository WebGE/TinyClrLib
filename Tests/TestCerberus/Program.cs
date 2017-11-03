using System.Threading;
using Bauland.Gadgeteer;
using GHIElectronics.TinyCLR.Pins;

namespace TestCerberus
{
    class Program
    {
        private static Tunes _tunes;

        static void Main()
        {
            //_display = new DisplayN18(FEZCerberus.GpioPin.Socket6.Pin3, FEZCerberus.GpioPin.Socket6.Pin4, FEZCerberus.GpioPin.Socket6.Pin5, FEZCerberus.SpiBus.Socket6, FEZCerberus.GpioPin.Socket6.Pin6);
            //_display.Clear();
            //_display.TurnOn();
            //_display.DrawText(10, 10, "OK !!!", Color.Blue);

            _tunes = new Tunes(FEZCerberus.PwmPin.Controller3.Id, FEZCerberus.PwmPin.Controller3.Socket4.Pin9);
            Melody melody = new Melody(new MusicNote(Tone.A3, 100), new MusicNote(Tone.C3, 200), new MusicNote(Tone.E3, 200));
            _tunes.Play(melody);
            while (true)
            {
                Thread.Sleep(20);
            }
        }
    }
}
