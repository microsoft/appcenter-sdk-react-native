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
	public string MainNuGetSpecFilename { get; set; }
	public string NuGetPackageName
	{
		get
		{
			return PackageId + "." + NuGetVersion + ".nupkg";
		}
	}
	public string MacNuGetSpecFilename 
	{
		get { return  "Mac" + MainNuGetSpecFilename; }
	}
	public string WindowsNuGetSpecFilename
	{
		get { return  "Windows" + MainNuGetSpecFilename; }
	}
	public MobileCenterModule(string android, string ios, string dotnet, string mainNuGetSpecFilename) {
		AndroidModule = android;
		IosModule = ios;
		DotNetModule = dotnet;
		MainNuGetSpecFilename = mainNuGetSpecFilename;
	}
}

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
		var nuspecText = FileReadText("./NuGetSpec/" + module.MainNuGetSpecFilename);
		var startTag = "<id>";
		var endTag = "</id>";
		int startIndex = nuspecText.IndexOf(startTag) + startTag.Length;
		int length = nuspecText.IndexOf(endTag) - startIndex;
		var id = nuspecText.Substring(startIndex, length);
		Information("id = " + id);
		module.PackageId = id;
	}
});

Task("Build").Does(()=>
{
	var targetName = IsRunningOnUnix() ? "MacBuild" : "WindowsBuild";
	RunTarget(targetName);
});

Task("MacBuild")
	.IsDependentOn("Externals")
	.Does(() => 
{
	// Build solution
	NuGetRestore("./MobileCenter-SDK-Build-Mac.sln");
	DotNetBuild("./MobileCenter-SDK-Build-Mac.sln", c => c.Configuration = "Release");
}).OnError(exception => {
	RunTarget("clean");
	throw exception;
});

// Building Windows code task
Task("WindowsBuild").Does(() => 
{
	// Build solution
	NuGetRestore("./MobileCenter-SDK-Build-Windows.sln");
	DotNetBuild("./MobileCenter-SDK-Build-Windows.sln", c => c.Configuration = "Release");
}).OnError(exception => {
	RunTarget("clean");
	throw exception;
});

// Build and prepare UWP dlls
Task("PrepareUWPAssemblies").Does(() =>
{
	NuGetRestore("./MobileCenter-SDK-Build-UWP.sln");
	DotNetBuild("./MobileCenter-SDK-Build-UWP.sln", c => c.Configuration = "Release");

	var assemblies = new string[] {	"SDK/MobileCenter/Microsoft.Azure.Mobile.UWP/bin/Release/Microsoft.Azure.Mobile.dll",
									"SDK/MobileCenterAnalytics/Microsoft.Azure.Mobile.Analytics.UWP/bin/Release/Microsoft.Azure.Mobile.Analytics.dll",
									"SDK/MobileCenterCrashes/Microsoft.Azure.Mobile.Crashes.UWP/bin/Release/Microsoft.Azure.Mobile.Crashes.UWP.dll" };
	CleanDirectory(UWP_ASSEMBLIES_FOLDER);
	foreach (var assembly in assemblies)
	{
		CopyFile(assembly, UWP_ASSEMBLIES_FOLDER + "/" + System.IO.Path.GetFileName(assembly));
	}
}).OnError(exception => {
	RunTarget("clean");
	throw exception;
});

// Build and prepare iOS dlls
Task("PrepareIosAssemblies").IsDependentOn("Externals-Ios").Does(() =>
{
	NuGetRestore("./MobileCenter-SDK-Build-iOS.sln");
	DotNetBuild("./MobileCenter-SDK-Build-iOS.sln", c => c.Configuration = "Release");

	var assemblies = new string[] {	"SDK/MobileCenter/Microsoft.Azure.Mobile.iOS/bin/Release/Microsoft.Azure.Mobile.dll",
									"SDK/MobileCenter/Microsoft.Azure.Mobile.iOS/bin/Release/Microsoft.Azure.Mobile.iOS.Bindings.dll",
									"SDK/MobileCenterAnalytics/Microsoft.Azure.Mobile.Analytics.iOS/bin/Release/Microsoft.Azure.Mobile.Analytics.dll",
									"SDK/MobileCenterAnalytics/Microsoft.Azure.Mobile.Analytics.iOS/bin/Release/Microsoft.Azure.Mobile.Analytics.iOS.Bindings.dll",
									"SDK/MobileCenterCrashes/Microsoft.Azure.Mobile.Crashes.iOS/bin/Release/Microsoft.Azure.Mobile.Crashes.dll",
									"SDK/MobileCenterCrashes/Microsoft.Azure.Mobile.Crashes.iOS/bin/Release/Microsoft.Azure.Mobile.Crashes.iOS.Bindings.dll" };

	CleanDirectory(IOS_ASSEMBLIES_FOLDER);
	foreach (var assembly in assemblies)
	{
		CopyFile(assembly, IOS_ASSEMBLIES_FOLDER + "/" + System.IO.Path.GetFileName(assembly));
	}
}).OnError(()=>RunTarget("clean"));

// Build and prepare Android dlls
Task("PrepareAndroidAssemblies").IsDependentOn("Externals-Android").Does(() =>
{
	NuGetRestore("./MobileCenter-SDK-Build-Android.sln");
	DotNetBuild("./MobileCenter-SDK-Build-Android.sln", c => c.Configuration = "Release");

	var assemblies = new string[] {	"SDK/MobileCenter/Microsoft.Azure.Mobile.Android/bin/Release/Microsoft.Azure.Mobile.dll",
									"SDK/MobileCenter/Microsoft.Azure.Mobile.Android/bin/Release/Microsoft.Azure.Mobile.Android.Bindings.dll",
									"SDK/MobileCenterAnalytics/Microsoft.Azure.Mobile.Analytics.Android/bin/Release/Microsoft.Azure.Mobile.Analytics.dll",
									"SDK/MobileCenterAnalytics/Microsoft.Azure.Mobile.Analytics.Android/bin/Release/Microsoft.Azure.Mobile.Analytics.Android.Bindings.dll",
									"SDK/MobileCenterCrashes/Microsoft.Azure.Mobile.Crashes.Android/bin/Release/Microsoft.Azure.Mobile.Crashes.dll",
									"SDK/MobileCenterCrashes/Microsoft.Azure.Mobile.Crashes.Android/bin/Release/Microsoft.Azure.Mobile.Crashes.Android.Bindings.dll" };

	CleanDirectory(ANDROID_ASSEMBLIES_FOLDER);
	foreach (var assembly in assemblies)
	{
		CopyFile(assembly, ANDROID_ASSEMBLIES_FOLDER + "/" + System.IO.Path.GetFileName(assembly));
	}
}).OnError(exception => {
	RunTarget("clean");
	throw exception;
});


// Build and prepare PCL dlls
Task("PreparePCLAssemblies").Does(() =>
{
	NuGetRestore("./MobileCenter-SDK-Build-PCL.sln");
	DotNetBuild("./MobileCenter-SDK-Build-PCL.sln", c => c.Configuration = "Release");

	var assemblies = new string[] {	"SDK/MobileCenter/Microsoft.Azure.Mobile/bin/Release/Microsoft.Azure.Mobile.dll",
									"SDK/MobileCenterAnalytics/Microsoft.Azure.Mobile.Analytics/bin/Release/Microsoft.Azure.Mobile.Analytics.dll",
									"SDK/MobileCenterCrashes/Microsoft.Azure.Mobile.Crashes/bin/Release/Microsoft.Azure.Mobile.Crashes.dll" };

	CleanDirectory(PCL_ASSEMBLIES_FOLDER);
	foreach (var assembly in assemblies)
	{
		CopyFile(assembly, PCL_ASSEMBLIES_FOLDER + "/" + System.IO.Path.GetFileName(assembly));
	}
}).OnError(exception => {
	RunTarget("clean");
	throw exception;
});


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
}).OnError(()=>RunTarget("clean"));

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
}).OnError(exception => {
	RunTarget("clean");
	throw exception;
});

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
		var nuspecFilename = IsRunningOnUnix() ? module.MacNuGetSpecFilename : module.WindowsNuGetSpecFilename;
		var spec = GetFiles("./NuGetSpec/" + nuspecFilename);
		Information("Building a NuGet package for " + module.DotNetModule + " version " + module.NuGetVersion);
		NuGetPack(spec, new NuGetPackSettings {
			BasePath = basePath,
			Verbosity = NuGetVerbosity.Detailed,
			Version = module.NuGetVersion
		});
	}
	MoveFiles("Microsoft.Azure.Mobile*.nupkg", "output");
}).OnError(exception => {
	RunTarget("clean");
	throw exception;
});


Task("PrepareAssemblies").IsDependentOn("PreparePCLAssemblies").Does(()=>
{
	if (IsRunningOnUnix())
	{
		RunTarget("PrepareIosAssemblies");
		RunTarget("PrepareAndroidAssemblies");
	}
	else
	{
		RunTarget("PrepareUWPAssemblies");
	}
});

Task("PrepareNuspecsForVSTS").IsDependentOn("Version").Does(()=>
{
	foreach (var module in MOBILECENTER_MODULES)
	{
		var macFolder = "../../mac/assemblies/";
		var windowsFolder = "../../windows/assemblies/";
		ReplaceTextInFiles("./NuGetSpec/" + module.MainNuGetSpecFilename, "$pcl_dir$", macFolder + "PCLAssemblies");
		ReplaceTextInFiles("./NuGetSpec/" + module.MainNuGetSpecFilename, "$ios_dir$", macFolder + "iOSAssemblies");
		ReplaceTextInFiles("./NuGetSpec/" + module.MainNuGetSpecFilename, "$windows_dir$", windowsFolder + "UWPAssemblies");
		ReplaceTextInFiles("./NuGetSpec/" + module.MainNuGetSpecFilename, "$android_dir$", macFolder + "AndroidAssemblies");
		ReplaceTextInFiles("./NuGetSpec/" + module.MainNuGetSpecFilename, "$version$", module.NuGetVersion);
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
}).OnError(exception => {
	RunTarget("clean");
	throw exception;
}).Finally(()=>RunTarget("RemoveTemporaries"));

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
		/* Prepare nuspec by making substitutions in a copied nuspec (to avoid altering the original) */
		CopyFile("NuGetSpec/" + module.MainNuGetSpecFilename, specCopyName);
		ReplaceTextInFiles(specCopyName, "$pcl_dir$", PCL_ASSEMBLIES_FOLDER);
		ReplaceTextInFiles(specCopyName, "$ios_dir$", IOS_ASSEMBLIES_FOLDER);
		ReplaceTextInFiles(specCopyName, "$windows_dir$", UWP_ASSEMBLIES_FOLDER);
		ReplaceTextInFiles(specCopyName, "$android_dir$", ANDROID_ASSEMBLIES_FOLDER);

		var spec = GetFiles(specCopyName);

		/* Create the NuGet package */
		Information("Building a NuGet package for " + module.DotNetModule + " version " + module.NuGetVersion);
		NuGetPack(spec, new NuGetPackSettings {
			Verbosity = NuGetVerbosity.Detailed,
			Version = module.NuGetVersion
		});

		/* Clean up */
		DeleteFiles(specCopyName);
	}
	
	DeleteDirectory(PCL_ASSEMBLIES_FOLDER, true);
	DeleteDirectory(ANDROID_ASSEMBLIES_FOLDER, true);
	DeleteDirectory(IOS_ASSEMBLIES_FOLDER, true);
	DeleteDirectory(UWP_ASSEMBLIES_FOLDER, true);
	DeleteDirectory(DOWNLOADED_ASSEMBLIES_FOLDER, true);
	CleanDirectory("output");
	MoveFiles("*.nupkg", "output");
}).OnError(exception => {
	RunTarget("clean");
	throw exception;
});

Task("TestApps").IsDependentOn("UITest").Does(() =>
{
	// Build tests and package the applications
	// It is important that the entire solution is built before rebuilding the iOS and Android versions due to a bug 
	// that causes improper linking of the forms application to iOS
	DotNetBuild("./MobileCenter-SDK-Test.sln", c => c.Configuration = "Release");
	MDToolBuild("./Tests/iOS/Contoso.Forms.Test.iOS.csproj", c => c.Configuration = "Release|iPhone");
	AndroidPackage("./Tests/Droid/Contoso.Forms.Test.Droid.csproj", false, c => c.Configuration = "Release");
	DotNetBuild("./Tests/UITests/Contoso.Forms.Test.UITests.csproj", c => c.Configuration = "Release");
});

Task("RestoreTestPackages").Does(() =>
{
	NuGetRestore("./MobileCenter-SDK-Test.sln");
	NuGetUpdate("./Tests/Contoso.Forms.Test/packages.config");
	NuGetUpdate("./Tests/iOS/packages.config");
	NuGetUpdate("./Tests/Droid/packages.config");
});

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
});

// Remove all temporary files and folders
Task("RemoveTemporaries").Does(()=>
{
	DeleteFiles(TEMPORARY_PREFIX + "*");
	var dirs = GetDirectories(TEMPORARY_PREFIX + "*");
	foreach (var directory in dirs)
	{
		DeleteDirectory(directory, true);
	}
	DeleteFiles("./NuGetSpec/*.temp.nuspec");
});


// Cleaning up files/directories.
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
}).OnError(exception => {
	RunTarget("clean");
	throw exception;
});

Task("NugetPackVSTS").Does(()=>
{
	foreach (var module in MOBILECENTER_MODULES)
	{
		var spec = GetFiles("windows/nuspecs/NuGetSpec/" + module.MainNuGetSpecFilename);
		/* Create the NuGet packages */
		Information("Building a NuGet package for " + spec);
		NuGetPack(spec, new NuGetPackSettings {
			Verbosity = NuGetVerbosity.Detailed,
		});
	}
});

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

RunTarget(TARGET);
