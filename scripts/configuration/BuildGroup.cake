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

    public BuildGroup(string platformId)
    {
        _platformId = platformId;
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
                _builds.Add(new BuildConfig(platform, configuration));
            }
        }
    }
}
