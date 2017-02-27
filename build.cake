#tool nuget:?package=XamarinComponent
#addin nuget:?package=Cake.Xamarin
#addin nuget:?package=Cake.FileHelpers
#addin "Cake.AzureStorage"

// MobileCenter module class definition.
class MobileCenterModule {
	static int id = 0;
	public String AndroidModule { get; set; }
	public String IosModule { get; set; }
	public String DotNetModule { get; set; }
	public String NuGetVersion { get; set; }
	public int Identifier { get; set; }
	public String MacNuGetSpecFilename 
	{
		get { return  "Mac" + MainNuGetSpecFilename; }
	}
	public String WindowsNuGetSpecFilename
	{
		get { return  "Windows" + MainNuGetSpecFilename; }
	}
	public String MainNuGetSpecFilename { get; set; }
	public MobileCenterModule(String android, String ios, String dotnet, String mainNuGetSpecFilename) {
		AndroidModule = android;
		IosModule = ios;
		DotNetModule = dotnet;
		MainNuGetSpecFilename = mainNuGetSpecFilename;
		Identifier = id++;
	}
}

// Platform specific nuget folders
var MAC_NUGETS_FOLDER = "./MacNuGetPackages";
var WINDOWS_NUGETS_FOLDER = "./WindowsNuGetPackages";

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

var MAC_NUGETS_ZIP = "MacNuGetPackages.zip";
var WINDOWS_NUGETS_ZIP = "WindowsNuGetPackages.zip";
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
	foreach (var module in MOBILECENTER_MODULES) {
		var assemblyInfo = ParseAssemblyInfo("./" + module.DotNetModule + "/Properties/AssemblyInfo.cs");
		module.NuGetVersion = assemblyInfo.AssemblyInformationalVersion;
	}
});

// Building code task.
Task("MacBuild")
	.IsDependentOn("Externals")
	.Does(() => 
{
	// Build solution
	NuGetRestore("./MobileCenter-SDK-Build-Mac.sln");
	DotNetBuild("./MobileCenter-SDK-Build-Mac.sln", c => c.Configuration = "Release");
});

// Building code task.
Task("WindowsBuild").Does(() => 
{
	// Build solution
	NuGetRestore("./MobileCenter-SDK-Build-Windows.sln");
	DotNetBuild("./MobileCenter-SDK-Build-Windows.sln", c => c.Configuration = "Release");
});

// Task dependencies for binding each platform.
Task("Bindings-Android").IsDependentOn("Externals-Android");
Task("Bindings-Ios").IsDependentOn("Externals-Ios");

// Downloading Android binaries.
Task("Externals-Android")
	.Does(() => 
{
	// Clean up download directory.
	if(DirectoryExists("./externals/android"))
		DeleteDirectory("./externals/android", true);
	CreateDirectory("./externals/android");

	// Download zip file.
	DownloadFile(ANDROID_URL, "./externals/android/android.zip");
	Unzip("./externals/android/android.zip", "./externals/android/");

	// Copy files to $DotNetModule$.Android.Bindings/Jars
	foreach (var module in MOBILECENTER_MODULES) {
		var files = GetFiles("./externals/android/*/" + module.AndroidModule);
		CopyFiles(files, module.DotNetModule + ".Android.Bindings/Jars/");
	}
});

// Downloading iOS binaries.
Task("Externals-Ios")
	.Does(() =>
{
	// Clean up download directory.
	if(DirectoryExists("./externals/ios"))
		DeleteDirectory("./externals/ios", true);
	CreateDirectory("./externals/ios");

	// Download zip file containing MobileCenter frameworks
	DownloadFile(IOS_URL, "./externals/ios/ios.zip");

	Unzip("./externals/ios/ios.zip", "./externals/ios/");

	// Copy the MobileCenter binaries directly from the frameworks and add the ".a" extension
	var files = GetFiles("./externals/ios/*/*.framework/MobileCenter*");
	foreach (var file in files) {
		MoveFile(file, "./externals/ios/" + file.GetFilename() + ".a");
	}
});

// Create a common externals task depending on platform specific ones
Task("Externals").IsDependentOn("Externals-Ios").IsDependentOn("Externals-Android");

// Packaging NuGets.
Task("MacNuGet")
	.IsDependentOn("MacBuild")
	.IsDependentOn("Version")
	.Does(() => 
{
	// NuGet on mac trims out the first ./ so adding it twice works around
	var basePath = IsRunningOnUnix() ? (System.IO.Directory.GetCurrentDirectory().ToString() + @"/.") : "./";

	if (DirectoryExists(MAC_NUGETS_FOLDER))
		DeleteDirectory(MAC_NUGETS_FOLDER, true);
	CreateDirectory(MAC_NUGETS_FOLDER);

	// Clean up output directory. 
	if(DirectoryExists("./output"))
		DeleteDirectory("./output", true);
	CreateDirectory("./output");

	// Packaging NuGets.
	foreach (var module in MOBILECENTER_MODULES) {
		var spec = GetFiles("./NuGetSpec/" + module.MacNuGetSpecFilename);
		Information("Building a NuGet package for " + module.DotNetModule + " version " + module.NuGetVersion);
		NuGetPack(spec, new NuGetPackSettings {
			BasePath = basePath,
			Verbosity = NuGetVerbosity.Detailed,
			Version = module.NuGetVersion
		});
	var files = GetFiles("Microsoft.Azure.Mobile*.nupkg");
		foreach (var file in files)
		{
			CopyFile(file, MAC_NUGETS_FOLDER + "/" + module.Identifier + ".nupkg");
		}
		MoveFiles("./Microsoft.Azure.Mobile*.nupkg", "./output");
	}
});

// Packaging Windows NuGets
Task("WindowsNuGet")
	.IsDependentOn("WindowsBuild")
	.IsDependentOn("Version")
	.Does(() => 
{
	// NuGet on mac trims out the first ./ so adding it twice works around
	var basePath = IsRunningOnUnix() ? (System.IO.Directory.GetCurrentDirectory().ToString() + @"/.") : "./";

	if (DirectoryExists(WINDOWS_NUGETS_FOLDER))
		DeleteDirectory(WINDOWS_NUGETS_FOLDER, true);
	CreateDirectory(WINDOWS_NUGETS_FOLDER);

	// Clean up output directory. 
	if(DirectoryExists("./output"))
		DeleteDirectory("./output", true);
	CreateDirectory("./output");

	// Packaging NuGets.
	foreach (var module in MOBILECENTER_MODULES) {
		var spec = GetFiles("./NuGetSpec/" + module.WindowsNuGetSpecFilename);
		Information("Building a NuGet package for " + module.DotNetModule + " version " + module.NuGetVersion);
		NuGetPack(spec, new NuGetPackSettings {
			BasePath = basePath,
			Verbosity = NuGetVerbosity.Detailed,
			Version = module.NuGetVersion
		});
		var files = GetFiles("Microsoft.Azure.Mobile*.nupkg");
		foreach (var file in files)
		{
			CopyFile(file, WINDOWS_NUGETS_FOLDER + "/" + module.Identifier + ".nupkg");
		}
		MoveFiles("./Microsoft.Azure.Mobile*.nupkg", "./output");
	}
});

// Main Task.
Task("Default").IsDependentOn("BuildEnvironmentNuGets");

// Build tests
Task("UITest").IsDependentOn("RestoreTestPackages").Does(() =>
{
	DotNetBuild("./Tests/UITests/Contoso.Forms.Test.UITests.csproj", c => c.Configuration = "Release");
});

Task("BuildEnvironmentNuGets").Does(()=>
{
	var NuGetTarget = IsRunningOnUnix() ? "MacNuGet" : "WindowsNuGet";
	RunTarget(NuGetTarget);
});

Task("UploadNuGets")
	.IsDependentOn("BuildEnvironmentNuGets")
	.Does(()=>
{
	var apiKey = EnvironmentVariable("AZURE_STORAGE_ACCESS_KEY");
	var accountName = EnvironmentVariable("AZURE_STORAGE_ACCOUNT");
	if (IsRunningOnUnix())
	{
		Zip(MAC_NUGETS_FOLDER, MAC_NUGETS_ZIP);
		AzureStorage.UploadFileToBlob(new AzureStorageSettings
		{
			AccountName = accountName,
			ContainerName = "sdk",
			BlobName = MAC_NUGETS_ZIP,
			Key = apiKey,
			UseHttps = true
		}, MAC_NUGETS_ZIP);
	}
	else
	{
		Zip(WINDOWS_NUGETS_FOLDER, WINDOWS_NUGETS_ZIP);
		AzureStorage.UploadFileToBlob(new AzureStorageSettings
		{
			AccountName = accountName,
			ContainerName = "sdk",
			BlobName = WINDOWS_NUGETS_ZIP,
			Key = apiKey,
			UseHttps = true
		}, WINDOWS_NUGETS_ZIP);
	}
});

Task("DownloadNuGets").Does(()=>
{
	if (IsRunningOnUnix())
	{
		CleanDirectory(WINDOWS_NUGETS_FOLDER);
		DownloadFile(WINDOWS_NUGETS_URL, WINDOWS_NUGETS_ZIP);
		Unzip(WINDOWS_NUGETS_ZIP, WINDOWS_NUGETS_FOLDER);
		DeleteFiles(WINDOWS_NUGETS_ZIP);
	}
	else
	{
		CleanDirectory(MAC_NUGETS_FOLDER);
		DownloadFile(MAC_NUGETS_URL, MAC_NUGETS_ZIP);
		Unzip(MAC_NUGETS_ZIP, MAC_NUGETS_FOLDER);
		DeleteFiles(MAC_NUGETS_ZIP);
	}
});

Task("MergeNuGets")
	.IsDependentOn("DownloadNuGets")
	.IsDependentOn("BuildEnvironmentNuGets")
	.IsDependentOn("Version")
	.Does(()=>
{
	var nugetMacUnzipped = "mac_nuget_folder";
	var nugetWindowsUnzipped = "windows_nuget_folder";
	CleanDirectory(nugetMacUnzipped);
	CleanDirectory(nugetWindowsUnzipped);

	foreach (var module in MOBILECENTER_MODULES)
	{

		var nugetMac = MAC_NUGETS_FOLDER + "/" + module.Identifier + ".nupkg";
		var nugetWindows = WINDOWS_NUGETS_FOLDER + "/" + module.Identifier + ".nupkg";

		/* Unzip nuget package */
		Unzip(nugetMac, nugetMacUnzipped);
		Unzip(nugetWindows, nugetWindowsUnzipped);

		/* Prepare nuspec by making substitutions */
		CopyFile("./NuGetSpec/" + module.MainNuGetSpecFilename, "spec_copy");
		ReplaceTextInFiles("spec_copy", "$mac_dir$", nugetMacUnzipped);
		ReplaceTextInFiles("spec_copy", "$windows_dir$", nugetWindowsUnzipped);
		var spec = GetFiles("spec_copy");
		Information("Building a NuGet package for " + module.DotNetModule + " version " + module.NuGetVersion);
		NuGetPack(spec, new NuGetPackSettings {
			Verbosity = NuGetVerbosity.Detailed,
			Version = module.NuGetVersion,
		});
		DeleteFiles("spec_copy");
		DeleteDirectory(nugetMacUnzipped, true);
		DeleteDirectory(nugetWindowsUnzipped, true);
	}
	
	DeleteDirectory(MAC_NUGETS_FOLDER, true);
	DeleteDirectory(WINDOWS_NUGETS_FOLDER, true);
	CleanDirectory("output");
	MoveFiles("*.nupkg", "./output");
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

// Cleaning up files/directories.
Task("clean").Does(() =>
{
	if(DirectoryExists("./externals"))
		DeleteDirectory("./externals", true);

	if(DirectoryExists("./output"))
		DeleteDirectory("./output", true);

	DeleteFiles("./*.nupkg");
	CleanDirectories("./**/bin");
	CleanDirectories("./**/obj");
});

void CleanDirectory(string directoryName)
{
	if(DirectoryExists(directoryName))
		DeleteDirectory(directoryName, true);
	CreateDirectory(directoryName);
}

RunTarget(TARGET);

