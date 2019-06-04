using System;
using Xamarin.Forms;

namespace Contoso.Forms.Puppet
{
    public partial class SignInInformationContentPage : ContentPage
    {

        public SignInInformationContentPage()
        {
            InitializeComponent();
        }

        public SignInInformationContentPage(string accountIdText, string accessTokenText, string idTokenText)
        {
            InitializeComponent();
            AccountIdEntry.Text = accountIdText;
            AccessTokenEntry.Text = accessTokenText;
            IdTokenEntry.Text = idTokenText;
        }

        async void OK(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}
