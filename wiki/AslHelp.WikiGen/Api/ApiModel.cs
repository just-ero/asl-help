using System.Collections.Generic;

namespace AslHelp.WikiGen.Api;

/// <summary>
///     A cross-reference.
/// </summary>
/// <param name="Display">The display text.</param>
/// <param name="Ref">The target page. <see langword="null"/> = unlinked/external.</param>
internal sealed record ApiLink(string Display, string? Ref);

/// <summary>
///     A link to a definition's source on GitHub.
/// </summary>
internal sealed record ApiSource(string FileName, string Url);

/// <summary>
///     A method parameter.
/// </summary>
internal sealed record ApiParameter(string Name, ApiLink Type, string? Summary);

internal enum MemberGroup
{
    Constructors,
    Properties,
    Fields,
    Methods,
    Operators,
    Events
}

/// <summary>
///     A single documented member of a type.
/// </summary>
internal sealed record ApiMember(
    string Name,
    MemberGroup Group,
    string Signature,
    string? Summary,
    ApiLink? ValueType,
    IReadOnlyList<ApiParameter> Parameters,
    ApiLink? ReturnType,
    string? ReturnSummary,
    ApiSource? Source)
{
    /// <summary>
    ///     The unique page basename used in links/URLs (e.g. <c>Result-1.IsOk</c>).
    /// </summary>
    public string Ref { get; init; } = "";

    /// <summary>
    ///     The nested file path written to (e.g. <c>AslHelp/AslHelp/Result-1/Result-1.IsOk</c>).
    /// </summary>
    public string File { get; init; } = "";

    /// <summary>
    ///     For an enum field, its constant value (e.g. <c>1</c>); otherwise <see langword="null"/>.
    /// </summary>
    public string? Value { get; init; }
}

/// <summary>
///     A documented type, with its members already grouped.
/// </summary>
internal sealed record ApiType(
    string Uid,
    string Ref,
    string File,
    string Display,
    string Kind,
    string Signature,
    string Namespace,
    string Assembly,
    string? Summary,
    string? Remarks,
    IReadOnlyList<ApiLink> Inheritance,
    IReadOnlyList<ApiLink> Implements,
    ApiSource? Source,
    IReadOnlyList<ApiMember> Members);
