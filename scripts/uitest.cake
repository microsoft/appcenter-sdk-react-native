Task("UITest").Does(() =>
{
    MSBuild("../Tests/UITests/Contoso.Forms.Test.UITests.csproj", c => c.Configuration = "Release");
});

RunTarget("UITest");
