using Android.App;
using Android.Content.PM;
using Android.OS;
using Microsoft.Sonoma.Xamarin.Core;
using Xamarin.Forms.Platform.Android;

namespace Contoso.Forms.Puppet.Droid
{
    [Activity(Label = "SXPuppet", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    // ReSharper disable once UnusedMember.Global
    public class MainActivity : FormsApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Xamarin.Forms.Forms.Init(this, bundle);
            Sonoma.Initialize("44cd8722-bfe0-4748-ac14-7692e031a8a5");
            LoadApplication(new App());
        }

        protected override void OnDestroy()
        {
            // cause a crash by not calling base method when exiting the main page with back button
        }
    }
}