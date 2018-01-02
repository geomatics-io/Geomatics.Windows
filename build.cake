#tool nuget:?package=NUnit.ConsoleRunner&version=3.7.0
//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory("./src/Geomatics.Windows/bin") + Directory(configuration);

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore("./src/Geomatics.Windows.sln");
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    if(IsRunningOnWindows())
    {
      // Use MSBuild
      MSBuild("./src/Geomatics.Windows.sln", settings =>
        settings.SetConfiguration(configuration));
    }
    else
    {
      // Use XBuild
      XBuild("./src/Geomatics.Windows.sln", settings =>
        settings.SetConfiguration(configuration));
    }
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    NUnit3("./src/**/bin/" + configuration + "/*.Tests.dll", new NUnit3Settings {
        NoResults = true
        });
});

Task("BuildPackages")
    .IsDependentOn("Run-Unit-Tests")
    .Does(() =>
{
    var nuGetPackSettings = new NuGetPackSettings
    {
        OutputDirectory = "./artifacts",
        IncludeReferencedProjects = true,
        Properties = new Dictionary<string, string>
        {
            { "Configuration", "Release" }
        }
    };

    MSBuild("./src/Geomatics.Windows.PInvoke.Gdi32/Geomatics.Windows.PInvoke.Gdi32.csproj", new MSBuildSettings().SetConfiguration("Release"));
    NuGetPack("./src/Geomatics.Windows.PInvoke.Gdi32/Geomatics.Windows.PInvoke.Gdi32.csproj", nuGetPackSettings);

    MSBuild("./src/Geomatics.Windows.PInvoke.Kernel32/Geomatics.Windows.PInvoke.Kernel32.csproj", new MSBuildSettings().SetConfiguration("Release"));
    NuGetPack("./src/Geomatics.Windows.PInvoke.Kernel32/Geomatics.Windows.PInvoke.Kernel32.csproj", nuGetPackSettings);

    MSBuild("./src/Geomatics.Windows.PInvoke.Shell32/Geomatics.Windows.PInvoke.Shell32.csproj", new MSBuildSettings().SetConfiguration("Release"));
    NuGetPack("./src/Geomatics.Windows.PInvoke.Shell32/Geomatics.Windows.PInvoke.Shell32.csproj", nuGetPackSettings);

    MSBuild("./src/Geomatics.Windows.PInvoke.User32/Geomatics.Windows.PInvoke.User32.csproj", new MSBuildSettings().SetConfiguration("Release"));
    NuGetPack("./src/Geomatics.Windows.PInvoke.User32/Geomatics.Windows.PInvoke.User32.csproj", nuGetPackSettings);

    MSBuild("./src/Geomatics.Windows.Extensions/Geomatics.Windows.Extensions.csproj", new MSBuildSettings().SetConfiguration("Release"));
    NuGetPack("./src/Geomatics.Windows.Extensions/Geomatics.Windows.Extensions.csproj", nuGetPackSettings);

    MSBuild("./src/Geomatics.Windows.Extensions.Tests/Geomatics.Windows.Extensions.Tests.csproj", new MSBuildSettings().SetConfiguration("Release"));
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("BuildPackages");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
