#tool nuget:?package=XamarinComponent
#addin nuget:?package=Cake.FileHelpers
#addin nuget:?package=Cake.Git
#addin nuget:?package=Cake.Incubator
#addin nuget:?package=Cake.Xamarin
#addin "Cake.AzureStorage"

using System.Net;
using System.Text;
using System.Text.RegularExpressions;

// AppCenter module class definition.
class AppCenterModule {
    public string AndroidModule { get; set; }
    public string DotNetModule { get; set; }
    public string NuGetVersion { get; set; }
    public string PackageId { get; set; }
    public string MainNuspecFilename { get; set; }
    public string NuGetPackageName
    {
        get
        {
            return PackageId + "." + NuGetVersion + ".nupkg";
        }
    }
    public string MacNuspecFilename
    {
        get { return  "Mac" + MainNuspecFilename; }
    }
    public string WindowsNuspecFilename
    {
        get { return  "Windows" + MainNuspecFilename; }
    }
    public AppCenterModule(string android, string dotnet, string mainNuspecFilename) {
        AndroidModule = android;
        DotNetModule = dotnet;
        MainNuspecFilename = mainNuspecFilename;
    }
}

// Prefix for temporary intermediates that are created by this script
var TEMPORARY_PREFIX = "CAKE_SCRIPT_TEMP";

var DOWNLOADED_ASSEMBLIES_FOLDER = TEMPORARY_PREFIX + "DownloadedAssemblies";
var MAC_ASSEMBLIES_ZIP = TEMPORARY_PREFIX + "MacAssemblies.zip";
var WINDOWS_ASSEMBLIES_ZIP = TEMPORARY_PREFIX + "WindowsAssemblies.zip";

// Assembly folders
var WINDOWS_DESKTOP_ASSEMBLIES_FOLDER = TEMPORARY_PREFIX + "WindowsDesktopAssemblies";
var UWP_ASSEMBLIES_FOLDER = TEMPORARY_PREFIX + "UWPAssemblies";
var IOS_ASSEMBLIES_FOLDER = TEMPORARY_PREFIX + "iOSAssemblies";
var ANDROID_ASSEMBLIES_FOLDER = TEMPORARY_PREFIX + "AndroidAssemblies";
var PCL_ASSEMBLIES_FOLDER = TEMPORARY_PREFIX + "PCLAssemblies";
var NETSTANDARD_ASSEMBLIES_FOLDER = TEMPORARY_PREFIX + "NETStandardAssemblies";

// Native SDK versions
var ANDROID_SDK_VERSION = "1.0.1-19+08f54bc";
var IOS_SDK_VERSION = "1.0.0";

var PLATFORM_PATHS = new PlatformPaths();

// URLs for downloading binaries.
/*
 * Read this: http://www.mono-project.com/docs/faq/security/.
 * On Windows,
 *     you have to do additional steps for SSL connection to download files.
 *     http://stackoverflow.com/questions/4926676/mono-webrequest-fails-with-https
 *     By running mozroots and install part of Mozilla's root certificates can make it work.
 */

var SDK_STORAGE_URL = "https://mobilecentersdkdev.blob.core.windows.net/sdk/";
var ANDROID_URL = SDK_STORAGE_URL + "AppCenter-SDK-Android-" + ANDROID_SDK_VERSION + ".zip";
var IOS_URL = SDK_STORAGE_URL + "AppCenter-SDK-Apple-" + IOS_SDK_VERSION + ".zip";
var MAC_ASSEMBLIES_URL = SDK_STORAGE_URL + MAC_ASSEMBLIES_ZIP;
var WINDOWS_ASSEMBLIES_URL = SDK_STORAGE_URL + WINDOWS_ASSEMBLIES_ZIP;

// Available AppCenter modules.
var APP_CENTER_MODULES = new [] {
    new AppCenterModule("app-center-release.aar", "SDK/AppCenter/Microsoft.AppCenter", "AppCenter.nuspec"),
    new AppCenterModule("app-center-analytics-release.aar", "SDK/AppCenterAnalytics/Microsoft.AppCenter.Analytics", "AppCenterAnalytics.nuspec"),
    new AppCenterModule("app-center-crashes-release.aar", "SDK/AppCenterCrashes/Microsoft.AppCenter.Crashes", "AppCenterCrashes.nuspec"),
    new AppCenterModule("app-center-distribute-release.aar", "SDK/AppCenterDistribute/Microsoft.AppCenter.Distribute", "AppCenterDistribute.nuspec"),
    new AppCenterModule("app-center-push-release.aar", "SDK/AppCenterPush/Microsoft.AppCenter.Push", "AppCenterPush.nuspec"),
    new AppCenterModule("app-center-rum-release.aar", "SDK/AppCenterRum/Microsoft.AppCenter.Rum", "AppCenterRum.nuspec")
};

// Task TARGET for build
var TARGET = Argument("target", Argument("t", "Default"));

// Storage id to append to upload and download file names in storage
var STORAGE_ID = Argument("StorageId", Argument("storage-id", ""));

class AssemblyGroup
{
    public string[] AssemblyPaths {get; set;}
    public string AssemblyFolder {get; set;}
}

// This class contains the assembly folder paths and other platform dependent paths involved in preparing assemblies for VSTS and Azure storage.
// When a new platform is supported, an AssemblyGroup must be created and added to the proper {OS}UploadAssemblyGroups array. Also, its
// AssemblyFolder must be added to the correct platform's "DownloadAssemblyFolders" array.
class PlatformPaths
{
    public PlatformPaths()
    {
        UploadAssemblyGroups = new List<AssemblyGroup>();
        DownloadAssemblyFolders = new List<string>();
    }

    // Folders for the assemblies that the current platform must create and upload
    public List<AssemblyGroup> UploadAssemblyGroups {get; set;}

    // The name of the zip file to upload
    public string UploadAssembliesZip {get; set;}

    // The name of the zip file to download
    public string DownloadAssembliesZip {get; set;}
    // The paths of downloaded assembly folders
    public List<string> DownloadAssemblyFolders {get; set;}

    // The URL to download files from
    public string DownloadUrl {get; set;}
}

// Prepare the platform paths for downloading, uploading, and preparing assemblies
Setup(context =>
{
    if (IsRunningOnUnix())
    {
        var iosAssemblyGroup = new AssemblyGroup {
            AssemblyFolder = IOS_ASSEMBLIES_FOLDER,
            AssemblyPaths = new string[] {  "SDK/AppCenter/Microsoft.AppCenter.iOS/bin/Release/Microsoft.AppCenter.dll",
                            "SDK/AppCenter/Microsoft.AppCenter.iOS/bin/Release/Microsoft.AppCenter.iOS.Bindings.dll",
                            "SDK/AppCenterAnalytics/Microsoft.AppCenter.Analytics.iOS/bin/Release/Microsoft.AppCenter.Analytics.dll",
                            "SDK/AppCenterAnalytics/Microsoft.AppCenter.Analytics.iOS/bin/Release/Microsoft.AppCenter.Analytics.iOS.Bindings.dll",
                            "SDK/AppCenterCrashes/Microsoft.AppCenter.Crashes.iOS/bin/Release/Microsoft.AppCenter.Crashes.dll",
                            "SDK/AppCenterCrashes/Microsoft.AppCenter.Crashes.iOS/bin/Release/Microsoft.AppCenter.Crashes.iOS.Bindings.dll",
                            "SDK/AppCenterDistribute/Microsoft.AppCenter.Distribute.iOS/bin/Release/Microsoft.AppCenter.Distribute.dll",
                            "SDK/AppCenterDistribute/Microsoft.AppCenter.Distribute.iOS/bin/Release/Microsoft.AppCenter.Distribute.iOS.Bindings.dll",
                            "SDK/AppCenterPush/Microsoft.AppCenter.Push.iOS/bin/Release/Microsoft.AppCenter.Push.dll",
                            "SDK/AppCenterPush/Microsoft.AppCenter.Push.iOS.Bindings/bin/Release/Microsoft.AppCenter.Push.iOS.Bindings.dll" }
        };
        var androidAssemblyGroup = new AssemblyGroup {
            AssemblyFolder = ANDROID_ASSEMBLIES_FOLDER,
            AssemblyPaths = new string[] {  "SDK/AppCenter/Microsoft.AppCenter.Android/bin/Release/Microsoft.AppCenter.dll",
                            "SDK/AppCenter/Microsoft.AppCenter.Android/bin/Release/Microsoft.AppCenter.Android.Bindings.dll",
                            "SDK/AppCenterAnalytics/Microsoft.AppCenter.Analytics.Android/bin/Release/Microsoft.AppCenter.Analytics.dll",
                            "SDK/AppCenterAnalytics/Microsoft.AppCenter.Analytics.Android/bin/Release/Microsoft.AppCenter.Analytics.Android.Bindings.dll",
                            "SDK/AppCenterCrashes/Microsoft.AppCenter.Crashes.Android/bin/Release/Microsoft.AppCenter.Crashes.dll",
                            "SDK/AppCenterCrashes/Microsoft.AppCenter.Crashes.Android/bin/Release/Microsoft.AppCenter.Crashes.Android.Bindings.dll",
                            "SDK/AppCenterDistribute/Microsoft.AppCenter.Distribute.Android/bin/Release/Microsoft.AppCenter.Distribute.dll",
                            "SDK/AppCenterDistribute/Microsoft.AppCenter.Distribute.Android/bin/Release/Microsoft.AppCenter.Distribute.Android.Bindings.dll",
                            "SDK/AppCenterPush/Microsoft.AppCenter.Push.Android/bin/Release/Microsoft.AppCenter.Push.dll",
                            "SDK/AppCenterPush/Microsoft.AppCenter.Push.Android.Bindings/bin/Release/Microsoft.AppCenter.Push.Android.Bindings.dll",
                            "SDK/AppCenterRum/Microsoft.AppCenter.Rum.Android/bin/Release/Microsoft.AppCenter.Rum.dll",
                            "SDK/AppCenterRum/Microsoft.AppCenter.Rum.Android.Bindings/bin/Release/Microsoft.AppCenter.Rum.Android.Bindings.dll" }
        };
        var pclAssemblyGroup = new AssemblyGroup {
            AssemblyFolder = PCL_ASSEMBLIES_FOLDER,
            AssemblyPaths = new string[] {	"SDK/AppCenter/Microsoft.AppCenter/bin/Release/portable-net45+win8+wpa81+wp8/Microsoft.AppCenter.dll",
                            "SDK/AppCenterAnalytics/Microsoft.AppCenter.Analytics/bin/Release/portable-net45+win8+wpa81+wp8/Microsoft.AppCenter.Analytics.dll",
                            "SDK/AppCenterCrashes/Microsoft.AppCenter.Crashes/bin/Release/portable-net45+win8+wpa81+wp8/Microsoft.AppCenter.Crashes.dll",
                            "SDK/AppCenterDistribute/Microsoft.AppCenter.Distribute/bin/Release/portable-net45+win8+wpa81+wp8/Microsoft.AppCenter.Distribute.dll",
                            "SDK/AppCenterPush/Microsoft.AppCenter.Push/bin/Release/portable-net45+win8+wpa81+wp8/Microsoft.AppCenter.Push.dll",
                            "SDK/AppCenterRum/Microsoft.AppCenter.Rum/bin/Release/portable-net45+win8+wpa81+wp8/Microsoft.AppCenter.Rum.dll" }
        };
        var netStandardAssemblyGroup = new AssemblyGroup {
            AssemblyFolder = NETSTANDARD_ASSEMBLIES_FOLDER,
            AssemblyPaths = new string[] {	"SDK/AppCenter/Microsoft.AppCenter/bin/Release/netstandard1.0/Microsoft.AppCenter.dll",
                            "SDK/AppCenterAnalytics/Microsoft.AppCenter.Analytics/bin/Release/netstandard1.0/Microsoft.AppCenter.Analytics.dll",
                            "SDK/AppCenterCrashes/Microsoft.AppCenter.Crashes/bin/Release/netstandard1.0/Microsoft.AppCenter.Crashes.dll",
                            "SDK/AppCenterDistribute/Microsoft.AppCenter.Distribute/bin/Release/netstandard1.0/Microsoft.AppCenter.Distribute.dll",
                            "SDK/AppCenterPush/Microsoft.AppCenter.Push/bin/Release/netstandard1.0/Microsoft.AppCenter.Push.dll",
                            "SDK/AppCenterRum/Microsoft.AppCenter.Rum/bin/Release/netstandard1.0/Microsoft.AppCenter.Rum.dll" }
        };
        PLATFORM_PATHS.UploadAssemblyGroups.Add(iosAssemblyGroup);
        PLATFORM_PATHS.UploadAssemblyGroups.Add(androidAssemblyGroup);
        PLATFORM_PATHS.UploadAssemblyGroups.Add(pclAssemblyGroup);
        PLATFORM_PATHS.DownloadAssemblyFolders.Add(WINDOWS_DESKTOP_ASSEMBLIES_FOLDER);
        PLATFORM_PATHS.UploadAssemblyGroups.Add(netStandardAssemblyGroup);
        PLATFORM_PATHS.DownloadAssemblyFolders.Add(UWP_ASSEMBLIES_FOLDER);
        PLATFORM_PATHS.DownloadAssemblyFolders.Add(UWP_ASSEMBLIES_FOLDER + "/x86");
        PLATFORM_PATHS.DownloadAssemblyFolders.Add(UWP_ASSEMBLIES_FOLDER + "/x64");
        PLATFORM_PATHS.DownloadAssemblyFolders.Add(UWP_ASSEMBLIES_FOLDER + "/ARM");
        PLATFORM_PATHS.UploadAssembliesZip = MAC_ASSEMBLIES_ZIP + STORAGE_ID;
        PLATFORM_PATHS.DownloadUrl = WINDOWS_ASSEMBLIES_URL + STORAGE_ID;
        PLATFORM_PATHS.DownloadAssembliesZip = WINDOWS_ASSEMBLIES_ZIP + STORAGE_ID;
    }
    else
    {
        var windowsDesktopAssemblyGroup = new AssemblyGroup {
            AssemblyFolder = WINDOWS_DESKTOP_ASSEMBLIES_FOLDER,
            AssemblyPaths = new string[] {
                "SDK/AppCenter/Microsoft.AppCenter.WindowsDesktop/bin/Release/Microsoft.AppCenter.dll",
                "SDK/AppCenterAnalytics/Microsoft.AppCenter.Analytics.WindowsDesktop/bin/Release/Microsoft.AppCenter.Analytics.dll",
                "SDK/AppCenterCrashes/Microsoft.AppCenter.Crashes.WindowsDesktop/bin/Release/Microsoft.AppCenter.Crashes.dll" }
            };
        var uwpAnyCpuAssemblyGroup = new AssemblyGroup {
            AssemblyFolder = UWP_ASSEMBLIES_FOLDER,
            AssemblyPaths = new string[] { "nuget/Microsoft.AppCenter.Crashes.targets",
                                "SDK/AppCenter/Microsoft.AppCenter.UWP/bin/Release/Microsoft.AppCenter.dll",
                                "SDK/AppCenterAnalytics/Microsoft.AppCenter.Analytics.UWP/bin/Release/Microsoft.AppCenter.Analytics.dll",
                                "SDK/AppCenterCrashes/Microsoft.AppCenter.Crashes.UWP/bin/Reference/Microsoft.AppCenter.Crashes.dll",
                                "SDK/AppCenterPush/Microsoft.AppCenter.Push.UWP/bin/Release/Microsoft.AppCenter.Push.dll",
                                "SDK/AppCenterRum/Microsoft.AppCenter.Rum.UWP/bin/Release/Microsoft.AppCenter.Rum.dll" }
        };
        var uwpX86AssemblyGroup = new AssemblyGroup {
            AssemblyFolder = UWP_ASSEMBLIES_FOLDER + "/x86",
            AssemblyPaths = new string[] { 	"SDK/AppCenterCrashes/Microsoft.AppCenter.Crashes.UWP/bin/x86/Release/Microsoft.AppCenter.Crashes.dll",
                                "Release/WatsonRegistrationUtility/WatsonRegistrationUtility.dll",
                                   "Release/WatsonRegistrationUtility/WatsonRegistrationUtility.winmd" }
        };
        var uwpX64AssemblyGroup = new AssemblyGroup {
            AssemblyFolder = UWP_ASSEMBLIES_FOLDER + "/x64",
            AssemblyPaths =  new string[] {	"SDK/AppCenterCrashes/Microsoft.AppCenter.Crashes.UWP/bin/x64/Release/Microsoft.AppCenter.Crashes.dll",
                                       "x64/Release/WatsonRegistrationUtility/WatsonRegistrationUtility.dll",
                                       "x64/Release/WatsonRegistrationUtility/WatsonRegistrationUtility.winmd" }
        };
        var uwpArmAssemblyGroup = new AssemblyGroup {
            AssemblyFolder = UWP_ASSEMBLIES_FOLDER + "/ARM",
            AssemblyPaths =  new string[] {  "SDK/AppCenterCrashes/Microsoft.AppCenter.Crashes.UWP/bin/ARM/Release/Microsoft.AppCenter.Crashes.dll",
                                    "ARM/Release/WatsonRegistrationUtility/WatsonRegistrationUtility.dll",
                                    "ARM/Release/WatsonRegistrationUtility/WatsonRegistrationUtility.winmd" }
        };
        PLATFORM_PATHS.UploadAssemblyGroups.Add(uwpAnyCpuAssemblyGroup);
        PLATFORM_PATHS.UploadAssemblyGroups.Add(uwpX86AssemblyGroup);
        PLATFORM_PATHS.UploadAssemblyGroups.Add(uwpX64AssemblyGroup);
        PLATFORM_PATHS.UploadAssemblyGroups.Add(uwpArmAssemblyGroup);
        PLATFORM_PATHS.UploadAssemblyGroups.Add(windowsDesktopAssemblyGroup);
        PLATFORM_PATHS.DownloadAssemblyFolders.Add(IOS_ASSEMBLIES_FOLDER);
        PLATFORM_PATHS.DownloadAssemblyFolders.Add(ANDROID_ASSEMBLIES_FOLDER);
        PLATFORM_PATHS.DownloadAssemblyFolders.Add(PCL_ASSEMBLIES_FOLDER);
        PLATFORM_PATHS.DownloadAssemblyFolders.Add(NETSTANDARD_ASSEMBLIES_FOLDER);
        PLATFORM_PATHS.UploadAssembliesZip = WINDOWS_ASSEMBLIES_ZIP + STORAGE_ID;
        PLATFORM_PATHS.DownloadUrl = MAC_ASSEMBLIES_URL + STORAGE_ID;
        PLATFORM_PATHS.DownloadAssembliesZip = MAC_ASSEMBLIES_ZIP + STORAGE_ID;
    }
});

// Versioning task.
Task("Version")
    .Does(() =>
{
    var project = ParseProject("./SDK/AppCenter/Microsoft.AppCenter/Microsoft.AppCenter.csproj", configuration: "Release");
    var version = project.NetCore.Version;
    // Extract versions for modules.
    foreach (var module in APP_CENTER_MODULES)
    {
        module.NuGetVersion = version;
    }
});

// Package id task
Task("PackageId")
    .Does(() =>
{
    // Extract package ids for modules.
    foreach (var module in APP_CENTER_MODULES)
    {
        var nuspecText = FileReadText("./nuget/" + module.MainNuspecFilename);
        var startTag = "<id>";
        var endTag = "</id>";
        int startIndex = nuspecText.IndexOf(startTag) + startTag.Length;
        int length = nuspecText.IndexOf(endTag) - startIndex;
        var id = nuspecText.Substring(startIndex, length);
        Information("id = " + id);
        module.PackageId = id;
    }
});

Task("Build").IsDependentOn("MacBuild").IsDependentOn("WindowsBuild");

Task("MacBuild")
    .WithCriteria(() => IsRunningOnUnix())
    .Does(() =>
{
    // Run externals here instead of using dependency so that this doesn't get called on windows
    RunTarget("Externals");
    // Build solution
    NuGetRestore("./AppCenter-SDK-Build-Mac.sln");
    MSBuild("./AppCenter-SDK-Build-Mac.sln", c => c.Configuration = "Release");
}).OnError(HandleError);

// Building Windows code task
Task("WindowsBuild")
    .WithCriteria(() => !IsRunningOnUnix())
    .Does(() =>
{
    // Build solution
    NuGetRestore("./AppCenter-SDK-Build-Windows.sln");
    MSBuild("./AppCenter-SDK-Build-Windows.sln", settings => settings.SetConfiguration("Release").WithProperty("Platform", "x86"));
    MSBuild("./AppCenter-SDK-Build-Windows.sln", settings => settings.SetConfiguration("Release").WithProperty("Platform", "x64"));
    MSBuild("./AppCenter-SDK-Build-Windows.sln", settings => settings.SetConfiguration("Release").WithProperty("Platform", "ARM"));
    MSBuild("./AppCenter-SDK-Build-Windows.sln", settings => settings.SetConfiguration("Release")); // any cpu
    MSBuild("./AppCenter-SDK-Build-Windows.sln", settings => settings.SetConfiguration("Reference")); // any cpu
}).OnError(HandleError);

Task("PrepareAssemblies").IsDependentOn("Build").Does(()=>
{
    foreach (var assemblyGroup in PLATFORM_PATHS.UploadAssemblyGroups)
    {
        CopyFiles(assemblyGroup.AssemblyPaths, assemblyGroup.AssemblyFolder);
    }
}).OnError(HandleError);

// Task dependencies for binding each platform.
Task("Bindings-Android").IsDependentOn("Externals-Android");
Task("Bindings-Ios").IsDependentOn("Externals-Ios");

// Downloading Android binaries.
Task("Externals-Android")
    .Does(() =>
{
    CleanDirectory("./externals/android");

    // Download zip file.
    DownloadFile(ANDROID_URL, "./externals/android/android.zip");
    Unzip("./externals/android/android.zip", "./externals/android/");

    // Move binaries to externals/android so that linked files don't have versions
    // in their paths
    var files = GetFiles("./externals/android/*/*");
    CopyFiles(files, "./externals/android/");
}).OnError(HandleError);

// Downloading iOS binaries.
Task("Externals-Ios")
    .Does(() =>
{
    CleanDirectory("./externals/ios");

    // Download zip file containing AppCenter frameworks
    DownloadFile(IOS_URL, "./externals/ios/ios.zip");
    Unzip("./externals/ios/ios.zip", "./externals/ios/");

    // Copy the AppCenter binaries directly from the frameworks and add the ".a" extension
    var files = GetFiles("./externals/ios/*/iOS/*.framework/AppCenter*");
    foreach (var file in files)
    {
        MoveFile(file, "./externals/ios/" + file.GetFilename() + ".a");
    }

    // Copy Distribute resource bundle and copy it to the externals directory.
    if(DirectoryExists("./externals/ios/AppCenter-SDK-Apple/iOS/AppCenterDistributeResources.bundle"))
    {
        MoveDirectory("./externals/ios/AppCenter-SDK-Apple/iOS/AppCenterDistributeResources.bundle", "./externals/ios/AppCenterDistributeResources.bundle");
    }
}).OnError(HandleError);

// Create a common externals task depending on platform specific ones
Task("Externals").IsDependentOn("Externals-Ios").IsDependentOn("Externals-Android");

// Main Task.
Task("Default").IsDependentOn("NuGet").IsDependentOn("RemoveTemporaries");

// Build tests
Task("UITest").IsDependentOn("RestoreTestPackages").Does(() =>
{
    MSBuild("./Tests/UITests/Contoso.Forms.Test.UITests.csproj", c => c.Configuration = "Release");
});

// Pack NuGets for appropriate platform
Task("NuGet")
    .IsDependentOn("PrepareAssemblies")
    .IsDependentOn("Version")
    .Does(()=>
{
    CleanDirectory("output");
    var specCopyName = TEMPORARY_PREFIX + "spec_copy.nuspec";

    // Package NuGets.
    foreach (var module in APP_CENTER_MODULES)
    {
        var nuspecFilename = "nuget/" + (IsRunningOnUnix() ? module.MacNuspecFilename : module.WindowsNuspecFilename);

        // Skip modules that don't have nuspecs.
        if (!FileExists(nuspecFilename))
        {
            continue;
        }

        // Prepare nuspec by making substitutions in a copied nuspec (to avoid altering the original)
        CopyFile(nuspecFilename, specCopyName);
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

// Add version to nuspecs for vsts (the release definition does not have the solutions and thus cannot extract a version from them)
Task("PrepareNuspecsForVSTS").IsDependentOn("Version").Does(()=>
{
    foreach (var module in APP_CENTER_MODULES)
    {
        ReplaceTextInFiles("./nuget/" + module.MainNuspecFilename, "$version$", module.NuGetVersion);
    }
});

// Upload assemblies to Azure storage
Task("UploadAssemblies")
    .IsDependentOn("PrepareAssemblies")
    .Does(()=>
{
    // The environment variables below must be set for this task to succeed
    var apiKey = EnvironmentVariable("AZURE_STORAGE_ACCESS_KEY");
    var accountName = EnvironmentVariable("AZURE_STORAGE_ACCOUNT");

    foreach (var assemblyGroup in PLATFORM_PATHS.UploadAssemblyGroups)
    {
        var destinationFolder =  DOWNLOADED_ASSEMBLIES_FOLDER + "/" + assemblyGroup.AssemblyFolder;
        CleanDirectory(destinationFolder);
        CopyFiles(assemblyGroup.AssemblyPaths, destinationFolder);
    }

    Information("Uploading to blob " + PLATFORM_PATHS.UploadAssembliesZip);
    Zip(DOWNLOADED_ASSEMBLIES_FOLDER, PLATFORM_PATHS.UploadAssembliesZip);
    AzureStorage.UploadFileToBlob(new AzureStorageSettings
    {
        AccountName = accountName,
        ContainerName = "sdk",
        BlobName = PLATFORM_PATHS.UploadAssembliesZip,
        Key = apiKey,
        UseHttps = true
    }, PLATFORM_PATHS.UploadAssembliesZip);

}).OnError(HandleError).Finally(()=>RunTarget("RemoveTemporaries"));

// Download assemblies from azure storage
Task("DownloadAssemblies").Does(()=>
{
    Information("Fetching assemblies from url: " + PLATFORM_PATHS.DownloadUrl);
    CleanDirectory(DOWNLOADED_ASSEMBLIES_FOLDER);
    DownloadFile(PLATFORM_PATHS.DownloadUrl, PLATFORM_PATHS.DownloadAssembliesZip);
    Unzip(PLATFORM_PATHS.DownloadAssembliesZip, DOWNLOADED_ASSEMBLIES_FOLDER);
    DeleteFiles(PLATFORM_PATHS.DownloadAssembliesZip);
    Information("Successfully downloaded assemblies.");
}).OnError(HandleError);

Task("MergeAssemblies")
    .IsDependentOn("PrepareAssemblies")
    .IsDependentOn("DownloadAssemblies")
    .IsDependentOn("Version")
    .Does(()=>
{
    Information("Beginning complete package creation...");

    // Copy the downloaded files to their proper locations so the structure is as if
    // the downloaded assemblies were built locally (for the nuspecs to work)
    foreach (var assemblyFolder in PLATFORM_PATHS.DownloadAssemblyFolders)
    {
        CleanDirectory(assemblyFolder);
        var files = GetFiles(DOWNLOADED_ASSEMBLIES_FOLDER + "/" + assemblyFolder + "/*");
        CopyFiles(files, assemblyFolder);
    }

    // Create NuGet packages
    foreach (var module in APP_CENTER_MODULES)
    {
        var specCopyName = TEMPORARY_PREFIX + "spec_copy.nuspec";

        // Prepare nuspec by making substitutions in a copied nuspec (to avoid altering the original)
        CopyFile("nuget/" + module.MainNuspecFilename, specCopyName);
        ReplaceAssemblyPathsInNuspecs(specCopyName);

        // Create the NuGet package
        Information("Building a NuGet package for " + module.DotNetModule + " version " + module.NuGetVersion);
        NuGetPack(File(specCopyName), new NuGetPackSettings {
            Verbosity = NuGetVerbosity.Detailed,
            Version = module.NuGetVersion,
            RequireLicenseAcceptance = true
        });

        // Clean up
        DeleteFiles(specCopyName);
    }
    CleanDirectory("output");
    MoveFiles("*.nupkg", "output");
}).OnError(HandleError).Finally(()=>RunTarget("RemoveTemporaries"));

Task("TestApps").IsDependentOn("UITest").Does(() =>
{
    // Build and package the test applications
    MSBuild("./Tests/iOS/Contoso.Forms.Test.iOS.csproj", settings => settings.SetConfiguration("Release")
      .WithTarget("Build")
      .WithProperty("Platform", "iPhone")
      .WithProperty("BuildIpa", "true")
      .WithProperty("OutputPath", "bin/iPhone/Release/")
      .WithProperty("AllowUnsafeBlocks", "true"));
    AndroidPackage("./Tests/Droid/Contoso.Forms.Test.Droid.csproj", false, c => c.Configuration = "Release");
}).OnError(HandleError);

Task("RestoreTestPackages").Does(() =>
{
    NuGetRestore("./AppCenter-SDK-Test.sln");
    NuGetUpdate("./Tests/Contoso.Forms.Test/packages.config");
    NuGetUpdate("./Tests/iOS/packages.config");
    NuGetUpdate("./Tests/Droid/packages.config", new NuGetUpdateSettings {

        // workaround for https://stackoverflow.com/questions/44861995/xamarin-build-error-building-target
        Source = new string[] { EnvironmentVariable("NUGET_URL") }
    });
}).OnError(HandleError);

// Remove any uploaded nugets from azure storage
Task("CleanAzureStorage").Does(()=>
{
    var apiKey = EnvironmentVariable("AZURE_STORAGE_ACCESS_KEY");
    var accountName = EnvironmentVariable("AZURE_STORAGE_ACCOUNT");

    try
    {
        AzureStorage.DeleteBlob(new AzureStorageSettings
        {
            AccountName = accountName,
            ContainerName = "sdk",
            BlobName = MAC_ASSEMBLIES_ZIP + STORAGE_ID,
            Key = apiKey,
            UseHttps = true
        });
        AzureStorage.DeleteBlob(new AzureStorageSettings
        {
            AccountName = accountName,
            ContainerName = "sdk",
            BlobName = WINDOWS_ASSEMBLIES_ZIP + STORAGE_ID,
            Key = apiKey,
            UseHttps = true
        });
    }
    catch
    {
        // not an error if the blob is not found
    }
}).OnError(HandleError);

// Remove all temporary files and folders
Task("RemoveTemporaries").Does(()=>
{
    DeleteFiles(TEMPORARY_PREFIX + "*");
    var dirs = GetDirectories(TEMPORARY_PREFIX + "*");
    foreach (var directory in dirs)
    {
        DeleteDirectoryIfExists(directory.ToString());
    }
    DeleteFiles("./nuget/*.temp.nuspec");
});

// Clean up files/directories.
Task("clean")
    .IsDependentOn("RemoveTemporaries")
    .Does(() =>
{
    DeleteDirectoryIfExists("externals");
    DeleteDirectoryIfExists("output");
    DeleteFiles("./*.nupkg");
    CleanDirectories("./**/bin");
    CleanDirectories("./**/obj");
});

Task("PrepareAssemblyPathsVSTS").Does(()=>
{
    var nuspecPathPrefix = EnvironmentVariable("NUSPEC_PATH");
    foreach (var module in APP_CENTER_MODULES)
    {
        var nuspecPath = System.IO.Path.Combine(nuspecPathPrefix, module.MainNuspecFilename);
        ReplaceAssemblyPathsInNuspecs(nuspecPath);
    }
}).OnError(HandleError);

Task("NugetPackVSTS").Does(()=>
{
    var nuspecPathPrefix = EnvironmentVariable("NUSPEC_PATH");
    foreach (var module in APP_CENTER_MODULES)
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

void ReplaceAssemblyPathsInNuspecs(string nuspecPath)
{
    // For the Tuples, Item1 is variable name, Item2 is variable value.
    var assemblyPathVars = new List<Tuple<string, string>> {
        Tuple.Create("$pcl_dir$", 
                    EnvironmentVariable("PCL_ASSEMBLY_PATH_NUSPEC", PCL_ASSEMBLIES_FOLDER)),
        Tuple.Create("$netstandard_dir$",
                    EnvironmentVariable("NETSTANDARD_ASSEMBLY_PATH_NUSPEC", NETSTANDARD_ASSEMBLIES_FOLDER)),
        Tuple.Create("$uwp_dir$",
                    EnvironmentVariable("UWP_ASSEMBLY_PATH_NUSPEC", UWP_ASSEMBLIES_FOLDER)),
        Tuple.Create("$ios_dir$",
                    EnvironmentVariable("IOS_ASSEMBLY_PATH_NUSPEC", IOS_ASSEMBLIES_FOLDER)),
        Tuple.Create("$android_dir$",
                    EnvironmentVariable("ANDROID_ASSEMBLY_PATH_NUSPEC", ANDROID_ASSEMBLIES_FOLDER)),
        Tuple.Create("$windows_desktop_dir$",
                    EnvironmentVariable("WINDOWS_DESKTOP_ASSEMBLY_PATH_NUSPEC", WINDOWS_DESKTOP_ASSEMBLIES_FOLDER))
    };
    foreach (var assemblyPathVar in assemblyPathVars)
    {
        ReplaceTextInFiles(nuspecPath, assemblyPathVar.Item1, assemblyPathVar.Item2);
    }
}

// Copy files to a clean directory using string names instead of FilePath[] and DirectoryPath
void CopyFiles(IEnumerable<string> files, string targetDirectory, bool clean = true)
{
    if (clean)
    {
        CleanDirectory(targetDirectory);
    }
    foreach (var file in files)
    {
        CopyFile(file, targetDirectory + "/" + System.IO.Path.GetFileName(file));
    }
}

void DeleteDirectoryIfExists(string directoryName)
{
    if (DirectoryExists(directoryName))
    {
        DeleteDirectory(directoryName, new DeleteDirectorySettings { Force = true, Recursive = true });
    }
}

void CleanDirectory(string directoryName)
{
    DeleteDirectoryIfExists(directoryName);
    CreateDirectory(directoryName);
}

void HandleError(Exception exception)
{
    RunTarget("clean");
    throw exception;
}

RunTarget(TARGET);
