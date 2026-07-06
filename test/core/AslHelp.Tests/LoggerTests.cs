using System;
using System.Collections.Generic;

using AslHelp.Logging;

using NUnit.Framework;

namespace AslHelp.Tests;

internal sealed class FakeSink : ILogSink
{
    public LogLevel MinimumLevel { get; init; } = LogLevel.Trace;

    public List<(LogEvent Event, int Indent)> Emitted { get; } = [];

    public void Emit(in LogEvent e, int indentLevel)
    {
        Emitted.Add((e, indentLevel));
    }
}

internal sealed class FakeDisposableSink : ILogSink, IDisposable
{
    public LogLevel MinimumLevel => LogLevel.Trace;

    public bool Disposed { get; private set; }

    public void Emit(in LogEvent e, int indentLevel) { }

    public void Dispose()
    {
        Disposed = true;
    }
}

[TestFixture]
public class LoggerTests
{
    [TestCase(LogLevel.Trace, false)]
    [TestCase(LogLevel.Warning, true)]
    [TestCase(LogLevel.Critical, true)]
    [TestCase(LogLevel.Off, false)]
    public void IsEnabled_ForLevel_ReflectsSinkMinimum(LogLevel level, bool expected)
    {
        Logger logger = new();
        logger.Sinks.Add(new FakeSink { MinimumLevel = LogLevel.Warning });

        Assert.That(logger.IsEnabled(level), Is.EqualTo(expected));
    }

    [Test]
    public void Log_WhenLevelBelowSinkMinimum_DoesNotEmit()
    {
        Logger logger = new();
        FakeSink sink = new() { MinimumLevel = LogLevel.Warning };
        logger.Sinks.Add(sink);

        logger.Log(new LogEvent(LogLevel.Trace, "low", "m", "f", 1));

        Assert.That(sink.Emitted, Is.Empty);
    }

    [Test]
    public void Log_WhenLevelMeetsSinkMinimum_Emits()
    {
        Logger logger = new();
        FakeSink sink = new() { MinimumLevel = LogLevel.Warning };
        logger.Sinks.Add(sink);

        logger.Log(new LogEvent(LogLevel.Error, "high", "m", "f", 1));

        Assert.That(sink.Emitted[0].Event.Message, Is.EqualTo("high"));
    }

    [Test]
    public void Log_WhenLevelOff_DoesNotEmit()
    {
        Logger logger = new();
        FakeSink sink = new();
        logger.Sinks.Add(sink);

        logger.Log(new LogEvent(LogLevel.Off, "ignored", "m", "f", 1));

        Assert.That(sink.Emitted, Is.Empty);
    }

    [Test]
    public void Log_WithMultipleSinks_RoutesByEachSinkMinimum()
    {
        Logger logger = new();
        FakeSink trace = new() { MinimumLevel = LogLevel.Trace };
        FakeSink error = new() { MinimumLevel = LogLevel.Error };
        logger.Sinks.Add(trace);
        logger.Sinks.Add(error);

        logger.Log(new LogEvent(LogLevel.Warning, "w", "m", "f", 1));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(trace.Emitted, Has.Count.EqualTo(1));
            Assert.That(error.Emitted, Is.Empty);
        }
    }

    [Test]
    public void BeginScope_WhenNested_IndentsAndRestores()
    {
        Logger logger = new();
        FakeSink sink = new() { MinimumLevel = LogLevel.Trace };
        logger.Sinks.Add(sink);

        using (logger.BeginScope(LogLevel.Trace))
        {
            logger.LogTrace("a");

            using (logger.BeginScope(LogLevel.Trace))
            {
                logger.LogTrace("b");
            }

            logger.LogTrace("c");
        }

        logger.LogTrace("d");

        Assert.That(
            sink.Emitted.ConvertAll(x => x.Indent),
            Is.EqualTo([1, 2, 1, 0]));
    }

    [Test]
    public void BeginScope_WhenScopeLevelBelowSinkMinimum_DoesNotIndent()
    {
        Logger logger = new();
        FakeSink sink = new() { MinimumLevel = LogLevel.Debug };
        logger.Sinks.Add(sink);

        // A Trace scope is below the Debug sink's threshold, so a Debug log inside it is not indented.
        using (logger.BeginScope(LogLevel.Trace))
        {
            logger.LogDebug("belowScope");
        }

        // An Information scope is at/above the threshold, so it does indent.
        using (logger.BeginScope(LogLevel.Information))
        {
            logger.LogDebug("aboveScope");
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(sink.Emitted[0].Indent, Is.EqualTo(0));
            Assert.That(sink.Emitted[1].Indent, Is.EqualTo(1));
        }
    }

    [Test]
    public void Dispose_WhenOwnsSinks_DisposesThem()
    {
        Logger logger = new();
        FakeDisposableSink sink = new();
        logger.Sinks.Add(sink);

        logger.Dispose();

        Assert.That(sink.Disposed, Is.True);
    }

    [Test]
    public void Dispose_CalledTwice_DoesNotThrow()
    {
        Logger logger = new();
        logger.Sinks.Add(new FakeDisposableSink());

        logger.Dispose();

        Assert.DoesNotThrow(logger.Dispose);
    }
}

[TestFixture]
public class LoggerExtensionsTests
{
    private static (Logger Logger, FakeSink Sink) NewLogger()
    {
        Logger logger = new();
        FakeSink sink = new() { MinimumLevel = LogLevel.Trace };
        logger.Sinks.Add(sink);

        return (logger, sink);
    }

    [Test]
    public void LogMethods_WhenCalled_EmitWithMatchingLevel()
    {
        var (logger, sink) = NewLogger();

        logger.LogTrace("t");
        logger.LogDebug("d");
        logger.LogInformation("i");
        logger.LogWarning("w");
        logger.LogError("e");
        logger.LogCritical("c");

        Assert.That(
            sink.Emitted.ConvertAll(x => x.Event.Level),
            Is.EqualTo(
            [
                LogLevel.Trace,
                LogLevel.Debug,
                LogLevel.Information,
                LogLevel.Warning,
                LogLevel.Error,
                LogLevel.Critical
            ]));
    }

    [Test]
    public void Log_WhenCalled_CapturesCallerMemberName()
    {
        var (logger, sink) = NewLogger();

        logger.LogInformation("hello");

        Assert.That(
            sink.Emitted[0].Event.CallerMemberName,
            Is.EqualTo(nameof(Log_WhenCalled_CapturesCallerMemberName)));
    }

    [Test]
    public void BeginScopeTrace_WithDebugSink_DoesNotIndentInnerDebugLog()
    {
        Logger logger = new();
        FakeSink sink = new() { MinimumLevel = LogLevel.Debug };
        logger.Sinks.Add(sink);

        // The Trace header is filtered out by the Debug sink, and because the scope's level is
        // below the sink minimum it adds no nesting, so the inner Debug log stays at depth 0.
        using (logger.BeginScopeTrace("header"))
        {
            logger.LogDebug("body");
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(sink.Emitted, Has.Count.EqualTo(1));
            Assert.That(sink.Emitted[0].Event.Message, Is.EqualTo("body"));
            Assert.That(sink.Emitted[0].Indent, Is.EqualTo(0));
        }
    }

    [Test]
    public void BeginScopeMethods_WhenCalled_LogHeaderThenIndent()
    {
        var (logger, sink) = NewLogger();

        using (logger.BeginScopeTrace("st"))
        {
            logger.LogTrace("inside trace");
        }

        using (logger.BeginScopeDebug("sd"))
        {
            logger.LogTrace("inside debug");
        }

        using (logger.BeginScopeInformation("si"))
        {
            logger.LogTrace("inside info");
        }

        using (logger.BeginScopeWarning("sw"))
        {
            logger.LogTrace("inside warn");
        }

        using (logger.BeginScopeError("se"))
        {
            logger.LogTrace("inside error");
        }

        using (logger.BeginScopeCritical("sc"))
        {
            logger.LogTrace("inside crit");
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(sink.Emitted, Has.Count.EqualTo(12));
            Assert.That(sink.Emitted[0].Event.Message, Is.EqualTo("st"));
            Assert.That(sink.Emitted[0].Indent, Is.EqualTo(0));
            Assert.That(sink.Emitted[1].Indent, Is.EqualTo(1));
        }
    }
}

[TestFixture]
public class LogFormatterTests
{
    [TestCase(LogLevel.Trace, "[trce]")]
    [TestCase(LogLevel.Debug, "[dbug]")]
    [TestCase(LogLevel.Information, "[info]")]
    [TestCase(LogLevel.Warning, "[warn]")]
    [TestCase(LogLevel.Error, "[fail]")]
    [TestCase(LogLevel.Critical, "[crit]")]
    [TestCase(LogLevel.Off, "[    ]")]
    public void Default_ForLevel_PrefixesLevelCode(LogLevel level, string expectedPrefix)
    {
        LogEvent e = new(level, "msg", "m", "f", 1);

        var formatted = LogFormatter.Default(in e, 0);

        Assert.That(formatted, Does.StartWith(expectedPrefix));
    }

    [Test]
    public void Default_WithIndent_PadsFourSpacesPerLevel()
    {
        LogEvent e = new(LogLevel.Information, "msg", "m", "f", 1);

        var formatted = LogFormatter.Default(in e, 2);

        // "[info] " (trailing space) + 8 indent spaces + message.
        Assert.That(formatted, Is.EqualTo("[info] " + new string(' ', 8) + "msg"));
    }
}
