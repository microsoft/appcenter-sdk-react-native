#tool nuget:?package=XamarinComponent
#addin nuget:?package=Cake.FileHelpers
#addin nuget:?package=Cake.Xamarin

// Task Target for build
var Target = Argument("Target", Argument("t", "TestApps"));

Task("UITest").IsDependentOn("RestoreTestPackages").Does(() =>
{
    MSBuild("./Tests/UITests/Contoso.Forms.Test.UITests.csproj", c => c.Configuration = "Release");
});

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

        // workaround for https://stackoverflow.com/questions/44861995/xamarin-build-error-building-Target
        Source = new string[] { EnvironmentVariable("NUGET_URL") }
    });
}).OnError(HandleError);

RunTarget(Target);
