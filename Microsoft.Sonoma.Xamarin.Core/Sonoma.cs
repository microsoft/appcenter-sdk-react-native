using System;

namespace Microsoft.Sonoma.Xamarin.Core
{
    public static class Sonoma
    {
        public static LogLevel LogLevel
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public static void SetServerUrl(string serverUrl)
        {
            throw new NotImplementedException();
        }

        public static void Initialize(string appSecret)
        {
            throw new NotImplementedException();
        }

        public static void Start(params Type[] features)
        {
            throw new NotImplementedException();
        }

        public static void Start(string appSecret, params Type[] features)
        {
            throw new NotImplementedException();
        }

        public static bool Enabled
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public static Guid InstallId
        {
            get { throw new NotImplementedException(); }
        }
    }
}