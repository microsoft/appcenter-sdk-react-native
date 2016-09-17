using System;

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


        private void CrashMe(object sender, EventArgs e)
        {
            /* Crash with a division by zero. */
            var count = 0;
            // ReSharper disable once RedundantAssignment
            count /= count;
        }
    }
}