// This class contains the assembly folder paths and other platform dependent paths involved in preparing assemblies for VSTS and Azure storage.
// When a new platform is supported, an AssemblyGroup must be created and added to the proper {OS}UploadAssemblyGroups array. Also, its
// AssemblyFolder must be added to the correct platform's "DownloadAssemblyFolders" array.
public class PlatformPaths
{
    public PlatformPaths(string uploadAssembliesZip, string downloadAssembliesZip, string downloadUrl)
    {
        UploadAssembliesZip = uploadAssembliesZip;
        DownloadAssembliesZip = downloadAssembliesZip;
        DownloadUrl = downloadUrl;
        UploadAssemblyGroups = new List<AssemblyGroup>();
        DownloadAssemblyGroups = new List<AssemblyGroup>();
        var groups = AssemblyGroup.ReadAssemblyGroups();
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