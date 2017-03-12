using System.Threading;

namespace GroveModule
{
    interface IApplication
    {
        
    }
    public abstract class Application
    {

        public virtual void Run()
        {
            this.ProgramStarted();
            while (true)
            {
                Thread.Sleep(5);
            }
        }

        public abstract void ProgramStarted();
    }

}