using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace AslHelp.LiveSplit.Asl.Attach;

/// <summary>
///     Matches a calling script module against candidate components' action-module maps.
/// </summary>
internal static class ScriptResolver
{
    /// <summary>
    ///     Attempts to find the candidate component owning <paramref name="callingModule"/>.
    /// </summary>
    /// <typeparam name="T">The component type.</typeparam>
    /// <param name="callingModule">The module of the calling compiled script.</param>
    /// <param name="candidates">Candidate components with their action-module maps.</param>
    /// <param name="component">The matched component, if any.</param>
    /// <param name="action">The name of the action owning the module, if any.</param>
    /// <returns>
    ///     <see langword="true"/> if a candidate owns the module; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool TryMatch<T>(
        Module callingModule,
        IEnumerable<(T Component, IReadOnlyDictionary<Module, string> Actions)> candidates,
        [MaybeNullWhen(false)] out T component,
        [NotNullWhen(true)] out string? action)
    {
        foreach ((T candidate, IReadOnlyDictionary<Module, string> actions) in candidates)
        {
            if (actions.TryGetValue(callingModule, out action))
            {
                component = candidate;
                return true;
            }
        }

        component = default;
        action = null;
        return false;
    }
}
