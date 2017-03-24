#tool nuget:?package=XamarinComponent
#addin nuget:?package=Cake.Xamarin
#addin nuget:?package=Cake.FileHelpers
#addin "Cake.AzureStorage"

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

// Platform specific nuget folders
var MAC_NUGETS_FOLDER = TEMPORARY_PREFIX + "MacNuGetPackages";
var WINDOWS_NUGETS_FOLDER = TEMPORARY_PREFIX + "WindowsNuGetPackages";

// Native SDK versions
var ANDROID_SDK_VERSION = "0.5.0";
var IOS_SDK_VERSION = "0.4.1";

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

var MAC_NUGETS_ZIP = TEMPORARY_PREFIX + "MacNuGetPackages.zip";
var WINDOWS_NUGETS_ZIP = TEMPORARY_PREFIX + "WindowsNuGetPackages.zip";
var MAC_NUGETS_URL = SDK_STORAGE_URL + MAC_NUGETS_ZIP;
var WINDOWS_NUGETS_URL = SDK_STORAGE_URL + WINDOWS_NUGETS_ZIP;

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
	// Read AssemblyInfo.cs and extract versions for modules.
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
}).OnError(()=>RunTarget("clean"));

// Building Windows code task. (UWP)
Task("WindowsBuild").Does(() => 
{
	// Build solution
	NuGetRestore("./MobileCenter-SDK-Build-Windows.sln");
	DotNetBuild("./MobileCenter-SDK-Build-Windows.sln", c => c.Configuration = "Release");
}).OnError(()=>RunTarget("clean"));

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
}).OnError(()=>RunTarget("clean"));

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
	}).OnError(()=>RunTarget("clean"));

Task("PrepareNuGetsForMerge")
	.IsDependentOn("DownloadNuGets")
	.IsDependentOn("NuGet")
	.IsDependentOn("PrepareNuGetsForRemoteAction");


Task("PrepareNuGetsForRemoteAction")
	.IsDependentOn("PackageId")
	.IsDependentOn("Version")
	.Does(()=>
{
	var nugetFolder = IsRunningOnUnix() ? MAC_NUGETS_FOLDER : WINDOWS_NUGETS_FOLDER;
	CleanDirectory(nugetFolder);
	var files = GetFiles("output/*.nupkg");
	foreach (var file in files)
	{
		foreach (var module in MOBILECENTER_MODULES)
		{
			if (file.GetFilename().ToString() == module.NuGetPackageName)
			{
				var destination = nugetFolder + "/" + module.NuGetPackageName;
				CopyFile(file, destination);
				break;
			}
		}
	}
});

Task("UploadNuGets")
	.IsDependentOn("NuGet")
	.IsDependentOn("PrepareNuGetsForRemoteAction")
	.Does(()=>
{
	//The environment variables below must be set for this task to succeed
	var apiKey = EnvironmentVariable("AZURE_STORAGE_ACCESS_KEY");
	var accountName = EnvironmentVariable("AZURE_STORAGE_ACCOUNT");
	var nugetsFolder = IsRunningOnUnix() ? MAC_NUGETS_FOLDER : WINDOWS_NUGETS_FOLDER;
	var nugetsZip = IsRunningOnUnix() ? MAC_NUGETS_ZIP : WINDOWS_NUGETS_ZIP;
	Zip(nugetsFolder, nugetsZip);
	AzureStorage.UploadFileToBlob(new AzureStorageSettings
	{
		AccountName = accountName,
		ContainerName = "sdk",
		BlobName = nugetsZip,
		Key = apiKey,
		UseHttps = true
	}, nugetsZip);
}).Finally(()=>RunTarget("RemoveTemporaries"));

Task("MergeNuGets")
	.IsDependentOn("NuGet")
	.IsDependentOn("PrepareNuGetsForMerge")
	.Does(()=>
{
	Information("Beginning NuGet merge...");
	var nugetMacUnzipped = TEMPORARY_PREFIX + "mac_nuget_folder";
	var nugetWindowsUnzipped = TEMPORARY_PREFIX + "windows_nuget_folder";
	var specCopyName = TEMPORARY_PREFIX + "spec_copy.nuspec";
	CleanDirectory(nugetMacUnzipped);
	CleanDirectory(nugetWindowsUnzipped);

	foreach (var module in MOBILECENTER_MODULES)
	{
		var nugetMac = MAC_NUGETS_FOLDER + "/" + module.NuGetPackageName;
		var nugetWindows = WINDOWS_NUGETS_FOLDER + "/" + module.NuGetPackageName;

		/* Unzip nuget package */
		Unzip(nugetMac, nugetMacUnzipped);
		Unzip(nugetWindows, nugetWindowsUnzipped);

		/* Prepare nuspec by making substitutions in a copied nuspec (to avoid altering the original) */
		CopyFile("NuGetSpec/" + module.MainNuGetSpecFilename, specCopyName);
		ReplaceTextInFiles(specCopyName, "$mac_dir$", nugetMacUnzipped);
		ReplaceTextInFiles(specCopyName, "$windows_dir$", nugetWindowsUnzipped);
		var spec = GetFiles(specCopyName);

		/* Create the NuGet package */
		Information("Building a NuGet package for " + module.DotNetModule + " version " + module.NuGetVersion);
		NuGetPack(spec, new NuGetPackSettings {
			Verbosity = NuGetVerbosity.Detailed,
			Version = module.NuGetVersion
		});

		/* Clean up */
		DeleteFiles(specCopyName);
		DeleteDirectory(nugetMacUnzipped, true);
		DeleteDirectory(nugetWindowsUnzipped, true);
	}
	
	DeleteDirectory(MAC_NUGETS_FOLDER, true);
	DeleteDirectory(WINDOWS_NUGETS_FOLDER, true);
	CleanDirectory("output");
	MoveFiles("*.nupkg", "output");
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

Task("UpdateDemoDependencies").Does(() =>
{
	NuGetUpdate("./Apps/Contoso.Forms.Demo/Contoso.Forms.Demo/packages.config");
	NuGetUpdate("./Apps/Contoso.Forms.Demo/Contoso.Forms.Demo.Droid/packages.config");
	NuGetUpdate("./Apps/Contoso.Forms.Demo/Contoso.Forms.Demo.iOS/packages.config");
});

// Remove any uploaded nugets from azure storage
Task("CleanAzureStorage").Does(()=>
{
	var nugetsZip = IsRunningOnUnix() ? MAC_NUGETS_ZIP : WINDOWS_NUGETS_ZIP;
	var apiKey = EnvironmentVariable("AZURE_STORAGE_ACCESS_KEY");
	var accountName = EnvironmentVariable("AZURE_STORAGE_ACCOUNT");

	AzureStorage.DeleteBlob(new AzureStorageSettings
	{
		AccountName = accountName,
		ContainerName = "sdk",
		BlobName = nugetsZip,
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

Task("DownloadNuGets").Does(()=>
{
	var otherTargetName = IsRunningOnUnix() ? "Windows machine" : "Mac";	
	Information("Downloading NuGet packages compiled on a " + otherTargetName + "...");
	var nugetsFolder = IsRunningOnUnix() ? WINDOWS_NUGETS_FOLDER : MAC_NUGETS_FOLDER;
	var nugetsZip = IsRunningOnUnix() ? WINDOWS_NUGETS_ZIP : MAC_NUGETS_ZIP;
	var nugetsUrl = IsRunningOnUnix() ? WINDOWS_NUGETS_URL : MAC_NUGETS_URL;
	CleanDirectory(nugetsFolder);
	DownloadFile(nugetsUrl, nugetsZip);
	Unzip(nugetsZip, nugetsFolder);
	DeleteFiles(nugetsZip);
	Information("Successfully downloaded NuGet packages.");
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
