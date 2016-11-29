#tool nuget:?package=XamarinComponent
#addin nuget:?package=Cake.Xamarin
#addin nuget:?package=Cake.FileHelpers

// MobileCenter module class definition.
class MobileCenterModule {
	public String AndroidModule { get; set; }
	public String IosModule { get; set; }
	public String XamarinModule { get; set; }
	public String NuGetVersion { get; set; }
	public String NuGetSpecFilename { get; set; }

	public MobileCenterModule(String android, String ios, String xamarin, String nuGetSpecFilename) {
		AndroidModule = android;
		IosModule = ios;
		XamarinModule = xamarin;
		NuGetSpecFilename = nuGetSpecFilename;
	}
}

// SDK versions
var ANDROID_SDK_VERSION = "0.3.1";
var IOS_SDK_VERSION = "0.3.2";

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
		var assemblyInfo = ParseAssemblyInfo("./" + module.XamarinModule + "/Properties/AssemblyInfo.cs");
		module.NuGetVersion = assemblyInfo.AssemblyInformationalVersion;
	}
});

// Building code task.
Task("Build")
	.IsDependentOn("Externals")
	.Does(() => 
{
	// Build solution
	NuGetRestore("./MobileCenter-SDK-Build.sln");
	DotNetBuild("./MobileCenter-SDK-Build.sln", c => c.Configuration = "Release");
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

	// Copy files to $XamarinModule$.Android.Bindings/Jars
	foreach (var module in MOBILECENTER_MODULES) {
		var files = GetFiles("./externals/android/*/" + module.AndroidModule);
		CopyFiles(files, module.XamarinModule + ".Android.Bindings/Jars/");
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
Task("NuGet")
	.IsDependentOn("Build")
	.IsDependentOn("Version")
	.Does(() => 
{
	// NuGet on mac trims out the first ./ so adding it twice works around
	var basePath = IsRunningOnUnix() ? (System.IO.Directory.GetCurrentDirectory().ToString() + @"/.") : "./";

	// Clean up output directory. 
	if(DirectoryExists("./output"))
		DeleteDirectory("./output", true);
	CreateDirectory("./output");

	// Packaging NuGets.
	foreach (var module in MOBILECENTER_MODULES) {
		var spec = GetFiles("./NuGetSpec/" + module.NuGetSpecFilename);
		Information("Building a NuGet package for " + module.XamarinModule + " version " + module.NuGetVersion);
		NuGetPack(spec, new NuGetPackSettings {
			BasePath = basePath,
			Verbosity = NuGetVerbosity.Detailed,
			Version = module.NuGetVersion
		});
	}

	MoveFiles("./Microsoft.Azure.Mobile*.nupkg", "./output");
});

// Main Task.
Task("Default").IsDependentOn("NuGet");

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

RunTarget(TARGET);
