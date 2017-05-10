using Microsoft.Azure.Mobile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Contoso.WinForms.Puppet
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            MobileCenter.LogLevel = LogLevel.Verbose;
            MobileCenter.Configure("42f4a839-c54c-44da-8072-a2f2a61751b2");
            MobileCenter.SetLogUrl("https://in-integration.dev.avalanch.es");
            MobileCenter.Enabled = true;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
