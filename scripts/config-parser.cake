#addin "nuget:?package=System.Xml.ReaderWriter"

using System.Xml;
using System.Collections.Generic;

// This class contains the assembly folder paths and other platform dependent paths involved in preparing assemblies for VSTS and Azure storage.
// When a new platform is supported, an AssemblyGroup must be created and added to the proper {OS}UploadAssemblyGroups array. Also, its
// AssemblyFolder must be added to the correct platform's "DownloadAssemblyFolders" array.
public class PlatformPaths
{
    public PlatformPaths(string uploadAssembliesZip, string downloadAssembliesZip, string downloadUrl, string configFilePath)
    {
        UploadAssembliesZip = uploadAssembliesZip;
        DownloadAssembliesZip = downloadAssembliesZip;
        DownloadUrl = downloadUrl;
        UploadAssemblyGroups = new List<AssemblyGroup>();
        DownloadAssemblyGroups = new List<AssemblyGroup>();
        var groups = AssemblyGroup.ReadAssemblyGroups(configFilePath);
        AddGroups(groups);
    }

    private void AddGroups(IList<AssemblyGroup> groups)
    {
        foreach (var group in groups)
        {
            AddGroups(group.Subgroups);
            if (group.AssemblyPaths.Count > 0)
            {
                if (group.Download)
                {
                    DownloadAssemblyGroups.Add(group);
                }
                else
                {
                    UploadAssemblyGroups.Add(group);
                }
            }
        }
    }

    // Folders for the assemblies that the current platform must create and upload
    public List<AssemblyGroup> UploadAssemblyGroups { get; set; }

    // The downloaded assembly groups
    public List<AssemblyGroup> DownloadAssemblyGroups { get; set; }

    // The name of the zip file to upload
    public string UploadAssembliesZip { get; set; }

    // The name of the zip file to download
    public string DownloadAssembliesZip { get; set; }

    // The URL to download files from
    public string DownloadUrl { get; set; }
}

// AppCenter module class definition.
class AppCenterModule
{
    public string AndroidModule { get; }
    public string DotNetModule { get; }
    public string NuGetVersion { get; }
    public string MainNuspecFilename { get; }
    public string NuGetPackageName => $"{_packageId}.{NuGetVersion}.nupkg";
    public string MacNuspecFilename =>  "Mac" + MainNuspecFilename;
    public string WindowsNuspecFilename => "Windows" + MainNuspecFilename;
    private string _packageId;

    private AppCenterModule(string android, string dotnet, string mainNuspecFilename) {
        AndroidModule = android;
        DotNetModule = dotnet;
        MainNuspecFilename = mainNuspecFilename;
    }

    private AppCenterModule(XmlNode moduleNode, string nuspecFolder, string nugetVersion)
    {
        AndroidModule = moduleNode.Attributes.GetNamedItem("androidModule").Value;
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

    public static IList<AppCenterModule> ReadAppCenterModules(string configFilePath, string nuspecFolder, string nugetVersion)
    {
        XmlReader reader = XmlReader.Create(configFilePath);
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

// An assembly group contains information about which assemblies to be packaged together
// for each supported platform
public class AssemblyGroup
{
    public static IList<AssemblyGroup> ReadAssemblyGroups(string configFilePath)
    {
        XmlReader reader = XmlReader.Create(configFilePath);
        IList<AssemblyGroup> groups = new List<AssemblyGroup>();
        while (reader.Read())
        {
            if (reader.Name == "group")
            {
                XmlDocument group = new XmlDocument();
                var node = group.ReadNode(reader);
                groups.Add(new AssemblyGroup(node));
            }
        }
        return groups;
    }

    public string NuspecKey => $"${Id}_dir$";
    public string Id { get; set; }
    public string Folder { get; set; }
    public IList<string> AssemblyPaths { get; set; }
    public IList<AssemblyGroup> Subgroups { get; set; }
    public bool Download { get; set; }
    private AssemblyGroup(XmlNode groupNode, AssemblyGroup parent = null)
    {
        AssemblyPaths = new List<string>();
        Subgroups = new List<AssemblyGroup>();
        Id = groupNode.Attributes.GetNamedItem("id").Value;
        var buildGroup = groupNode.Attributes.GetNamedItem("buildGroup")?.Value;
        var platformString = Statics.Context.IsRunningOnUnix() ? "mac" : "windows";
        if (buildGroup != null)
        {
            Download = (buildGroup != platformString);
        }
        else if (parent != null)
        {
            Download = parent.Download;
        }
        string parentFolder = parent?.Folder ?? string.Empty;
        Folder = groupNode.Attributes.GetNamedItem("folder")?.Value ?? string.Empty;
        Folder = Statics.TemporaryPrefix + System.IO.Path.Combine(parentFolder, Folder);
        for (int i = 0; i < groupNode.ChildNodes.Count; ++i)
        {
            var childNode = groupNode.ChildNodes.Item(i);
            if (childNode.Name == "assembly")
            {
                AssemblyPaths.Add(childNode.Attributes.GetNamedItem("path").Value);
            }
            else if (childNode.Name == "group")
            {
                Subgroups.Add(new AssemblyGroup(childNode, this));
            }
        }
    }
}

// A Build Group contains information on what solutions to build for which platform,
// and how to do so.
public class BuildGroup
{
    private string _platformId;
    private string _solutionPath;
    private IList<BuildConfig> _builds;

    private class BuildConfig
    {
        private string _platform { get; set; }
        private string _configuration { get; set; }
        public BuildConfig(string platform, string configuration)
        {
            _platform = platform;
            _configuration = configuration;
        }

        public void Build(string solutionPath)
        {
            Statics.Context.MSBuild(solutionPath, settings => {
                if (_platform != null)
                {
                    settings.SetConfiguration(_configuration).WithProperty("Platform", _platform);
                }
                else
                {
                    settings.Configuration = _configuration;
                }
            });
        }
    }

    public BuildGroup(string platformId, string configFilePath)
    {
        _platformId = platformId;
        var reader = XmlReader.Create(configFilePath);
        _builds = new List<BuildConfig>();
        while (reader.Read())
        {
            if (reader.Name == "buildGroup")
            {
                var buildGroup = new XmlDocument();
                var node = buildGroup.ReadNode(reader);
                ApplyBuildGroupNode(node);
            }
        }
    }

    public void ExecuteBuilds()
    {
        Statics.Context.NuGetRestore(_solutionPath);
        foreach (var buildConfig in _builds)
        {
            buildConfig.Build(_solutionPath);
        }
    }

    private void ApplyBuildGroupNode(XmlNode buildGroup)
    {
        if (buildGroup.Attributes.GetNamedItem("platformId").Value != _platformId)
        {
            return;
        }
        _solutionPath = buildGroup.Attributes.GetNamedItem("solutionPath").Value;
        for (int i = 0; i < buildGroup.ChildNodes.Count; ++i)
        {
            var childNode = buildGroup.ChildNodes.Item(i);
            if (childNode.Name == "build")
            {
                var platform = childNode.Attributes.GetNamedItem("platform")?.Value;
                var configuration = childNode.Attributes.GetNamedItem("configuration")?.Value;
                _builds.Add(new BuildConfig(platform, configuration));
            }
        }
    }
}
