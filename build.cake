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
var ANDROID_SDK_VERSION = "0.5.0";
var IOS_SDK_VERSION = "0.5.1";

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
var IOS_URL = SDK_STORAGE_URL + "MobileCenter-SDK-iOS-" + IOS_SDK_VERSION + ".zip";
var MAC_ASSEMBLIES_URL = SDK_STORAGE_URL + MAC_ASSEMBLIES_ZIP;
var WINDOWS_ASSEMBLIES_URL = SDK_STORAGE_URL + WINDOWS_ASSEMBLIES_ZIP;

// Available MobileCenter modules.
var MOBILECENTER_MODULES = new [] {
	new MobileCenterModule("mobile-center-release.aar", "MobileCenter.framework.zip", "SDK/MobileCenter/Microsoft.Azure.Mobile", "MobileCenter.nuspec"),
	new MobileCenterModule("mobile-center-analytics-release.aar", "MobileCenterAnalytics.framework.zip", "SDK/MobileCenterAnalytics/Microsoft.Azure.Mobile.Analytics", "MobileCenterAnalytics.nuspec"),
	new MobileCenterModule("mobile-center-crashes-release.aar", "MobileCenterCrashes.framework.zip", "SDK/MobileCenterCrashes/Microsoft.Azure.Mobile.Crashes", "MobileCenterCrashes.nuspec")
};

// Task TARGET for build
var TARGET = Argument("target", Argument("t", "Default"));

// Versioning task.
Task("Version")
	.Does(() =>
{
	// Read AssemblyInfo.cs and extract versions for modules.
	foreach (var module in MOBILECENTER_MODULES)
	{
		var assemblyInfo = ParseAssemblyInfo("./" + module.DotNetModule + "/Properties/AssemblyInfo.cs");
		module.NuGetVersion = assemblyInfo.AssemblyInformationalVersion;
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
	DotNetBuild("./MobileCenter-SDK-Build-Mac.sln", c => c.Configuration = "Release");
}).OnError(HandleError);

// Building Windows code task
Task("WindowsBuild")
	.WithCriteria(() => !IsRunningOnUnix())
	.Does(() => 
{
	// Build solution
	NuGetRestore("./MobileCenter-SDK-Build-Windows.sln");
	DotNetBuild("./MobileCenter-SDK-Build-Windows.sln", settings => settings.SetConfiguration("Release").WithProperty("Platform", "x86"));
	DotNetBuild("./MobileCenter-SDK-Build-Windows.sln", settings => settings.SetConfiguration("Release").WithProperty("Platform", "x64"));
	DotNetBuild("./MobileCenter-SDK-Build-Windows.sln", settings => settings.SetConfiguration("Release").WithProperty("Platform", "ARM"));
	DotNetBuild("./MobileCenter-SDK-Build-Windows.sln", settings => settings.SetConfiguration("Release")); // any cpu
}).OnError(HandleError);

Task("PrepareAssemblies").IsDependentOn("PrepareMacAssemblies").IsDependentOn("PrepareWindowsAssemblies");

// Mac agent prepares Android, iOS, and PCL assemblies
Task("PrepareMacAssemblies")
	.WithCriteria(() => IsRunningOnUnix())
	.IsDependentOn("MacBuild")
	.Does(() =>
{
	var iosAssemblies = new string[] {	"SDK/MobileCenter/Microsoft.Azure.Mobile.iOS/bin/Release/Microsoft.Azure.Mobile.dll",
									"SDK/MobileCenter/Microsoft.Azure.Mobile.iOS/bin/Release/Microsoft.Azure.Mobile.iOS.Bindings.dll",
									"SDK/MobileCenterAnalytics/Microsoft.Azure.Mobile.Analytics.iOS/bin/Release/Microsoft.Azure.Mobile.Analytics.dll",
									"SDK/MobileCenterAnalytics/Microsoft.Azure.Mobile.Analytics.iOS/bin/Release/Microsoft.Azure.Mobile.Analytics.iOS.Bindings.dll",
									"SDK/MobileCenterCrashes/Microsoft.Azure.Mobile.Crashes.iOS/bin/Release/Microsoft.Azure.Mobile.Crashes.dll",
									"SDK/MobileCenterCrashes/Microsoft.Azure.Mobile.Crashes.iOS/bin/Release/Microsoft.Azure.Mobile.Crashes.iOS.Bindings.dll" };
	var androidAssemblies = new string[] {	"SDK/MobileCenter/Microsoft.Azure.Mobile.Android/bin/Release/Microsoft.Azure.Mobile.dll",
									"SDK/MobileCenter/Microsoft.Azure.Mobile.Android/bin/Release/Microsoft.Azure.Mobile.Android.Bindings.dll",
									"SDK/MobileCenterAnalytics/Microsoft.Azure.Mobile.Analytics.Android/bin/Release/Microsoft.Azure.Mobile.Analytics.dll",
									"SDK/MobileCenterAnalytics/Microsoft.Azure.Mobile.Analytics.Android/bin/Release/Microsoft.Azure.Mobile.Analytics.Android.Bindings.dll",
									"SDK/MobileCenterCrashes/Microsoft.Azure.Mobile.Crashes.Android/bin/Release/Microsoft.Azure.Mobile.Crashes.dll",
									"SDK/MobileCenterCrashes/Microsoft.Azure.Mobile.Crashes.Android/bin/Release/Microsoft.Azure.Mobile.Crashes.Android.Bindings.dll" };
	var pclAssemblies = new string[] {	"SDK/MobileCenter/Microsoft.Azure.Mobile/bin/Release/Microsoft.Azure.Mobile.dll",
									"SDK/MobileCenterAnalytics/Microsoft.Azure.Mobile.Analytics/bin/Release/Microsoft.Azure.Mobile.Analytics.dll",
									"SDK/MobileCenterCrashes/Microsoft.Azure.Mobile.Crashes/bin/Release/Microsoft.Azure.Mobile.Crashes.dll" };

	CopyFiles(iosAssemblies, IOS_ASSEMBLIES_FOLDER);
	CopyFiles(androidAssemblies, ANDROID_ASSEMBLIES_FOLDER);
	CopyFiles(pclAssemblies, PCL_ASSEMBLIES_FOLDER);
}).OnError(HandleError);


// Windows agent prepares windows assemblies
Task("PrepareWindowsAssemblies")
	.WithCriteria(() => !IsRunningOnUnix())
	.IsDependentOn("WindowsBuild")
	.Does(() =>
{
	var anyCpuAssemblies = new string[] {	"nuget/Microsoft.Azure.Mobile.targets",
										  	"SDK/MobileCenterCrashes/Microsoft.Azure.Mobile.Crashes.UWP/bin/Release/Microsoft.Azure.Mobile.Crashes.UWP.dll" };

	var x86Assemblies = new string[] { 	"SDK/MobileCenterAnalytics/Microsoft.Azure.Mobile.Analytics.UWP/bin/x86/Release/Microsoft.Azure.Mobile.Analytics.dll",
										"SDK/MobileCenter/Microsoft.Azure.Mobile.UWP/bin/x86/Release/Microsoft.Azure.Mobile.dll",
    									"Release/WatsonRegistrationUtility/WatsonRegistrationUtility.dll",
   										"Release/WatsonRegistrationUtility/WatsonRegistrationUtility.winmd" };

   var x64Assemblies = new string[] {	"SDK/MobileCenter/Microsoft.Azure.Mobile.UWP/bin/x64/Release/Microsoft.Azure.Mobile.dll",
  										"SDK/MobileCenterAnalytics/Microsoft.Azure.Mobile.Analytics.UWP/bin/x64/Release/Microsoft.Azure.Mobile.Analytics.dll",
   										"x64/Release/WatsonRegistrationUtility/WatsonRegistrationUtility.dll",
   										"x64/Release/WatsonRegistrationUtility/WatsonRegistrationUtility.winmd"};
	
	var armAssemblies = new string[] {  "SDK/MobileCenter/Microsoft.Azure.Mobile.UWP/bin/ARM/Release/Microsoft.Azure.Mobile.dll",
										"SDK/MobileCenterAnalytics/Microsoft.Azure.Mobile.Analytics.UWP/bin/ARM/Release/Microsoft.Azure.Mobile.Analytics.dll",
										"ARM/Release/WatsonRegistrationUtility/WatsonRegistrationUtility.dll",
										"ARM/Release/WatsonRegistrationUtility/WatsonRegistrationUtility.winmd"};

	var armFolder = UWP_ASSEMBLIES_FOLDER + "/ARM";
	var x86Folder = UWP_ASSEMBLIES_FOLDER + "/x86";
	var x64Folder = UWP_ASSEMBLIES_FOLDER + "/x64";

	CleanDirectory(UWP_ASSEMBLIES_FOLDER);
	CopyFiles(anyCpuAssemblies, UWP_ASSEMBLIES_FOLDER, false);
	CopyFiles(x86Assemblies, x86Folder);
	CopyFiles(x64Assemblies, x64Folder);
	CopyFiles(armAssemblies, armFolder);

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
	var files = GetFiles("./externals/ios/*/*.framework/MobileCenter*");
	foreach (var file in files)
	{
		MoveFile(file, "./externals/ios/" + file.GetFilename() + ".a");
	}
}).OnError(HandleError);

// Create a common externals task depending on platform specific ones
Task("Externals").IsDependentOn("Externals-Ios").IsDependentOn("Externals-Android");

// Main Task.
Task("Default").IsDependentOn("NuGet").IsDependentOn("RemoveTemporaries");

// Build tests
Task("UITest").IsDependentOn("RestoreTestPackages").Does(() =>
{
	DotNetBuild("./Tests/UITests/Contoso.Forms.Test.UITests.csproj", c => c.Configuration = "Release");
});

// Pack NuGets for appropriate platform
Task("NuGet")
	.IsDependentOn("Build")
	.IsDependentOn("Version")
	.Does(()=>
{
	// NuGet on mac trims out the first ./ so adding it twice works around
	var basePath = IsRunningOnUnix() ? (System.IO.Directory.GetCurrentDirectory().ToString() + @"/.") : "./";
	CleanDirectory("output");

	// Packaging NuGets.
	foreach (var module in MOBILECENTER_MODULES)
	{
		var nuspecFilename = IsRunningOnUnix() ? module.MacNuspecFilename : module.WindowsNuspecFilename;
		var spec = GetFiles("./nuget/" + nuspecFilename);
		Information("Building a NuGet package for " + module.DotNetModule + " version " + module.NuGetVersion);
		NuGetPack(spec, new NuGetPackSettings {
			BasePath = basePath,
			Verbosity = NuGetVerbosity.Detailed,
			Version = module.NuGetVersion
		});
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

Task("UploadAssemblies")
	.IsDependentOn("PrepareAssemblies")
	.Does(()=>
{
	//The environment variables below must be set for this task to succeed
	var apiKey = EnvironmentVariable("AZURE_STORAGE_ACCESS_KEY");
	var accountName = EnvironmentVariable("AZURE_STORAGE_ACCOUNT");

	var assembliesZip = IsRunningOnUnix() ? MAC_ASSEMBLIES_ZIP : WINDOWS_ASSEMBLIES_ZIP;

	var pclAssemblies = GetFiles(PCL_ASSEMBLIES_FOLDER + "/*.dll");
	CleanDirectory(DOWNLOADED_ASSEMBLIES_FOLDER + "/" + PCL_ASSEMBLIES_FOLDER);
	CopyFiles(pclAssemblies, DOWNLOADED_ASSEMBLIES_FOLDER + "/" + PCL_ASSEMBLIES_FOLDER);

	if (IsRunningOnUnix())
	{
		CleanDirectory( DOWNLOADED_ASSEMBLIES_FOLDER + "/" + IOS_ASSEMBLIES_FOLDER);
		var iosAssemblies = GetFiles(IOS_ASSEMBLIES_FOLDER + "/*.dll");
		CopyFiles(iosAssemblies, DOWNLOADED_ASSEMBLIES_FOLDER + "/" + IOS_ASSEMBLIES_FOLDER);
		CleanDirectory( DOWNLOADED_ASSEMBLIES_FOLDER + "/" + ANDROID_ASSEMBLIES_FOLDER);
		var androidAssemblies = GetFiles(ANDROID_ASSEMBLIES_FOLDER + "/*.dll");
		CopyFiles(androidAssemblies, DOWNLOADED_ASSEMBLIES_FOLDER + "/" + ANDROID_ASSEMBLIES_FOLDER);
	}
	else
	{
		CleanDirectory( DOWNLOADED_ASSEMBLIES_FOLDER + "/" + UWP_ASSEMBLIES_FOLDER);
		var uwpAssemblies = GetFiles(UWP_ASSEMBLIES_FOLDER + "/*.dll");
		CopyFiles(uwpAssemblies, DOWNLOADED_ASSEMBLIES_FOLDER + "/" + UWP_ASSEMBLIES_FOLDER);
	}

	Zip(DOWNLOADED_ASSEMBLIES_FOLDER, assembliesZip);
	AzureStorage.UploadFileToBlob(new AzureStorageSettings
	{
		AccountName = accountName,
		ContainerName = "sdk",
		BlobName = assembliesZip,
		Key = apiKey,
		UseHttps = true
	}, assembliesZip);
}).OnError(HandleError).Finally(()=>RunTarget("RemoveTemporaries"));

Task("MergeAssemblies")
	.IsDependentOn("PrepareAssemblies")
	.IsDependentOn("DownloadAssemblies")
	.IsDependentOn("Version")
	.Does(()=>
{
	Information("Beginning NuGet merge...");
	var specCopyName = TEMPORARY_PREFIX + "spec_copy.nuspec";

	if (IsRunningOnUnix())
	{
		//extract the uwp packages
		CleanDirectory(UWP_ASSEMBLIES_FOLDER);
		var files = GetFiles(DOWNLOADED_ASSEMBLIES_FOLDER + "/" + UWP_ASSEMBLIES_FOLDER + "/*.dll");
		CopyFiles(files, UWP_ASSEMBLIES_FOLDER);
	}
	else
	{
		//extract the ios packages
		CleanDirectory(IOS_ASSEMBLIES_FOLDER);
		var files = GetFiles(DOWNLOADED_ASSEMBLIES_FOLDER + "/" + IOS_ASSEMBLIES_FOLDER + "/*.dll");
		CopyFiles(files, IOS_ASSEMBLIES_FOLDER);
		
		//extract the android packages
		CleanDirectory(ANDROID_ASSEMBLIES_FOLDER);
		files = GetFiles(DOWNLOADED_ASSEMBLIES_FOLDER + "/" + ANDROID_ASSEMBLIES_FOLDER + "/*.dll");
		CopyFiles(files, ANDROID_ASSEMBLIES_FOLDER);
	}

	foreach (var module in MOBILECENTER_MODULES)
	{
		// Prepare nuspec by making substitutions in a copied nuspec (to avoid altering the original)
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

	DeleteDirectory(PCL_ASSEMBLIES_FOLDER, true);
	DeleteDirectory(ANDROID_ASSEMBLIES_FOLDER, true);
	DeleteDirectory(IOS_ASSEMBLIES_FOLDER, true);
	DeleteDirectory(UWP_ASSEMBLIES_FOLDER, true);
	DeleteDirectory(DOWNLOADED_ASSEMBLIES_FOLDER, true);
	CleanDirectory("output");
	MoveFiles("*.nupkg", "output");
}).OnError(HandleError);

Task("TestApps").IsDependentOn("UITest").Does(() =>
{
	// Build tests and package the applications
	// It is important that the entire solution is built before rebuilding the iOS and Android versions
	// due to an apparent bug that causes improper linking of the forms application to iOS
	DotNetBuild("./MobileCenter-SDK-Test.sln", c => c.Configuration = "Release");
	MDToolBuild("./Tests/iOS/Contoso.Forms.Test.iOS.csproj", c => c.Configuration = "Release|iPhone");
	AndroidPackage("./Tests/Droid/Contoso.Forms.Test.Droid.csproj", false, c => c.Configuration = "Release");
	DotNetBuild("./Tests/UITests/Contoso.Forms.Test.UITests.csproj", c => c.Configuration = "Release");
}).OnError(HandleError);

Task("RestoreTestPackages").Does(() =>
{
	NuGetRestore("./MobileCenter-SDK-Test.sln");
	NuGetUpdate("./Tests/Contoso.Forms.Test/packages.config");
	NuGetUpdate("./Tests/iOS/packages.config");
	NuGetUpdate("./Tests/Droid/packages.config");
}).OnError(HandleError);

// Remove any uploaded nugets from azure storage
Task("CleanAzureStorage").Does(()=>
{
	var apiKey = EnvironmentVariable("AZURE_STORAGE_ACCESS_KEY");
	var accountName = EnvironmentVariable("AZURE_STORAGE_ACCOUNT");

	AzureStorage.DeleteBlob(new AzureStorageSettings
	{
		AccountName = accountName,
		ContainerName = "sdk",
		BlobName = MAC_ASSEMBLIES_ZIP,
		Key = apiKey,
		UseHttps = true
	});

	AzureStorage.DeleteBlob(new AzureStorageSettings
	{
		AccountName = accountName,
		ContainerName = "sdk",
		BlobName = WINDOWS_ASSEMBLIES_ZIP,
		Key = apiKey,
		UseHttps = true
	});
}).OnError(HandleError);

// Remove all temporary files and folders
Task("RemoveTemporaries").Does(()=>
{
	DeleteFiles(TEMPORARY_PREFIX + "*");
	var dirs = GetDirectories(TEMPORARY_PREFIX + "*");
	foreach (var directory in dirs)
	{
		DeleteDirectory(directory, true);
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

Task("DownloadAssemblies").Does(()=>
{
	var otherTargetName = IsRunningOnUnix() ? "Windows machine" : "Mac";	
	Information("Downloading assemblies compiled on a " + otherTargetName + "...");
	var assembliesZip = IsRunningOnUnix() ? WINDOWS_ASSEMBLIES_ZIP : MAC_ASSEMBLIES_ZIP;
	var assembliesUrl = IsRunningOnUnix() ? WINDOWS_ASSEMBLIES_URL : MAC_ASSEMBLIES_URL;
	CleanDirectory(DOWNLOADED_ASSEMBLIES_FOLDER);
	DownloadFile(assembliesUrl, assembliesZip);
	Unzip(assembliesZip, DOWNLOADED_ASSEMBLIES_FOLDER);
	DeleteFiles(assembliesZip);
	Information("Successfully downloaded assemblies.");
}).OnError(HandleError);

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
		DeleteDirectory(directoryName, true);	
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
