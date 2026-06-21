using AslHelp.WikiGen;
using System.CommandLine;
using System.IO;
using System.Linq;

var outputArg = new Argument<DirectoryInfo>(
    "--output")
{
    Description = "The wiki directory to write pages into."
};

var nameOption = new Option<string>(
    "--name", "-n")
{
    Description = "The assembly/display name (e.g. AslHelp).",
    Required = true,
};

var metadataOption = new Option<DirectoryInfo>(
    "--metadata", "-m")
{
    Description = "The docfx metadata (.yml) directory.",
    Required = true,
}.AcceptExistingOnly();

var assemblyOption = new Option<FileInfo[]>(
    "--assembly", "-a")
{
    Description = "Compiled assemblies (.dll) for extension collection; repeatable.",
    Required = true,
    AllowMultipleArgumentsPerToken = true,
};

var xmlOption = new Option<FileInfo[]>(
    "--xml", "-x")
{
    Description = "InheritDoc-expanded XML doc files; repeatable.",
    Required = true,
    AllowMultipleArgumentsPerToken = true,
};

var repoOption = new Option<DirectoryInfo>(
    "--repo", "-r")
{
    DefaultValueFactory = _ => new DirectoryInfo("."),
    Description = "Repository root (resolves source files to skip attribute lines in source links).",
};

var sidebarOption = new Option<SidebarStyle>(
    "--sidebar")
{
    DefaultValueFactory = _ => SidebarStyle.Collapsible,
    Description = "Sidebar layout.",
};

var root = new RootCommand(
    "Generates the asl-help GitHub wiki from docfx metadata + the compiled assembly.")
{
    outputArg,
    nameOption,
    metadataOption,
    assemblyOption,
    xmlOption,
    repoOption,
    sidebarOption,
};

root.SetAction(parseResult =>
{
    var options = new GeneratorOptions(
        parseResult.GetValue(outputArg)!.FullName,
        parseResult.GetRequiredValue(nameOption),
        parseResult.GetRequiredValue(metadataOption).FullName,
        [.. parseResult.GetRequiredValue(assemblyOption).Select(a => a.FullName)],
        [.. parseResult.GetRequiredValue(xmlOption).Select(x => x.FullName)],
        parseResult.GetValue(repoOption)!.FullName,
        parseResult.GetValue(sidebarOption));
    new WikiGenerator(options).Run();
});

return root.Parse(args).Invoke();
