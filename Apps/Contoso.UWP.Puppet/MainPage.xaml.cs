using System;
using Microsoft.AppCenter.Analytics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Contoso.UWP.Puppet
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            Application.Current.UnhandledException += OnUnhandledException;
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            e.Handled = HandleExceptions.IsOn;
        }

        private void TrackEvent(object sender, RoutedEventArgs e)
        {
            Analytics.TrackEvent("Test");
        }

        private void ThrowException(object sender, RoutedEventArgs e)
        {
            throw new Exception();
        }
    }
}
