// This file contains various utilities that are used or can be used by multiple cake scripts.

// Static variables defined outside of a class can cause issues.
public class Statics
{
    // Cake context.
    public static ICakeContext Context { get; set; }

    // Prefix for temporary intermediates that are created by this script.
    public const string TemporaryPrefix = "CAKE_SCRIPT_TEMP";
}

// Can't reference Context within the class, so set value outside
Statics.Context = Context;

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
        DeleteDirectory(directoryName, new DeleteDirectorySettings { Force = true, Recursive = true });
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

// Remove all temporary files and folders
Task("RemoveTemporaries").Does(()=>
{
    DeleteFiles(Statics.TemporaryPrefix + "*");
    var dirs = GetDirectories(Statics.TemporaryPrefix + "*");
    foreach (var directory in dirs)
    {
        DeleteDirectoryIfExists(directory.ToString());
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
