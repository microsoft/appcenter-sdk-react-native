using System;

namespace Microsoft.Sonoma.Xamarin.Core
{
    internal class PlatformSonoma : IPlatformSonoma
    {
        public bool Enabled
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public LogLevel LogLevel
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Guid InstallId
        {
            get { throw new NotImplementedException(); }
        }

        public void Start(string appSecret, params Type[] features)
        {
            throw new NotImplementedException();
        }
    }
}