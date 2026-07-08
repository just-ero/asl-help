# Memory Milestone 1 Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Give `AslHelp.Memory` a complete, RPM-backed `IProcessMemory` — read, write, typed reads/deref, module enumeration, page enumeration, tiered symbol lookup — plus an ASL-only JSON config system, all shaped so a native (injected) backend can slot in later without changing consumers.

**Architecture:** One `IProcessMemory` interface, one concrete `RemoteProcessMemory` implementation over the existing Win32 P/Invoke layer (`ReadProcessMemory`/`WriteProcessMemory`/`VirtualQueryEx`/ToolHelp32/DbgHelp). Typed reads, deref chains, and scanning are extension methods over the interface so they work for any backend. Symbol lookup is tiered: parse the PE export table remotely first (cheap, no symbol files), fall back to DbgHelp/PDB. A plain config data model lives in `AslHelp.Memory`; its JSON loader is invoked explicitly at ASL init.

**Tech Stack:** C# 14 / .NET (`netstandard2.0` for the library, multi-targeted `net4.8.1;net10.0` tests), NUnit 4, `Result`/`Result<T>` railway types, `System.Text.Json`.

## Global Constraints

- Target framework of `AslHelp.Memory`: `netstandard2.0` (copy verbatim from `AslHelp.Memory.csproj`). Do not raise it. This forbids newer BCL APIs; use `System.Memory` (already referenced) for `Span<T>`.
- `TreatWarningsAsErrors` is on with `AnalysisLevel=latest-all`. Every file must be warning-clean, including XML doc comments (`GenerateDocumentationFile=true`) on public members.
- `Nullable` is enabled. `AllowUnsafeBlocks` is on.
- Error handling uses the existing `Result` / `Result<T>` types. Public seams return `Result`/`Result<T>`; do not throw across them. Construct with `Result.Ok()`, `Result.Ok(value)`, `Result.Err("message")`, `Result.Err<T>("message")` (these are static members exposed through `ResultExtensions`).
- `PInvoke` and the `Win32` types are `internal`; `InternalsVisibleTo $(MSBuildProjectName).Tests` is already configured, so tests can see internals.
- Tests must run against the **current process** (self-inspection) so CI needs no game — mirror the existing `FakeMemoryReader` / scanner test style. Never require a specific external game to be running.
- `var` style is used throughout the codebase (see commit `61a306f`). Match it.
- Naming: engine/backend types read `RemoteProcessMemory` (RPM) and, later, `NativeProcessMemory`. The interface stays `IProcessMemory`.

---

## File Structure

New/changed files in `src/memory/AslHelp.Memory/` unless noted:

| Path | Responsibility |
|------|----------------|
| `IProcessMemory.cs` (modify) | The full contract: read, write, modules, pages, symbols, bitness. |
| `Module.cs` (create) | `readonly record struct Module` — name, base, size, path. |
| `Symbol.cs` (create) | `readonly record struct Symbol` — name, address, module base. |
| `RemoteProcessMemory.cs` (create) | RPM implementation: open process, read/write, bitness. |
| `RemoteProcessMemory.Pages.cs` (create) | `GetMemoryPages` via `VirtualQuery` loop. |
| `RemoteProcessMemory.Modules.cs` (create) | Module enumeration via ToolHelp32. |
| `RemoteProcessMemory.Symbols.cs` (create) | Tiered symbol lookup (exports → DbgHelp). |
| `ProcessMemoryExtensions.cs` (create) | Typed `Read<T>`, `Write<T>`, deref chains over `IProcessMemory`. |
| `Pe/PeExportReader.cs` (create) | Parse a module's PE export table via RPM. |
| `Scanning/Scan.cs` (modify) | Add module/page-scoped convenience entry points. |
| `Config/GameConfig.cs` (create) | Config data model (module overrides, symbols, signatures). |
| `Config/GameConfigLoader.cs` (create) | Explicit JSON loader + validation. |
| `Win32/pinvoke/PInvoke.PsApi.cs` (modify, if needed) | Ensure `GetModuleInformation`/`EnumProcessModules` exist for symbol sizing. |

Tests in `test/memory/AslHelp.Memory.Tests/`:

| Path | Responsibility |
|------|----------------|
| `FakeMemoryReader.cs` (modify) | Implement the merged `IProcessMemory`, not the deleted `IMemoryReader`. |
| `RemoteProcessMemoryTests.cs` (create) | Self-inspection: open own process, read/write, pages, modules. |
| `ProcessMemoryExtensionsTests.cs` (create) | Typed reads/deref against a `FakeMemoryReader`. |
| `Pe/PeExportReaderTests.cs` (create) | Resolve a known export (`kernel32!Sleep`) in the current process. |
| `Config/GameConfigLoaderTests.cs` (create) | Round-trip + validation of the JSON config. |

---

## Task 1: Stabilize the tree (finish the IMemoryReader → IProcessMemory merge)

The working tree is mid-refactor: `src/memory/AslHelp.Memory/IMemoryReader.cs` was deleted and its `Read` folded into `IProcessMemory`, but `test/.../FakeMemoryReader.cs` still names `IMemoryReader`, so nothing builds. Fix that first so every later task starts from green.

**Files:**
- Modify: `test/memory/AslHelp.Memory.Tests/FakeMemoryReader.cs`
- Verify: `src/memory/AslHelp.Memory/IProcessMemory.cs` (already merged in the working tree)

**Interfaces:**
- Consumes: `IProcessMemory { Result Read(nint, Span<byte>); IEnumerable<MemoryPage> GetMemoryPages(nint start, nint size); }`
- Produces: a compiling test project; `FakeMemoryReader : IProcessMemory` and `FailingMemoryReader : IProcessMemory` usable by later tasks' tests.

- [ ] **Step 1: Confirm the build is currently red**

Run: `dotnet build test/memory/AslHelp.Memory.Tests/AslHelp.Memory.Tests.csproj --nologo`
Expected: FAIL with `error CS0246: The type or namespace name 'IMemoryReader' could not be found` (from `FakeMemoryReader.cs`).

- [ ] **Step 2: Update the test fakes to implement `IProcessMemory`**

Replace the whole body of `test/memory/AslHelp.Memory.Tests/FakeMemoryReader.cs` with:

```csharp
using System;
using System.Collections.Generic;

using AslHelp.Memory;

namespace AslHelp.Memory.Tests;

/// <summary>
///     An <see cref="IProcessMemory"/> backed by a byte array whose first byte sits at a chosen base
///     address. A read fully inside the backing range succeeds; anything else fails. Page
///     enumeration is not modeled and yields nothing.
/// </summary>
internal sealed class FakeMemoryReader : IProcessMemory
{
    private readonly nint _base;
    private readonly byte[] _data;

    public FakeMemoryReader(nint baseAddress, byte[] data)
    {
        _base = baseAddress;
        _data = data;
    }

    /// <summary>
    ///     Gets the number of times <see cref="Read"/> has been called.
    /// </summary>
    public int Reads { get; private set; }

    public Result Read(nint address, Span<byte> buffer)
    {
        Reads++;

        var offset = (long)address - _base;
        if (offset < 0 || offset + buffer.Length > _data.Length)
        {
            return Result.Err($"Unreadable [0x{(long)address:X}, +0x{buffer.Length:X}).");
        }

        _data.AsSpan((int)offset, buffer.Length).CopyTo(buffer);
        return Result.Ok();
    }

    public IEnumerable<MemoryPage> GetMemoryPages(nint start, nint size)
    {
        yield break;
    }
}

/// <summary>
///     An <see cref="IProcessMemory"/> whose every read fails, simulating freed or protected pages.
/// </summary>
internal sealed class FailingMemoryReader : IProcessMemory
{
    public Result Read(nint address, Span<byte> buffer)
    {
        return Result.Err("unreadable");
    }

    public IEnumerable<MemoryPage> GetMemoryPages(nint start, nint size)
    {
        yield break;
    }
}
```

- [ ] **Step 3: Build and run the existing suite to confirm green**

Run: `dotnet test test/memory/AslHelp.Memory.Tests/AslHelp.Memory.Tests.csproj --nologo`
Expected: PASS (build succeeds; all pre-existing scanner/page tests pass).

- [ ] **Step 4: Commit**

```bash
git add -A
git commit -m "fix: finish IMemoryReader→IProcessMemory merge in test fakes"
```

---

## Task 2: Module and Symbol value types

Introduce the two small record structs later tasks return, before wiring any enumeration.

**Files:**
- Create: `src/memory/AslHelp.Memory/Module.cs`
- Create: `src/memory/AslHelp.Memory/Symbol.cs`
- Test: `test/memory/AslHelp.Memory.Tests/ModuleTests.cs`

**Interfaces:**
- Produces:
  - `readonly record struct Module(string Name, nint Base, int Size, string Path)` with `nint End => Base + Size;`
  - `readonly record struct Symbol(string Name, nint Address, nint ModuleBase)`

- [ ] **Step 1: Write the failing test**

Create `test/memory/AslHelp.Memory.Tests/ModuleTests.cs`:

```csharp
using AslHelp.Memory;

using NUnit.Framework;

namespace AslHelp.Memory.Tests;

[TestFixture]
public sealed class ModuleTests
{
    [Test]
    public void End_IsBasePlusSize()
    {
        var module = new Module("game.exe", 0x400000, 0x1000, @"C:\game\game.exe");

        Assert.That(module.End, Is.EqualTo((nint)0x401000));
    }
}
```

- [ ] **Step 2: Run it to verify it fails**

Run: `dotnet test test/memory/AslHelp.Memory.Tests/AslHelp.Memory.Tests.csproj --nologo --filter ModuleTests`
Expected: FAIL — `Module` does not exist (compile error).

- [ ] **Step 3: Create `Module.cs`**

```csharp
namespace AslHelp.Memory;

/// <summary>
///     Describes a module loaded in a process: its file name, load address, size, and full path.
/// </summary>
/// <param name="Name">The module file name, e.g. <c>UnityPlayer.dll</c>.</param>
/// <param name="Base">The module's load address in the owning process.</param>
/// <param name="Size">The module's size in memory, in bytes.</param>
/// <param name="Path">The module's full file-system path.</param>
public readonly record struct Module(string Name, nint Base, int Size, string Path)
{
    /// <summary>
    ///     Gets the address one past the last byte of the module, i.e. <c>Base + Size</c>.
    /// </summary>
    public nint End => Base + Size;
}
```

- [ ] **Step 4: Create `Symbol.cs`**

```csharp
namespace AslHelp.Memory;

/// <summary>
///     Describes a resolved native symbol: its name, absolute address in the process, and the base
///     of the module that exports or defines it.
/// </summary>
/// <param name="Name">The symbol name, e.g. <c>mono_get_root_domain</c>.</param>
/// <param name="Address">The absolute address of the symbol in the owning process.</param>
/// <param name="ModuleBase">The load address of the module the symbol belongs to.</param>
public readonly record struct Symbol(string Name, nint Address, nint ModuleBase);
```

- [ ] **Step 5: Run the test to verify it passes**

Run: `dotnet test test/memory/AslHelp.Memory.Tests/AslHelp.Memory.Tests.csproj --nologo --filter ModuleTests`
Expected: PASS.

- [ ] **Step 6: Commit**

```bash
git add src/memory/AslHelp.Memory/Module.cs src/memory/AslHelp.Memory/Symbol.cs test/memory/AslHelp.Memory.Tests/ModuleTests.cs
git commit -m "feat(memory): add Module and Symbol value types"
```

---

## Task 3: Extend `IProcessMemory` to the full contract

Grow the interface to everything the memory layer must expose, so the concrete backend (this milestone) and the native backend (later) implement one shape.

**Files:**
- Modify: `src/memory/AslHelp.Memory/IProcessMemory.cs`

**Interfaces:**
- Consumes: `Module`, `Symbol`, `MemoryPage`, `Result`.
- Produces the interface later tasks implement:

```csharp
public interface IProcessMemory
{
    bool Is64Bit { get; }
    int PointerSize { get; }               // 8 when Is64Bit, else 4
    Module MainModule { get; }

    Result Read(nint address, Span<byte> buffer);
    Result Write(nint address, ReadOnlySpan<byte> data);

    IEnumerable<MemoryPage> GetMemoryPages(nint start, nint size);
    IEnumerable<Module> GetModules();

    bool TryGetModule(string name, out Module module);
    Result<Symbol> GetSymbol(string moduleName, string symbolName);
}
```

- [ ] **Step 1: Replace the interface body**

Overwrite `src/memory/AslHelp.Memory/IProcessMemory.cs`:

```csharp
using System;
using System.Collections.Generic;

namespace AslHelp.Memory;

/// <summary>
///     Reads and writes a process's virtual address space and describes its modules, pages, and
///     symbols. Implementations may be remote (out-of-process, via the OS) or native (in-process,
///     inside an injected agent); consumers must not depend on which.
/// </summary>
public interface IProcessMemory
{
    /// <summary>
    ///     Gets whether the target process is 64-bit.
    /// </summary>
    bool Is64Bit { get; }

    /// <summary>
    ///     Gets the size of a pointer in the target process, in bytes: <c>8</c> when
    ///     <see cref="Is64Bit"/>, otherwise <c>4</c>.
    /// </summary>
    int PointerSize { get; }

    /// <summary>
    ///     Gets the process's main module (its executable image).
    /// </summary>
    Module MainModule { get; }

    /// <summary>
    ///     Reads bytes from <paramref name="address"/> into <paramref name="buffer"/>, filling it
    ///     completely.
    /// </summary>
    /// <param name="address">The address to read from.</param>
    /// <param name="buffer">The destination buffer; its length is the number of bytes to read.</param>
    /// <returns>
    ///     A successful <see cref="Result"/> when the whole buffer was read; otherwise, a failed
    ///     result carrying the error.
    /// </returns>
    Result Read(nint address, Span<byte> buffer);

    /// <summary>
    ///     Writes <paramref name="data"/> to <paramref name="address"/> in full.
    /// </summary>
    /// <param name="address">The address to write to.</param>
    /// <param name="data">The bytes to write.</param>
    /// <returns>
    ///     A successful <see cref="Result"/> when all bytes were written; otherwise, a failed result
    ///     carrying the error.
    /// </returns>
    Result Write(nint address, ReadOnlySpan<byte> data);

    /// <summary>
    ///     Enumerates the memory pages overlapping <c>[start, start + size)</c>, in ascending
    ///     address order.
    /// </summary>
    /// <param name="start">The inclusive start address of the range to walk.</param>
    /// <param name="size">The length of the range, in bytes.</param>
    /// <returns>The pages overlapping the range.</returns>
    IEnumerable<MemoryPage> GetMemoryPages(nint start, nint size);

    /// <summary>
    ///     Enumerates the modules loaded in the process.
    /// </summary>
    /// <returns>The loaded modules.</returns>
    IEnumerable<Module> GetModules();

    /// <summary>
    ///     Finds a loaded module by file name, case-insensitively.
    /// </summary>
    /// <param name="name">The module file name, e.g. <c>UnityPlayer.dll</c>.</param>
    /// <param name="module">The matching module when found; otherwise, <see langword="default"/>.</param>
    /// <returns><see langword="true"/> when a module was found.</returns>
    bool TryGetModule(string name, out Module module);

    /// <summary>
    ///     Resolves a native symbol by module and name.
    /// </summary>
    /// <param name="moduleName">The module file name to resolve the symbol in.</param>
    /// <param name="symbolName">The symbol name, e.g. <c>mono_get_root_domain</c>.</param>
    /// <returns>
    ///     The resolved <see cref="Symbol"/> on success; otherwise, a failed result.
    /// </returns>
    Result<Symbol> GetSymbol(string moduleName, string symbolName);
}
```

- [ ] **Step 2: Update the test fakes to satisfy the wider interface**

The `FakeMemoryReader`/`FailingMemoryReader` now miss members. Add the following members to **both** classes in `test/memory/AslHelp.Memory.Tests/FakeMemoryReader.cs` (they are read-focused fakes; the rest throw or return empty):

```csharp
    public bool Is64Bit => nint.Size == 8;
    public int PointerSize => nint.Size;
    public Module MainModule => default;

    public Result Write(nint address, System.ReadOnlySpan<byte> data)
    {
        return Result.Err("write not supported by fake");
    }

    public IEnumerable<Module> GetModules()
    {
        yield break;
    }

    public bool TryGetModule(string name, out Module module)
    {
        module = default;
        return false;
    }

    public Result<Symbol> GetSymbol(string moduleName, string symbolName)
    {
        return Result.Err<Symbol>("symbols not supported by fake");
    }
```

Add `using System.Collections.Generic;` if not present (it is).

- [ ] **Step 3: Build to verify the interface and fakes compile together**

Run: `dotnet build test/memory/AslHelp.Memory.Tests/AslHelp.Memory.Tests.csproj --nologo`
Expected: PASS (no errors; the existing scanner tests still compile — they only call `Read`).

- [ ] **Step 4: Run the full suite**

Run: `dotnet test test/memory/AslHelp.Memory.Tests/AslHelp.Memory.Tests.csproj --nologo`
Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add -A
git commit -m "feat(memory): widen IProcessMemory to full read/write/module/page/symbol contract"
```

---

## Task 4: `RemoteProcessMemory` core (open, read, write, bitness)

The RPM backend's constructor and byte-level primitives. Modules/pages/symbols come in later tasks as partials.

**Files:**
- Create: `src/memory/AslHelp.Memory/RemoteProcessMemory.cs`
- Test: `test/memory/AslHelp.Memory.Tests/RemoteProcessMemoryTests.cs`

**Interfaces:**
- Consumes: `PInvoke.OpenProcess`, `PInvoke.ReadProcessMemory`, `PInvoke.WriteProcessMemory`, `PInvoke.IsWow64Process`, `ProcessAccess`, `SafeProcessHandle`, `Module`.
- Produces:
  - `public sealed partial class RemoteProcessMemory : IProcessMemory, IDisposable`
  - `public static Result<RemoteProcessMemory> Open(int processId)`
  - Properties `Is64Bit`, `PointerSize`, `MainModule` (MainModule filled here as `default` and set in Task 6).

Note: `MainModule` needs module enumeration, which lands in Task 6. To keep Task 4 self-contained and testable, `MainModule` is a settable-once field initialized in `Open` via a private helper that Task 6 provides. For Task 4, `Open` sets `MainModule = default` and a `// TODO(Task 6)` marker; the property returns the backing field. Task 6 replaces the initialization. This keeps each task independently green.

- [ ] **Step 1: Write the failing test**

Create `test/memory/AslHelp.Memory.Tests/RemoteProcessMemoryTests.cs`:

```csharp
using System;
using System.Diagnostics;

using AslHelp.Memory;

using NUnit.Framework;

namespace AslHelp.Memory.Tests;

[TestFixture]
public sealed class RemoteProcessMemoryTests
{
    private static RemoteProcessMemory OpenSelf()
    {
        var result = RemoteProcessMemory.Open(Process.GetCurrentProcess().Id);
        Assert.That(result.IsOk, Is.True, () => $"Open failed: {result.Error?.Message}");
        return result.Value!;
    }

    [Test]
    public void PointerSize_MatchesCurrentProcess()
    {
        using var mem = OpenSelf();

        Assert.That(mem.PointerSize, Is.EqualTo(nint.Size));
        Assert.That(mem.Is64Bit, Is.EqualTo(nint.Size == 8));
    }

    [Test]
    public unsafe void Read_RoundTripsKnownBytes()
    {
        using var mem = OpenSelf();

        var source = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        fixed (byte* p = source)
        {
            var buffer = new byte[source.Length];
            var read = mem.Read((nint)p, buffer);

            Assert.That(read.IsOk, Is.True, () => read.Error?.Message);
            Assert.That(buffer, Is.EqualTo(source));
        }
    }

    [Test]
    public unsafe void Write_ModifiesTargetBytes()
    {
        using var mem = OpenSelf();

        var target = new byte[4];
        fixed (byte* p = target)
        {
            var written = mem.Write((nint)p, new byte[] { 0xDE, 0xAD, 0xBE, 0xEF });

            Assert.That(written.IsOk, Is.True, () => written.Error?.Message);
            Assert.That(target, Is.EqualTo(new byte[] { 0xDE, 0xAD, 0xBE, 0xEF }));
        }
    }

    [Test]
    public void Read_FailsForUnmappedAddress()
    {
        using var mem = OpenSelf();

        var read = mem.Read(0, new byte[8]);

        Assert.That(read.IsErr, Is.True);
    }
}
```

- [ ] **Step 2: Run it to verify it fails**

Run: `dotnet test test/memory/AslHelp.Memory.Tests/AslHelp.Memory.Tests.csproj --nologo --filter RemoteProcessMemoryTests`
Expected: FAIL — `RemoteProcessMemory` does not exist.

- [ ] **Step 3: Create `RemoteProcessMemory.cs`**

```csharp
using System;
using System.Runtime.InteropServices;

using AslHelp.Memory.Win32;

namespace AslHelp.Memory;

/// <summary>
///     An <see cref="IProcessMemory"/> that reads and writes another process out-of-process via the
///     Win32 memory APIs (<c>ReadProcessMemory</c>/<c>WriteProcessMemory</c>).
/// </summary>
public sealed partial class RemoteProcessMemory : IProcessMemory, IDisposable
{
    private readonly SafeProcessHandle _handle;
    private Module _mainModule;

    private RemoteProcessMemory(SafeProcessHandle handle, bool is64Bit)
    {
        _handle = handle;
        Is64Bit = is64Bit;
    }

    /// <inheritdoc/>
    public bool Is64Bit { get; }

    /// <inheritdoc/>
    public int PointerSize => Is64Bit ? 8 : 4;

    /// <inheritdoc/>
    public Module MainModule => _mainModule;

    /// <summary>
    ///     Opens the process with the given id for reading, writing, and querying.
    /// </summary>
    /// <param name="processId">The id of the process to open.</param>
    /// <returns>
    ///     The opened <see cref="RemoteProcessMemory"/> on success; otherwise, a failed result.
    /// </returns>
    public static Result<RemoteProcessMemory> Open(int processId)
    {
        const ProcessAccess access =
            ProcessAccess.VmRead
            | ProcessAccess.VmWrite
            | ProcessAccess.VmOperation
            | ProcessAccess.QueryInformation;

        var handle = PInvoke.OpenProcess((uint)processId, access, false);
        if (handle.IsInvalid)
        {
            return Result.Err<RemoteProcessMemory>(
                $"OpenProcess({processId}) failed (win32 error {Marshal.GetLastWin32Error()}).");
        }

        if (!TryDetermineBitness(handle, out var is64Bit, out var error))
        {
            handle.Dispose();
            return Result.Err<RemoteProcessMemory>(error);
        }

        var memory = new RemoteProcessMemory(handle, is64Bit);

        // TODO(Task 6): initialize _mainModule from the module list. Left default for now.
        memory._mainModule = default;

        return memory;
    }

    private static bool TryDetermineBitness(SafeProcessHandle handle, out bool is64Bit, out string error)
    {
        // A process is 32-bit iff it runs under WOW64 on a 64-bit OS. This library only supports
        // 64-bit Windows hosts, so !isWow64 means the target is native 64-bit.
        if (!PInvoke.IsWow64Process(handle, out var isWow64))
        {
            is64Bit = false;
            error = $"IsWow64Process failed (win32 error {Marshal.GetLastWin32Error()}).";
            return false;
        }

        is64Bit = !isWow64;
        error = "";
        return true;
    }

    /// <inheritdoc/>
    public unsafe Result Read(nint address, Span<byte> buffer)
    {
        if (buffer.IsEmpty)
        {
            return Result.Ok();
        }

        fixed (byte* pBuffer = buffer)
        {
            var ok = PInvoke.ReadProcessMemory(
                _handle, (nuint)address, pBuffer, (nuint)buffer.Length, out var read);

            if (!ok || read != (nuint)buffer.Length)
            {
                return Result.Err(
                    $"ReadProcessMemory([0x{(long)address:X}, +0x{buffer.Length:X}) failed "
                    + $"(win32 error {Marshal.GetLastWin32Error()}, read 0x{(long)read:X}).");
            }
        }

        return Result.Ok();
    }

    /// <inheritdoc/>
    public unsafe Result Write(nint address, ReadOnlySpan<byte> data)
    {
        if (data.IsEmpty)
        {
            return Result.Ok();
        }

        fixed (byte* pData = data)
        {
            var ok = PInvoke.WriteProcessMemory(
                _handle, (nuint)address, pData, (nuint)data.Length, out var written);

            if (!ok || written != (nuint)data.Length)
            {
                return Result.Err(
                    $"WriteProcessMemory([0x{(long)address:X}, +0x{data.Length:X}) failed "
                    + $"(win32 error {Marshal.GetLastWin32Error()}, wrote 0x{(long)written:X}).");
            }
        }

        return Result.Ok();
    }

    /// <summary>
    ///     Releases the underlying process handle.
    /// </summary>
    public void Dispose()
    {
        _handle.Dispose();
    }
}
```

Note the `WriteProcessMemory` P/Invoke takes `void* buffer` (non-const); passing a `fixed` pointer from a `ReadOnlySpan<byte>` is fine because P/Invoke only reads it.

- [ ] **Step 4: Run the test to verify it passes**

Run: `dotnet test test/memory/AslHelp.Memory.Tests/AslHelp.Memory.Tests.csproj --nologo --filter RemoteProcessMemoryTests`
Expected: PASS (all four tests). If `Write_ModifiesTargetBytes` fails with an access error, confirm `ProcessAccess.VmWrite | VmOperation` are in the `Open` access mask.

- [ ] **Step 5: Commit**

```bash
git add src/memory/AslHelp.Memory/RemoteProcessMemory.cs test/memory/AslHelp.Memory.Tests/RemoteProcessMemoryTests.cs
git commit -m "feat(memory): add RemoteProcessMemory with RPM read/write and bitness"
```

---

## Task 5: Page enumeration (`GetMemoryPages`)

Walk the target's address range with `VirtualQuery`, yielding `MemoryPage`s.

**Files:**
- Create: `src/memory/AslHelp.Memory/RemoteProcessMemory.Pages.cs`
- Modify: `test/memory/AslHelp.Memory.Tests/RemoteProcessMemoryTests.cs` (add a page test)

**Interfaces:**
- Consumes: `PInvoke.VirtualQuery`, `MemoryBasicInformation`, `MemoryPage`.
- Produces: `IEnumerable<MemoryPage> RemoteProcessMemory.GetMemoryPages(nint start, nint size)`.

- [ ] **Step 1: Write the failing test**

Add to `RemoteProcessMemoryTests.cs`:

```csharp
    [Test]
    public unsafe void GetMemoryPages_CoversAKnownAllocation()
    {
        using var mem = OpenSelf();

        var block = new byte[0x2000];
        fixed (byte* p = block)
        {
            var start = (nint)p;
            var found = false;

            foreach (var page in mem.GetMemoryPages(start, block.Length))
            {
                if (page.Base <= start && start < page.Base + page.RegionSize)
                {
                    found = true;
                    Assert.That(page.State, Is.EqualTo(Win32.MemoryPageState.Commit));
                    break;
                }
            }

            Assert.That(found, Is.True, "The managed heap allocation should be covered by a page.");
        }
    }
```

Add `using AslHelp.Memory.Win32;` is not needed since the test qualifies `Win32.MemoryPageState`; keep it qualified because `MemoryPageState` is internal but visible via `InternalsVisibleTo`.

- [ ] **Step 2: Run it to verify it fails**

Run: `dotnet test test/memory/AslHelp.Memory.Tests/AslHelp.Memory.Tests.csproj --nologo --filter GetMemoryPages_CoversAKnownAllocation`
Expected: FAIL — `GetMemoryPages` currently comes from the interface but `RemoteProcessMemory` doesn't implement it (compile error: does not implement interface member). If Task 4 compiled, that's because `RemoteProcessMemory` is `partial` and the members land here — so before this file exists, the build is red.

- [ ] **Step 3: Create `RemoteProcessMemory.Pages.cs`**

```csharp
using System.Collections.Generic;

using AslHelp.Memory.Win32;

namespace AslHelp.Memory;

public sealed partial class RemoteProcessMemory
{
    /// <inheritdoc/>
    public IEnumerable<MemoryPage> GetMemoryPages(nint start, nint size)
    {
        var end = start + size;
        var address = start;

        while (address < end)
        {
            if (PInvoke.VirtualQuery(_handle, (nuint)address, out var mbi) == 0)
            {
                yield break;
            }

            var page = new MemoryPage(mbi);

            // VirtualQuery snaps its base down to the region start; never step backwards, and always
            // advance by at least one page-sized region to guarantee termination.
            var regionSize = page.RegionSize <= 0 ? 0x1000 : page.RegionSize;

            yield return page;

            address = page.Base + regionSize;
        }
    }
}
```

- [ ] **Step 4: Run the test to verify it passes**

Run: `dotnet test test/memory/AslHelp.Memory.Tests/AslHelp.Memory.Tests.csproj --nologo --filter RemoteProcessMemoryTests`
Expected: PASS (all Task 4 + Task 5 tests).

- [ ] **Step 5: Commit**

```bash
git add src/memory/AslHelp.Memory/RemoteProcessMemory.Pages.cs test/memory/AslHelp.Memory.Tests/RemoteProcessMemoryTests.cs
git commit -m "feat(memory): enumerate memory pages via VirtualQuery"
```

---

## Task 6: Module enumeration (`GetModules`, `TryGetModule`, `MainModule`)

Snapshot the target's modules with ToolHelp32; fill `MainModule` in `Open`.

**Files:**
- Create: `src/memory/AslHelp.Memory/RemoteProcessMemory.Modules.cs`
- Modify: `src/memory/AslHelp.Memory/RemoteProcessMemory.cs` (initialize `_mainModule`)
- Modify: `test/memory/AslHelp.Memory.Tests/RemoteProcessMemoryTests.cs` (add module tests)

**Interfaces:**
- Consumes: `PInvoke.CreateToolhelp32Snapshot`, `PInvoke.Module32First`, `PInvoke.Module32Next`, `ModuleEntry32`, `ThFlags`, `SafeSnapshotHandle`.
- Produces:
  - `IEnumerable<Module> GetModules()`
  - `bool TryGetModule(string name, out Module module)`
  - private `Module GetFirstModule()` used to set `MainModule`.

Helper to decode a fixed `ushort[]` UTF-16 field into a string is needed. `ModuleEntry32` stores `ModuleName`/`ExePath` as `fixed ushort[]`.

- [ ] **Step 1: Write the failing test**

Add to `RemoteProcessMemoryTests.cs`:

```csharp
    [Test]
    public void GetModules_IncludesTheTestHostExecutable()
    {
        using var mem = OpenSelf();

        var any = false;
        foreach (var module in mem.GetModules())
        {
            any = true;
            Assert.That(module.Name, Is.Not.Empty);
            Assert.That(module.Base, Is.Not.EqualTo((nint)0));
        }

        Assert.That(any, Is.True, "A process always has at least its main module.");
    }

    [Test]
    public void TryGetModule_FindsKernel32CaseInsensitively()
    {
        using var mem = OpenSelf();

        var found = mem.TryGetModule("KERNEL32.DLL", out var module);

        Assert.That(found, Is.True);
        Assert.That(module.Name, Is.EqualTo("KERNEL32.DLL").IgnoreCase);
    }

    [Test]
    public void MainModule_IsPopulated()
    {
        using var mem = OpenSelf();

        Assert.That(mem.MainModule.Base, Is.Not.EqualTo((nint)0));
        Assert.That(mem.MainModule.Name, Is.Not.Empty);
    }
```

- [ ] **Step 2: Run it to verify it fails**

Run: `dotnet test test/memory/AslHelp.Memory.Tests/AslHelp.Memory.Tests.csproj --nologo --filter RemoteProcessMemoryTests`
Expected: FAIL — `GetModules`/`TryGetModule` not implemented; `MainModule` is `default`.

- [ ] **Step 3: Create `RemoteProcessMemory.Modules.cs`**

```csharp
using System;
using System.Collections.Generic;

using AslHelp.Memory.Win32;

namespace AslHelp.Memory;

public sealed partial class RemoteProcessMemory
{
    private readonly uint _processId;

    /// <inheritdoc/>
    public IEnumerable<Module> GetModules()
    {
        using var snapshot = PInvoke.CreateToolhelp32Snapshot(_processId, ThFlags.Module | ThFlags.Module32);
        if (snapshot.IsInvalid)
        {
            yield break;
        }

        if (!PInvoke.Module32First(snapshot, out var entry))
        {
            yield break;
        }

        do
        {
            yield return ToModule(entry);
        }
        while (PInvoke.Module32Next(snapshot, ref entry));
    }

    /// <inheritdoc/>
    public bool TryGetModule(string name, out Module module)
    {
        foreach (var candidate in GetModules())
        {
            if (string.Equals(candidate.Name, name, StringComparison.OrdinalIgnoreCase))
            {
                module = candidate;
                return true;
            }
        }

        module = default;
        return false;
    }

    private Module GetFirstModule()
    {
        // The first module in a ToolHelp32 snapshot is always the process's main executable image.
        using var snapshot = PInvoke.CreateToolhelp32Snapshot(_processId, ThFlags.Module | ThFlags.Module32);
        if (!snapshot.IsInvalid && PInvoke.Module32First(snapshot, out var entry))
        {
            return ToModule(entry);
        }

        return default;
    }

    private static unsafe Module ToModule(ModuleEntry32 entry)
    {
        var name = DecodeFixed(entry.ModuleName, ModuleEntry32.ModuleLength);
        var path = DecodeFixed(entry.ExePath, ModuleEntry32.ExePathLength);

        return new Module(name, (nint)entry.ModuleBaseAddress, (int)entry.ModuleBaseSize, path);
    }

    private static unsafe string DecodeFixed(ushort* buffer, int maxLength)
    {
        var length = 0;
        while (length < maxLength && buffer[length] != 0)
        {
            length++;
        }

        return new string((char*)buffer, 0, length);
    }
}
```

Because `ModuleName`/`ExePath` are `fixed ushort[]` fields, calling `ToModule(entry)` copies the struct by value; taking `entry.ModuleName` yields a `ushort*` into that copy. To keep the pointer valid, decode inside `ToModule` while `entry` is a live parameter — which the code above does. This is safe: `entry` lives for the duration of `ToModule`.

- [ ] **Step 4: Wire `_processId` and `MainModule` in `Open`**

In `RemoteProcessMemory.cs`, update the constructor and `Open`:

Change the constructor to capture the id:

```csharp
    private RemoteProcessMemory(SafeProcessHandle handle, uint processId, bool is64Bit)
    {
        _handle = handle;
        _processId = processId;
        Is64Bit = is64Bit;
    }
```

Remove the `private readonly uint _processId;` line you would otherwise duplicate — it now lives in `RemoteProcessMemory.Modules.cs` (Step 3 declares it). Keep the field declared in exactly one partial. (It is declared in `.Modules.cs`; the constructor in `.cs` assigns it. Partial classes share fields, so this compiles.)

In `Open`, replace the construction + TODO block with:

```csharp
        var memory = new RemoteProcessMemory(handle, (uint)processId, is64Bit);
        memory._mainModule = memory.GetFirstModule();

        return memory;
```

- [ ] **Step 5: Run the test to verify it passes**

Run: `dotnet test test/memory/AslHelp.Memory.Tests/AslHelp.Memory.Tests.csproj --nologo --filter RemoteProcessMemoryTests`
Expected: PASS (all tests including the three new module tests).

- [ ] **Step 6: Commit**

```bash
git add src/memory/AslHelp.Memory/RemoteProcessMemory.cs src/memory/AslHelp.Memory/RemoteProcessMemory.Modules.cs test/memory/AslHelp.Memory.Tests/RemoteProcessMemoryTests.cs
git commit -m "feat(memory): enumerate modules and populate MainModule via ToolHelp32"
```

---

## Task 7: Typed reads and deref chains (`ProcessMemoryExtensions`)

Give consumers `Read<T>`, `Write<T>`, and pointer-size-aware deref chains over any `IProcessMemory`. These are extension methods so both backends inherit them for free.

**Files:**
- Create: `src/memory/AslHelp.Memory/ProcessMemoryExtensions.cs`
- Test: `test/memory/AslHelp.Memory.Tests/ProcessMemoryExtensionsTests.cs`

**Interfaces:**
- Consumes: `IProcessMemory.Read`, `IProcessMemory.PointerSize`, `MemoryMarshal`.
- Produces (all in `public static class ProcessMemoryExtensions`, using the repo's `extension(IProcessMemory self)` syntax):
  - `Result<T> Read<T>(nint address) where T : unmanaged`
  - `Result<T> Read<T>(nint baseAddress, params int[] offsets) where T : unmanaged`
  - `Result Write<T>(nint address, T value) where T : unmanaged`
  - `Result<nint> Deref(nint baseAddress, params int[] offsets)`

Pointer hops read `PointerSize` bytes and zero-extend to `nint`, so a 32-bit target derefs correctly from a 64-bit host.

- [ ] **Step 1: Write the failing test**

Create `test/memory/AslHelp.Memory.Tests/ProcessMemoryExtensionsTests.cs`:

```csharp
using System;

using AslHelp.Memory;

using NUnit.Framework;

namespace AslHelp.Memory.Tests;

[TestFixture]
public sealed class ProcessMemoryExtensionsTests
{
    [Test]
    public void ReadOfT_ReadsLittleEndianInt32()
    {
        var mem = new FakeMemoryReader(0x1000, new byte[] { 0x78, 0x56, 0x34, 0x12 });

        var result = mem.Read<int>(0x1000);

        Assert.That(result.IsOk, Is.True, () => result.Error?.Message);
        Assert.That(result.Value, Is.EqualTo(0x12345678));
    }

    [Test]
    public void Deref_FollowsA64BitPointerChain()
    {
        // At 0x1000: pointer 0x2000. At 0x2008 (0x2000 + 8): pointer 0x3000.
        var data = new byte[0x2000];
        WritePointer(data, offset: 0x0000, target: 0x2000);   // 0x1000 -> 0x2000
        WritePointer(data, offset: 0x1008, target: 0x3000);   // 0x2008 -> 0x3000
        var mem = new FakeMemoryReader(0x1000, data);          // base 0x1000 covers [0x1000, 0x3000)

        var result = mem.Deref(0x1000, 0x8);

        Assert.That(result.IsOk, Is.True, () => result.Error?.Message);
        Assert.That(result.Value, Is.EqualTo((nint)0x3000));
    }

    private static void WritePointer(byte[] data, int offset, long target)
    {
        for (var i = 0; i < 8; i++)
        {
            data[offset + i] = (byte)(target >> (8 * i));
        }
    }
}
```

Note: `FakeMemoryReader.Is64Bit` returns `nint.Size == 8`; on the CI host that is true, so the 8-byte pointer chain matches `PointerSize == 8`.

- [ ] **Step 2: Run it to verify it fails**

Run: `dotnet test test/memory/AslHelp.Memory.Tests/AslHelp.Memory.Tests.csproj --nologo --filter ProcessMemoryExtensionsTests`
Expected: FAIL — `Read<int>` / `Deref` don't exist.

- [ ] **Step 3: Create `ProcessMemoryExtensions.cs`**

```csharp
using System;
using System.Runtime.InteropServices;

namespace AslHelp.Memory;

/// <summary>
///     Typed reads, writes, and pointer-size-aware dereference chains over any
///     <see cref="IProcessMemory"/>.
/// </summary>
public static class ProcessMemoryExtensions
{
    extension(IProcessMemory self)
    {
        /// <summary>
        ///     Reads an unmanaged value of type <typeparamref name="T"/> from <paramref name="address"/>.
        /// </summary>
        /// <typeparam name="T">The unmanaged value type to read.</typeparam>
        /// <param name="address">The address to read from.</param>
        /// <returns>The value on success; otherwise, a failed result.</returns>
        public Result<T> Read<T>(nint address)
            where T : unmanaged
        {
            Span<byte> buffer = stackalloc byte[Marshal.SizeOf<T>()];

            var read = self.Read(address, buffer);
            if (read.IsErr)
            {
                return Result.Err<T>(read.Error);
            }

            return MemoryMarshal.Read<T>(buffer);
        }

        /// <summary>
        ///     Dereferences <paramref name="baseAddress"/> through <paramref name="offsets"/>, then
        ///     reads an unmanaged value of type <typeparamref name="T"/> at the final address.
        /// </summary>
        /// <typeparam name="T">The unmanaged value type to read.</typeparam>
        /// <param name="baseAddress">The starting address of the pointer chain.</param>
        /// <param name="offsets">The offsets added at each hop; the last locates the value.</param>
        /// <returns>The value on success; otherwise, a failed result.</returns>
        public Result<T> Read<T>(nint baseAddress, params int[] offsets)
            where T : unmanaged
        {
            if (offsets is null || offsets.Length == 0)
            {
                return self.Read<T>(baseAddress);
            }

            var deref = self.Deref(baseAddress, offsets[..^1]);
            if (deref.IsErr)
            {
                return Result.Err<T>(deref.Error);
            }

            return self.Read<T>(deref.Value + offsets[^1]);
        }

        /// <summary>
        ///     Writes an unmanaged value of type <typeparamref name="T"/> to <paramref name="address"/>.
        /// </summary>
        /// <typeparam name="T">The unmanaged value type to write.</typeparam>
        /// <param name="address">The address to write to.</param>
        /// <param name="value">The value to write.</param>
        /// <returns>A successful result on success; otherwise, a failed result.</returns>
        public Result Write<T>(nint address, T value)
            where T : unmanaged
        {
            Span<byte> buffer = stackalloc byte[Marshal.SizeOf<T>()];
            MemoryMarshal.Write(buffer, ref value);   // ref (not in) — matches the System.Memory netstandard2.0 overload

            return self.Write(address, buffer);
        }

        /// <summary>
        ///     Follows a pointer chain from <paramref name="baseAddress"/> through
        ///     <paramref name="offsets"/>, reading <see cref="IProcessMemory.PointerSize"/> bytes at
        ///     each hop.
        /// </summary>
        /// <param name="baseAddress">The starting address of the pointer chain.</param>
        /// <param name="offsets">The offset added before each pointer read.</param>
        /// <returns>The final address on success; otherwise, a failed result.</returns>
        public Result<nint> Deref(nint baseAddress, params int[] offsets)
        {
            var address = baseAddress;

            foreach (var offset in offsets ?? [])
            {
                var hop = self.ReadPointer(address + offset);
                if (hop.IsErr)
                {
                    return Result.Err<nint>(hop.Error);
                }

                address = hop.Value;
            }

            return address;
        }

        private Result<nint> ReadPointer(nint address)
        {
            Span<byte> buffer = stackalloc byte[self.PointerSize];

            var read = self.Read(address, buffer);
            if (read.IsErr)
            {
                return Result.Err<nint>(read.Error);
            }

            // Zero-extend so a 4-byte pointer from a 32-bit target lands correctly in an nint.
            nint value = 0;
            for (var i = 0; i < buffer.Length; i++)
            {
                value |= (nint)buffer[i] << (8 * i);
            }

            return value;
        }
    }
}
```

`Result.Err<T>(read.Error)` uses the `IResultError` overload — `read.Error` is non-null when `IsErr`.

- [ ] **Step 4: Run the test to verify it passes**

Run: `dotnet test test/memory/AslHelp.Memory.Tests/AslHelp.Memory.Tests.csproj --nologo --filter ProcessMemoryExtensionsTests`
Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add src/memory/AslHelp.Memory/ProcessMemoryExtensions.cs test/memory/AslHelp.Memory.Tests/ProcessMemoryExtensionsTests.cs
git commit -m "feat(memory): add typed Read/Write and pointer-size-aware Deref extensions"
```

---

## Task 8: Symbol lookup tier 1 — PE export table via RPM

Resolve exported symbols by reading the target module's PE export directory remotely. No symbol files, no DbgHelp — this covers what engine layers need (e.g. `mono_get_root_domain`).

**Files:**
- Create: `src/memory/AslHelp.Memory/Pe/PeExportReader.cs`
- Test: `test/memory/AslHelp.Memory.Tests/Pe/PeExportReaderTests.cs`

**Interfaces:**
- Consumes: `IProcessMemory.Read`, `Module`.
- Produces: `internal static class PeExportReader` with
  `static Result<nint> TryResolve(IProcessMemory memory, Module module, string symbolName)`.

PE layout walked (all offsets from the module base, read remotely):
1. DOS header `e_lfanew` at `+0x3C` (int) → NT headers offset.
2. NT headers: `Signature`(4) + `FILE_HEADER`(20) → Optional header at `ntBase + 0x18`.
3. Optional header magic at optional+0: `0x10B` = PE32 (32-bit), `0x20B` = PE32+ (64-bit). Export directory RVA sits in the data directory: at `optional + 0x60` for PE32, `optional + 0x70` for PE32+.
4. Export directory: `NumberOfFunctions`(+0x14), `NumberOfNames`(+0x18), `AddressOfFunctions`RVA(+0x1C), `AddressOfNames`RVA(+0x20), `AddressOfNameOrdinals`RVA(+0x24).
5. Binary-independent linear scan: for each name index, read the name RVA, read the C string, compare; on match read the ordinal (`ushort` at ordinals + i*2), then the function RVA (`uint` at functions + ordinal*4). Address = `module.Base + functionRva`.

- [ ] **Step 1: Write the failing test**

Create `test/memory/AslHelp.Memory.Tests/Pe/PeExportReaderTests.cs`:

```csharp
using System.Diagnostics;

using AslHelp.Memory;

using NUnit.Framework;

namespace AslHelp.Memory.Tests.Pe;

[TestFixture]
public sealed class PeExportReaderTests
{
    [Test]
    public void TryResolve_FindsKnownKernel32Export()
    {
        using var mem = RemoteProcessMemory.Open(Process.GetCurrentProcess().Id).Value!;
        Assert.That(mem.TryGetModule("kernel32.dll", out var kernel32), Is.True);

        var result = AslHelp.Memory.Pe.PeExportReader.TryResolve(mem, kernel32, "Sleep");

        Assert.That(result.IsOk, Is.True, () => result.Error?.Message);
        Assert.That(result.Value, Is.GreaterThanOrEqualTo(kernel32.Base));
        Assert.That(result.Value, Is.LessThan(kernel32.End));
    }

    [Test]
    public void TryResolve_FailsForUnknownExport()
    {
        using var mem = RemoteProcessMemory.Open(Process.GetCurrentProcess().Id).Value!;
        Assert.That(mem.TryGetModule("kernel32.dll", out var kernel32), Is.True);

        var result = AslHelp.Memory.Pe.PeExportReader.TryResolve(mem, kernel32, "ThisIsNotAnExport");

        Assert.That(result.IsErr, Is.True);
    }
}
```

- [ ] **Step 2: Run it to verify it fails**

Run: `dotnet test test/memory/AslHelp.Memory.Tests/AslHelp.Memory.Tests.csproj --nologo --filter PeExportReaderTests`
Expected: FAIL — `PeExportReader` doesn't exist.

- [ ] **Step 3: Create `Pe/PeExportReader.cs`**

```csharp
using System;
using System.Text;

namespace AslHelp.Memory.Pe;

/// <summary>
///     Resolves exported symbols of a loaded module by walking its PE export directory in the target
///     process's memory. Works for both PE32 and PE32+ images.
/// </summary>
internal static class PeExportReader
{
    /// <summary>
    ///     Resolves the absolute address of <paramref name="symbolName"/> exported by
    ///     <paramref name="module"/>.
    /// </summary>
    /// <param name="memory">The reader for the module's process.</param>
    /// <param name="module">The module whose exports are searched.</param>
    /// <param name="symbolName">The export name to resolve.</param>
    /// <returns>The absolute address on success; otherwise, a failed result.</returns>
    public static Result<nint> TryResolve(IProcessMemory memory, Module module, string symbolName)
    {
        var @base = module.Base;

        var lfanew = memory.Read<int>(@base + 0x3C);
        if (lfanew.IsErr)
        {
            return Result.Err<nint>(lfanew.Error);
        }

        var ntBase = @base + lfanew.Value;
        var optional = ntBase + 0x18;

        var magic = memory.Read<ushort>(optional);
        if (magic.IsErr)
        {
            return Result.Err<nint>(magic.Error);
        }

        // Export directory RVA lives in the first data directory entry, whose offset depends on the
        // optional header size: 0x60 for PE32, 0x70 for PE32+.
        var exportDirRvaOffset = magic.Value == 0x20B ? 0x70 : 0x60;

        var exportRva = memory.Read<uint>(optional + exportDirRvaOffset);
        if (exportRva.IsErr)
        {
            return Result.Err<nint>(exportRva.Error);
        }

        if (exportRva.Value == 0)
        {
            return Result.Err<nint>($"'{module.Name}' has no export directory.");
        }

        var exportDir = @base + (nint)exportRva.Value;

        var numberOfNames = memory.Read<uint>(exportDir + 0x18);
        var addrOfFunctions = memory.Read<uint>(exportDir + 0x1C);
        var addrOfNames = memory.Read<uint>(exportDir + 0x20);
        var addrOfOrdinals = memory.Read<uint>(exportDir + 0x24);

        if (numberOfNames.IsErr || addrOfFunctions.IsErr || addrOfNames.IsErr || addrOfOrdinals.IsErr)
        {
            return Result.Err<nint>($"Failed to read the export directory of '{module.Name}'.");
        }

        var namesTable = @base + (nint)addrOfNames.Value;
        var ordinalsTable = @base + (nint)addrOfOrdinals.Value;
        var functionsTable = @base + (nint)addrOfFunctions.Value;

        for (var i = 0u; i < numberOfNames.Value; i++)
        {
            var nameRva = memory.Read<uint>(namesTable + (nint)(i * 4));
            if (nameRva.IsErr)
            {
                return Result.Err<nint>(nameRva.Error);
            }

            var name = ReadCString(memory, @base + (nint)nameRva.Value);
            if (name.IsErr)
            {
                return Result.Err<nint>(name.Error);
            }

            if (!string.Equals(name.Value, symbolName, StringComparison.Ordinal))
            {
                continue;
            }

            var ordinal = memory.Read<ushort>(ordinalsTable + (nint)(i * 2));
            if (ordinal.IsErr)
            {
                return Result.Err<nint>(ordinal.Error);
            }

            var functionRva = memory.Read<uint>(functionsTable + (nint)(ordinal.Value * 4));
            if (functionRva.IsErr)
            {
                return Result.Err<nint>(functionRva.Error);
            }

            return @base + (nint)functionRva.Value;
        }

        return Result.Err<nint>($"'{module.Name}' does not export '{symbolName}'.");
    }

    private static Result<string> ReadCString(IProcessMemory memory, nint address)
    {
        var sb = new StringBuilder();
        Span<byte> chunk = stackalloc byte[32];
        var cursor = address;

        // Read in small chunks until a NUL byte. Cap length to guard against a runaway on corrupt data.
        while (sb.Length < 512)
        {
            var read = memory.Read(cursor, chunk);
            if (read.IsErr)
            {
                return Result.Err<string>(read.Error);
            }

            foreach (var b in chunk)
            {
                if (b == 0)
                {
                    return sb.ToString();
                }

                _ = sb.Append((char)b);
            }

            cursor += chunk.Length;
        }

        return sb.ToString();
    }
}
```

- [ ] **Step 4: Run the test to verify it passes**

Run: `dotnet test test/memory/AslHelp.Memory.Tests/AslHelp.Memory.Tests.csproj --nologo --filter PeExportReaderTests`
Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add src/memory/AslHelp.Memory/Pe/PeExportReader.cs test/memory/AslHelp.Memory.Tests/Pe/PeExportReaderTests.cs
git commit -m "feat(memory): resolve module exports by walking the PE export table over RPM"
```

---

## Task 9: Symbol lookup tier 2 + unified `GetSymbol`

Wire `RemoteProcessMemory.GetSymbol` to try exports first, then fall back to DbgHelp/PDB for non-exported names.

**Files:**
- Create: `src/memory/AslHelp.Memory/RemoteProcessMemory.Symbols.cs`
- Modify: `src/memory/AslHelp.Memory/RemoteProcessMemory.cs` (dispose DbgHelp if initialized)
- Modify: `test/memory/AslHelp.Memory.Tests/RemoteProcessMemoryTests.cs` (add symbol tests)

**Interfaces:**
- Consumes: `PeExportReader.TryResolve`, `PInvoke.SymInitialize`, `PInvoke.SymFromName`, `PInvoke.SymCleanup`, `SymbolInfo`, `TryGetModule`.
- Produces: `Result<Symbol> GetSymbol(string moduleName, string symbolName)` — export tier, then DbgHelp tier; a `bool _symInitialized` flag disposed with the handle.

DbgHelp note: `SymInitialize(handle, null, invadeProcess: true)` loads symbols for all modules. `SymFromName` returns a `SymbolInfo` whose `Address` is absolute. Guard it behind a one-time init; on failure, return the export-tier error so the message stays useful. DbgHelp is not thread-safe; this milestone is single-threaded per instance, which is fine.

- [ ] **Step 1: Write the failing test**

Add to `RemoteProcessMemoryTests.cs`:

```csharp
    [Test]
    public void GetSymbol_ResolvesExportedSleepInKernel32()
    {
        using var mem = OpenSelf();

        var symbol = mem.GetSymbol("kernel32.dll", "Sleep");

        Assert.That(symbol.IsOk, Is.True, () => symbol.Error?.Message);
        Assert.That(symbol.Value.Name, Is.EqualTo("Sleep"));
        Assert.That(symbol.Value.Address, Is.Not.EqualTo((nint)0));
    }

    [Test]
    public void GetSymbol_FailsForUnknownModule()
    {
        using var mem = OpenSelf();

        var symbol = mem.GetSymbol("does-not-exist.dll", "whatever");

        Assert.That(symbol.IsErr, Is.True);
    }
```

- [ ] **Step 2: Run it to verify it fails**

Run: `dotnet test test/memory/AslHelp.Memory.Tests/AslHelp.Memory.Tests.csproj --nologo --filter GetSymbol`
Expected: FAIL — `GetSymbol` throws `NotImplementedException`? No — it's currently unimplemented on the class, so the build is red (interface member missing). Confirm red.

- [ ] **Step 3: Create `RemoteProcessMemory.Symbols.cs`**

```csharp
using System;

using AslHelp.Memory.Pe;
using AslHelp.Memory.Win32;

namespace AslHelp.Memory;

public sealed partial class RemoteProcessMemory
{
    private bool _symInitialized;

    /// <inheritdoc/>
    public Result<Symbol> GetSymbol(string moduleName, string symbolName)
    {
        if (!TryGetModule(moduleName, out var module))
        {
            return Result.Err<Symbol>($"Module '{moduleName}' is not loaded.");
        }

        // Tier 1: PE export table (cheap, no symbol files).
        var exported = PeExportReader.TryResolve(this, module, symbolName);
        if (exported.IsOk)
        {
            return new Symbol(symbolName, exported.Value, module.Base);
        }

        // Tier 2: DbgHelp / PDB (covers non-exported names; may need symbol files).
        var viaDbgHelp = ResolveWithDbgHelp(symbolName, module.Base);
        if (viaDbgHelp.IsOk)
        {
            return viaDbgHelp;
        }

        // Prefer the export-tier message; it's the common, actionable case.
        return Result.Err<Symbol>(exported.Error);
    }

    private Result<Symbol> ResolveWithDbgHelp(string symbolName, nint moduleBase)
    {
        if (!EnsureSymInitialized())
        {
            return Result.Err<Symbol>("DbgHelp symbol handler could not be initialized.");
        }

        if (!PInvoke.SymFromName(_handle, symbolName, out var info))
        {
            return Result.Err<Symbol>($"DbgHelp could not resolve '{symbolName}'.");
        }

        return new Symbol(symbolName, (nint)info.Address, moduleBase);
    }

    private bool EnsureSymInitialized()
    {
        if (_symInitialized)
        {
            return true;
        }

        _symInitialized = PInvoke.SymInitialize(_handle, null, invadeProcess: true);
        return _symInitialized;
    }
}
```

- [ ] **Step 4: Clean up DbgHelp in `Dispose`**

In `RemoteProcessMemory.cs`, replace the `Dispose` body with:

```csharp
    public void Dispose()
    {
        if (_symInitialized)
        {
            _ = PInvoke.SymCleanup(_handle);
            _symInitialized = false;
        }

        _handle.Dispose();
    }
```

- [ ] **Step 5: Run the test to verify it passes**

Run: `dotnet test test/memory/AslHelp.Memory.Tests/AslHelp.Memory.Tests.csproj --nologo --filter RemoteProcessMemoryTests`
Expected: PASS (all tests). `GetSymbol_ResolvesExportedSleepInKernel32` resolves via tier 1, so it does not depend on symbol files being present.

- [ ] **Step 6: Commit**

```bash
git add src/memory/AslHelp.Memory/RemoteProcessMemory.cs src/memory/AslHelp.Memory/RemoteProcessMemory.Symbols.cs test/memory/AslHelp.Memory.Tests/RemoteProcessMemoryTests.cs
git commit -m "feat(memory): tiered GetSymbol (PE exports then DbgHelp fallback)"
```

---

## Task 10: Scanner convenience entry points

Add module- and page-scoped scan helpers so callers don't hand-roll region math. Reuse the existing `Scan.Memory` and `MemoryPageExtensions.AsContiguousRanges`.

**Files:**
- Create: `src/memory/AslHelp.Memory/Scanning/ScanExtensions.cs`
- Test: `test/memory/AslHelp.Memory.Tests/Scanning/ScanExtensionsTests.cs`

**Interfaces:**
- Consumes: `Scan.Memory(IProcessMemory, nint, int, ScanStep[])`, `IProcessMemory.GetMemoryPages`, `MemoryPageExtensions.AsContiguousRanges`, `MemoryPage.IsReadable`, `Module`.
- Produces (in `public static class ScanExtensions`, `extension(IProcessMemory self)`):
  - `IEnumerable<nint> ScanModule(Module module, params ScanStep[] steps)`
  - `IEnumerable<nint> ScanPages(nint start, nint size, params ScanStep[] steps)`

`MemoryPageExtensions` and its `IsReadable` are `internal`, and this file lives in the same assembly, so both are usable.

- [ ] **Step 1: Write the failing test**

Create `test/memory/AslHelp.Memory.Tests/Scanning/ScanExtensionsTests.cs`:

```csharp
using System.Linq;

using AslHelp.Memory;
using AslHelp.Memory.Scanning;

using NUnit.Framework;

namespace AslHelp.Memory.Tests.Scanning;

[TestFixture]
public sealed class ScanExtensionsTests
{
    [Test]
    public void ScanPages_FindsAPatternPlantedInABuffer()
    {
        // FakeMemoryReader models a single readable region [0x1000, 0x1000 + data.Length).
        var data = new byte[64];
        data[10] = 0xAB;
        data[11] = 0xCD;
        data[12] = 0xEF;
        var mem = new PlantedPagesReader(0x1000, data);

        var hits = mem.ScanPages(0x1000, data.Length, ScanStep.Forward("AB CD EF")).ToList();

        Assert.That(hits, Does.Contain((nint)0x100A));
    }
}
```

This needs a fake that reports one readable page over its backing range. Add it to the same file:

```csharp
namespace AslHelp.Memory.Tests.Scanning;

internal sealed class PlantedPagesReader : IProcessMemory
{
    private readonly nint _base;
    private readonly byte[] _data;

    public PlantedPagesReader(nint baseAddress, byte[] data)
    {
        _base = baseAddress;
        _data = data;
    }

    public bool Is64Bit => nint.Size == 8;
    public int PointerSize => nint.Size;
    public Module MainModule => default;

    public Result Read(nint address, System.Span<byte> buffer)
    {
        var offset = (long)address - _base;
        if (offset < 0 || offset + buffer.Length > _data.Length)
        {
            return Result.Err("unreadable");
        }

        _data.AsSpan((int)offset, buffer.Length).CopyTo(buffer);
        return Result.Ok();
    }

    public Result Write(nint address, System.ReadOnlySpan<byte> data) => Result.Err("no");

    public System.Collections.Generic.IEnumerable<MemoryPage> GetMemoryPages(nint start, nint size)
    {
        yield return new MemoryPage(
            _base, _data.Length,
            Win32.MemoryPageProtect.ReadWrite, Win32.MemoryPageState.Commit, Win32.MemoryPageType.Private);
    }

    public System.Collections.Generic.IEnumerable<Module> GetModules() { yield break; }
    public bool TryGetModule(string name, out Module module) { module = default; return false; }
    public Result<Symbol> GetSymbol(string moduleName, string symbolName) => Result.Err<Symbol>("no");
}
```

Confirm the exact member names of `MemoryPageProtect` (`ReadWrite`), `MemoryPageState` (`Commit`), and `MemoryPageType` (`Private`) against `src/memory/AslHelp.Memory/Win32/enums/`; adjust if the identifiers differ.

- [ ] **Step 2: Run it to verify it fails**

Run: `dotnet test test/memory/AslHelp.Memory.Tests/AslHelp.Memory.Tests.csproj --nologo --filter ScanExtensionsTests`
Expected: FAIL — `ScanPages` doesn't exist.

- [ ] **Step 3: Create `Scanning/ScanExtensions.cs`**

```csharp
using System.Collections.Generic;

namespace AslHelp.Memory.Scanning;

/// <summary>
///     Convenience scan entry points scoped to a module or an address range, layered over
///     <see cref="Scan.Memory(IProcessMemory, nint, int, ScanStep[])"/>.
/// </summary>
public static class ScanExtensions
{
    extension(IProcessMemory self)
    {
        /// <summary>
        ///     Runs <paramref name="steps"/> across the readable pages overlapping
        ///     <paramref name="module"/>.
        /// </summary>
        /// <param name="module">The module to scan.</param>
        /// <param name="steps">The scan steps to apply.</param>
        /// <returns>The matching addresses, enumerated lazily.</returns>
        public IEnumerable<nint> ScanModule(Module module, params ScanStep[] steps)
        {
            return self.ScanPages(module.Base, module.Size, steps);
        }

        /// <summary>
        ///     Runs <paramref name="steps"/> across the readable, contiguous chunks of
        ///     <c>[start, start + size)</c>.
        /// </summary>
        /// <param name="start">The inclusive start address of the range to scan.</param>
        /// <param name="size">The length of the range, in bytes.</param>
        /// <param name="steps">The scan steps to apply.</param>
        /// <returns>The matching addresses, enumerated lazily.</returns>
        public IEnumerable<nint> ScanPages(nint start, nint size, params ScanStep[] steps)
        {
            var readablePages = FilterReadable(self.GetMemoryPages(start, size));

            foreach (var range in readablePages.AsContiguousRanges())
            {
                foreach (var hit in Scan.Memory(self, range.Base, range.Size, steps))
                {
                    yield return hit;
                }
            }
        }

        private static IEnumerable<MemoryPage> FilterReadable(IEnumerable<MemoryPage> pages)
        {
            foreach (var page in pages)
            {
                if (MemoryPage.IsReadable(page))
                {
                    yield return page;
                }
            }
        }
    }
}
```

`MemoryPage.IsReadable(page)` is the existing `internal` static extension on `MemoryPage`; this file is in the same assembly. If the analyzer flags the static-in-extension shape, inline the check: `page.State == MemoryPageState.Commit && page.Protect != 0 && (page.Protect & (MemoryPageProtect.NoAccess | MemoryPageProtect.Guard)) == 0`.

- [ ] **Step 4: Run the test to verify it passes**

Run: `dotnet test test/memory/AslHelp.Memory.Tests/AslHelp.Memory.Tests.csproj --nologo --filter ScanExtensionsTests`
Expected: PASS.

- [ ] **Step 5: Run the whole suite (no regressions)**

Run: `dotnet test test/memory/AslHelp.Memory.Tests/AslHelp.Memory.Tests.csproj --nologo`
Expected: PASS.

- [ ] **Step 6: Commit**

```bash
git add src/memory/AslHelp.Memory/Scanning/ScanExtensions.cs test/memory/AslHelp.Memory.Tests/Scanning/ScanExtensionsTests.cs
git commit -m "feat(memory): add module- and page-scoped scan convenience entry points"
```

---

## Task 11: ASL config data model + explicit JSON loader

A plain, ASL-only config model plus a loader that is called explicitly at ASL init. Holds module-name/location overrides, named symbols, and named AOB signatures. Lives in `AslHelp.Memory` so the memory layer can consume it; parsed with `System.Text.Json`.

**Files:**
- Create: `src/memory/AslHelp.Memory/Config/GameConfig.cs`
- Create: `src/memory/AslHelp.Memory/Config/GameConfigLoader.cs`
- Modify: `src/memory/AslHelp.Memory/AslHelp.Memory.csproj` (add `System.Text.Json` package reference)
- Modify: `Directory.Packages.props` (add `System.Text.Json` version)
- Test: `test/memory/AslHelp.Memory.Tests/Config/GameConfigLoaderTests.cs`

**Interfaces:**
- Produces:
  - `public sealed record GameConfig` with:
    - `IReadOnlyDictionary<string, string> ModuleOverrides` — logical name → actual file name.
    - `IReadOnlyDictionary<string, string> Symbols` — logical name → `"module!symbol"`.
    - `IReadOnlyDictionary<string, string> Signatures` — logical name → AOB pattern string.
    - helpers: `string ResolveModule(string logicalName)` (returns the override or the input unchanged); `bool TryGetSignature(string name, out string pattern)`; `bool TryGetSymbol(string name, out string module, out string symbol)`.
  - `public static class GameConfigLoader` with `static Result<GameConfig> Load(string path)`.

`netstandard2.0` requires an explicit `System.Text.Json` package reference (it isn't in-box there).

- [ ] **Step 1: Add the `System.Text.Json` package**

In `Directory.Packages.props`, add under `<!-- Shared package versions. -->`:

```xml
    <PackageVersion Include="System.Text.Json" Version="8.0.5" />
```

In `src/memory/AslHelp.Memory/AslHelp.Memory.csproj`, add to the existing `<ItemGroup>` with package references:

```xml
    <PackageReference Include="System.Text.Json" />
```

Verify it restores:

Run: `dotnet restore src/memory/AslHelp.Memory/AslHelp.Memory.csproj --nologo`
Expected: PASS.

- [ ] **Step 2: Write the failing test**

Create `test/memory/AslHelp.Memory.Tests/Config/GameConfigLoaderTests.cs`:

```csharp
using System.IO;

using AslHelp.Memory.Config;

using NUnit.Framework;

namespace AslHelp.Memory.Tests.Config;

[TestFixture]
public sealed class GameConfigLoaderTests
{
    private static string WriteTemp(string json)
    {
        var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".json");
        File.WriteAllText(path, json);
        return path;
    }

    [Test]
    public void Load_ParsesAllSections()
    {
        var path = WriteTemp("""
        {
          "moduleOverrides": { "GameAssembly": "MyGame.dll" },
          "symbols": { "rootDomain": "mono-2.0-bdwgc.dll!mono_get_root_domain" },
          "signatures": { "playerBase": "48 8B 05 ?? ?? ?? ??" }
        }
        """);

        try
        {
            var result = GameConfigLoader.Load(path);

            Assert.That(result.IsOk, Is.True, () => result.Error?.Message);
            var config = result.Value!;

            Assert.That(config.ResolveModule("GameAssembly"), Is.EqualTo("MyGame.dll"));
            Assert.That(config.ResolveModule("UnityPlayer.dll"), Is.EqualTo("UnityPlayer.dll"));

            Assert.That(config.TryGetSymbol("rootDomain", out var module, out var symbol), Is.True);
            Assert.That(module, Is.EqualTo("mono-2.0-bdwgc.dll"));
            Assert.That(symbol, Is.EqualTo("mono_get_root_domain"));

            Assert.That(config.TryGetSignature("playerBase", out var pattern), Is.True);
            Assert.That(pattern, Is.EqualTo("48 8B 05 ?? ?? ?? ??"));
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    public void Load_FailsForMissingFile()
    {
        var result = GameConfigLoader.Load(Path.Combine(Path.GetTempPath(), "definitely-missing.json"));

        Assert.That(result.IsErr, Is.True);
    }

    [Test]
    public void Load_FailsForMalformedSymbol()
    {
        var path = WriteTemp("""
        { "symbols": { "bad": "no-bang-here" } }
        """);

        try
        {
            var result = GameConfigLoader.Load(path);
            Assert.That(result.IsErr, Is.True);
        }
        finally
        {
            File.Delete(path);
        }
    }
}
```

- [ ] **Step 3: Run it to verify it fails**

Run: `dotnet test test/memory/AslHelp.Memory.Tests/AslHelp.Memory.Tests.csproj --nologo --filter GameConfigLoaderTests`
Expected: FAIL — `GameConfig` / `GameConfigLoader` don't exist.

- [ ] **Step 4: Create `Config/GameConfig.cs`**

```csharp
using System;
using System.Collections.Generic;

namespace AslHelp.Memory.Config;

/// <summary>
///     ASL-only configuration read from a JSON file at script initialization. Holds per-game or
///     per-version overrides that scripts would otherwise hardcode: module name overrides, named
///     native symbols, and named AOB signatures.
/// </summary>
/// <param name="ModuleOverrides">Maps a logical module name to the actual file name to load.</param>
/// <param name="Symbols">Maps a logical name to a <c>"module!symbol"</c> pair.</param>
/// <param name="Signatures">Maps a logical name to an AOB pattern string.</param>
public sealed record GameConfig(
    IReadOnlyDictionary<string, string> ModuleOverrides,
    IReadOnlyDictionary<string, string> Symbols,
    IReadOnlyDictionary<string, string> Signatures)
{
    /// <summary>
    ///     Returns the configured file name override for <paramref name="logicalName"/>, or
    ///     <paramref name="logicalName"/> itself when there is no override.
    /// </summary>
    /// <param name="logicalName">The logical module name to resolve.</param>
    /// <returns>The overridden module file name, or the input unchanged.</returns>
    public string ResolveModule(string logicalName)
    {
        return ModuleOverrides.TryGetValue(logicalName, out var actual)
            ? actual
            : logicalName;
    }

    /// <summary>
    ///     Gets the AOB pattern configured under <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The logical signature name.</param>
    /// <param name="pattern">The AOB pattern when found; otherwise, <see langword="null"/>.</param>
    /// <returns><see langword="true"/> when a signature was found.</returns>
    public bool TryGetSignature(string name, out string pattern)
    {
        return Signatures.TryGetValue(name, out pattern!);
    }

    /// <summary>
    ///     Gets the module and symbol configured under <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The logical symbol name.</param>
    /// <param name="module">The module file name when found; otherwise, <see langword="null"/>.</param>
    /// <param name="symbol">The symbol name when found; otherwise, <see langword="null"/>.</param>
    /// <returns><see langword="true"/> when a symbol was found.</returns>
    public bool TryGetSymbol(string name, out string module, out string symbol)
    {
        if (Symbols.TryGetValue(name, out var combined))
        {
            var bang = combined.IndexOf('!');
            if (bang > 0 && bang < combined.Length - 1)
            {
                module = combined[..bang];
                symbol = combined[(bang + 1)..];
                return true;
            }
        }

        module = null!;
        symbol = null!;
        return false;
    }
}
```

- [ ] **Step 5: Create `Config/GameConfigLoader.cs`**

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AslHelp.Memory.Config;

/// <summary>
///     Loads a <see cref="GameConfig"/> from a JSON file. Call this explicitly during ASL
///     initialization; it performs no implicit discovery.
/// </summary>
public static class GameConfigLoader
{
    private static readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Performance", "CA1812:Avoid uninstantiated internal classes",
        Justification = "Instantiated by the JSON deserializer via reflection.")]
    private sealed record ConfigDto(
        [property: JsonPropertyName("moduleOverrides")] Dictionary<string, string>? ModuleOverrides,
        [property: JsonPropertyName("symbols")] Dictionary<string, string>? Symbols,
        [property: JsonPropertyName("signatures")] Dictionary<string, string>? Signatures);

    /// <summary>
    ///     Loads and validates the config at <paramref name="path"/>.
    /// </summary>
    /// <param name="path">The full path of the JSON config file.</param>
    /// <returns>The parsed <see cref="GameConfig"/> on success; otherwise, a failed result.</returns>
    public static Result<GameConfig> Load(string path)
    {
        if (!File.Exists(path))
        {
            return Result.Err<GameConfig>($"Config file not found: '{path}'.");
        }

        ConfigDto? dto;
        try
        {
            using var fs = File.OpenRead(path);
            dto = JsonSerializer.Deserialize<ConfigDto>(fs, _options);
        }
        catch (JsonException ex)
        {
            return Result.Err<GameConfig>($"Config '{path}' is not valid JSON: {ex.Message}");
        }
        catch (IOException ex)
        {
            return Result.Err<GameConfig>($"Config '{path}' could not be read: {ex.Message}");
        }

        if (dto is null)
        {
            return Result.Err<GameConfig>($"Config '{path}' deserialized to null.");
        }

        var config = new GameConfig(
            dto.ModuleOverrides ?? new Dictionary<string, string>(),
            dto.Symbols ?? new Dictionary<string, string>(),
            dto.Signatures ?? new Dictionary<string, string>());

        // Validate that every symbol entry is a well-formed "module!symbol" pair.
        foreach (var entry in config.Symbols)
        {
            if (!config.TryGetSymbol(entry.Key, out _, out _))
            {
                return Result.Err<GameConfig>(
                    $"Symbol '{entry.Key}' must be of the form \"module!symbol\"; got '{entry.Value}'.");
            }
        }

        return config;
    }
}
```

- [ ] **Step 6: Run the test to verify it passes**

Run: `dotnet test test/memory/AslHelp.Memory.Tests/AslHelp.Memory.Tests.csproj --nologo --filter GameConfigLoaderTests`
Expected: PASS (all three tests).

- [ ] **Step 7: Run the whole suite**

Run: `dotnet test test/memory/AslHelp.Memory.Tests/AslHelp.Memory.Tests.csproj --nologo`
Expected: PASS.

- [ ] **Step 8: Commit**

```bash
git add src/memory/AslHelp.Memory/Config/ src/memory/AslHelp.Memory/AslHelp.Memory.csproj Directory.Packages.props test/memory/AslHelp.Memory.Tests/Config/
git commit -m "feat(memory): add ASL config model and explicit JSON loader"
```

---

## Task 12: Wire config into module and signature resolution

Make the config actually reduce hardcoding: resolve modules through overrides, and turn named signatures into scans. These are thin extension methods so the config stays optional.

**Files:**
- Create: `src/memory/AslHelp.Memory/Config/GameConfigExtensions.cs`
- Test: `test/memory/AslHelp.Memory.Tests/Config/GameConfigExtensionsTests.cs`

**Interfaces:**
- Consumes: `IProcessMemory.TryGetModule`, `IProcessMemory.GetSymbol`, `IProcessMemory.ScanModule`, `GameConfig.ResolveModule`, `GameConfig.TryGetSymbol`, `GameConfig.TryGetSignature`, `ScanStep.Forward`.
- Produces (in `public static class GameConfigExtensions`, `extension(IProcessMemory self)`):
  - `bool TryGetConfiguredModule(GameConfig config, string logicalName, out Module module)`
  - `Result<Symbol> GetConfiguredSymbol(GameConfig config, string name)`

- [ ] **Step 1: Write the failing test**

Create `test/memory/AslHelp.Memory.Tests/Config/GameConfigExtensionsTests.cs`:

```csharp
using System.Collections.Generic;
using System.Diagnostics;

using AslHelp.Memory;
using AslHelp.Memory.Config;

using NUnit.Framework;

namespace AslHelp.Memory.Tests.Config;

[TestFixture]
public sealed class GameConfigExtensionsTests
{
    [Test]
    public void TryGetConfiguredModule_UsesTheOverride()
    {
        using var mem = RemoteProcessMemory.Open(Process.GetCurrentProcess().Id).Value!;

        // Map a logical name "engine" to the real, always-present kernel32.dll.
        var config = new GameConfig(
            new Dictionary<string, string> { ["engine"] = "kernel32.dll" },
            new Dictionary<string, string>(),
            new Dictionary<string, string>());

        var found = mem.TryGetConfiguredModule(config, "engine", out var module);

        Assert.That(found, Is.True);
        Assert.That(module.Name, Is.EqualTo("kernel32.dll").IgnoreCase);
    }

    [Test]
    public void GetConfiguredSymbol_ResolvesThroughTheConfig()
    {
        using var mem = RemoteProcessMemory.Open(Process.GetCurrentProcess().Id).Value!;

        var config = new GameConfig(
            new Dictionary<string, string>(),
            new Dictionary<string, string> { ["sleep"] = "kernel32.dll!Sleep" },
            new Dictionary<string, string>());

        var symbol = mem.GetConfiguredSymbol(config, "sleep");

        Assert.That(symbol.IsOk, Is.True, () => symbol.Error?.Message);
        Assert.That(symbol.Value.Name, Is.EqualTo("Sleep"));
    }
}
```

- [ ] **Step 2: Run it to verify it fails**

Run: `dotnet test test/memory/AslHelp.Memory.Tests/AslHelp.Memory.Tests.csproj --nologo --filter GameConfigExtensionsTests`
Expected: FAIL — the extension methods don't exist.

- [ ] **Step 3: Create `Config/GameConfigExtensions.cs`**

```csharp
namespace AslHelp.Memory.Config;

/// <summary>
///     Resolves modules and symbols through a <see cref="GameConfig"/>, so scripts reference logical
///     names and the config supplies the per-game or per-version specifics.
/// </summary>
public static class GameConfigExtensions
{
    extension(IProcessMemory self)
    {
        /// <summary>
        ///     Finds the loaded module for <paramref name="logicalName"/>, applying any override in
        ///     <paramref name="config"/> before looking it up.
        /// </summary>
        /// <param name="config">The config supplying module overrides.</param>
        /// <param name="logicalName">The logical module name to resolve.</param>
        /// <param name="module">The matching module when found; otherwise, <see langword="default"/>.</param>
        /// <returns><see langword="true"/> when a module was found.</returns>
        public bool TryGetConfiguredModule(GameConfig config, string logicalName, out Module module)
        {
            var actualName = config.ResolveModule(logicalName);
            return self.TryGetModule(actualName, out module);
        }

        /// <summary>
        ///     Resolves the symbol configured under <paramref name="name"/> in
        ///     <paramref name="config"/>.
        /// </summary>
        /// <param name="config">The config supplying the <c>module!symbol</c> pair.</param>
        /// <param name="name">The logical symbol name.</param>
        /// <returns>The resolved <see cref="Symbol"/> on success; otherwise, a failed result.</returns>
        public Result<Symbol> GetConfiguredSymbol(GameConfig config, string name)
        {
            if (!config.TryGetSymbol(name, out var module, out var symbol))
            {
                return Result.Err<Symbol>($"Config has no symbol named '{name}'.");
            }

            return self.GetSymbol(module, symbol);
        }
    }
}
```

- [ ] **Step 4: Run the test to verify it passes**

Run: `dotnet test test/memory/AslHelp.Memory.Tests/AslHelp.Memory.Tests.csproj --nologo --filter GameConfigExtensionsTests`
Expected: PASS.

- [ ] **Step 5: Run the entire memory suite one final time**

Run: `dotnet test test/memory/AslHelp.Memory.Tests/AslHelp.Memory.Tests.csproj --nologo`
Expected: PASS (all tasks' tests green).

- [ ] **Step 6: Commit**

```bash
git add src/memory/AslHelp.Memory/Config/GameConfigExtensions.cs test/memory/AslHelp.Memory.Tests/Config/GameConfigExtensionsTests.cs
git commit -m "feat(memory): resolve modules and symbols through GameConfig"
```

---

## Done criteria for Milestone 1

- `dotnet build asl-help.slnx` is warning-clean.
- `dotnet test test/memory/AslHelp.Memory.Tests/AslHelp.Memory.Tests.csproj` is green on both target frameworks.
- `RemoteProcessMemory` implements the full `IProcessMemory`: read, write, pages, modules, symbols, bitness.
- Typed `Read<T>`/`Write<T>`/`Deref` work against any `IProcessMemory`.
- Symbols resolve via PE exports with a DbgHelp fallback.
- The scanner has module- and page-scoped entry points.
- A JSON `GameConfig` loads explicitly and feeds module/symbol resolution.
- Nothing references the deleted `IMemoryReader`.

## Deferred to later milestones (out of scope, tracked here so nothing is lost)

- `AslHelp.Ipc`: port the generic named-pipe framing from the old repo.
- `AslHelp.Memory.Mono`: the Mono engine contract + RPM implementation (remote struct walk).
- `AslHelp.Agent.Mono`: NAOT injected library (win-x86 + win-x64), embedded in the plugin dll; the `NativeProcessMemory` backend; Mono C API bindings; `InstanceRegistry`/liveness.
- ASL-facing facade in `AslHelp.LiveSplit` (`vars.Helper`, `vars.Helper.Mono`).
- 32-bit host support and IL2CPP.
