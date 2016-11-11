using Android.App;
using Android.Content.PM;
using Android.OS;
using Microsoft.Azure.Mobile;

namespace Contoso.Forms.Puppet.Droid
{
    [Activity(Label = "Contoso.Forms.Puppet.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Forms.Forms.Init(this, savedInstanceState);

            MobileCenter.Initialize("7f222d3c-0f5e-421b-93e7-f862c462e07e");

            LoadApplication(new App());
        }
    }
}
