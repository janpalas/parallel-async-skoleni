using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Demo08
{
    class Program
    {
        static void Main(string[] args)
        {
            //int i = 10;
            //int x = Volatile.Read(ref i);
            //Volatile.Write(ref x, 10000);


            //int old = Interlocked.Exchange(ref x, 10);
            //int old2 = Interlocked.CompareExchange(ref x, 10, 10);
            //Debugger.Launch();



            //using (var mre = new ManualResetEvent(false))
            //{
            //    mre.WaitOne();

            //    mre.Reset();
            //    mre.Set();
            //}

            //using (var are = new AutoResetEvent(false))
            //{
            //    are.WaitOne();

            //    are.Reset();
            //    are.Set();
            //}

            //using (var s = new Semaphore(10, 10))
            //{
            //    s.WaitOne();
            //    s.Release();
            //}

            //Uzamkne pro vsechny instance aplikace
            using (var mutex = new Mutex(false, @"Global\My-Mutex-Name"))
            {
                Console.ReadLine();
                mutex.WaitOne();
                mutex.ReleaseMutex();
            }



            Console.ReadLine();
        }


        public class SimpleLock
        {
            private int _lockTaken;

            public void Enter()
            {
                while (Interlocked.Exchange(ref _lockTaken, 1) == 1)
                    Thread.SpinWait(4000);
            }

            public void Exit()
            {
                Volatile.Write(ref _lockTaken, 0);
            }

        }
    }
}
