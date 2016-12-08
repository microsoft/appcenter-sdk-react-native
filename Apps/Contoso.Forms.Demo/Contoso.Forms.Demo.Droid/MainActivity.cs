using Android.App;
using Android.Content.PM;
using Android.OS;

using Microsoft.Azure.Mobile;

namespace Contoso.Forms.Demo.Droid
{
    [Activity(Label = "MCFDemo", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Forms.Forms.Init(this, savedInstanceState);

            MobileCenter.Configure("987b5941-4fac-4968-933e-98a7ff29237c");

            LoadApplication(new App());
        }
    }
}
