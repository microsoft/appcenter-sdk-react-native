using System;
using Microsoft.AppCenter;
using Xamarin.Forms;

namespace Contoso.Forms.Demo
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public partial class AppCenterContentPage : ContentPage
    {
        public AppCenterContentPage()
        {
            InitializeComponent();
            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS)
            {
                Icon = "bolt.png";
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            AppCenterEnabledSwitchCell.On = await AppCenter.IsEnabledAsync();
        }

        async void UpdateEnabled(object sender, ToggledEventArgs e)
        {
            await AppCenter.SetEnabledAsync(e.Value);
        }

        void UserIdCompleted(object sender, EventArgs e)
        {
            //TODO: Enable this once NuGet package is updated as part of release process.
            var text = string.IsNullOrEmpty(UserIdEntryCell.Text) ? null : UserIdEntryCell.Text;
            AppCenter.SetUserId(text);
        }
    }
}
