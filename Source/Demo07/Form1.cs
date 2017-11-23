using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Demo07
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            byte[] imageData = await DownloadImageAsync();

            using (var ms = new MemoryStream(imageData))
            {
                pictureBox1.Image = Image.FromStream(ms);
            }


            //await DoSomething();

            //await DoSomething();
            //ThreadPool.QueueUserWorkItem(o =>
            //{
            //    button1.Text = "OK";
            //});

            button1.Text = "OK";
        }

        async Task DoSomething()
        {
            //Zakaze prepnuti synchronization contextu zpatky
            //await Task.Delay(2000).ConfigureAwait(false);
            //await Task.Delay(2000).ConfigureAwait(false);

            await Task.Delay(2000);
            throw new Exception("ddddd");
            await Task.Delay(2000);
        }

        async Task<byte[]> DownloadImageAsync()
        {
            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(new Uri("http://atom.mu/wp-content/uploads/2017/01/London-Expat-Explore-Xmas-2017.jpg"))
                    .ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                byte[] bytes = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                return bytes;
            }
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
        }
    }
}
