#addin nuget:?package=Cake.Npm&version=0.16.0
var task = Argument("task", "Build");

///////////////////////////////////////////////////////////////////////////////
// .Net Core Build
///////////////////////////////////////////////////////////////////////////////
Task("Build project")
    .Does(() =>
{
    DotNetCoreBuild("./src/Api");
    DotNetCoreBuild("./tests/Api");
});

Task("Run Tests")
    .IsDependentOn("Build project")
    .Does(() =>
{
    var fullPath = "./tests/Api/Test.Api.csproj";
    DotNetCoreTest(
        fullPath,
        new DotNetCoreTestSettings()
        {
            NoRestore = true,
            NoBuild = true,
        }
    );
});

Task("Publish Api project")
    .IsDependentOn("Build project")
    .Does(() =>
{
    var settings = new DotNetCorePublishSettings
     {
         Configuration = "Release",
         OutputDirectory = "./dist/"
     };

     DotNetCorePublish("./src/Api", settings);
});

///////////////////////////////////////////////////////////////////////////////
// NPM & vue-cli-service
///////////////////////////////////////////////////////////////////////////////
Task("Install Client dependencies")
    .Does(() => 
{
    var settings = new NpmInstallSettings 
    { 
        WorkingDirectory = "src/Client/" 
    };
    NpmInstall(settings);
});

Task("Publish Client")
    .IsDependentOn("Install Client dependencies")
    .Does(() => 
{
    var settings = new NpmRunScriptSettings 
    {
        WorkingDirectory = "src/Client/", 
        ScriptName = "build"
    };
    NpmRunScript(settings);
});

Task("Copy Published client into /wwwroot")
    .IsDependentOn("Publish Client")
    .Does(() =>
{
    CopyDirectory("src/Client/dist", "./dist/wwwroot");
});

///////////////////////////////////////////////////////////////////////////////

Task("Build")
    .IsDependentOn("Run Tests")
    .IsDependentOn("Install Client dependencies");

Task("Publish")
    .IsDependentOn("Publish Api project")
    .IsDependentOn("Publish Client")
    .IsDependentOn("Copy Published client into /wwwroot");

RunTarget(task);