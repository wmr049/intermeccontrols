#pragma warning disable 0114,0219
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Hasci.TestApp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [MTAThread]
        static void Main()
        {
            try
            {
                Application.Run(new Form1());
            }
            catch { }
        }
    }
}