using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Cache;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Demo09
{
    class Program
    {
        private static readonly Semaphore Semaphore = new Semaphore(5, 5);

        static async Task Main(string[] args)
        {
            var tasks = new Task[1000];
            for (int i = 0; i < 1000; i++)
            {
                tasks[i] = DownloadContentAsync(i);
            }

            await Task.WhenAll();


            //lock a Monitor - vice mene ekvivalentni

            //var syncRoot = new object();
            //lock (syncRoot)
            //{
                
            //}


            //Monitor.Enter(syncRoot);
            //Monitor.Exit(syncRoot);
            //bool entered = Monitor.TryEnter(syncRoot);

            Console.ReadLine();
        }


        private static async Task<byte[]> DownloadContentAsync(int i)
        {
            using (var client = new HttpClient())
            {
                Semaphore.WaitOne();
                try
                {
                    Console.WriteLine("Downloading {0}...", i);
                    byte[] result = await client.GetByteArrayAsync(new Uri("http://www.bing.com"));
                    Console.WriteLine("Downloaded {0}.", i);
                    return result;
                }
                finally
                {
                    Semaphore.Release();
                }
                
            }
        }

    }

    public class Countdown : IDisposable
    {
        private int _count;
        private ManualResetEvent _event;

        public Countdown(int count)
        {
            _event = new ManualResetEvent(false);
            _count = count;
        }


        public void WaitOne()
        {
            _event.WaitOne();
        }

        public void Signal()
        {
            if (Interlocked.Decrement(ref _count) == 0)
                _event.Set();
        }

        public void Dispose()
        {
            _event.Dispose();
        }
    }
}
