using System;
using System.Diagnostics;
using System.Threading;

namespace Demo01
{
    public class Program
    {
        /// <summary>
        /// Test pro zjisteni maximalni poctu vlaken, ktera lze vytvorit
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            TestThreadPool();

            Console.ReadLine();
        }

        private static void TestThreadPool()
        {
            for (int i = 0; i < 5000; i++)
            {
                //ThreadPool si resi vytvareni vlaken sam, nejdrive kazdych 500 ms vyhodnocuje, jestli ma aplikace dostatek vlaken.
                //Pokud ne, tak prida dalsi, pokud jich je prebytek, tak zase ubere
                ThreadPool.QueueUserWorkItem(o => { Thread.Sleep(-1); }, null);

                //Thread.Sleep(0) = Thread.Yield = vzdavam se procesoru, ale ten mi muze byt hned zase pridelen
                //Thread.Sleep(16) - to je minimalni rozliseni procesoru (16 = 1sec./64Hz = 1000 ms / 64 = 15.625)
            }
            Console.WriteLine("Done");

            var sw = Stopwatch.StartNew();
            Console.WriteLine(sw.Elapsed);
            Thread.Sleep(1);
            Console.WriteLine(sw.Elapsed);
        }

        private static void TestMaxThreads()
        {
            for (int i = 0; i < int.MaxValue; i++)
            {
                var t = new Thread(o => { Thread.Sleep(-1); });

                //Pokud chceme jednomu vlaknu zvysit prioritu, tak je lepsi vsem ostatnim vlaknum
                //prioritu snizit, nez jednomu zvysit
                t.Priority = ThreadPriority.BelowNormal;
                t.Name = "Testovaci-vlakno";

                t.Start();
                Console.WriteLine(i);
            }
        }
    }
}