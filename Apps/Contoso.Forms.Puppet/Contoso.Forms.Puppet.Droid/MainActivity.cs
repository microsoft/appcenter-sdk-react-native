using Android.App;
using Android.Content.PM;
using Android.OS;
using Com.Microsoft.Azure.Mobile.Analytics;
using Com.Microsoft.Azure.Mobile.Analytics.Channel;
using Com.Microsoft.Azure.Mobile.Ingestion.Models;
using Microsoft.Azure.Mobile;
using Firebase.Iid;
using Android.Gms.Common;

namespace Contoso.Forms.Puppet.Droid
{
    [Activity(Label = "MCFPuppet", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            if (Intent.Extras != null)
            {
                foreach (var key in Intent.Extras.KeySet())
                {
                    var val = Intent.Extras.GetString(key);
                    MobileCenterLog.Debug("zander", $"Key: {key} Value: {val}");
                }
            }
            IsPlayServicesAvailable();
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Forms.Forms.Init(this, savedInstanceState);

            AndroidAnalytics.SetListener(new AndroidAnalyticsListener());
            
            LoadApplication(new App());
            MobileCenterLog.Assert("zander", "InstanceID token: " + FirebaseInstanceId.Instance.Token);

        }

        public bool IsPlayServicesAvailable()
        {
        	int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
        	if (resultCode != ConnectionResult.Success)
        	{
        		if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                    MobileCenterLog.Assert("IsPlayServicesAvailable", GoogleApiAvailability.Instance.GetErrorString(resultCode));
        		else
        		{
                    MobileCenterLog.Assert("IsPlayServicesAvailable","This device is not supported");
        		}
        		return false;
        	}
        	else
        	{
                MobileCenterLog.Assert("IsPlayServicesAvailable","Google Play Services is available.");
        		return true;
            }
        }
    }

    public class AndroidAnalyticsListener : Java.Lang.Object, IAnalyticsListener
    {
        public void OnSendingFailed(ILog log, Java.Lang.Exception e)
        {
            MobileCenterLog.Debug(App.LogTag, "Analytics listener OnSendingFailed with exception: " + e);
        }

        public void OnSendingSucceeded(ILog log)
        {
            MobileCenterLog.Debug(App.LogTag, "Analytics listener OnSendingSucceeded");
        }

        public void OnBeforeSending(ILog log)
        {
            MobileCenterLog.Debug(App.LogTag, "Analytics listener OnBeforeSendingEventLog");
        }
    }

}
