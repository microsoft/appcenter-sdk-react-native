using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using System;
using System.Windows.Forms;

namespace Contoso.WinForms.Demo
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
            MobileCenter.LogLevel = LogLevel.Verbose;
            MobileCenter.SetLogUrl("https://in-integration.dev.avalanch.es");
            MobileCenter.Start("42f4a839-c54c-44da-8072-a2f2a61751b2", typeof(Analytics), typeof(Crashes));

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
