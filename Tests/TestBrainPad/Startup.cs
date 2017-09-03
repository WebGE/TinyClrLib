using System.Diagnostics;
using System.Threading;
using GHIElectronics.TinyCLR.BrainPad;

namespace TestBrainPad
{
    [DebuggerNonUserCode]
    public class Startup
    {
        public static void Main()
        {
            var p = new Program();

            p.BrainPadSetup();

            while (true)
            {
                p.BrainPadLoop();

                Thread.Sleep(10);
            }
        }
    }

    [DebuggerNonUserCode]
    public static class BrainPad
    {
        public static void WriteToComputer(string message) => Debug.WriteLine(message);
        public static void WriteToComputer(int message) => BrainPad.WriteToComputer(message.ToString("N0"));
        public static void WriteToComputer(double message) => BrainPad.WriteToComputer(message.ToString("N4"));

        public static Accelerometer Accelerometer { get; } = new Accelerometer();
        public static Buttons Buttons { get; } = new Buttons();
        public static Buzzer Buzzer { get; } = new Buzzer();
        public static Display Display { get; } = new Display();
        public static LightBulb LightBulb { get; } = new LightBulb();
        public static LightSensor LightSensor { get; } = new LightSensor();
        public static ServoMotors ServoMotors { get; } = new ServoMotors();
        public static TemperatureSensor TemperatureSensor { get; } = new TemperatureSensor();
        public static Wait Wait { get; } = new Wait();
    }
}
