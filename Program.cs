/*
 * 
 * ShoePalaceBot - Written by Ascending
 * Basic Selenium automation checkout software
 * 
 * Credits go to the developers of FlatUI for the framework
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShoePalaceBot
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form());
        }
    }
}
