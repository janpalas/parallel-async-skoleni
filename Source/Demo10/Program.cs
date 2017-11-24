using System;
using System.Threading;

namespace Demo10
{
    class Program
    {
        static void Main()
        {
            using (new Mutex(false, @"Global\SingleInstanceApp", out bool createdNew))
            {
                if (!createdNew)
                {
                    Console.WriteLine("Another instance already running. Exiting...");
                    Console.ReadLine();
                    return;
                }

                Console.WriteLine("Running...");
                while (true)
                {
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
