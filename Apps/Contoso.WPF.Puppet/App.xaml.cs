using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using System.Windows;

namespace Contoso.WPF.Puppet
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            MobileCenter.LogLevel = LogLevel.Verbose;
            MobileCenter.SetLogUrl("https://in-integration.dev.avalanch.es");
            MobileCenter.Start("42f4a839-c54c-44da-8072-a2f2a61751b2", typeof(Analytics), typeof(Crashes));
        }
    }
}
