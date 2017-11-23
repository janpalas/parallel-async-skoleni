using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Demo06
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;

            var tasks = new Task[200];
            for (int i = 0; i < 200; i++)
            {
                tasks[i] = ReadUrlContentAsync("http://www.bing.com");
                Console.WriteLine("Request {0}...", i);
            }

            await Task.WhenAll(tasks);
            Console.ReadLine();
        }

        private static async Task ReadUrlContentAsync(string url)
        {
            HttpWebRequest request = WebRequest.CreateHttp(url);
            using (WebResponse response = await request.GetResponseAsync())
            using (Stream responseStream = response.GetResponseStream())
            {
                if (responseStream == null)
                    return;

                using (var destinationStream = new MemoryStream())
                {
                    await ReadStreamAsync(responseStream, destinationStream);
                }
                

                Console.WriteLine("All read: {0}", response.ContentLength);
            }
        }

        private static async Task ReadStreamAsync(Stream source, MemoryStream destination)
        {
            var buffer = new byte[1024];
            while (true)
            {
                try
                {
                    int read = await source.ReadAsync(buffer, 0, buffer.Length);
                    if (read == 0)
                        break;

                    await destination.WriteAsync(buffer, 0, read);
                }
                catch (IOException e)
                {
                    //...
                }
            }
        }

        private static Task<WebResponse> GetResponseAsync(WebRequest request)
        {
            return Task.Factory.FromAsync(request.BeginGetResponse, request.EndGetResponse, null);
        }
    }
}
