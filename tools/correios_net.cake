#addin "Cake.FileHelpers"
#addin "Octokit"
using Octokit;

var configuration = Argument("configuration", "Release");
var isRunningOnUnix = IsRunningOnUnix();
var isRunningOnWindows = IsRunningOnWindows();
var isRunningOnAppVeyor = AppVeyor.IsRunningOnAppVeyor;
var isPullRequest = AppVeyor.Environment.PullRequest.IsPullRequest;
var buildNumber = AppVeyor.Environment.Build.Number;
var releaseNotes = ParseReleaseNotes("./CHANGELOG.md");
var version = releaseNotes.Version.ToString();
var buildDir = Directory($"./src/{projectName}/bin") + Directory(configuration);
var buildResultDir = Directory("./bin") + Directory(version);
var nugetRoot = buildResultDir + Directory("nuget");

if (!isRunningOnWindows)
{
    frameworks.Remove("net46");
}

// Initialization
// ----------------------------------------

Setup(_ =>
{
    Information($"Building version {version} of {projectName}.");
});

// Tasks
// ----------------------------------------

Task("Clean")
    .Does(() =>
    {
        CleanDirectories(new DirectoryPath[] { buildDir, buildResultDir, nugetRoot });
    });

Task("Restore-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        NuGetRestore($"./src/{solutionName}.sln", new NuGetRestoreSettings
        {
            ToolPath = "tools/nuget.exe",
        });
    });

Task("Build")
    .IsDependentOn("Restore-Packages")
    .Does(() =>
    {
        ReplaceRegexInFiles("./src/Directory.Build.props", "(?<=<Version>)(.+?)(?=</Version>)", version);
        DotNetCoreBuild($"./src/{solutionName}.sln", new DotNetCoreBuildSettings
        {
           Configuration = configuration,
        });
    });

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
    {
        var settings = new DotNetCoreTestSettings
        {
            Configuration = configuration,
        };

        if (isRunningOnAppVeyor)
        {
            //settings.TestAdapterPath = Directory(".");
            //settings.Logger = "Appveyor";
            // TODO Finds a way to exclude tests not allowed to run on appveyor
            // Not used in current code
            //settings.Where = "cat != ExcludeFromAppVeyor";
        }

        DotNetCoreTest($"./src/{solutionName}.Tests/", settings);
    });

Task("Update-AppVeyor-Build-Number")
    .WithCriteria(() => isRunningOnAppVeyor)
    .Does(() =>
    {
        var num = AppVeyor.Environment.Build.Number;
        AppVeyor.UpdateBuildVersion($"{version}-{num}");
    });

// Targets
// ----------------------------------------

Task("Default")
    .IsDependentOn("Run-Unit-Tests");

Task("AppVeyor")
    .IsDependentOn("Run-Unit-Tests")
    .IsDependentOn("Update-AppVeyor-Build-Number");