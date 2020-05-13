using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FlatUI;
using OpenQA.Selenium.Remote;

namespace ShoePalaceBot
{
    public partial class Form : System.Windows.Forms.Form
    {
        public Form() {
            InitializeComponent();
        }

        private void StartTask_Click(object sender, EventArgs e)
        {
            Task shoePalaceTask = Task.Run(() => {
                var palaceTask = new ShoePalaceTask();
                palaceTask.textBox = OutputBox;
                palaceTask.monitorDelay = Convert.ToInt32(MonitorDelayInput.Text);
                palaceTask.retryDelay = Convert.ToInt32(RetryDelayInput.Text);
                palaceTask.getProductDetails(LinkInput.Text);
                palaceTask.addToCart(SizeInput.Text);
                palaceTask.Checkout();
            });
        }
    }
}
