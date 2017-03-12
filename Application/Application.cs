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
            this.ProgramStarted();
            while (true)
            {
                Thread.Sleep(5);
            }
        }

        public abstract void ProgramStarted();
    }

}