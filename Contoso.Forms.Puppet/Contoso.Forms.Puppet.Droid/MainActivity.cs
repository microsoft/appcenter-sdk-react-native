using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Microsoft.Sonoma.Core;

namespace Contoso.Forms.Puppet.Droid
{
    [Activity(Label = "Contoso.Forms.Puppet.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
                    
            // Sonoma must be initialized before the App is created
            Sonoma.Initialize("44cd8722-bfe0-4748-ac14-7692e031a8a5");

            LoadApplication(new App());
        }
    }
}
