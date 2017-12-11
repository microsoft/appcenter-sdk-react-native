using Android.App;
using Android.Content;
using Android.Runtime;

namespace Microsoft.AppCenter.Push
{
    [Preserve]
    [BroadcastReceiver(Permission = "com.google.android.c2dm.permission.SEND")]
    [IntentFilter(new[] { "com.google.android.c2dm.intent.REGISTRATION", "com.google.android.c2dm.intent.RECEIVE" },
                  Categories = new[] { "${applicationId}" })]
    public class PushReceiver : Com.Microsoft.Appcenter.Push.PushReceiver
    {
    }
}
