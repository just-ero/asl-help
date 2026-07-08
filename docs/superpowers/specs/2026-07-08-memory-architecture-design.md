# asl-help Memory Architecture — Design

Date: 2026-07-08
Status: Approved direction; milestone 1 scoped.

## Context

asl-help is a helper library for LiveSplit autosplitters (ASL scripts) that reads game
process memory and engine internals. Two codebases exist:

- **just-ero/cs/asl-help** — the old, working-ish codebase. 12 projects. Has the full
  vertical (RPM memory, Mono engine seam, named-pipe IPC, injected NAOT agent,
  InstanceRegistry/liveness), but one feature is spread across 6 assemblies
  (`ipc/`, `interop/`, `memory/`), and the ASL-facing facade was never built.
- **ero-qt/cs/asl-help** (this repo) — the rewrite. Clean core (`Result<T>`, logging,
  reflection), complete LiveSplit attachment layer, complete + tested AOB scanner,
  comprehensive Win32 pinvoke incl. DbgHelp. Stalled exactly at the memory-reader and
  engine layers: `IProcessMemory` is declared with no concrete implementation.

**Decision: this repo (ero-qt) is the foundation.** The old repo is a knowledge source
to port from, not a base to refactor in place.

## Core decisions

### 1. One interface, two implementations

A single `IProcessMemory` abstraction covering read, write, module enumeration, page
enumeration, and symbol lookup, with two backends:

- `RemoteProcessMemory` — RPM/WPM via a process handle, from outside the game
  (milestone 1).
- `NativeProcessMemory` — runs inside the injected agent, raw pointer access
  (later milestone; the interface must be shaped so it slots in without change).

Consumers never know which backend they hold. Deref chains, typed reads, scanning,
and symbol lookup layer over the interface (extension methods or thin wrappers).

### 2. Per-engine projects under the Memory family

Engine support lives in sibling projects: `AslHelp.Memory.Mono` (Unity/Mono; IL2CPP
variant later), `AslHelp.Memory.Unreal`, `AslHelp.Memory.GameMaker`, and eventually
more (Godot, Flash, Clickteam, …). Each engine project has the same internal shape:

1. **The contract**: an interface in that engine's own vocabulary (Mono speaks
   images/klasses/vtables; Unreal speaks GObjects/FName/UProperty). Most engines share
   the *ideas* — assemblies/images/packages, classes, fields, offsets, instances,
   static data — but there is **no forced universal base interface**. A shared dynamic
   seam (for the DynamicObject binder) can be layered on top later if wanted.
2. **Two implementations** of the contract:
   - **RPM**: walks engine structures from outside via `IProcessMemory`.
   - **Native**: an IPC client that proxies the contract over a named pipe to the
     injected agent, which calls the real engine C API in-process.
3. **The protocol DTOs** for that engine's IPC commands (the agent references these).

### 3. Keep a generic IPC project

`AslHelp.Ipc` stays a standalone project: generic named-pipe client/server framing and
serialization. Rationale: transport is categorically different from memory/engine
concerns, and every engine's native backend plus every agent reuses it.

### 4. Per-engine NAOT agents, embedded in the plugin dll

`agents/AslHelp.Agent.Mono` (first; others follow the pattern) is a NativeAOT library
(net10.0) compiled for **win-x86 and win-x64**. Both outputs are embedded as resources
in the managed plugin dll and extracted at runtime for injection; optionally also
offered as standalone downloads. The agent references `AslHelp.Ipc` (server side) and
the engine project's protocol DTOs, and hosts the in-process implementations ported
from the old `AslHelp.Interop.Native`: Mono C API bindings, `InstanceRegistry`,
liveness walking.

Known constraint (learned the hard way in the old repo): Win32 structs used inside the
agent must match the **target process's** bitness; the x86 build cannot reuse x64
struct layouts (e.g. `MEMORY_BASIC_INFORMATION`).

### 5. Symbols: tiered lookup, engine symbols stay separate

"Symbols" in the memory layer means native module symbols, resolved in tiers:

1. **PE export table**, parsed remotely via RPM — cheap, dependency-free, covers what
   engine layers need (e.g. `mono_get_root_domain` in `mono-2.0-bdwgc.dll`).
   (`GetProcAddress` is not an option cross-process.)
2. **DbgHelp/PDB** as fallback for non-exported names (pinvokes already exist in
   `Win32/pinvoke/PInvoke.DbgHelp.cs`).

Engine-level name resolution (Mono classes/fields, etc.) is **not** part of the memory
layer; it belongs to the per-engine contracts.

### 6. ASL config system

A JSON config file, ASL-related only, read **explicitly** when initializing asl-help
in the ASL script (path passed at init; no implicit discovery). It holds
per-game/per-version data that scripts currently hardcode:

- module name/location overrides (some games rename `GameAssembly.dll` or relocate
  `UnityPlayer.dll`),
- symbol names/locations,
- AOB signatures keyed by name.

The loader lives in the LiveSplit-facing layer (it is ASL-only), but the data model is
plain and consumable by `AslHelp.Memory` (e.g. module-name overrides consulted during
module/symbol resolution; named signatures fed to the scanner).

## Target layout

```
src/
├─ core/AslHelp/               core: Result<T>, logging, reflection      (exists)
├─ core/AslHelp.LiveSplit/     attach, script context, settings,          (exists)
│                              ASL config loader, ASL-facing facade       (facade later)
├─ ipc/AslHelp.Ipc/            generic named-pipe framing + serialization (port later)
├─ memory/AslHelp.Memory/      IProcessMemory, RemoteProcessMemory,       (extend now)
│                              modules, pages, symbols, scanning
├─ memory/AslHelp.Memory.Mono/ Mono contract + RPM impl + IPC client      (later)
│                              + protocol DTOs
├─ memory/AslHelp.Memory.*/    Unreal, GameMaker, Godot, …                (future)
└─ agents/AslHelp.Agent.Mono/  NAOT injected lib (x86 + x64), pipe        (later)
                               server, Mono C API, InstanceRegistry
```

## Milestone 1 (this plan's scope)

`AslHelp.Memory` only — get unstuck on the normal memory stuff:

- Extend `IProcessMemory` to the full contract: read, write, typed read/deref chains,
  module enumeration, page enumeration, symbol lookup — shaped so the native backend
  slots in later.
- Implement `RemoteProcessMemory` (open process, RPM/WPM, ToolHelp32 modules,
  VirtualQueryEx pages, bitness detection).
- Tiered symbol lookup: PE export-table parser over RPM, DbgHelp/PDB fallback.
- Wire the existing scanner onto `IProcessMemory` (it already consumes it for region
  reads; add module/page-scoped convenience entry points).
- ASL config: data model + explicit JSON loader, consumed by module/symbol/signature
  resolution.

Out of scope for milestone 1: `AslHelp.Ipc` port, `AslHelp.Memory.Mono`, the agent,
the ASL-facing `vars.Helper` facade (beyond what config init requires), IL2CPP.

## Later milestones (sketch)

- **M2**: port `AslHelp.Ipc` framing from the old repo (trimmed), design the Mono
  protocol DTOs.
- **M3**: `AslHelp.Memory.Mono` — contract + RPM implementation (remote struct walk).
- **M4**: `AslHelp.Agent.Mono` (NAOT, x86+x64) + native backend of the Mono contract;
  embed agent binaries in the plugin dll.
- **M5**: ASL-facing facade in `AslHelp.LiveSplit` (`vars.Helper`, `vars.Helper.Mono`),
  plugin packaging.

## Error handling & testing

- Error handling follows the repo's existing `Result`/`Result<T>` railway style; no
  exceptions across public seams (and, later, none across the pipe).
- Testing follows the repo's existing xunit setup. Memory-layer tests run against the
  current process (self-inspection: read own modules/pages/exports) so CI needs no
  game process; scanner tests already model this pattern.
