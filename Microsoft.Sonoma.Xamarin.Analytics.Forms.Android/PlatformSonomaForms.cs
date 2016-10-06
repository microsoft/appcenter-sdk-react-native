using Android.App;
using Android.OS;

namespace Microsoft.Sonoma.Xamarin.Analytics.Forms
{
    public class PlatformSonomaForms : IPlatformSonomaForms
    {
        private static readonly object Lock = new object();

        private static bool _initialized;

        public void Initialize()
        {
            lock (Lock)
            {
                if (!_initialized)
                {
                    ((Application) Application.Context).RegisterActivityLifecycleCallbacks(new ApplicationLifecycleCallbacks());
                    _initialized = true;
                }
            }
        }

        private class ApplicationLifecycleCallbacks : Java.Lang.Object, Application.IActivityLifecycleCallbacks
        { 
            public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
            {
            }

            public void OnActivityDestroyed(Activity activity)
            {
            }

            public void OnActivityPaused(Activity activity)
            {
            }

            public void OnActivityResumed(Activity activity)
            {
                SonomaForms.NotifyOnResume();
            }

            public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
            {
            }

            public void OnActivityStarted(Activity activity)
            {
            }

            public void OnActivityStopped(Activity activity)
            {
            }
        }
    }
}
