#addin nuget:?package=Cake.FileHelpers
#addin nuget:?package=Cake.Git

using System.Net;
using System.Text;
using System.Text.RegularExpressions;

// Task TARGET for build
var TARGET = Argument("target", Argument("t", ""));

Task("IncrementRevisionNumberWithHash").Does(()=>
{
	IncrementRevisionNumber(true);
});

Task("IncrementRevisionNumber").Does(()=>
{
	IncrementRevisionNumber(false);
});

Task("SetReleaseVersion").Does(()=>
{
	// Get base version of PCL core
	var baseSemanticVersion = GetPCLBaseSemanticVersion();

	// Replace versions in all non-demo app files
	var informationalVersionPattern = @"AssemblyInformationalVersion\(" + "\".*\"" + @"\)";
	var assemblyInfoFiles = GetFiles("**/AssemblyInfo.cs");
	ReplaceRegexInFilesWithExclusion("**/AssemblyInfo.cs", informationalVersionPattern, "AssemblyInformationalVersion(\"" + baseSemanticVersion + "\")", "Demo");

	//Replace version in wrapper sdk
	UpdateWrapperSdkVersion(baseSemanticVersion);
});

Task("UpdateDemoVersion").Does(()=>
{
	var newVersion = Argument<string>("DemoVersion");
	// Replace version in all the demo application assemblies
	var demoAssemblyInfoGlob = "Apps/**/*Demo*/**/AssemblyInfo.cs";
	var informationalVersionPattern = @"AssemblyInformationalVersion\(" + "\".*\"" + @"\)";
	ReplaceRegexInFiles(demoAssemblyInfoGlob, informationalVersionPattern, "AssemblyInformationalVersion(\"" + newVersion + "\")");
	var fileVersionPattern = @"AssemblyFileVersion\(" + "\".*\"" + @"\)";
	ReplaceRegexInFiles(demoAssemblyInfoGlob, fileVersionPattern, "AssemblyFileVersion(\"" + newVersion + ".0\")");

	// Replace android versions
	var manifestGlob = "Apps/**/*Demo*/**/AndroidManifest.xml";
	// Manifest version name tag
	var versionNamePattern = "android:versionName=\"[^\"]+\"";
	var newVersionName = "android:versionName=\"" + newVersion + "\"";
	ReplaceRegexInFiles(manifestGlob, versionNamePattern, newVersionName);
	// Manifest version code
	var manifests = GetFiles("Apps/**/*Demo*/**/AndroidManifest.xml");
	foreach (var manifest in manifests)
	{
		IncrementManifestVersionCode(manifest);
	}

	//Replace UWP version
	var uwpManifestGlob = "Apps/**/*Demo*/**/Package.appxmanifest";
	var versionTagPattern = " Version=\"[^\"]+\"";
	var newVersionTagText = " Version=\""+newVersion+".0\"";
	ReplaceRegexInFiles(uwpManifestGlob, versionTagPattern, newVersionTagText);

	//Replace iOS version
	var bundleVersionPattern = @"<key>CFBundleVersion<\/key>\s*<string>[^<]*<\/string>";
	var newBundleVersionString = "<key>CFBundleVersion</key>\n\t<string>" + newVersion + "</string>";
	ReplaceRegexInFilesWithExclusion("Apps/**/*Demo*/**/Info.plist", bundleVersionPattern, newBundleVersionString, "/bin/", "/obj/");
	var bundleShortVersionPattern = @"<key>CFBundleShortVersionString<\/key>\s*<string>[^<]*<\/string>";
	var newBundleShortVersionString = "<key>CFBundleShortVersionString</key>\n\t<string>" + newVersion + "</string>";
	ReplaceRegexInFilesWithExclusion("Apps/**/*Demo*/**/Info.plist", bundleShortVersionPattern, newBundleShortVersionString, "/bin/", "/obj/");

	RunTarget("UpdateDemoDependencies");
});

Task("StartNewVersion").Does(()=>
{
	var newVersion = Argument<string>("NewVersion");
	var snapshotVersion = newVersion + "-SNAPSHOT";

	// Replace version in all the demo application assemblies
	var assemblyInfoGlob = "**/AssemblyInfo.cs";
	var informationalVersionPattern = @"AssemblyInformationalVersion\(" + "\".*\"" + @"\)";
	ReplaceRegexInFilesWithExclusion(assemblyInfoGlob, informationalVersionPattern, "AssemblyInformationalVersion(\"" + snapshotVersion + "\")", "Demo");
	var fileVersionPattern = @"AssemblyFileVersion\(" + "\".*\"" + @"\)";
	ReplaceRegexInFilesWithExclusion(assemblyInfoGlob, fileVersionPattern, "AssemblyFileVersion(\"" + newVersion + ".0\")");

	// Update wrapper sdk version
	UpdateWrapperSdkVersion(snapshotVersion);

	// Replace android manifest version name tag
	var androidManifestGlob = "Apps/**/AndroidManifest.xml";
	var versionNamePattern = "android:versionName=\"[^\"]+\"";
	var newVersionName = "android:versionName=\"" + snapshotVersion + "\"";
	ReplaceRegexInFilesWithExclusion(androidManifestGlob, versionNamePattern, newVersionName, "Demo");

	// Replace android manifest version code
	var manifests = GetFiles(androidManifestGlob);
	foreach (var manifest in manifests)
	{
		if (!manifest.FullPath.Contains("Demo"))
		{
			IncrementManifestVersionCode(manifest);
		}
	}

	//Replace UWP version
	var uwpManifestGlob = "Apps/**/Package.appxmanifest";
	var versionTagPattern = " Version=\"[^\"]+\"";
	var newVersionTagText = " Version=\""+newVersion+".0\"";
	ReplaceRegexInFilesWithExclusion(uwpManifestGlob, versionTagPattern, newVersionTagText, "Demo");

	//Replace iOS version
	var bundleVersionPattern = @"<key>CFBundleVersion<\/key>\s*<string>[^<]*<\/string>";
	var newBundleVersionString = "<key>CFBundleVersion</key>\n\t<string>" + newVersion + "</string>";
	ReplaceRegexInFilesWithExclusion("Apps/**/Info.plist", bundleVersionPattern, newBundleVersionString, "/bin/", "/obj/", "Demo");
	var bundleShortVersionPattern = @"<key>CFBundleShortVersionString<\/key>\s*<string>[^<]*<\/string>";
	var newBundleShortVersionString = "<key>CFBundleShortVersionString</key>\n\t<string>" + newVersion + "</string>";
	ReplaceRegexInFilesWithExclusion("Apps/**/Info.plist", bundleShortVersionPattern, newBundleShortVersionString, "/bin/", "/obj/", "Demo");
});

Task("UpdateDemoDependencies").Does(() =>
{
	NuGetUpdate("./Apps/Contoso.Forms.Demo/Contoso.Forms.Demo/packages.config");
	NuGetUpdate("./Apps/Contoso.Forms.Demo/Contoso.Forms.Demo.Droid/packages.config");
	NuGetUpdate("./Apps/Contoso.Forms.Demo/Contoso.Forms.Demo.iOS/packages.config");
});

void IncrementRevisionNumber(bool useHash)
{
	// Get base version of PCL core
	var baseSemanticVersion = GetPCLBaseSemanticVersion();

	var nugetVer = GetLatestNuGetVersion();
    var baseVersion = GetBaseVersion(nugetVer);
	var newRevNum = baseSemanticVersion == baseVersion ? GetRevisionNumber(nugetVer) + 1 : 1; 
	var newRevString = GetPaddedString(newRevNum, 4);
	var newVersion = baseVersion + "-r" + newRevString;
	if (useHash)
	{
		newVersion += "-" + GetShortCommitHash();
	}

	//Replace AssemblyInformationalVersion in all AssemblyInfo files
	var informationalVersionPattern = @"AssemblyInformationalVersion\(" + "\".*\"" + @"\)";
	ReplaceRegexInFiles("**/AssemblyInfo.cs", informationalVersionPattern, "AssemblyInformationalVersion(\"" + newVersion + "\")");

	// Increment revision number of AssemblyFileVersion
	var fileVersionPattern = @"AssemblyFileVersion\(" + "\".*\"" + @"\)";
	var files = FindRegexInFiles("**/AssemblyInfo.cs", fileVersionPattern);
	foreach (var file in files)
	{
		var fileVersionTrimmedPattern = @"AssemblyFileVersion\("+ "\"" + @"([0-9]+.){3}";
		var fullVersion = FindRegexMatchInFile(file, fileVersionPattern, RegexOptions.None);
		var trimmedVersion = FindRegexMatchInFile(file, fileVersionTrimmedPattern, RegexOptions.None);
		var newFileVersion = trimmedVersion + newRevNum + "\")";
		ReplaceTextInFiles(file.FullPath, fullVersion, newFileVersion);
	}
}

string GetPCLBaseSemanticVersion()
{
	var assemblyInfo = ParseAssemblyInfo("./SDK/MobileCenter/Microsoft.Azure.Mobile/Properties/AssemblyInfo.cs");
	return GetBaseVersion(assemblyInfo.AssemblyInformationalVersion);
}

string GetShortCommitHash()
{
	var lastCommit = GitLogTip(".");
	return lastCommit.Sha.Substring(0, 7);
}

string GetLatestNuGetVersion()
{
	//Since password and feed id are secret variables in VSTS (and thus cannot be accessed like other environment variables),
	//provide the option to pass them as parameters to the cake script
	var nugetUser = EnvironmentVariable("NUGET_USER");
	var nugetPassword = Argument("NuGetPassword", EnvironmentVariable("NUGET_PASSWORD"));
	var nugetFeedId = Argument("NuGetFeedId", EnvironmentVariable("NUGET_FEED_ID"));
	var url = "https://msmobilecenter.pkgs.visualstudio.com/_packaging/" + nugetFeedId + "/nuget/v2/Search()?\\$filter=IsAbsoluteLatestVersion+and+Id+eq+'Microsoft.Azure.Mobile'&includePrerelease=true";
	HttpWebRequest request = (HttpWebRequest)WebRequest.Create (url);
	request.Headers["X-NuGet-ApiKey"] = nugetPassword;
    request.Credentials = new NetworkCredential(nugetUser, nugetPassword);
    HttpWebResponse response = (HttpWebResponse)request.GetResponse ();
	var responseString = String.Empty;
	using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
	{
		responseString = reader.ReadToEnd();
	}
	var startTag = "<d:Version>";
	var endTag = "</d:Version>";
	int start = responseString.IndexOf(startTag);
	int end = responseString.IndexOf(endTag);
	var tag = responseString.Substring(start + startTag.Length, end - start - startTag.Length);
	return tag;
}

void IncrementManifestVersionCode(FilePath manifest)
{
	var versionCodePattern = "android:versionCode=\"[^\"]+\"";
	var versionCodeText = FindRegexMatchInFile(manifest, versionCodePattern, RegexOptions.None);
	var firstPart = "android:versionCode=\"";
	var length = versionCodeText.Length - 1 - firstPart.Length;
	var versionCode = int.Parse(versionCodeText.Substring(firstPart.Length, length));
	var newVersionCodeText = firstPart + (versionCode + 1) + "\"";
	ReplaceRegexInFiles(manifest.FullPath, versionCodePattern, newVersionCodeText);
}

string GetBaseVersion(string fullVersion)
{
	var indexDash = fullVersion.IndexOf("-");
	if (indexDash == -1)
	{
		return fullVersion;
	}
	return fullVersion.Substring(0, indexDash);
}

// Changes the Version field in WrapperSdk.cs to the given version
void UpdateWrapperSdkVersion(string newVersion)
{
	var patternString = "Version = \"[^\"]+\";";
	var newString = "Version = \"" + newVersion + "\";";
	ReplaceRegexInFiles("SDK/MobileCenter/Microsoft.Azure.Mobile.Shared/WrapperSdk.cs", patternString, newString);
}

// Gets the revision number from a version string containing revision -r****
int GetRevisionNumber(string fullVersion)
{
	var revStart = fullVersion.IndexOf("-r");
	if (revStart == -1)
	{
		return 0;
	}
	var revEnd = fullVersion.IndexOf("-", revStart + 1);
	if (revEnd == -1)
	{
		revEnd = fullVersion.Length;
	}
	var revString = fullVersion.Substring(revStart + 2, revEnd - revStart - 2);
	try
	{
		return Int32.Parse(revString);
	}
	catch
	{
		return 0; //if the revision number could not be parsed, start new revision
	}
}

// Returns the given integer as a string with a number of leading zeroes to 
// pad the string to numDigits digits
string GetPaddedString(int num, int numDigits)
{
	var numString = num.ToString();
	while (numString.Length < numDigits)
	{
		numString = "0" + numString;
	}
	return numString;
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

// Run "ReplaceRegexInFiles" methods but exclude all file paths containing the strings in "excludeFilePathsContaining"
void ReplaceRegexInFilesWithExclusion(string globberPattern, string regEx, string replacement, params string[] excludeFilePathsContaining)
{
	var assemblyInfoFiles = GetFiles(globberPattern);
	foreach (var file in assemblyInfoFiles)
	{
		bool shouldReplace = true;
		foreach (var excludeString in excludeFilePathsContaining)
		{
			if (file.FullPath.Contains(excludeString))
			{
				shouldReplace = false;
				break;
			}
		}
		if (shouldReplace)
		{
			ReplaceRegexInFiles(file.FullPath, regEx, replacement);
		}
	}
}

RunTarget(TARGET);
