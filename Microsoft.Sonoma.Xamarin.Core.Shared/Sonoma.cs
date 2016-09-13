using System;

namespace Microsoft.Sonoma.Xamarin.Core
{
    public static class Sonoma
    {
		private static readonly IPlatformSonoma PlatformSonoma = new PlatformSonoma();

        public static bool Enabled
        {
            get { return PlatformSonoma.Enabled; }
            set { PlatformSonoma.Enabled = value; }
        }

        public static LogLevel LogLevel
        {
            get { return PlatformSonoma.LogLevel; }
            set { PlatformSonoma.LogLevel = value; }
        }

        public static Guid InstallId => PlatformSonoma.InstallId;

        public static void Start(string appSecret, params Type[] features)
        {
            PlatformSonoma.Start(appSecret, features);
        }
    }
}