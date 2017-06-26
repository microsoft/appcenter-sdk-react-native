namespace Microsoft.Azure.Mobile
{
    public static class WrapperSdk
    {
        public const string Name = "mobilecenter.xamarin";

        /* We can't use reflection for assemblyInformationalVersion on iOS with "Link All" optimization. */
        internal const string Version = "0.13.2-SNAPSHOT";
    }
}
