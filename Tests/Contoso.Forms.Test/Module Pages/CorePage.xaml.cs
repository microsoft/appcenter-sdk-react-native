using Microsoft.Azure.Mobile;

using Xamarin.Forms;

namespace Contoso.Forms.Test
{
    public partial class CorePage : ContentPage
    {
        public CorePage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (InstallIdLabel != null)
            {
                InstallIdLabel.Text = MobileCenter.InstallId?.ToString();
            }
        }

        void DismissPage(object sender, System.EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}
