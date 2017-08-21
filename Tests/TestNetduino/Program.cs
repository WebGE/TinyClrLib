using System;
using System.Collections;
using System.Text;
using System.Threading;
using TinyClrCore;

namespace TestNetduino
{
    class Program
    {
        static void Main()
        {
            MyNetduino3App app=new MyNetduino3App();
            app.Run();
        }
    }
}
