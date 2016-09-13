using System;

namespace Microsoft.Sonoma.Xamarin.Core
{
    internal interface IPlatformSonoma
    {
        bool Enabled { get; set; }

        LogLevel LogLevel { get; set; }

        Guid InstallId { get; }

        void Start(string appSecret, params Type[] features);
    }
}