using System;
using System.Diagnostics;

namespace Example
{
    public class Program
    {
        public static void Main()
        {
            try
            {
                MyApp app = new MyApp();
                app.Run();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}