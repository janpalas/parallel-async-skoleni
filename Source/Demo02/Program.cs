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
            //Spusti debugger
            //Debugger.Launch();
            //Breakpoint v kodu
            //Debugger.Break();


            //Zpusoby pro vytvareni tasku:

            //Task t1 = Task.Run(() => Console.WriteLine("Test"));
            //Task t2 = Task.Factory.StartNew();

            //Task<int> t3 = new Task<int>();
            //t3.Start();

            //long durationWithoutTasks = SumWithoutTasks();
            //long durationTasks = SumWithTasks();
            //long durationParallel = SumInParallel();

            //Console.WriteLine("Ratio: {0}", durationWithoutTasks / (decimal) durationTasks);


            //SumMembersWithTasks();
            CancelingTasks();

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


        private static void SumMembersWithTasks()
        {
            int[] pole = Enumerable.Range(0, 128).ToArray();
            var sw = Stopwatch.StartNew();

            int result = SumArray(pole);
            Console.WriteLine("Sum members: {0}, {1}", sw.Elapsed, result);
        }

        private static void CancelingTasks()
        {
            using (var cts = new CancellationTokenSource())
            {
                CancellationToken token = cts.Token;
                cts.CancelAfter(2000);
                //cts.Cancel();

                var primaryTask = Task.Run(() =>
                {
                    token.ThrowIfCancellationRequested();
                }, token);
                Thread.Sleep(200);

                Task continuationTask = primaryTask.ContinueWith(t2 => { }, TaskContinuationOptions.OnlyOnCanceled);
                //Tohle spadne pokud ma continuationTaskNastaveno TaskContinuationOptions.OnlyOnCanceled a zaroven 
                //primaryTask dobehl v poradku. V tomto pripade je zde continuationTask ve stavu Cancelled
                continuationTask.Wait();

                Console.WriteLine(continuationTask.Status);
            }
        }

        private static int SumArray(int[] pole)
        {
            switch (pole.Length)
            {
                case 0:
                    return 0;
                case 1:
                    return pole[0];
                case 2:
                    return Sum(pole[0], pole[1]);
                default:
                    var left = pole.Take(pole.Length / 2).ToArray();
                    var right = pole.Skip(pole.Length / 2).ToArray();
                    var leftTask = Task.Run(() => SumArray(left));
                    var rightTask = Task.Run(() => SumArray(right));
                    return Sum(leftTask.Result, rightTask.Result);
            }
        }

        private static int SumArray(int[] pole, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            switch (pole.Length)
            {
                case 0:
                    return 0;
                case 1:
                    return pole[0];
                case 2:
                    return Sum(pole[0], pole[1]);
                default:
                    var left = pole.Take(pole.Length / 2).ToArray();
                    var right = pole.Skip(pole.Length / 2).ToArray();
                    var leftTask = Task.Run(() => SumArray(left, token));
                    var rightTask = Task.Run(() => SumArray(right, token));
                    return Sum(leftTask.Result, rightTask.Result, token);
            }
        }

        static int Sum(int a, int b, CancellationToken token = default(CancellationToken))
        {
            for (int i = 0; i < 4; i++)
            {
                //Simuluje ze vlakno neco dela (narozdil od Thread.Sleep, ktera vlakno odlozi)
                Thread.SpinWait(4_000_000);
                token.ThrowIfCancellationRequested();
            }

            
            return a + b;
        }
    }
}
