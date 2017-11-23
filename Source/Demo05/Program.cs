using System;
using System.IO;
using System.Net;

namespace Demo05
{
    class Program
    {
        static void Main(string[] args)
        {
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;

            CorrectSolution();


            Console.ReadLine();
        }

        private static void WrongSolution()
        {
            for (int i = 0; i < 10; i++)
            {
                HttpWebRequest request = WebRequest.CreateHttp("http://www.bing.com");
                request.BeginGetResponse(ar =>
                {
                    WebResponse response = request.EndGetResponse(ar);


                    Stream stream = response.GetResponseStream();
                    if (stream == null)
                        return;

                    int responseIndex = (int) ar.AsyncState;
                    var streamData = new byte[response.ContentLength];
                    int totalReadBytes = 0;
                    bool isCompleted = false;

                    while (!isCompleted)
                    {
                        stream.BeginRead(streamData, totalReadBytes, streamData.Length, ar2 =>
                        {
                            int readBytesCount = stream.EndRead(ar2);
                            totalReadBytes += readBytesCount;
                            isCompleted = readBytesCount == 0;

                            FileStream fileStream =
                                File.Open(Path.Combine("data", $"bing-{responseIndex}.txt"), FileMode.Append);
                            fileStream.BeginWrite(streamData, 0, readBytesCount, ar3 => { fileStream.EndWrite(ar3); }, null);
                        }, null);
                    }


                    Console.WriteLine(response.ContentLength);
                }, i);

                Console.WriteLine(i);
            }
        }

        private static void CorrectSolution()
        {
            for (int i = 0; i < 10; i++)
            {
                HttpWebRequest request = WebRequest.CreateHttp("http://www.bing.com");
                request.BeginGetResponse(ar =>
                {
                    WebResponse response = request.EndGetResponse(ar);
                    Stream source = response.GetResponseStream();
                    if (source == null)
                        return;

                    var index = (int) ar.AsyncState;
                    FileStream destination = File.Open(Path.Combine("data", $"bing-{index}.txt"), FileMode.Append);

                    ReadStream(source, destination,
                        () =>
                        {
                            response.Dispose();
                            source.Dispose();
                            destination.Dispose();
                        },
                        ex =>
                        {
                            response.Dispose();
                            source.Dispose();
                            destination.Dispose();
                        });

                }, i);

                Console.WriteLine(i);
            }
        }

        static void ReadStream(Stream source, Stream destination, Action onDone, Action<Exception> onException)
        {
            var buffer = new byte[1024];
            source.BeginRead(buffer, 0, buffer.Length, ar =>
            {
                int read = source.EndRead(ar);
                if (read == 0)
                {
                    onDone();
                    return;
                }

                destination.BeginWrite(buffer, 0, read, ar2 =>
                {
                    destination.EndWrite(ar2);
                    ReadStream(source, destination, onDone, onException);

                }, null);

            }, null);
        }
    }
}
