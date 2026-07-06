using System;
using System.Collections.Generic;
using System.Reflection;

using AslHelp.LiveSplit.Asl.Attach;

using NUnit.Framework;

namespace AslHelp.LiveSplit.Tests;

[TestFixture]
public class ScriptResolverTests
{
    // Two distinct, real modules from different assemblies.
    private static readonly Module _moduleA = typeof(string).Module;
    private static readonly Module _moduleB = typeof(ScriptResolverTests).Module;

    private static (string Component, IReadOnlyDictionary<Module, string> Actions) Candidate(
        string component,
        Module module,
        string action)
    {
        return (component, new Dictionary<Module, string> { [module] = action });
    }

    [Test]
    public void TryMatch_ModuleOwnedByCandidate_ReturnsComponentAndAction()
    {
        var candidates = new[] { Candidate("compA", _moduleA, "startup") };

        var matched = ScriptResolver.TryMatch(_moduleA, candidates, out var component, out var action);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(matched, Is.True);
            Assert.That(component, Is.EqualTo("compA"));
            Assert.That(action, Is.EqualTo("startup"));
        }
    }

    [Test]
    public void TryMatch_ModuleNotOwned_ReturnsFalseWithDefaults()
    {
        var candidates = new[] { Candidate("compA", _moduleA, "startup") };

        var matched = ScriptResolver.TryMatch(_moduleB, candidates, out var component, out var action);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(matched, Is.False);
            Assert.That(component, Is.Null);
            Assert.That(action, Is.Null);
        }
    }

    [Test]
    public void TryMatch_NoCandidates_ReturnsFalse()
    {
        var candidates = Array.Empty<(string, IReadOnlyDictionary<Module, string>)>();

        Assert.That(ScriptResolver.TryMatch(_moduleA, candidates, out _, out _), Is.False);
    }

    [Test]
    public void TryMatch_MultipleOwners_ReturnsFirstMatch()
    {
        var candidates = new[]
        {
            Candidate("first", _moduleA, "startup"),
            Candidate("second", _moduleA, "update"),
        };

        ScriptResolver.TryMatch(_moduleA, candidates, out var component, out var action);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(component, Is.EqualTo("first"));
            Assert.That(action, Is.EqualTo("startup"));
        }
    }

    [Test]
    public void TryMatch_FirstCandidateLacksModule_FindsLaterOwner()
    {
        var candidates = new[]
        {
            Candidate("first", _moduleB, "update"),
            Candidate("second", _moduleA, "startup"),
        };

        var matched = ScriptResolver.TryMatch(_moduleA, candidates, out var component, out var action);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(matched, Is.True);
            Assert.That(component, Is.EqualTo("second"));
            Assert.That(action, Is.EqualTo("startup"));
        }
    }
}
