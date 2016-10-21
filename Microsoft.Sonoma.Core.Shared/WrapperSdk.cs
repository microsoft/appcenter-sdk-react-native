using System.Reflection;

namespace Microsoft.Sonoma.Core
{
    internal static class WrapperSdk
    {
        internal const string Name = "sonoma.xamarin";

        internal static readonly string Version = typeof (WrapperSdk).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
    }
}