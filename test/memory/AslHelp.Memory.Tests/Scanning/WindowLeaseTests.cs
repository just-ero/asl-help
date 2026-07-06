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
