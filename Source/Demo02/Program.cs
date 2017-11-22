using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Demo02
{
    class Program
    {
        static void Main(string[] args)
        {
            //Zpusoby pro vytvareni tasku:

            //Task t1 = Task.Run(() => Console.WriteLine("Test"));
            //Task t2 = Task.Factory.StartNew();

            //Task<int> t3 = new Task<int>();
            //t3.Start();

            Thread.SpinWait(4_000_000);

            long durationWithoutTasks = SumWithoutTasks();
            long durationTasks = SumWithTasks();
            long durationParallel = SumInParallel();

            Console.WriteLine("Ratio: {0}", durationWithoutTasks / (decimal) durationTasks);
            Console.ReadLine();
        }

        private static long SumWithoutTasks()
        {
            int[] pole = Enumerable.Range(0, 128).ToArray();
            var sw = Stopwatch.StartNew();

            for (int i = 0; i < pole.Length; i++)
            {
                pole[i] = Sum(pole[i], pole[i]);
            }

            sw.Stop();
            Console.WriteLine("Without tasks: {0}, {1}", sw.Elapsed, pole.Sum());
            return sw.ElapsedMilliseconds;
        }

        private static long SumWithTasks()
        {
            int[] pole = Enumerable.Range(0, 128).ToArray();
            var tasks = new Task[pole.Length];

            var sw = Stopwatch.StartNew();

            for (int i = 0; i < pole.Length; i++)
            {
                //Tohle je ekvivalentni tomu nize, ale je riziko, kdyz k tomu nekdo prijde, tak hrozi, ze 
                //nepochopi to vytvareni promenne index a refaktoruje to tak, ze index odstrani a to uz je chybne...

                //int index = i;
                //tasks[i] = Task.Run(() => pole[index] = Sum(pole[index], pole[index]));


                tasks[i] = Task.Factory.StartNew(o =>
                {
                    int index1 = (int) o;
                    return pole[index1] = Sum(pole[index1], pole[index1]);
                }, i);
            }

            Task.WaitAll(tasks);

            sw.Stop();
            Console.WriteLine("With tasks: {0}, {1}", sw.Elapsed, pole.Sum());
            return sw.ElapsedMilliseconds;
        }

        private static long SumInParallel()
        {
            int[] pole = Enumerable.Range(0, 128).ToArray();
            var sw = Stopwatch.StartNew();

            Parallel.For(0, pole.Length, new ParallelOptions {MaxDegreeOfParallelism = 4},
                i => pole[i] = Sum(pole[i], pole[i]));

            sw.Stop();
            Console.WriteLine("Parallel: {0}, {1}", sw.Elapsed, pole.Sum());
            return sw.ElapsedMilliseconds;
        }

        static int Sum(int a, int b)
        {
            //Simuluje ze vlakno neco dela (narozdil od Thread.Sleep, ktera vlakno odlozi)
            Thread.SpinWait(4_000_000);
            return a + b;
        }
    }
}
