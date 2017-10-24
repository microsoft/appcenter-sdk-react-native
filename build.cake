#tool nuget:?package=XamarinComponent
#addin nuget:?package=Cake.Xamarin
#addin nuget:?package=Cake.FileHelpers
#addin "Cake.AzureStorage"
#addin nuget:?package=Cake.Git

using System.Net;
using System.Text;
using System.Text.RegularExpressions;

// MobileCenter module class definition.
class MobileCenterModule {
    public string AndroidModule { get; set; }
    public string IosModule { get; set; }
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
    public MobileCenterModule(string android, string ios, string dotnet, string mainNuspecFilename) {
        AndroidModule = android;
        IosModule = ios;
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
var UWP_ASSEMBLIES_FOLDER = TEMPORARY_PREFIX + "UWPAssemblies";
var IOS_ASSEMBLIES_FOLDER = TEMPORARY_PREFIX + "iOSAssemblies";
var ANDROID_ASSEMBLIES_FOLDER = TEMPORARY_PREFIX + "AndroidAssemblies";
var PCL_ASSEMBLIES_FOLDER = TEMPORARY_PREFIX + "PCLAssemblies";

// Native SDK versions
var ANDROID_SDK_VERSION = "0.13.0";
var IOS_SDK_VERSION = "0.13.0";

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
var ANDROID_URL = SDK_STORAGE_URL + "MobileCenter-SDK-Android-" + ANDROID_SDK_VERSION + ".zip";
var IOS_URL = SDK_STORAGE_URL + "MobileCenter-SDK-Apple-" + IOS_SDK_VERSION + ".zip";
var MAC_ASSEMBLIES_URL = SDK_STORAGE_URL + MAC_ASSEMBLIES_ZIP;
var WINDOWS_ASSEMBLIES_URL = SDK_STORAGE_URL + WINDOWS_ASSEMBLIES_ZIP;

// Available MobileCenter modules.
var MOBILECENTER_MODULES = new [] {
    new MobileCenterModule("mobile-center-release.aar", "MobileCenter.framework.zip", "SDK/MobileCenter/Microsoft.Azure.Mobile", "MobileCenter.nuspec"),
    new MobileCenterModule("mobile-center-analytics-release.aar", "MobileCenterAnalytics.framework.zip", "SDK/MobileCenterAnalytics/Microsoft.Azure.Mobile.Analytics", "MobileCenterAnalytics.nuspec"),
    new MobileCenterModule("mobile-center-crashes-release.aar", "MobileCenterCrashes.framework.zip", "SDK/MobileCenterCrashes/Microsoft.Azure.Mobile.Crashes", "MobileCenterCrashes.nuspec"),
    new MobileCenterModule("mobile-center-distribute-release.aar", "MobileCenterDistribute.framework.zip", "SDK/MobileCenterDistribute/Microsoft.Azure.Mobile.Distribute", "MobileCenterDistribute.nuspec"),
    new MobileCenterModule("mobile-center-push-release.aar", "MobileCenterPush.framework.zip", "SDK/MobileCenterPush/Microsoft.Azure.Mobile.Push", "MobileCenterPush.nuspec"),
    new MobileCenterModule("mobile-center-rum-release.aar", null, "SDK/MobileCenterRum/Microsoft.Azure.Mobile.Rum", "MobileCenterRum.nuspec")
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
            AssemblyPaths = new string[] {	"SDK/MobileCenter/Microsoft.Azure.Mobile.iOS/bin/Release/Microsoft.Azure.Mobile.dll",
                            "SDK/MobileCenter/Microsoft.Azure.Mobile.iOS/bin/Release/Microsoft.Azure.Mobile.iOS.Bindings.dll",
                            "SDK/MobileCenterAnalytics/Microsoft.Azure.Mobile.Analytics.iOS/bin/Release/Microsoft.Azure.Mobile.Analytics.dll",
                            "SDK/MobileCenterAnalytics/Microsoft.Azure.Mobile.Analytics.iOS/bin/Release/Microsoft.Azure.Mobile.Analytics.iOS.Bindings.dll",
                            "SDK/MobileCenterCrashes/Microsoft.Azure.Mobile.Crashes.iOS/bin/Release/Microsoft.Azure.Mobile.Crashes.dll",
                            "SDK/MobileCenterCrashes/Microsoft.Azure.Mobile.Crashes.iOS/bin/Release/Microsoft.Azure.Mobile.Crashes.iOS.Bindings.dll",
                            "SDK/MobileCenterDistribute/Microsoft.Azure.Mobile.Distribute.iOS/bin/Release/Microsoft.Azure.Mobile.Distribute.dll",
                            "SDK/MobileCenterDistribute/Microsoft.Azure.Mobile.Distribute.iOS/bin/Release/Microsoft.Azure.Mobile.Distribute.iOS.Bindings.dll",
                            "SDK/MobileCenterPush/Microsoft.Azure.Mobile.Push.iOS/bin/Release/Microsoft.Azure.Mobile.Push.dll",
                            "SDK/MobileCenterPush/Microsoft.Azure.Mobile.Push.iOS.Bindings/bin/Release/Microsoft.Azure.Mobile.Push.iOS.Bindings.dll" }
        };
        var androidAssemblyGroup = new AssemblyGroup {
            AssemblyFolder = ANDROID_ASSEMBLIES_FOLDER,
            AssemblyPaths = new string[] {	"SDK/MobileCenter/Microsoft.Azure.Mobile.Android/bin/Release/Microsoft.Azure.Mobile.dll",
                            "SDK/MobileCenter/Microsoft.Azure.Mobile.Android/bin/Release/Microsoft.Azure.Mobile.Android.Bindings.dll",
                            "SDK/MobileCenterAnalytics/Microsoft.Azure.Mobile.Analytics.Android/bin/Release/Microsoft.Azure.Mobile.Analytics.dll",
                            "SDK/MobileCenterAnalytics/Microsoft.Azure.Mobile.Analytics.Android/bin/Release/Microsoft.Azure.Mobile.Analytics.Android.Bindings.dll",
                            "SDK/MobileCenterCrashes/Microsoft.Azure.Mobile.Crashes.Android/bin/Release/Microsoft.Azure.Mobile.Crashes.dll",
                            "SDK/MobileCenterCrashes/Microsoft.Azure.Mobile.Crashes.Android/bin/Release/Microsoft.Azure.Mobile.Crashes.Android.Bindings.dll",
                            "SDK/MobileCenterDistribute/Microsoft.Azure.Mobile.Distribute.Android/bin/Release/Microsoft.Azure.Mobile.Distribute.dll",
                            "SDK/MobileCenterDistribute/Microsoft.Azure.Mobile.Distribute.Android/bin/Release/Microsoft.Azure.Mobile.Distribute.Android.Bindings.dll",
                            "SDK/MobileCenterPush/Microsoft.Azure.Mobile.Push.Android/bin/Release/Microsoft.Azure.Mobile.Push.dll",
                            "SDK/MobileCenterPush/Microsoft.Azure.Mobile.Push.Android.Bindings/bin/Release/Microsoft.Azure.Mobile.Push.Android.Bindings.dll",
                            "SDK/MobileCenterRum/Microsoft.Azure.Mobile.Rum.Android/bin/Release/Microsoft.Azure.Mobile.Rum.dll",
                            "SDK/MobileCenterRum/Microsoft.Azure.Mobile.Rum.Android.Bindings/bin/Release/Microsoft.Azure.Mobile.Rum.Android.Bindings.dll" }
        };
        var pclAssemblyGroup = new AssemblyGroup {
            AssemblyFolder = PCL_ASSEMBLIES_FOLDER,
            AssemblyPaths = new string[] {	"SDK/MobileCenter/Microsoft.Azure.Mobile/bin/Release/Microsoft.Azure.Mobile.dll",
                            "SDK/MobileCenterAnalytics/Microsoft.Azure.Mobile.Analytics/bin/Release/Microsoft.Azure.Mobile.Analytics.dll",
                            "SDK/MobileCenterCrashes/Microsoft.Azure.Mobile.Crashes/bin/Release/Microsoft.Azure.Mobile.Crashes.dll",
                            "SDK/MobileCenterDistribute/Microsoft.Azure.Mobile.Distribute/bin/Release/Microsoft.Azure.Mobile.Distribute.dll",
                            "SDK/MobileCenterPush/Microsoft.Azure.Mobile.Push/bin/Release/Microsoft.Azure.Mobile.Push.dll",
                            "SDK/MobileCenterRum/Microsoft.Azure.Mobile.Rum/bin/Release/Microsoft.Azure.Mobile.Rum.dll" }
        };
        PLATFORM_PATHS.UploadAssemblyGroups.Add(iosAssemblyGroup);
        PLATFORM_PATHS.UploadAssemblyGroups.Add(androidAssemblyGroup);
        PLATFORM_PATHS.UploadAssemblyGroups.Add(pclAssemblyGroup);
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
        var uwpAnyCpuAssemblyGroup = new AssemblyGroup {
            AssemblyFolder = UWP_ASSEMBLIES_FOLDER,
            AssemblyPaths = new string[] { "nuget/Microsoft.Azure.Mobile.Crashes.targets",
                                "SDK/MobileCenter/Microsoft.Azure.Mobile.UWP/bin/Release/Microsoft.Azure.Mobile.dll",
                                "SDK/MobileCenterAnalytics/Microsoft.Azure.Mobile.Analytics.UWP/bin/Release/Microsoft.Azure.Mobile.Analytics.dll",
                                "SDK/MobileCenterCrashes/Microsoft.Azure.Mobile.Crashes.UWP/bin/Reference/Microsoft.Azure.Mobile.Crashes.dll",
                                "SDK/MobileCenterPush/Microsoft.Azure.Mobile.Push.UWP/bin/Release/Microsoft.Azure.Mobile.Push.dll",
                                "SDK/MobileCenterRum/Microsoft.Azure.Mobile.Rum.UWP/bin/Release/Microsoft.Azure.Mobile.Rum.dll" }
        };
        var uwpX86AssemblyGroup = new AssemblyGroup {
            AssemblyFolder = UWP_ASSEMBLIES_FOLDER + "/x86",
            AssemblyPaths = new string[] { 	"SDK/MobileCenterCrashes/Microsoft.Azure.Mobile.Crashes.UWP/bin/x86/Release/Microsoft.Azure.Mobile.Crashes.dll",
                                "Release/WatsonRegistrationUtility/WatsonRegistrationUtility.dll",
                                   "Release/WatsonRegistrationUtility/WatsonRegistrationUtility.winmd" }
        };
        var uwpX64AssemblyGroup = new AssemblyGroup {
            AssemblyFolder = UWP_ASSEMBLIES_FOLDER + "/x64",
            AssemblyPaths =  new string[] {	"SDK/MobileCenterCrashes/Microsoft.Azure.Mobile.Crashes.UWP/bin/x64/Release/Microsoft.Azure.Mobile.Crashes.dll",
                                       "x64/Release/WatsonRegistrationUtility/WatsonRegistrationUtility.dll",
                                       "x64/Release/WatsonRegistrationUtility/WatsonRegistrationUtility.winmd" }
        };
        var uwpArmAssemblyGroup = new AssemblyGroup {
            AssemblyFolder = UWP_ASSEMBLIES_FOLDER + "/ARM",
            AssemblyPaths =  new string[] {  "SDK/MobileCenterCrashes/Microsoft.Azure.Mobile.Crashes.UWP/bin/ARM/Release/Microsoft.Azure.Mobile.Crashes.dll",
                                    "ARM/Release/WatsonRegistrationUtility/WatsonRegistrationUtility.dll",
                                    "ARM/Release/WatsonRegistrationUtility/WatsonRegistrationUtility.winmd" }
        };
        PLATFORM_PATHS.UploadAssemblyGroups.Add(uwpAnyCpuAssemblyGroup);
        PLATFORM_PATHS.UploadAssemblyGroups.Add(uwpX86AssemblyGroup);
        PLATFORM_PATHS.UploadAssemblyGroups.Add(uwpX64AssemblyGroup);
        PLATFORM_PATHS.UploadAssemblyGroups.Add(uwpArmAssemblyGroup);
        PLATFORM_PATHS.DownloadAssemblyFolders.Add(IOS_ASSEMBLIES_FOLDER);
        PLATFORM_PATHS.DownloadAssemblyFolders.Add(ANDROID_ASSEMBLIES_FOLDER);
        PLATFORM_PATHS.DownloadAssemblyFolders.Add(PCL_ASSEMBLIES_FOLDER);
        PLATFORM_PATHS.UploadAssembliesZip = WINDOWS_ASSEMBLIES_ZIP + STORAGE_ID;
        PLATFORM_PATHS.DownloadUrl = MAC_ASSEMBLIES_URL + STORAGE_ID;
        PLATFORM_PATHS.DownloadAssembliesZip = MAC_ASSEMBLIES_ZIP + STORAGE_ID;
    }
});

// Versioning task.
Task("Version")
    .Does(() =>
{
    var assemblyInfo = ParseAssemblyInfo("./" + "SDK/MobileCenter/Microsoft.Azure.Mobile/Properties/AssemblyInfo.cs");
    var version = assemblyInfo.AssemblyInformationalVersion;
    // Read AssemblyInfo.cs and extract versions for modules.
    foreach (var module in MOBILECENTER_MODULES)
    {
        module.NuGetVersion = version;
    }
});

// Package id task
Task("PackageId")
    .Does(() =>
{
    // Read AssemblyInfo.cs and extract package ids for modules.
    foreach (var module in MOBILECENTER_MODULES)
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
    NuGetRestore("./MobileCenter-SDK-Build-Mac.sln");
    MSBuild("./MobileCenter-SDK-Build-Mac.sln", c => c.Configuration = "Release");
}).OnError(HandleError);

// Building Windows code task
Task("WindowsBuild")
    .WithCriteria(() => !IsRunningOnUnix())
    .Does(() => 
{
    // Build solution
    NuGetRestore("./MobileCenter-SDK-Build-Windows.sln");
    MSBuild("./MobileCenter-SDK-Build-Windows.sln", settings => settings.SetConfiguration("Release").WithProperty("Platform", "x86"));
    MSBuild("./MobileCenter-SDK-Build-Windows.sln", settings => settings.SetConfiguration("Release").WithProperty("Platform", "x64"));
    MSBuild("./MobileCenter-SDK-Build-Windows.sln", settings => settings.SetConfiguration("Release").WithProperty("Platform", "ARM"));
    MSBuild("./MobileCenter-SDK-Build-Windows.sln", settings => settings.SetConfiguration("Release")); // any cpu
    MSBuild("./MobileCenter-SDK-Build-Windows.sln", settings => settings.SetConfiguration("Reference")); // any cpu
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

    // Copy files to {DotNetModule}.Android.Bindings/Jars
    foreach (var module in MOBILECENTER_MODULES)
    {
        var files = GetFiles("./externals/android/*/" + module.AndroidModule);
        CopyFiles(files, module.DotNetModule + ".Android.Bindings/Jars/");
    }
}).OnError(HandleError);

// Downloading iOS binaries.
Task("Externals-Ios")
    .Does(() =>
{
    CleanDirectory("./externals/ios");

    // Download zip file containing MobileCenter frameworks
    DownloadFile(IOS_URL, "./externals/ios/ios.zip");
    Unzip("./externals/ios/ios.zip", "./externals/ios/");

    // Copy the MobileCenter binaries directly from the frameworks and add the ".a" extension
    var files = GetFiles("./externals/ios/*/iOS/*.framework/MobileCenter*");
    foreach (var file in files)
    {
        MoveFile(file, "./externals/ios/" + file.GetFilename() + ".a");
    }

    // Copy Distribute resource bundle and copy it to the externals directory. There is no method in cake to get all subdirectories.
    if(DirectoryExists("./externals/ios/MobileCenter-SDK-Apple/iOS/MobileCenterDistributeResources.bundle"))
    {
        MoveDirectory("./externals/ios/MobileCenter-SDK-Apple/iOS/MobileCenterDistributeResources.bundle", "./externals/ios/MobileCenterDistributeResources.bundle");
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
    // NuGet on mac trims out the first ./ so adding it twice works around
    var basePath = IsRunningOnUnix() ? (System.IO.Directory.GetCurrentDirectory().ToString() + @"/.") : "./";
    CleanDirectory("output");

    var specCopyName = TEMPORARY_PREFIX + "spec_copy.nuspec";

    // Packaging NuGets.
    foreach (var module in MOBILECENTER_MODULES)
    {
        var nuspecFilename = IsRunningOnUnix() ? module.MacNuspecFilename : module.WindowsNuspecFilename;

        // Skipping not exists modules.
        if (!FileExists("nuget/" + nuspecFilename))
        {
            continue;
        }

        // Prepare nuspec by making substitutions in a copied nuspec (to avoid altering the original)
        CopyFile("nuget/" + nuspecFilename, specCopyName);
        ReplaceTextInFiles(specCopyName, "$pcl_dir$", PCL_ASSEMBLIES_FOLDER);
        ReplaceTextInFiles(specCopyName, "$ios_dir$", IOS_ASSEMBLIES_FOLDER);
        ReplaceTextInFiles(specCopyName, "$windows_dir$", UWP_ASSEMBLIES_FOLDER);
        ReplaceTextInFiles(specCopyName, "$android_dir$", ANDROID_ASSEMBLIES_FOLDER);

        var spec = GetFiles(specCopyName);
        Information("Building a NuGet package for " + module.DotNetModule + " version " + module.NuGetVersion);
        NuGetPack(spec, new NuGetPackSettings {
            BasePath = basePath,
            Verbosity = NuGetVerbosity.Detailed,
            Version = module.NuGetVersion
        });

        // Clean up
        DeleteFiles(specCopyName);
    }
    MoveFiles("Microsoft.Azure.Mobile*.nupkg", "output");
}).OnError(HandleError);

// Add version to nuspecs for vsts (the release definition does not have the solutions and thus cannot extract a version from them)
Task("PrepareNuspecsForVSTS").IsDependentOn("Version").Does(()=>
{
    foreach (var module in MOBILECENTER_MODULES)
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
    foreach (var module in MOBILECENTER_MODULES)
    {
        // Prepare nuspec by making substitutions in a copied nuspec (to avoid altering the original)
        var specCopyName = TEMPORARY_PREFIX + "spec_copy.nuspec";
        CopyFile("nuget/" + module.MainNuspecFilename, specCopyName);
        ReplaceTextInFiles(specCopyName, "$pcl_dir$", PCL_ASSEMBLIES_FOLDER);
        ReplaceTextInFiles(specCopyName, "$ios_dir$", IOS_ASSEMBLIES_FOLDER);
        ReplaceTextInFiles(specCopyName, "$windows_dir$", UWP_ASSEMBLIES_FOLDER);
        ReplaceTextInFiles(specCopyName, "$android_dir$", ANDROID_ASSEMBLIES_FOLDER);

        var spec = GetFiles(specCopyName);

        // Create the NuGet package
        Information("Building a NuGet package for " + module.DotNetModule + " version " + module.NuGetVersion);
        NuGetPack(spec, new NuGetPackSettings {
            Verbosity = NuGetVerbosity.Detailed,
            Version = module.NuGetVersion
        });

        // Clean up
        DeleteFiles(specCopyName);
    }
    CleanDirectory("output");
    MoveFiles("*.nupkg", "output");
}).OnError(HandleError).Finally(()=>RunTarget("RemoveTemporaries"));

Task("TestApps").IsDependentOn("UITest").Does(() =>
{
    // Build tests and package the applications
    // It is important that the entire solution is built before rebuilding the iOS and Android versions
    // due to an apparent bug that causes improper linking of the forms application to iOS
    MSBuild("./MobileCenter-SDK-Test.sln", c => c.Configuration = "Release");
    MDToolBuild("./Tests/iOS/Contoso.Forms.Test.iOS.csproj", c => c.Configuration = "Release|iPhone");
    AndroidPackage("./Tests/Droid/Contoso.Forms.Test.Droid.csproj", false, c => c.Configuration = "Release");
    MSBuild("./Tests/UITests/Contoso.Forms.Test.UITests.csproj", c => c.Configuration = "Release");
}).OnError(HandleError);

Task("RestoreTestPackages").Does(() =>
{
    NuGetRestore("./MobileCenter-SDK-Test.sln");
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
        var iosAssemblies = EnvironmentVariable("IOS_ASSEMBLY_PATH_NUSPEC");
        var androidAssemblies = EnvironmentVariable("ANDROID_ASSEMBLY_PATH_NUSPEC");
        var pclAssemblies = EnvironmentVariable("PCL_ASSEMBLY_PATH_NUSPEC");
        var uwpAssemblies = EnvironmentVariable("UWP_ASSEMBLY_PATH_NUSPEC");
        var nuspecPathPrefix = EnvironmentVariable("NUSPEC_PATH");
        
        foreach (var module in MOBILECENTER_MODULES)
        {
            ReplaceTextInFiles(nuspecPathPrefix + module.MainNuspecFilename, "$pcl_dir$", pclAssemblies);
            ReplaceTextInFiles(nuspecPathPrefix + module.MainNuspecFilename, "$ios_dir$", iosAssemblies);
            ReplaceTextInFiles(nuspecPathPrefix + module.MainNuspecFilename, "$windows_dir$", uwpAssemblies);
            ReplaceTextInFiles(nuspecPathPrefix + module.MainNuspecFilename, "$android_dir$", androidAssemblies);
        }
}).OnError(HandleError);

Task("NugetPackVSTS").Does(()=>
{
    var nuspecPathPrefix = EnvironmentVariable("NUSPEC_PATH");
    foreach (var module in MOBILECENTER_MODULES)
    {
        var spec = GetFiles(nuspecPathPrefix + module.MainNuspecFilename);
        // Create the NuGet packages
        Information("Building a NuGet package for " + module.MainNuspecFilename);
        NuGetPack(spec, new NuGetPackSettings {
            Verbosity = NuGetVerbosity.Detailed,
        });
    }
}).OnError(HandleError);

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
