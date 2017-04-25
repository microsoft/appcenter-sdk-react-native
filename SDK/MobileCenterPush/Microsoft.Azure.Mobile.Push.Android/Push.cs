using System;
using Android.Runtime;

namespace Microsoft.Azure.Mobile.Push
{
    using AndroidPush = Com.Microsoft.Azure.Mobile.Push.Push;
    public partial class Push
    {
        private static bool PlatformEnabled
        {
        	get { return AndroidPush.Enabled; }
        	set { AndroidPush.Enabled = value; }
        }
        /// <summary>
        /// Internal SDK property not intended for public use.
        /// </summary>
        /// <value>
        /// The iOS SDK Analytics bindings type.
        /// </value>
        [Preserve]
        public static Type BindingType => typeof(AndroidPush);
    }
}
