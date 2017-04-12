#addin nuget:?package=Cake.FileHelpers
#addin nuget:?package=Cake.Git
#tool "nuget:?package=gitreleasemanager"

// Task TARGET for build
var TARGET = Argument("target", Argument("t", "Default"));

Task("Default").IsDependentOn("GitRelease");

// Create a tag and release on GitHub
Task("GitRelease")
	.Does(() =>
{
    var assemblyInfo = ParseAssemblyInfo("SDK/MobileCenter/Microsoft.Azure.Mobile/Properties/AssemblyInfo.cs");
	  var publishVersion = assemblyInfo.AssemblyInformationalVersion;
    var username = "user";
    var password = Argument<string>("GithubToken");
    var owner = "Microsoft";
    var repo = "mobile-center-sdk-dotnet";

    // Oddly, there is no obvious API to create a file, so we have to create a file by copying an existing
    // file and replacing its contents
    var releaseFile = File("tempRelease.md");
    CopyFile(File("SDK/MobileCenter/Microsoft.Azure.Mobile/Properties/AssemblyInfo.cs"), releaseFile);
    FileWriteText(releaseFile,"Please update description. It will be pulled out automatically from release.md next time.");

    // Build a string containing paths to NuGet packages
    var files = GetFiles("../../**/*Microsoft.Azure.Mobile*.nupkg");
    var assets = string.Empty;
    foreach (var file in files)
    {
      assets += file.FullPath + ",";
    }
    assets = assets.Substring(0,assets.Length-1);
    GitReleaseManagerCreate(username, password, owner, repo, new GitReleaseManagerCreateSettings {
      Prerelease        = true,
      Assets            = assets,
      TargetCommitish   = "master",
      InputFilePath = releaseFile.Path.FullPath,
      Name = publishVersion
    });
    GitReleaseManagerPublish(username, password, owner, repo,  publishVersion);
    DeleteFile(releaseFile);
});

RunTarget(TARGET);
