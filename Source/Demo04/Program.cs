using System;
using System.Threading;

namespace Demo04
{
    class Program
    {
        static void Main(string[] args)
        {
            var timer = new Timer(o =>
            {
                Console.WriteLine(DateTime.Now);
            }, null, 1000, 2000);

            //Zastaveni
            timer.Change(-1, -1);

            Console.ReadLine();
        }
    }
}
