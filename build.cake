// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#tool nuget:?package=XamarinComponent
#addin nuget:?package=Cake.FileHelpers&version=3.0.0
#addin nuget:?package=Cake.Git&version=0.18.0
#addin nuget:?package=Cake.Incubator&version=2.0.2
#addin nuget:?package=Cake.Xamarin
#load "scripts/utility.cake"
#load "scripts/configuration/config-parser.cake"

using System.Net;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

var DownloadedAssembliesFolder = Statics.TemporaryPrefix + "DownloadedAssemblies";
var MacAssembliesZip = Statics.TemporaryPrefix + "MacAssemblies.zip";
var WindowsAssembliesZip = Statics.TemporaryPrefix + "WindowsAssemblies.zip";

// Contains all assembly paths and how to use them
PlatformPaths AssemblyPlatformPaths;

// Available AppCenter modules.
IList<AppCenterModule> AppCenterModules = null;

// URLs for downloading binaries.
/*
 * Read this: http://www.mono-project.com/docs/faq/security/.
 * On Windows,
 *     you have to do additional steps for SSL connection to download files.
 *     http://stackoverflow.com/questions/4926676/mono-webrequest-fails-with-https
 *     By running mozroots and install part of Mozilla's root certificates can make it work.
 */

var ExternalsDirectory = "externals";
var AndroidExternals = $"{ExternalsDirectory}/android";
var IosExternals = $"{ExternalsDirectory}/ios";

var SdkStorageUrl = "https://mobilecentersdkdev.blob.core.windows.net/sdk/";
var MacAssembliesUrl = SdkStorageUrl + MacAssembliesZip;
var WindowsAssembliesUrl = SdkStorageUrl + WindowsAssembliesZip;

// Need to read versions before setting url values
VersionReader.ReadVersions();
var AndroidUrl = $"{SdkStorageUrl}AppCenter-SDK-Android-{VersionReader.AndroidVersion}.zip";
var IosUrl = $"{SdkStorageUrl}AppCenter-SDK-Apple-{VersionReader.IosVersion}.zip";

// Task Target for build
var Target = Argument("Target", Argument("t", "Default"));

var NuspecFolder = "nuget";

// Prepare the platform paths for downloading, uploading, and preparing assemblies
Setup(context =>
{
    // Get assembly paths.
    var uploadAssembliesZip = (IsRunningOnUnix() ? MacAssembliesZip : WindowsAssembliesZip);
    var downloadUrl = (IsRunningOnUnix() ? WindowsAssembliesUrl : MacAssembliesUrl);
    var downloadAssembliesZip = (IsRunningOnUnix() ? WindowsAssembliesZip : MacAssembliesZip);
    AssemblyPlatformPaths = new PlatformPaths(uploadAssembliesZip, downloadAssembliesZip, downloadUrl);
    AppCenterModules = AppCenterModule.ReadAppCenterModules(NuspecFolder, VersionReader.SdkVersion);
});

Task("Build")
    .IsDependentOn("Externals")
    .Does(() =>
{
    var platformId = IsRunningOnUnix() ? "mac" : "windows";
    var buildGroup = new BuildGroup(platformId);
    buildGroup.ExecuteBuilds();
}).OnError(HandleError);

Task("PrepareAssemblies").IsDependentOn("Build")
.Does(()=>
{
    // Clean all directories before copying. Doing so before each operation
    // could cause subdirectories that are created first to be deleted.
    foreach (var assemblyGroup in AssemblyPlatformPaths.UploadAssemblyGroups)
    {
        CleanDirectory(assemblyGroup.Folder);
    }
    foreach (var assemblyGroup in AssemblyPlatformPaths.UploadAssemblyGroups)
    {
        CopyFiles(assemblyGroup.AssemblyPaths.Where(i => FileExists(i)), assemblyGroup.Folder, false);
    }
}).OnError(HandleError);

// Downloading Android binaries.
Task("Externals-Android")
    .Does(() =>
{
    var zipFile = System.IO.Path.Combine(AndroidExternals, "android.zip");
    CleanDirectory(AndroidExternals);

    // Download zip file.
    DownloadFile(AndroidUrl, zipFile);
    Unzip(zipFile, AndroidExternals);

    // Move binaries to externals/android so that linked files don't have versions
    // in their paths
    var files = GetFiles($"{AndroidExternals}/*/*");
    CopyFiles(files, AndroidExternals);

    // Edit push manifest due to Xamarin manifest merge limitations
    var pushLib = "appcenter-push-release";
    var pushLibFile = $"{AndroidExternals}/{pushLib}.aar";
    var pushLibUnzippedPath = $"{AndroidExternals}/{pushLib}";
    var manifestUpdateFile = "SDK/AppCenterPush/Microsoft.AppCenter.Push.Android.Bindings/AndroidManifest.xml";
    Unzip(pushLibFile, pushLibUnzippedPath);
    DeleteFile(pushLibFile);
    CopyFiles(manifestUpdateFile, pushLibUnzippedPath);
    Zip(pushLibUnzippedPath, pushLibFile);
}).OnError(HandleError);

// Downloading iOS binaries.
Task("Externals-Ios")
    .Does(() =>
{
    CleanDirectory(IosExternals);
    var zipFile = System.IO.Path.Combine(IosExternals, "ios.zip");

    // Download zip file containing AppCenter frameworks
    DownloadFile(IosUrl, zipFile);
    Unzip(zipFile, IosExternals);
    var frameworksLocation = System.IO.Path.Combine(IosExternals, "AppCenter-SDK-Apple/iOS");

    // Copy the AppCenter binaries directly from the frameworks and add the ".a" extension
    var files = GetFiles($"{frameworksLocation}/*.framework/AppCenter*");
    foreach (var file in files)
    {
        var filename = file.GetFilename();
        MoveFile(file, $"{IosExternals}/{filename}.a");
    }
    
    // Copy Distribute resource bundle and copy it to the externals directory.
    var distributeBundle = "AppCenterDistributeResources.bundle";
    if(DirectoryExists($"{frameworksLocation}/{distributeBundle}"))
    {
        MoveDirectory($"{frameworksLocation}/{distributeBundle}", $"{IosExternals}/{distributeBundle}");
    }
}).OnError(HandleError);

// Create a common externals task depending on platform specific ones
Task("Externals").IsDependentOn("Externals-Ios").IsDependentOn("Externals-Android");

// Main Task.
Task("Default").IsDependentOn("NuGet").IsDependentOn("RemoveTemporaries");

// Pack NuGets for appropriate platform
Task("NuGet")
    .IsDependentOn("PrepareAssemblies")
    .Does(()=>
{
    CleanDirectory("output");
    var specCopyName = Statics.TemporaryPrefix + "spec_copy.nuspec";

    // Package NuGets.
    foreach (var module in AppCenterModules)
    {
        var nuspecFilename = (IsRunningOnUnix() ? module.MacNuspecFilename : module.WindowsNuspecFilename);
        var nuspecPath = System.IO.Path.Combine(NuspecFolder, nuspecFilename);

        // Skip modules that don't have nuspecs.
        if (!FileExists(nuspecPath))
        {
            continue;
        }

        // Prepare nuspec by making substitutions in a copied nuspec (to avoid altering the original)
        CopyFile(nuspecPath, specCopyName);
        ReplaceAssemblyPathsInNuspecs(specCopyName);
        Information("Building a NuGet package for " + module.DotNetModule + " version " + module.NuGetVersion);
        NuGetPack(File(specCopyName), new NuGetPackSettings {
            Verbosity = NuGetVerbosity.Detailed,
            Version = module.NuGetVersion,
            RequireLicenseAcceptance = true
        });

        // Clean up
        DeleteFiles(specCopyName);
    }
    MoveFiles("Microsoft.AppCenter*.nupkg", "output");
}).OnError(HandleError);

// Replace version placeholder in nuspecs
Task("PrepareNuspecsForVSTS").Does(()=>
{
    foreach (var module in AppCenterModules)
    {
        ReplaceTextInFiles(System.IO.Path.Combine(NuspecFolder, module.MainNuspecFilename), "$version$", module.NuGetVersion);
    }
});

Task("PrepareAssemblyPathsVSTS").Does(()=>
{
    var nuspecPathPrefix = EnvironmentVariable("NUSPEC_PATH");
    foreach (var module in AppCenterModules)
    {
        var nuspecPath = System.IO.Path.Combine(nuspecPathPrefix, module.MainNuspecFilename);
        ReplaceAssemblyPathsInNuspecs(nuspecPath);
    }
}).OnError(HandleError);

Task("NugetPackVSTS").Does(()=>
{
    var nuspecPathPrefix = EnvironmentVariable("NUSPEC_PATH");
    foreach (var module in AppCenterModules)
    {
        var spec = GetFiles(nuspecPathPrefix + module.MainNuspecFilename);

        // Create the NuGet packages.
        Information("Building a NuGet package for " + module.MainNuspecFilename);
        NuGetPack(spec, new NuGetPackSettings {
            Verbosity = NuGetVerbosity.Detailed,
            RequireLicenseAcceptance = true
        });
    }
}).OnError(HandleError);

// In VSTS, the assembly path environment variable names should be in the format
// "{uppercase group id}_ASSEMBLY_PATH_NUSPEC"
void ReplaceAssemblyPathsInNuspecs(string nuspecPath)
{
    // For the Tuples, Item1 is variable name, Item2 is variable value.
    var assemblyPathVars = new List<Tuple<string, string>>();
    var allAssemblyGroups = AssemblyPlatformPaths.UploadAssemblyGroups.Union(AssemblyPlatformPaths.DownloadAssemblyGroups);
    foreach (var group in allAssemblyGroups)
    {
        if (group.NuspecKey == null)
        {
            continue;
        }
        var environmentVariableName = group.Id.ToUpper() + "_ASSEMBLY_PATH_NUSPEC";
        var assemblyPath = EnvironmentVariable(environmentVariableName, group.Folder);
        var tuple = Tuple.Create(group.NuspecKey, assemblyPath);
        assemblyPathVars.Add(tuple);
    }
    foreach (var assemblyPathVar in assemblyPathVars)
    {
        ReplaceTextInFiles(nuspecPath, assemblyPathVar.Item1, assemblyPathVar.Item2);
    }
}

RunTarget(Target);
