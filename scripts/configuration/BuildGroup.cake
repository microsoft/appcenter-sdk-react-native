// A Build Group contains information on what solutions to build for which platform,
// and how to do so.
using Cake.Common.Tools.MSBuild;

public class BuildGroup
{
    private string _platformId;
    private string _toolVersion;
    private string _solutionPath;
    private string _vs2019path;
    private IList<BuildConfig> _builds;

    private class BuildConfig
    {
        private string _platform { get; set; }
        private string _configuration { get; set; }
        private string _toolPath { get; set; }

        public BuildConfig(string platform, string configuration, string toolPath)
        {
            _platform = platform;
            _configuration = configuration;
            _toolPath = toolPath;
        }

        public void Build(string solutionPath)
        {
            Statics.Context.MSBuild(solutionPath, settings => {
                if (_toolPath != null)
                {
                    settings.ToolPath = _toolPath;
                }
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

    public BuildGroup(string platformId, string toolVersion)
    {
        _platformId = platformId;
        _toolVersion = toolVersion;
        if (_toolVersion.Length >= 6 && _toolVersion.Substring(_toolVersion.Length - 6) == "VS2019")
        {
            _vs2019path = GetVisualStudio2019Path();
        }

        var reader = ConfigFile.CreateReader();
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
                _builds.Add(new BuildConfig(platform, configuration, _vs2019path));
            }
        }
    }

    // Copy from the cake's internal MSBuildRunner.GetVisualStudio2019Path()
    private string GetVisualStudio2019Path()
    {
        var vsEditions = new[]
        {
            "Enterprise",
            "Professional",
            "Community",
            "BuildTools"
        };

        var programFilesDir = Statics.Context.EnvironmentVariable("ProgramFiles(x86)");
        if (string.IsNullOrEmpty(programFilesDir))
        {
            programFilesDir = Statics.Context.EnvironmentVariable("ProgramFiles");
        }

        foreach (var edition in vsEditions)
        {
            // Get the bin path.
            // Cake has its own Cake.Core.IO.Path class.
            var binPath = System.IO.Path.Combine(programFilesDir, 
                string.Join(System.IO.Path.DirectorySeparatorChar.ToString(),
                    "Microsoft Visual Studio", "2019", edition, "MSBuild", "Current", "Bin")
                ); 

            if (!string.IsNullOrEmpty(binPath) && System.IO.Directory.Exists(binPath))
            {
                return string.Join(System.IO.Path.DirectorySeparatorChar.ToString(), 
                    (Environment.Is64BitOperatingSystem ? System.IO.Path.Combine(binPath, "amd64") : binPath),
                    "MSBuild.exe");
            }
        }
        return System.IO.Path.Combine(programFilesDir, "Microsoft Visual Studio/2019/Professional/MSBuild/16.0/Bin/MSbuild.exe");
    }
}
