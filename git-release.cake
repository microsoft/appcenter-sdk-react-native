#addin nuget:?package=Cake.FileHelpers
#addin nuget:?package=Cake.Git
#addin nuget:?package=Cake.Incubator
#tool "nuget:?package=gitreleasemanager"

// Task TARGET for build
var TARGET = Argument("target", Argument("t", "Default"));
Task("Default").IsDependentOn("GitRelease");

// Create a tag and release on GitHub
Task("GitRelease")
	.Does(() =>
{
    var project = ParseProject("./SDK/MobileCenter/Microsoft.Azure.Mobile/Microsoft.Azure.Mobile.csproj", configuration: "Release");
    var publishVersion = project.NetCore.Version;
    var username = "user";
    var password = Argument<string>("GithubToken");
    var owner = "Microsoft";
    var repo = "mobile-center-sdk-dotnet";

    System.IO.File.Create("tempRelease.md").Dispose();
    var releaseFile = File("tempRelease.md");
    FileWriteText(releaseFile,"Please update description. It will be pulled out automatically from release.md next time.");

    // Build a string containing paths to NuGet packages.
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
