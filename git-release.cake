#addin nuget:?package=Cake.FileHelpers
#addin nuget:?package=Cake.Git
#addin nuget:?package=Cake.Incubator
#tool "nuget:?package=gitreleasemanager"

using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;

// Task TARGET for build
var TARGET = Argument("target", Argument("t", "Default"));
Task("Default").IsDependentOn("GitRelease");

// Create a tag and release on GitHub
Task("GitRelease")
    .Does(() =>
{
    // Use the package version for AppCenter package as the release version.
    var nuspecPathPrefix = EnvironmentVariable("NUSPEC_PATH", "nuget");
    var nuspecPath = System.IO.Path.Combine(nuspecPathPrefix, "AppCenter.nuspec");
    var nuspecXml = XDocument.Load(nuspecPath);
    var publishVersion = nuspecXml.XPathSelectElement("/package/metadata/version").Value;

    // Create temporary release notes.
    var releaseNotesFileName = "tempRelease.md";
    System.IO.File.Create(releaseNotesFileName).Dispose();
    var releaseFile = File(releaseNotesFileName);
    FileWriteText(releaseFile,"Please update description. It will be pulled out automatically from release.md next time.");

    // Build a string containing paths to NuGet packages.
    var files = GetFiles("../../**/Microsoft.AppCenter*.nupkg");
    var assets = string.Empty;
    foreach (var file in files)
    {
      assets += file.FullPath + ",";
    }
    assets = assets.Substring(0,assets.Length-1);
    var username = "user";
    var password = Argument<string>("GithubToken");
    var owner = "Microsoft";
    var repo = "appcenter-sdk-dotnet";
    GitReleaseManagerCreate(username, password,owner, repo,
                            new GitReleaseManagerCreateSettings {
                                Prerelease        = true,
                                Assets            = assets,
                                TargetCommitish   = "master",
                                InputFilePath = releaseFile.Path.FullPath,
                                Name = publishVersion
                            });
    GitReleaseManagerPublish(username, password, owner, repo, publishVersion);
    DeleteFile(releaseFile);
});
RunTarget(TARGET);
