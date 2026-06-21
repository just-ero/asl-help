using System.Collections.Generic;

namespace AslHelp.WikiGen;

/// <summary>
///     How the generated <c>_Sidebar.md</c> is laid out.
/// </summary>
internal enum SidebarStyle
{
    /// <summary>
    ///     Always-expanded indented list.
    /// </summary>
    Nested,

    /// <summary>
    ///     The whole API tree tucked behind one collapsed <c>&lt;details&gt;</c>, so usage docs lead.
    /// </summary>
    Collapsible,
}

/// <summary>
///     Inputs for a single generation run.
/// </summary>
internal sealed record GeneratorOptions(
    string OutputDir,
    string AssemblyName,
    string MetadataDir,
    IReadOnlyList<string> AssemblyPaths,
    IReadOnlyList<string> XmlPaths,
    string RepoDir,
    SidebarStyle Sidebar);
