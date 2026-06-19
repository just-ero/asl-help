using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using AslHelp.Logging;
using AslHelp.Reflection;

using NUnit.Framework;

namespace AslHelp.Tests;

[TestFixture]
public class OdsSinkTests
{
    private sealed class CapturingListener : TraceListener
    {
        public List<string> Lines { get; } = [];

        public override void Write(string? message)
        {
            if (message is not null)
            {
                Lines.Add(message);
            }
        }

        public override void WriteLine(string? message)
        {
            if (message is not null)
            {
                Lines.Add(message);
            }
        }
    }

    private static string Marker(in LogEvent e, int indentLevel)
    {
        return "X:" + e.Message;
    }

    [Test]
    public void Emit_WhenCalled_WritesFormattedTextToTrace()
    {
        CapturingListener listener = new();
        Trace.Listeners.Add(listener);
        try
        {
            OdsSink sink = new(LogLevel.Trace);
            sink.Emit(new LogEvent(LogLevel.Information, "hello", "m", "f", 1), 0);
        }
        finally
        {
            Trace.Listeners.Remove(listener);
        }

        Assert.That(listener.Lines, Has.Some.Contains("hello"));
    }

    [Test]
    public void Emit_WithCustomFormatter_UsesIt()
    {
        CapturingListener listener = new();
        Trace.Listeners.Add(listener);
        try
        {
            OdsSink sink = new(LogLevel.Trace, Marker);
            sink.Emit(new LogEvent(LogLevel.Information, "hello", "m", "f", 1), 0);
        }
        finally
        {
            Trace.Listeners.Remove(listener);
        }

        Assert.That(listener.Lines, Has.Some.EqualTo("X:hello"));
    }
}

[TestFixture]
public class FileSinkTests
{
    private string _path = "";

    [SetUp]
    public void SetUp()
    {
        _path = Path.Combine(Path.GetTempPath(), $"aslhelp-{Guid.NewGuid():N}.log");
    }

    [TearDown]
    public void TearDown()
    {
        if (File.Exists(_path))
        {
            File.Delete(_path);
        }
    }

    private static string Marker(in LogEvent e, int indentLevel)
    {
        return "X:" + e.Message;
    }

    [Test]
    public void Dispose_AfterEmit_FlushesMessageToFile()
    {
        FileSink sink = new(_path, LogLevel.Trace);
        sink.Emit(new LogEvent(LogLevel.Information, "hello", "m", "f", 1), 0);
        sink.Dispose();

        Assert.That(File.ReadAllText(_path), Does.Contain("hello"));
    }

    [Test]
    public void Emit_WithCustomFormatter_UsesIt()
    {
        FileSink sink = new(_path, LogLevel.Trace, formatter: Marker);
        sink.Emit(new LogEvent(LogLevel.Information, "hello", "m", "f", 1), 0);
        sink.Dispose();

        Assert.That(File.ReadAllText(_path), Does.Contain("X:hello"));
    }

    [Test]
    public void Constructor_OverExistingFile_SeedsLineCount()
    {
        File.WriteAllLines(_path, ["one", "two", "three"]);

        FileSink sink = new(_path, LogLevel.Trace);
        try
        {
            Assert.That(sink.GetFieldValue<int>("_currentLines"), Is.EqualTo(3));
        }
        finally
        {
            sink.Dispose();
        }
    }

    [Test]
    public void Emit_WhenExceedingMaximumLines_RotatesOldestAway()
    {
        FileSink sink = new(_path, LogLevel.Trace, maximumLines: 4, linesToErase: 2);
        for (int i = 1; i <= 10; i++)
        {
            sink.Emit(new LogEvent(LogLevel.Information, $"line-{i:00}", "m", "f", 1), 0);
        }

        sink.Dispose();

        string[] lines = File.ReadAllLines(_path);
        string text = string.Join("\n", lines);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(lines.Length, Is.LessThanOrEqualTo(4));
            Assert.That(text, Does.Contain("line-10"));
            Assert.That(text, Does.Not.Contain("line-01"));
        }
    }

    [TestCase(0, 1)]
    [TestCase(4, 0)]
    [TestCase(2, 4)]
    public void Constructor_WithInvalidArguments_Throws(int maximumLines, int linesToErase)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new FileSink(_path, LogLevel.Trace, maximumLines, linesToErase));
    }

    [Test]
    public void Emit_AfterDispose_Throws()
    {
        FileSink sink = new(_path, LogLevel.Trace);
        sink.Dispose();

        Assert.Throws<ObjectDisposedException>(
            () => sink.Emit(new LogEvent(LogLevel.Information, "late", "m", "f", 1), 0));
    }

    [Test]
    public void Dispose_CalledTwice_DoesNotThrow()
    {
        FileSink sink = new(_path, LogLevel.Trace);
        sink.Dispose();

        Assert.DoesNotThrow(sink.Dispose);
    }
}
