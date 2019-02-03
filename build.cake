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

Task("Run Test")
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
    .IsDependentOn("Run Test")
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
    .IsDependentOn("Build project")
    .IsDependentOn("Run Test")
    .IsDependentOn("Install Client dependencies");

Task("Publish")
    .IsDependentOn("Build project")
    .IsDependentOn("Publish Api project")
    .IsDependentOn("Publish Client")
    .IsDependentOn("Copy Published client into /wwwroot");

Task("Api Publish")
    .IsDependentOn("Build project")
    .IsDependentOn("Publish Api project");

RunTarget(task);