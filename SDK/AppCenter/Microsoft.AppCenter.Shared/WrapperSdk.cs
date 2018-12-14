namespace Microsoft.AppCenter
{
    public static class WrapperSdk
    {
        public const string Name = "appcenter.xamarin";

        /* We can't use reflection for assemblyInformationalVersion on iOS with "Link All" optimization. */
        internal const string Version = "1.12.1-SNAPSHOT";
    }
}
