using System.Threading;

namespace TinyClrCore
{
    interface IApplication
    {
        
    }
    public abstract class Application
    {

        public virtual void Run()
        {
            ProgramStarted();
            while (true)
            {
                UpdateLoop();
                Thread.Sleep(5);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        public abstract void UpdateLoop();

        public abstract void ProgramStarted();
    }

}