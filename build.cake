#tool nuget:?package=XamarinComponent
#addin nuget:?package=Cake.Xamarin
#addin nuget:?package=Cake.FileHelpers

// Sonoma module class definition.
class SonomaModule {
	public String AndroidModule { get; set; }
	public String IosModule { get; set; }
	public String XamarinModule { get; set; }
	public String NuGetVersion { get; set; }
	public String NuGetSpecFilename { get; set; }

	public SonomaModule(String android, String ios, String xamarin, String nuGetSpecFilename) {
		AndroidModule = android;
		IosModule = ios;
		XamarinModule = xamarin;
		NuGetSpecFilename = nuGetSpecFilename;
	}
}

// URLs for downloading binaries.
/*
 * Read this: http://www.mono-project.com/docs/faq/security/.
 * On Windows,
 *     you have to do additional steps for SSL connection to download files.
 *     http://stackoverflow.com/questions/4926676/mono-webrequest-fails-with-https
 *     By running mozroots and install part of Mozilla's root certificates can make it work. 
 */
var ANDROID_URL = "https://aka.ms/p8ujpf";
var IOS_URL = "https://aka.ms/ehvc9e";

// Available Sonoma modules.
var SONOMA_MODULES = new [] {
	new SonomaModule("core-release.aar", "SonomaCore.framework.zip", "Microsoft.Sonoma.Core", "SonomaCore.nuspec"),
	new SonomaModule("analytics-release.aar", "SonomaAnalytics.framework.zip", "Microsoft.Sonoma.Analytics", "SonomaAnalytics.nuspec"),
	new SonomaModule("crashes-release.aar", "SonomaCrashes.framework.zip", "Microsoft.Sonoma.Crashes", "SonomaCrashes.nuspec")
};

// Task TARGET for build
var TARGET = Argument("target", Argument("t", "Default"));

// Versioning task.
Task("Version")
	.Does(() =>
{
	// Read AssemblyInfo.cs and extract versions for modules.
	foreach (var module in SONOMA_MODULES) {
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
	NuGetRestore("./Sonoma-SDK-Xamarin.sln");
	DotNetBuild("./Sonoma-SDK-Xamarin.sln", c => c.Configuration = "Release");
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
	foreach (var module in SONOMA_MODULES) {
		var aar = "./externals/android/" + module.AndroidModule;
		CopyFile(aar, module.XamarinModule + ".Android.Bindings/Jars/" + module.AndroidModule);
	}
});

// Downloading iOS binaries.
Task("Externals-Ios")
	.Does(() =>
{
	// Does nothing until the project has iOS binding.
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
	foreach (var module in SONOMA_MODULES) {
		var spec = GetFiles("./NuGetSpec/" + module.NuGetSpecFilename);
		Information("Building a NuGet package for " + module.XamarinModule + " version " + module.NuGetVersion);
		NuGetPack(spec, new NuGetPackSettings {
			BasePath = basePath,
			Verbosity = NuGetVerbosity.Detailed,
			Version = module.NuGetVersion
		});
	}

	MoveFiles("./Microsoft.Sonoma*.nupkg", "./output");
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
