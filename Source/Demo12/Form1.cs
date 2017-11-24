using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Demo12
{
    public partial class Form1 : Form
    {
        private SemaphoreSlim _sem;

        public Form1()
        {
            InitializeComponent();

            _sem = new SemaphoreSlim(5, 5);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await _sem.WaitAsync();
            await Task.Delay(2000);
            _sem.Release();

        }
    }
}
