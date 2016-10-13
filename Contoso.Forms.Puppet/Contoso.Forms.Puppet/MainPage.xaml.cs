using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Sonoma.Xamarin.Crashes;

namespace Contoso.Forms.Puppet
{
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void GoToSubPage(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SubPage());
        }

        private void CrashWithDivsionByZero(object sender, EventArgs e)
        {
            Debug.WriteLine(0 / int.Parse("0"));
        }

        private void TrackHandledAggregateException(object sender, EventArgs e)
        {
            Crashes.TrackException(PrepareException());
        }

        private static Exception PrepareException()
        {
            try
            {
                throw new AggregateException(SendHttp(), new ArgumentException("Invalid parameter", ValidateLength()));
            }
            catch (Exception e)
            {
                return e;
            }
        }

        private static Exception SendHttp()
        {
            try
            {
                throw new IOException("Network down");
            }
            catch (Exception e)
            {
                return e;
            }
        }

        private static Exception ValidateLength()
        {
            try
            {
                throw new ArgumentOutOfRangeException(null, "It's over 9000!");
            }
            catch (Exception e)
            {
                return e;
            }
        }
    }
}