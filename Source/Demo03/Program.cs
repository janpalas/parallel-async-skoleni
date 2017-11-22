using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Demo03
{
    class Program
    {
        static void Main(string[] args)
        {
            //ParallelComputing();

            var sw = Stopwatch.StartNew();

            



            PlayingWithPartitioner();
            return;

            int[] pole = Enumerable.Range(0, 128).ToArray();

            int sum = 0;
            Parallel.For(0, pole.Length,
                () => 0,
                (i, pls, local) => Sum(local, pole[i]),
                local => Interlocked.Add(ref sum, local));

            Console.WriteLine(sum);

            Console.ReadLine();
        }

        private static void PlayingWithPartitioner()
        {
            int[] pole = Enumerable.Range(0, 128).ToArray();

            Parallel.For(0, pole.Length, i => { Console.WriteLine(i); });
        }

        private static void ParallelComputing()
        {
            var sw = Stopwatch.StartNew();

            int[] pole = Enumerable.Range(0, 128).ToArray();

            //int[] result = pole.AsParallel()
            //    .WithDegreeOfParallelism(3)
            //    .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
            //    .WithMergeOptions(ParallelMergeOptions.NotBuffered)
            //    .Select(x => Sum(x, x))
            //    .ToArray();

            Parallel.Invoke();


            Parallel.For(0, pole.Length, (i, parallelLoopState) =>
            {
                //Okamzite ukonceni smycky
                //parallelLoopState.Stop();

                //Necha dojet iterace s indexy o jedna nizsi nez je index v momente volani
                //Nerika to nic o indexech vyssich - muzou a nemusi se vykonat
                //parallelLoopState.Break();

                pole[i] = Sum(pole[i], pole[i]);
            });

            Console.WriteLine(sw.Elapsed);
        }

        static void InvokeWithCallBack(Action[] actions, Action onCompletedCallback)
        {
            Parallel.ForEach(actions, action =>
            {
                action();
                onCompletedCallback();
            });
        }




        static int Sum(int a, int b)
        {
            //Simuluje ze vlakno neco dela (narozdil od Thread.Sleep, ktera vlakno odlozi)
            Thread.SpinWait(4_000_000);

            return a + b;
        }
    }
}
