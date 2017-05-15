using GHIElectronics.TinyCLR.BrainPad;

[System.Diagnostics.DebuggerNonUserCode]
class Startup
{
    static void Main()
    {
        var p = new Program();

        p.BrainPadSetup();

        while (true)
        {
            p.BrainPadLoop();

            System.Threading.Thread.Sleep(10);
        }
    }
}

partial class Program
{
    public Board BrainPad { get; }

    public Program()
    {
        this.BrainPad = new Board();
    }
}