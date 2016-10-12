using System;
using System.Diagnostics;
using System.IO;

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

        private void CrashWithAggregateException(object sender, EventArgs e)
        {
            throw new AggregateException(new IOException("Network down"), new ArgumentException("Invalid parameter", new ArgumentOutOfRangeException(nameof(sender), "It's over 9000!")));
        }
    }
}