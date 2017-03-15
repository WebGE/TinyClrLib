using System.Threading;

namespace Application
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
                Thread.Sleep(5);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        public abstract void ProgramStarted();
    }

}