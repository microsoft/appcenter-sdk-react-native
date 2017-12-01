public class VersionReader
{
    public static void ReadVersions()
    {
        XmlReader reader = ConfigFile.CreateReader();
        string sdkVersion = null;
        string iosVersion = null;
        string androidVersion = null;
        while (reader.Read())
        {
            ReadVersion("sdkVersion", reader, ref sdkVersion);
            ReadVersion("iosVersion", reader, ref iosVersion);
            ReadVersion("androidVersion", reader, ref androidVersion);
        }
        SdkVersion = sdkVersion;
        IosVersion = iosVersion;
        AndroidVersion = androidVersion;
    }

    private static void ReadVersion(string versionName, XmlReader reader, ref string versionVar)
    {
        if (reader.Name == versionName)
        {
            var versionNode = new XmlDocument();
            var node = versionNode.ReadNode(reader);
            versionVar = node.FirstChild.Value;
            Statics.Context.Information($"Found {versionName} = {versionVar}");
        }
    }

    public static string SdkVersion { get; private set; }
    public static string IosVersion { get; private set; }
    public static string AndroidVersion { get; private set; }

}