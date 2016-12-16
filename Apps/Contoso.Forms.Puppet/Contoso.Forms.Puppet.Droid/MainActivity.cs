using Android.App;
using Android.Content.PM;
using Android.OS;
using Microsoft.Azure.Mobile;

namespace Contoso.Forms.Puppet.Droid
{
    [Activity(Label = "MCFPuppet", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Forms.Forms.Init(this, savedInstanceState);

            MobileCenterLog.Assert(App.LogTag, "MobileCenter.Configured=" + MobileCenter.Configured);
            MobileCenterLog.Assert(App.LogTag, "MobileCenter.InstallId (before configure)=" + MobileCenter.InstallId);
            MobileCenter.SetServerUrl("https://in-integration.dev.avalanch.es");
            MobileCenter.Configure("7f222d3c-0f5e-421b-93e7-f862c462e07e");

            LoadApplication(new App());
        }
    }
}
