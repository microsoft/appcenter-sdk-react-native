// AppCenter module class definition.
class AppCenterModule
{
    public string DotNetModule { get; }
    public string NuGetVersion { get; }
    public string MainNuspecFilename { get; }
    public string NuGetPackageName => $"{_packageId}.{NuGetVersion}.nupkg";
    public string MacNuspecFilename =>  "Mac" + MainNuspecFilename;
    public string WindowsNuspecFilename => "Windows" + MainNuspecFilename;
    private string _packageId;

    private AppCenterModule(string dotnet, string mainNuspecFilename) {
        DotNetModule = dotnet;
        MainNuspecFilename = mainNuspecFilename;
    }

    private AppCenterModule(XmlNode moduleNode, string nuspecFolder, string nugetVersion)
    {
        DotNetModule = moduleNode.Attributes.GetNamedItem("dotnetModule").Value;
        MainNuspecFilename = moduleNode.Attributes.GetNamedItem("nuspec").Value;
        NuGetVersion = nugetVersion;
        var nuspecText = Statics.Context.FileReadText(System.IO.Path.Combine(nuspecFolder, MainNuspecFilename));
        var startTag = "<id>";
        var endTag = "</id>";
        int startIndex = nuspecText.IndexOf(startTag) + startTag.Length;
        int length = nuspecText.IndexOf(endTag) - startIndex;
        _packageId = nuspecText.Substring(startIndex, length);
    }

    public static IList<AppCenterModule> ReadAppCenterModules(string nuspecFolder, string nugetVersion)
    {
        XmlReader reader = ConfigFile.CreateReader();
        IList<AppCenterModule> modules = new List<AppCenterModule>();
        while (reader.Read())
        {
            if (reader.Name == "module")
            {
                XmlDocument module = new XmlDocument();
                var node = module.ReadNode(reader);
                modules.Add(new AppCenterModule(node, nuspecFolder, nugetVersion));
            }
        }
        return modules;
    }
}
