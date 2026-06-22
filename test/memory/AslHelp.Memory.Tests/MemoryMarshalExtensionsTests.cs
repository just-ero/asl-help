using System;
using System.Runtime.InteropServices;

using AslHelp.Memory.Win32;

using NUnit.Framework;

namespace AslHelp.Memory.Tests;

[TestFixture]
public unsafe class MemoryMarshalExtensionsTests
{
    // ---- ReadOnlySpan<char> ----

    [Test]
    public void CreateStringFromNullTerminated_CharSpan_WithoutTerminator_ReturnsWholeSpan()
    {
        ReadOnlySpan<char> chars = "hello";

        string result = MemoryMarshal.CreateStringFromNullTerminated(chars);

        Assert.That(result, Is.EqualTo("hello"));
    }

    [Test]
    public void CreateStringFromNullTerminated_CharSpan_WithTerminator_ReturnsPrefix()
    {
        ReadOnlySpan<char> chars = "hello\0world";

        string result = MemoryMarshal.CreateStringFromNullTerminated(chars);

        Assert.That(result, Is.EqualTo("hello"));
    }

    [Test]
    public void CreateStringFromNullTerminated_CharSpan_LeadingTerminator_ReturnsEmpty()
    {
        ReadOnlySpan<char> chars = "\0hello";

        string result = MemoryMarshal.CreateStringFromNullTerminated(chars);

        Assert.That(result, Is.EqualTo(""));
    }

    [Test]
    public void CreateStringFromNullTerminated_CharSpan_Empty_ReturnsEmpty()
    {
        ReadOnlySpan<char> chars = [];

        string result = MemoryMarshal.CreateStringFromNullTerminated(chars);

        Assert.That(result, Is.EqualTo(""));
    }

    // ---- char*, int maxLength ----

    [Test]
    public void CreateStringFromNullTerminated_CharPointer_WithTerminator_ReturnsPrefix()
    {
        ReadOnlySpan<char> chars = "hi\0xx";
        fixed (char* pChars = chars)
        {
            string result = MemoryMarshal.CreateStringFromNullTerminated(pChars, chars.Length);

            Assert.That(result, Is.EqualTo("hi"));
        }
    }

    [Test]
    public void CreateStringFromNullTerminated_CharPointer_MaxLengthBoundsRead_StopsBeforeTerminator()
    {
        ReadOnlySpan<char> chars = "hello\0";
        fixed (char* pChars = chars)
        {
            string result = MemoryMarshal.CreateStringFromNullTerminated(pChars, 3);

            Assert.That(result, Is.EqualTo("hel"));
        }
    }

    [Test]
    public void CreateStringFromNullTerminated_CharPointer_WithoutTerminator_ReturnsUpToMaxLength()
    {
        ReadOnlySpan<char> chars = "hello";
        fixed (char* pChars = chars)
        {
            string result = MemoryMarshal.CreateStringFromNullTerminated(pChars, chars.Length);

            Assert.That(result, Is.EqualTo("hello"));
        }
    }

    // ---- ReadOnlySpan<byte> ----

    [Test]
    public void CreateStringFromNullTerminated_ByteSpan_WithoutTerminator_ReturnsWholeSpan()
    {
        var bytes = "hello"u8;

        string result = MemoryMarshal.CreateStringFromNullTerminated(bytes);

        Assert.That(result, Is.EqualTo("hello"));
    }

    [Test]
    public void CreateStringFromNullTerminated_ByteSpan_WithTerminator_ReturnsPrefix()
    {
        var bytes = "hello\0world"u8;

        string result = MemoryMarshal.CreateStringFromNullTerminated(bytes);

        Assert.That(result, Is.EqualTo("hello"));
    }

    [Test]
    public void CreateStringFromNullTerminated_ByteSpan_LeadingTerminator_ReturnsEmpty()
    {
        var bytes = "\0hello"u8;

        string result = MemoryMarshal.CreateStringFromNullTerminated(bytes);

        Assert.That(result, Is.EqualTo(""));
    }

    [Test]
    public void CreateStringFromNullTerminated_ByteSpan_Empty_ReturnsEmpty()
    {
        var bytes = ReadOnlySpan<byte>.Empty;

        string result = MemoryMarshal.CreateStringFromNullTerminated(bytes);

        Assert.That(result, Is.EqualTo(""));
    }

    [Test]
    public void CreateStringFromNullTerminated_ByteSpan_MultiByteUtf8_DecodesBeforeTerminator()
    {
        var prefix = "日本語"u8;
        ReadOnlySpan<byte> bytes = [.. prefix, 0, .. "x"u8];

        string result = MemoryMarshal.CreateStringFromNullTerminated(bytes);

        Assert.That(result, Is.EqualTo("日本語"));
    }

    // ---- byte*, int maxLength ----

    [Test]
    public void CreateStringFromNullTerminated_BytePointer_WithTerminator_ReturnsPrefix()
    {
        var bytes = "hi\0xx"u8;
        fixed (byte* pBytes = bytes)
        {
            string result = MemoryMarshal.CreateStringFromNullTerminated(pBytes, bytes.Length);

            Assert.That(result, Is.EqualTo("hi"));
        }
    }

    [Test]
    public void CreateStringFromNullTerminated_BytePointer_MaxLengthBoundsRead_StopsBeforeTerminator()
    {
        var bytes = "hello\0"u8;
        fixed (byte* pBytes = bytes)
        {
            string result = MemoryMarshal.CreateStringFromNullTerminated(pBytes, 3);

            Assert.That(result, Is.EqualTo("hel"));
        }
    }

    [Test]
    public void CreateStringFromNullTerminated_BytePointer_MultiByteUtf8_WithoutTerminator_DecodesAll()
    {
        var bytes = "日本語"u8;
        fixed (byte* pBytes = bytes)
        {
            string result = MemoryMarshal.CreateStringFromNullTerminated(pBytes, bytes.Length);

            Assert.That(result, Is.EqualTo("日本語"));
        }
    }
}
