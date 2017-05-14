using System;
using System.Collections;
using System.Text;
using System.Threading;

namespace TestG400S
{
    public class Program
    {
        public static void Main()
        {
            TinyClrCore.Application app=new MyG400SApp();
            app.Run();
        }
    }
}
