using System.Reflection;

namespace Microsoft.Azure.Mobile
{
    public static class WrapperSdk
    {
        public const string Name = "sonoma.xamarin";

        internal static readonly string Version = typeof(WrapperSdk).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
    }
}