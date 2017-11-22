using System;
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
           for(int i = 0; i < int.MaxValue; i++)
           {
                Console.WriteLine(i);
                var t = new Thread(o =>
                {
                    Thread.Sleep(-1);
                    //Console.WriteLine(i);
                    //Thread.Sleep(int.MaxValue);
                });

                //Pokud chceme jednomu vlaknu zvysit prioritu, tak je lepsi vsem ostatnim vlaknum
                //prioritu snizit, nez jednomu zvysit
                t.Priority = ThreadPriority.BelowNormal;
                t.Name = "Testovaci-vlakno";

                t.Start();
            }

            Console.ReadLine();
        }
    }
}
