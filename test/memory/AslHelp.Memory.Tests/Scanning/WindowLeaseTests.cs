using System;
using System.Buffers;
using System.Linq;

using AslHelp.Memory.Scanning;

using NUnit.Framework;

namespace AslHelp.Memory.Tests.Scanning;

[TestFixture]
public class WindowLeaseTests
{
    [Test]
    public void Empty_HasNoBytesAndZeroStart()
    {
        var lease = WindowLease.Empty;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(lease.Bytes.IsEmpty, Is.True);
            Assert.That(lease.Start, Is.EqualTo((nint)0));
        }
    }

    [Test]
    public void Constructor_ExposesStartAndBytes()
    {
        byte[] data = [1, 2, 3, 4];
        byte[] expected = [2, 3];

        WindowLease lease = new(data.AsMemory(1, 2), 0x2000, rented: null);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(lease.Start, Is.EqualTo((nint)0x2000));
            Assert.That(lease.Bytes.ToArray(), Is.EqualTo(expected));
        }
    }

    [Test]
    public void Dispose_WithoutRentedBuffer_DoesNotThrow()
    {
        WindowLease lease = new(new byte[] { 1 }.AsMemory(), 0x10, rented: null);

        Assert.DoesNotThrow(lease.Dispose);
    }

    [Test]
    public void Dispose_WithRentedBuffer_ReturnsItCleanly()
    {
        var rented = ArrayPool<byte>.Shared.Rent(8);

        WindowLease lease = new(rented.AsMemory(0, 8), 0x10, rented);

        Assert.DoesNotThrow(lease.Dispose);
    }

    [Test]
    [NonParallelizable]
    public void Dispose_CalledTwice_ReturnsBufferOnlyOnce()
    {
        const int Size = 4096;
        var rented = ArrayPool<byte>.Shared.Rent(Size);

        WindowLease lease = new(rented.AsMemory(0, Size), 0x10, rented);
        lease.Dispose();
        lease.Dispose();

        // A double return would seat the same instance in the pool twice, so a batch of rents
        // could hand it out more than once. Count how many of our buffer the pool yields.
        var batch = new byte[16][];
        for (var i = 0; i < batch.Length; i++)
        {
            batch[i] = ArrayPool<byte>.Shared.Rent(Size);
        }

        var copies = batch.Count(b => ReferenceEquals(b, rented));

        foreach (var b in batch)
        {
            ArrayPool<byte>.Shared.Return(b);
        }

        Assert.That(copies, Is.EqualTo(1), "the pooled buffer must be returned exactly once");
    }
}
