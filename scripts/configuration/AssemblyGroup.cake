// An assembly group contains information about which assemblies to be packaged together
// for each supported platform
public class AssemblyGroup
{
    public string NuspecKey => $"${Id}_dir$";
    public string Id { get; set; }
    public string Folder { get; set; }
    public IList<string> AssemblyPaths { get; set; }
    public IList<AssemblyGroup> Subgroups { get; set; }
    public bool Download { get; set; }

    public static IList<AssemblyGroup> ReadAssemblyGroups()
    {
        XmlReader reader = ConfigFile.CreateReader();
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
        var parentFolder = parent?.Folder ?? string.Empty;
        Folder = groupNode.Attributes.GetNamedItem("folder")?.Value ?? string.Empty;
        Folder = System.IO.Path.Combine(parentFolder, Folder);
        if (!Folder.StartsWith(Statics.TemporaryPrefix))
        {
            Folder = Statics.TemporaryPrefix + Folder;
        }
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
