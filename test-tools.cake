#tool nuget:?package=XamarinComponent
#addin nuget:?package=Cake.Xamarin
#addin nuget:?package=Cake.FileHelpers
#addin nuget:?package=Newtonsoft.Json

using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json.Linq;

// Task Target for build
var Target = Argument("target", Argument("t", "Default"));

string ArchiveDirectory = "archives";
bool IsMandatory = false;
string DistributionGroup = "Private Release Script Group";
string Token = EnvironmentVariable("APP_CENTER_API_TOKEN");
string BaseUrl = "https://api.appcenter.ms";
ApplicationInfo CurrentApp = null;

public enum Environment
{
    Int,
    Prod
}

public enum Platform
{
    iOS,
    Android,
    UWP
}

public class ApplicationInfo
{
    public static ICakeContext Context;
    public static string OutputDirectory;
    public Environment AppEnvironment { get; }
    public Platform AppPlatform { get; }
    public string AppOwner { get; }
    public string AppId { get; }
    public string AppPath
    {
        get
        {
            return OutputDirectory + "/" + AppId + "." + _appExtension;
        }
    }
    public string ProjectPath
    {
        get
        {
            if (_projectPath == null)
            {
                _projectPath = Context.GetFiles("**/" + _projectFile).Single().ToString();
            }
            return _projectPath;
        }
    }

    public string ProjectDirectory
    {
        get
        {
            return System.IO.Path.GetDirectoryName(ProjectPath);
        }
    }
    
    private string _appExtension = null;
    private string _projectPath = null;
    private string _projectFile = null;
    public ApplicationInfo(Environment environment, Platform platform, string appOwner, string appId, string projectFile, string appExtension)
    {
        AppOwner = appOwner;
        AppId = appId;
        AppEnvironment = environment;
        AppPlatform = platform;
        _projectFile = projectFile;
        _appExtension = appExtension;
    }
}

ApplicationInfo.Context = Context;
ApplicationInfo.OutputDirectory = ArchiveDirectory;
IList<ApplicationInfo> Applications = new List<ApplicationInfo>
{
    new ApplicationInfo(Environment.Prod, Platform.iOS, "app-center", "xamarin-demo-ios", "Contoso.Forms.Demo.iOS.csproj", "ipa"),
    new ApplicationInfo(Environment.Prod, Platform.Android, "app-center", "xamarin-demo-android", "Contoso.Forms.Demo.Droid.csproj", "apk"),
    new ApplicationInfo(Environment.Prod, Platform.UWP, "app-center", "UWP-Forms-Puppet", "Contoso.Forms.Demo.UWP.csproj", ""),
    new ApplicationInfo(Environment.Int, Platform.iOS, "app-center-sdk", "xamarin-puppet-ios", "Contoso.Forms.Puppet.iOS.csproj", "ipa"),
    new ApplicationInfo(Environment.Int, Platform.Android, "app-center-sdk", "xamarin-puppet-android-03", "Contoso.Forms.Puppet.Droid.csproj", "apk"),
    new ApplicationInfo(Environment.Int, Platform.UWP, "app-center-sdk", "xamarin-forms-puppet-uwp-2", "Contoso.Forms.Puppet.UWP.csproj", "")
};

Setup(context =>
{
    // Arguments:
    //  -environment (-e):  App "environment" ("prod" or "int") -- Default is "int"
    //  -group (-g):        Distribution group name -- Default is "Private Release Script Group"
    //  -mandatory (-m):    Should the release be mandatory ("true" or "false") -- Default is "false"
    //  -platform (-p):     ios, android, or uwp -- Default is ios

    // Read arguments
    var environment = Environment.Prod;
    if (Argument("Environment", "int") == "int")
    {
        environment = Environment.Int;
        Token = EnvironmentVariable("APP_CENTER_INT_API_TOKEN");
        BaseUrl = "https://appcenter-int.trafficmanager.net/api";
    }
    var platformString = Argument<string>("Platform", "ios");
    var platform = Platform.iOS;
    if (platformString == "android")
    {
        platform = Platform.Android;
    }
    else if (platformString == "uwp")
    {
        platform = Platform.UWP;
    }
    
    CurrentApp = (  from    app in Applications
                    where   app.AppPlatform == platform &&
                            app.AppEnvironment == environment
                    select  app
    ).Single();

    DistributionGroup = Argument<string>("Group", DistributionGroup);
    DistributionGroup = DistributionGroup.Replace('_', ' ');
    IsMandatory = Argument<bool>("Mandatory", false);
});

// Distribution Tasks

Task("CreateIosArchive").IsDependentOn("IncreaseIosVersion").Does(()=>
{
    MSBuild(CurrentApp.ProjectPath, settings => settings.SetConfiguration("Release")
      .WithTarget("Build")
      .WithProperty("Platform", "iPhone")
      .WithProperty("BuildIpa", "true")
      .WithProperty("OutputPath", "bin/Release/")
      .WithProperty("AllowUnsafeBlocks", "true"));
    var projectLocation = CurrentApp.ProjectDirectory;
    var ipaLocation = projectLocation + 
                      "/bin/Release/" + 
                      System.IO.Path.GetFileNameWithoutExtension(CurrentApp.ProjectPath) +
                      ".ipa";
    EnsureDirectoryExists(ArchiveDirectory);
    if (System.IO.File.Exists(CurrentApp.AppPath))
    {
        System.IO.File.Delete(CurrentApp.AppPath);
    }
    CopyFile(ipaLocation, CurrentApp.AppPath);
});

Task("CreateAndroidArchive").IsDependentOn("IncreaseAndroidVersion").Does(()=>
{
    AndroidPackage(CurrentApp.ProjectPath, true, c => c.Configuration = "Release");
    var projectLocation = CurrentApp.ProjectDirectory;
    var apks = GetFiles(projectLocation + "/bin/Release/*.apk");
    var unsignedApk = "";
    foreach (var path in apks)
    {
        if (!path.ToString().EndsWith("-Signed.apk"))
        {
            unsignedApk = path.ToString();
            break;
        }
    }
    EnsureDirectoryExists(ArchiveDirectory);
    if (System.IO.File.Exists(CurrentApp.AppPath))
    {
        System.IO.File.Delete(CurrentApp.AppPath);
    }
    CopyFile(unsignedApk, CurrentApp.AppPath);
});

Task("IncreaseIosVersion").Does(()=>
{
    var infoPlistLocation = CurrentApp.ProjectDirectory + "/Info.plist";
    var plist = File(infoPlistLocation);
    var bundleVersionPattern = @"<key>CFBundleVersion<\/key>\s*<string>[^<]*<\/string>";
    var match = FindRegexMatchInFile(File(infoPlistLocation), bundleVersionPattern, System.Text.RegularExpressions.RegexOptions.None);
    var openTag = "<string>";
    var closeTag = "</string>";
    var currentVersion = match.Substring(match.IndexOf(openTag) + openTag.Length, match.IndexOf(closeTag) - match.IndexOf(openTag) - openTag.Length);
    var newVersion = IncrementPatch(currentVersion);
    var newBundleVersionString = "<key>CFBundleVersion</key>\n\t<string>" + newVersion + "</string>";
    ReplaceRegexInFiles(infoPlistLocation, bundleVersionPattern, newBundleVersionString);
    Information("iOS Version increased to " + newVersion);
});

Task("IncreaseAndroidVersion").Does(()=>
{
    // Setup
    var manifestLocation = CurrentApp.ProjectDirectory + "/Properties/AndroidManifest.xml";
    var xmlNamespaces = new Dictionary<string, string> {{"android", "http://schemas.android.com/apk/res/android"}};
    var peekSettings = new XmlPeekSettings();
    peekSettings.Namespaces = xmlNamespaces;
    var pokeSettings = new XmlPokeSettings();
    pokeSettings.Namespaces = xmlNamespaces;

    // Manifest version code
    var versionCode = int.Parse(XmlPeek(manifestLocation, "manifest/@android:versionCode", peekSettings));
    var newVersionCode = versionCode + 1;
    XmlPoke(manifestLocation, "manifest/@android:versionCode", newVersionCode.ToString(), pokeSettings);

    // Manifest version name
    var versionName = XmlPeek(manifestLocation, "manifest/@android:versionName", peekSettings);
    var suffix = "-SNAPSHOT";
    if (versionName.Contains(suffix))
    {
        versionName = versionName.Substring(0, versionName.IndexOf(suffix));
    }
    var newVersionName = IncrementPatch(versionName);
    XmlPoke(manifestLocation, "manifest/@android:versionName", newVersionName, pokeSettings);
    Information("Android version name changed to " + newVersionName + ", version code increased to " + newVersionCode);
});

Task("ReleaseApplication")
.Does(()=>
{
    if (CurrentApp.AppPlatform == Platform.iOS)
    {
        RunTarget("CreateIosArchive");
    }
    else if (CurrentApp.AppPlatform == Platform.Android)
    {
        RunTarget("CreateAndroidArchive");
    }
    else
    {
        Error("Cannot distribute for this platform.");
        return;
    }
    
    // Start the upload.
    Information("Initiating distribution process...");
    var startUploadUrl = GetApiUrl(BaseUrl, CurrentApp.AppOwner, CurrentApp.AppId, "release_uploads");
    var startUploadRequest = GetWebRequest(startUploadUrl, Token);
    var startUploadResponse = GetResponseJson(startUploadRequest);

    // Upload the file to the given endpoint. The label "ipa" is correct for all platforms.
    var uploadUrl = startUploadResponse["upload_url"].ToString();
    HttpUploadFile(uploadUrl, CurrentApp.AppPath, "ipa");
    
    // Commit the upload
    Information("Committing distribution...");
    var uploadId = startUploadResponse["upload_id"].ToString();
    var commitRequestUrl = startUploadUrl + "/" + uploadId;
    var commitRequest = GetWebRequest(commitRequestUrl, Token, "PATCH");
    AttachJsonPayload(commitRequest,
                        new JObject(
                            new JProperty("status", "committed")));
    var commitResponse = GetResponseJson(commitRequest);
    var releaseUrl = BaseUrl + "/" + commitResponse["release_url"].ToString();

    // Release the upload
    Information("Finalizing release...");
    var releaseRequest = GetWebRequest(releaseUrl, Token, "PATCH");
    var releaseNotes = "This release has been created by the script test-tools.cake.";
    AttachJsonPayload(releaseRequest, 
                        new JObject(
                            new JProperty("destination_name", DistributionGroup),
                            new JProperty("release_notes", releaseNotes),
                            new JProperty("mandatory", IsMandatory.ToString().ToLower())));
    releaseRequest.GetResponse().Dispose();
    var mandatorySuffix = IsMandatory ? " as a mandatory update" : "";
    Information("Successfully released " + CurrentApp.AppOwner + 
                    "/" + CurrentApp.AppId + " to group " + 
                    DistributionGroup + mandatorySuffix + ".");
});

// Push tasks
Task("SendPushNotification")
.Does(()=>
{
    var name = "Test Notification";
    var title = "Test Notification";
    var timeSent = DateTime.Now.ToString();
    var body = "Notification sent from test script at " + timeSent + ".";
    var properties = new Dictionary<string, string> {{"time_sent", timeSent}};
    var notificationJson = new JObject(
                            new JProperty("notification_content",
                                new JObject(
                                    new JProperty("name", name),
                                    new JProperty("title", title),
                                    new JProperty("body", body),
                                    new JProperty("custom_data",
                                        new JObject(
                                            from key in properties.Keys
                                            select new JProperty(key, properties[key]))))));
    Information("Sending notification:\n" + notificationJson.ToString());
    var url = GetApiUrl(BaseUrl, CurrentApp.AppOwner, CurrentApp.AppId, "push/notifications");
    var request = GetWebRequest(url, Token);
    AttachJsonPayload(request, notificationJson);
    var responseJson = GetResponseJson(request);
    Information("Successfully sent push notification and received result:\n" + responseJson.ToString());
});

// Helper methods

string GetApiUrl(string baseUrl, string appOwner, string appId, string apiName)
{
    return string.Format("{0}/v0.1/apps/{1}/{2}/{3}", baseUrl, appOwner, appId, apiName);
}

JObject GetResponseJson(HttpWebRequest request)
{
    using (var response = request.GetResponse())
    using (var reader = new StreamReader(response.GetResponseStream()))
    {
        return JObject.Parse(reader.ReadToEnd());
    }
}

HttpWebRequest GetWebRequest(string url, string token, string method = "POST")
{
    Information(string.Format("About to call url '{0}'", url));
    var request = (HttpWebRequest)WebRequest.Create(url);
    request.Headers["X-API-Token"] = token;
    request.ContentType = "application/json";
    request.Accept = "application/json";
    request.Method = method;
    return request;
}

void AttachJsonPayload(HttpWebRequest request, JObject json)
{
    using (var stream = request.GetRequestStream())
    using (var sr = new StreamWriter(stream))
    {
        sr.Write(json.ToString());
    }
}

string IncrementPatch(string semVer)
{
    int patchIdx = 0;
    for (int i = semVer.Length - 1; i >= 0; --i)
    {
        if (semVer[i] == '.')
        {
            patchIdx = i + 1;
            break;
        }
    }
    var newPatch = Convert.ToInt32(semVer.Substring(patchIdx, semVer.Length - patchIdx)) + 1;
    return semVer.Substring(0, patchIdx) + newPatch;
}

// Adapted from https://stackoverflow.com/questions/566462/upload-files-with-httpwebrequest-multipart-form-data/2996904#2996904
void HttpUploadFile(string url, string file, string paramName)
{
    Information(string.Format("Uploading {0} to {1}", file, url));
    var boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
    byte[] boundaryBytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
    var request = (HttpWebRequest)WebRequest.Create(url);
    request.ContentType = "multipart/form-data; boundary=" + boundary;
    request.Method = "POST";
    request.KeepAlive = true;
    using (var requestStream = request.GetRequestStream())
    {
        requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);
        var headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n\r\n";
        var header = string.Format(headerTemplate, paramName, file);
        byte[] headerBytes = System.Text.Encoding.UTF8.GetBytes(header);
        requestStream.Write(headerBytes, 0, headerBytes.Length);
        using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
        {
            byte[] buffer = new byte[4096];
            var bytesRead = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                requestStream.Write(buffer, 0, bytesRead);
            }
        }
        byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
        requestStream.Write(trailer, 0, trailer.Length);
    }
    request.GetResponse().Dispose();
    Information("File uploaded.");
}

Task("Default").Does(()=>
{
    Error("Please run a specific target.");
});

RunTarget(Target);
