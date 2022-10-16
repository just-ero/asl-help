# Basic Class
## Definition

Namespace: \<root>

Contains a large amount of helpful and quality of life features that are not specific to any game engines or platforms.

<caption>$\textcolor{#519ABA}{\tt{.cs}}$</caption>

```cs
public class Basic
```

Inheritance: [Object](https://learn.microsoft.com/dotnet/api/System.Object) → Basic  
Derived: [GameHelperBase]()

## Examples

The following example creates the Basic helper instance.

<caption>$\textcolor{#519ABA}{\tt{.asl}}$</caption>

```cs
startup
{
  // Loads the helper library as an array of bytes.
  // This prevents LiveSplit from putting a long-term handle on the file.
  byte[] bytes = File.ReadAllBytes("Components/asl-help");

  // Creates an Assembly object from the loaded bytes.
  Assembly asm = Assembly.Load(bytes);

  // Creates a persistent instance of the Basic class.
  // Along with this, various code snippets and variables are generated:
  //   * the Basic class instance gets set to `vars.Helper`,
  //   * an Action<object> gets set to `vars.Log` for logging,
  //   * the line `vars.Helper.Dispose();` is added to the beginning of shutdown.
  asm.CreateInstance("Basic");

  // `vars.Helper` and `vars.Log` can now be used in the rest of the script.
}
```

The above startup code can also be shortened:

<caption>$\textcolor{#519ABA}{\tt{.asl}}$</caption>

```cs
startup
{
  Assembly.Load(File.ReadAllBytes("Components/asl-help")).CreateInstance("Basic");
}
```

If you would like to ***opt-out of code generation***, please use the following code:

<caption>$\textcolor{#519ABA}{\tt{.asl}}$</caption>

```cs
startup
{
  byte[] bytes = File.ReadAllBytes("Components/asl-help");
  Assembly asm = Assembly.Load(bytes);

  // Finds the Basic class' Type in the assembly.
  Type type = asm.GetType("Basic");

  // Sets the Basic class instance to a developer-chosen variable.
  // This variable does not need to be named `vars.Helper`.
  vars.Helper = Activator.CreateInstance(type, args: false);

  // `vars.Helper` can now be used in the rest of the script.
}

shutdown
{
  // It is highly recommended to call Dispose() on the helper
  // in shutdown, as it acts as clean-up.
  vars.Helper.Dispose();
}
```

The above startup code can also be shortened:

<caption>$\textcolor{#519ABA}{\tt{.asl}}$</caption>

```cs
startup
{
  Type type = Assembly.Load(File.ReadAllBytes("Components/asl-help")).GetType("Basic");
  vars.Helper = Activator.CreateInstance(type, args: false);
}

shutdown
{
  vars.Helper.Dispose();
}
```

## Remarks

## Constructors

| Constructor     | Summary
|-----------------|---------
| [Basic()]()     | Initializes a new instance of the Basic class. Code generation is enabled by default.
| [Basic(bool)]() | Initializes a new instance of the Basic class and optionally generates specific code.

## Properties

| Property         | Summary
|------------------|---------
| [Game]()         | Gets or sets the game [Process]() used by the ASL script.
| [GameName]()     | Gets or sets an identifier for the game.
| [Is64Bit]()      | Gets a value indicating whether the [Game]() is a 64-bit process.
| [Item[string]]() | Gets or sets the [MemoryWatcher]() or [Pointer]() with the specified name.
| [MainModule]()   | Gets the main module of the [Game]().
| [Modules]()      | Gets an iterable, cached collection of all [Modules]() loaded by the [Game]().
| [Pages]()        | Gets an iterable collection of all [MemoryPages]() loaded by the [Game]().
| [PtrSize]()      | Gets the pointer size of the [Game]().
| [Settings]()     | Gets a [SettingsCreator]() instance.
| [Texts]()        | Gets a [TextComponentManager]() instance.

## Methods

| Method                                                              | Summary
|---------------------------------------------------------------------|---------
| [AlertGameTime()]()                                                 | When not already the case, displays a [MessageBox]() asking the user whether they would like to change their [CurrentTimingMethod]() to [TimingMethod.GameTime]().
| [AlertLoadless()]()                                                 | When not already the case, displays a [MessageBox]() asking the user whether they would like to change their [CurrentTimingMethod]() to [TimingMethod.GameTime]().
| [AlertRealTime()]()                                                 | When not already the case, displays a [MessageBox]() asking the user whether they would like to change their [CurrentTimingMethod]() to [TimingMethod.RealTime]().
| [Define(string, string[])]()                                        | Compiles a [TypeDefinition]() from an input source code [String]() using the specified assembly references.
| [Deref(int, int[])]()                                               | Dereferences a pointer path starting from the [MainModule]()'s [Base]() plus a specified base offset, following an optional amount of offsets.
| [Deref(IntPtr, int[])]()                                            | Dereferences a pointer path starting from a specified address, following an optional amount of offsets.
| [Deref(Module, int, int[])]()                                       | Dereferences a pointer path starting from a [Module]()'s [Base]() plus a specified base offset, following an optional amount of offsets.
| [Deref(string, int, int[])]()                                       | Dereferences a pointer path starting from the [Base]() of a [Module]() specified by its file name plus a specified base offset, following an optional amount of offsets.
| [Dispose()]()                                                       | Releases all resources used by the helper. Removal of all custom text components is enabled by default.
| [Dispose(bool)]()                                                   | Releases all resources used by the helper and optionally removes all custom text components.
| [FromAbsoluteAddress(IntPtr)]()                                     | Reads another address at an absolute position in the [Game]() memory.
| [FromAssemblyAddress(IntPtr)]()                                     | Calls [FromRelativeAddress(IntPtr)]() when [Is64Bit]() is `true`, otherwise, calls [FromAbsoluteAddress(IntPtr)]().
| [FromRelativeAddress(IntPtr)]()                                     | Reads another address at a relative position in the [Game]() memory.
| [GetMD5Hash()]()                                                    | Computes the MD5 hash for the [MainModule]().
| [GetMD5Hash(Module)]()                                              | Computes the MD5 hash for a specified [Module]().
| [GetMD5Hash(string)]()                                              | Computes the MD5 hash for a [Module]() specified by its file name.
| [GetMemorySize()]()                                                 | Retrieves the size of the memory space the [MainModule]() occupies, in bytes.
| [GetMemorySize(Module)]()                                           | Retrieves the size of the memory space a specified [Module]() occupies, in bytes.
| [GetMemorySize(string)]()                                           | Retrieves the size of the memory space a [Module]() specified by its file name occupies, in bytes.
| [GetSHA1Hash()]()                                                   | Computes the SHA1 hash for the [MainModule]().
| [GetSHA1Hash(Module)]()                                             | Computes the SHA1 hash for a specified [Module]().
| [GetSHA1Hash(string)]()                                             | Computes the SHA1 hash for a [Module]() specified by its file name.
| [GetSHA256Hash()]()                                                 | Computes the SHA256 hash for the [MainModule]().
| [GetSHA256Hash(Module)]()                                           | Computes the SHA256 hash for a specified [Module]().
| [GetSHA256Hash(string)]()                                           | Computes the SHA256 hash for a [Module]() specified by its file name.
| [GetSHA384Hash()]()                                                 | Computes the SHA384 hash for the [MainModule]().
| [GetSHA384Hash(Module)]()                                           | Computes the SHA384 hash for a specified [Module]().
| [GetSHA384Hash(string)]()                                           | Computes the SHA384 hash for a [Module]() specified by its file name.
| [GetSHA512Hash()]()                                                 | Computes the SHA512 hash for the [MainModule]().
| [GetSHA512Hash(Module)]()                                           | Computes the SHA512 hash for a specified [Module]().
| [GetSHA512Hash(string)]()                                           | Computes the SHA512 hash for a [Module]() specified by its file name.
| [Make\<T>(int, int[])]()                                            | Builds a [Pointer\<T>]() from the [MainModule]()'s [Base]() plus a specified base offset and an optional amount of offsets.
| [Make\<T>(IntPtr, int[])]()                                         | Builds a [Pointer\<T>]() from a base address and an optional amount of offsets.
| [Make\<T>(Module, int, int[])]()                                    | Builds a [Pointer\<T>]() from a [Module]()'s [Base]() plus a specified base offset and an optional amount of offsets.
| [Make\<T>(string, int, int[])]()                                    | Builds a [Pointer\<T>]() from the [Base]() of a [Module]() specified by its file name plus a specified base offset and an optional amount of offsets.
| [MakeSpan\<T>(int, int, int[])]()                                   | Builds a [SpanPointer\<T>]() with the specified length from the [MainModule]()'s [Base]() plus a specified base offset and an optional amount of offsets.
| [MakeSpan\<T>(int, IntPtr, int[])]()                                | Builds a [SpanPointer\<T>]() with the specified length from a base address and an optional amount of offsets.
| [MakeSpan\<T>(int, Module, int, int[])]()                           | Builds a [SpanPointer\<T>]() with the specified length from a [Module]()'s [Base]() plus a specified base offset and an optional amount of offsets.
| [MakeSpan\<T>(int, string, int, int[])]()                           | Builds a [SpanPointer\<T>]() with the specified length from the [Base]() of a [Module]() specified by its file name plus a specified base offset and an optional amount of offsets.
| [MakeString(int, int, int[])]()                                     | Builds a [StringPointer]() with the specified length from the [MainModule]()'s [Base]() plus a specified base offset and an optional amount of offsets.
| [MakeString(int, IntPtr, int[])]()                                  | Builds a [StringPointer]() with the specified length from a base address and an optional amount of offsets.
| [MakeString(int, Module, int, int[])]()                             | Builds a [StringPointer]() with the specified length from a [Module]()'s [Base]() plus a specified base offset and an optional amount of offsets.
| [MakeString(int, ReadStringType, int, int[])]()                     | Builds a [StringPointer]() with the specified length and [ReadStringType]() from the [MainModule]()'s [Base]() plus a specified base offset and an optional amount of offsets.
| [MakeString(int, ReadStringType, IntPtr, int[])]()                  | Builds a [StringPointer]() with the specified length and [ReadStringType]() from a base address and an optional amount of offsets.
| [MakeString(int, ReadStringType, Module, int, int[])]()             | Builds a [StringPointer]() with the specified length and [ReadStringType]() from a [Module]()'s [Base]() plus a specified base offset and an optional amount of offsets.
| [MakeString(int, ReadStringType, string, int, int[])]()             | Builds a [StringPointer]() with the specified length and [ReadStringType]() from the [Base]() of a [Module]() specified by its file name plus a specified base offset and an optional amount of offsets.
| [MakeString(int, string, int, int[])]()                             | Builds a [StringPointer]() with the specified length from the [Base]() of a [Module]() specified by its file name plus a specified base offset and an optional amount of offsets.
| [MapPointers()]()                                                   | Iterates over all [MemoryWatchers]() in the helper and sets their [Current]() values to the script's `current` object using their names.
| [Read\<T>(int, int[])]()                                            | Reads an `unmanaged` [Type]() `T`'s value at the end of a pointer path starting from the [MainModule]()'s [Base]() plus a specified base offset, following an optional amount of offsets.
| [Read\<T>(IntPtr, int[])]()                                         | Reads an `unmanaged` [Type]() `T`'s value at the end of a pointer path starting at a base address, following an optional amount of offsets.
| [Read\<T>(Module, int, int[])]()                                    | Reads an `unmanaged` [Type]() `T`'s value at the end of a pointer path starting at a [Module]()'s [Base]() plus a specified base offset, following an optional amount of offsets.
| [Read\<T>(string, int, int[])]()                                    | Reads an `unmanaged` [Type]() `T`'s value at the end of a pointer path starting at the [Base]() of a [Module]() specified by its file name plus a specified base offset, following an optional amount of offsets.
| [ReadCustom(TypeDefinition, int, int[])]()                          | Reads a custom-defined `unmanaged` [Type]() created from [Define(string, string[])]() at the end of a pointer path starting from the [MainModule]()'s [Base]() plus a specified base offset, following an optional amount of offsets.
| [ReadCustom(TypeDefinition, IntPtr, int[])]()                       | Reads a custom-defined `unmanaged` [Type]() created from [Define(string, string[])]() at the end of a pointer path starting at a base address, following an optional amount of offsets.
| [ReadCustom(TypeDefinition, Module, int, int[])]()                  | Reads a custom-defined `unmanaged` [Type]() created from [Define(string, string[])]() at the end of a pointer path starting at a [Module]()'s [Base]() plus a specified base offset, following an optional amount of offsets.
| [ReadCustom(TypeDefinition, string, int, int[])]()                  | Reads a custom-defined `unmanaged` [Type]() created from [Define(string, string[])]() at the end of a pointer path starting at the [Base]() of a [Module]() specified by its file name plus a specified base offset, following an optional amount of offsets.
| [ReadSpan\<T>(int, int, int[])]()                                   | Reads a C-style array of `unmanaged` [Type]() `T`s with the specified length at the end of a pointer path starting from the [MainModule]()'s [Base]() plus a specified base offset, following an optional amount of offsets.
| [ReadSpan\<T>(int, IntPtr, int[])]()                                | Reads a C-style array of `unmanaged` [Type]() `T`s with the specified length at the end of a pointer path starting at a base address, following an optional amount of offsets.
| [ReadSpan\<T>(int, Module, int, int[])]()                           | Reads a C-style array of `unmanaged` [Type]() `T`s with the specified length at the end of a pointer path starting at a [Module]()'s [Base]() plus a specified base offset, following an optional amount of offsets.
| [ReadSpan\<T>(int, string, int, int[])]()                           | Reads a C-style array of `unmanaged` [Type]() `T`s with the specified length at the end of a pointer path starting at the [Base]() of a [Module]() specified by its file name plus a specified base offset, following an optional amount of offsets.
| [ReadSpanCustom(int, TypeDefinition, int, int[])]()                 | Reads a C-style array of custom-defined `unmanaged` [Types]() created from [Define(string, string[])]() with the specified length at the end of a pointer path starting from the [MainModule]()'s [Base]() plus a specified base offset, following an optional amount of offsets.
| [ReadSpanCustom(int, TypeDefinition, IntPtr, int[])]()              | Reads a C-style array of custom-defined `unmanaged` [Types]() created from [Define(string, string[])]() with the specified length at the end of a pointer path starting at a base address, following an optional amount of offsets.
| [ReadSpanCustom(int, TypeDefinition, Module, int, int[])]()         | Reads a C-style array of custom-defined `unmanaged` [Types]() created from [Define(string, string[])]() with the specified length at the end of a pointer path starting at a [Module]()'s [Base]() plus a specified base offset, following an optional amount of offsets.
| [ReadSpanCustom(int, TypeDefinition, string, int, int[])]()         | Reads a C-style array of custom-defined `unmanaged` [Types]() created from [Define(string, string[])]() with the specified length at the end of a pointer path starting at the [Base]() of a [Module]() specified by its file name plus a specified base offset, following an optional amount of offsets.
| [ReadString(int, int, int[])]()                                     | Reads a string with the specified length at the end of a pointer path starting from the [MainModule]()'s [Base]() plus a specified base offset, following an optional amount of offsets.
| [ReadString(int, IntPtr, int[])]()                                  | Reads a string with the specified length at the end of a pointer path starting at a base address, following an optional amount of offsets.
| [ReadString(int, Module, int, int[])]()                             | Reads a string with the specified length at the end of a pointer path starting at a [Module]()'s [Base]() plus a specified base offset, following an optional amount of offsets.
| [ReadString(int, ReadStringType, int, int[])]()                     | Reads a string with the specified length and [ReadStringType]() at the end of a pointer path starting from the [MainModule]()'s [Base]() plus a specified base offset, following an optional amount of offsets.
| [ReadString(int, ReadStringType, IntPtr, int[])]()                  | Reads a string with the specified length and [ReadStringType]() at the end of a pointer path starting at a base address, following an optional amount of offsets.
| [ReadString(int, ReadStringType, Module, int, int[])]()             | Reads a string with the specified length and [ReadStringType]() at the end of a pointer path starting at a [Module]()'s [Base]() plus a specified base offset, following an optional amount of offsets.
| [ReadString(int, ReadStringType, string, int, int[])]()             | Reads a string with the specified length and [ReadStringType]() at the end of a pointer path starting at the [Base]() of a [Module]() specified by its file name plus a specified base offset, following an optional amount of offsets.
| [ReadString(int, string, int, int[])]()                             | Reads a string with the specified length at the end of a pointer path starting at the [Base]() of a [Module]() specified by its file name plus a specified base offset, following an optional amount of offsets.
| [Reject(int[])]()                                                   | Searches for an optional amount of [MemorySizes](), setting the script's `game` variable `null` when one matches the [MainModule]()'s [MemorySize]().
| [Reject(Module, int[])]()                                           | 
| [Reject(string, int[])]()                                           | 
| [RemoveWatcher(string)]()                                           | 
| [Scan(int, byte[])]()                                               | 
| [Scan(int, string[])]()                                             | 
| [Scan(IntPtr, int, int, string[])]()                                | 
| [Scan(IntPtr, IntPtr, int, string[])]()                             | 
| [Scan(Module, int, byte[])]()                                       | 
| [Scan(Module, int, string[])]()                                     | 
| [Scan(Signature, int)]()                                            | 
| [Scan(Signature, int, int)]()                                       | 
| [Scan(Signature, IntPtr, int, int)]()                               | 
| [Scan(Signature, IntPtr, IntPtr, int)]()                            | 
| [Scan(Signature, Module, int)]()                                    | 
| [Scan(Signature, Module, int, int)]()                               | 
| [Scan(Signature, string, int)]()                                    | 
| [Scan(Signature, string, int, int)]()                               | 
| [Scan(string, int, byte[])]()                                       | 
| [Scan(string, int, string[])]()                                     | 
| [ScanAll(Signature, int)]()                                         | 
| [ScanAll(Signature, int, int)]()                                    | 
| [ScanAll(Signature, IntPtr, int, int)]()                            | 
| [ScanAll(Signature, IntPtr, IntPtr, int)]()                         | 
| [ScanAll(Signature, Module, int)]()                                 | 
| [ScanAll(Signature, Module, int, int)]()                            | 
| [ScanAll(Signature, string, int)]()                                 | 
| [ScanAll(Signature, string, int, int)]()                            | 
| [ScanAllRel(Signature, int)]()                                      | 
| [ScanAllRel(Signature, int, int)]()                                 | 
| [ScanAllRel(Signature, IntPtr, int, int)]()                         | 
| [ScanAllRel(Signature, IntPtr, IntPtr, int)]()                      | 
| [ScanAllRel(Signature, Module, int)]()                              | 
| [ScanAllRel(Signature, Module, int, int)]()                         | 
| [ScanAllRel(Signature, string, int)]()                              | 
| [ScanAllRel(Signature, string, int, int)]()                         | 
| [ScanPages(bool, Signature, int)]()                                 | 
| [ScanPages(Signature, int)]()                                       | 
| [ScanPagesAll(bool, Signature, int)]()                              | 
| [ScanPagesAll(Signature, int)]()                                    | 
| [ScanPagesAllRel(bool, Signature, int)]()                           | 
| [ScanPagesAllRel(Signature, int)]()                                 | 
| [ScanPagesRel(bool, Signature, int)]()                              | 
| [ScanPagesRel(Signature, int)]()                                    | 
| [ScanRel(Signature, int)]()                                         | 
| [ScanRel(Signature, int, int)]()                                    | 
| [ScanRel(Signature, IntPtr, int, int)]()                            | 
| [ScanRel(Signature, IntPtr, IntPtr, int)]()                         | 
| [ScanRel(Signature, Module, int)]()                                 | 
| [ScanRel(Signature, Module, int, int)]()                            | 
| [ScanRel(Signature, string, int)]()                                 | 
| [ScanRel(Signature, string, int, int)]()                            | 
| [StartFileLogger(string, int, int)]()                               | 
| [TryDeref(IntPtr, int, int[])]()                                    | 
| [TryDeref(IntPtr, IntPtr, int[])]()                                 | 
| [TryDeref(IntPtr, Module, int, int[])]()                            | 
| [TryDeref(IntPtr, string, int, int[])]()                            | 
| [TryRead\<T>(T, int, int[])]()                                      | 
| [TryRead\<T>(T, IntPtr, int[])]()                                   | 
| [TryRead\<T>(T, Module, int, int[])]()                              | 
| [TryRead\<T>(T, string, int, int[])]()                              | 
| [TryReadCustom(T, TypeDefinition, int, int[])]()                    | 
| [TryReadCustom(T, TypeDefinition, IntPtr, int[])]()                 | 
| [TryReadCustom(T, TypeDefinition, Module, int, int[])]()            | 
| [TryReadCustom(T, TypeDefinition, string, int, int[])]()            | 
| [TryReadSpan\<T>(T[], int, int, int[])]()                           | 
| [TryReadSpan\<T>(T[], int, int[])]()                                | 
| [TryReadSpan\<T>(T[], int, IntPtr, int[])]()                        | 
| [TryReadSpan\<T>(T[], int, Module, int, int[])]()                   | 
| [TryReadSpan\<T>(T[], int, string, int, int[])]()                   | 
| [TryReadSpan\<T>(T[], IntPtr, int[])]()                             | 
| [TryReadSpan\<T>(T[], Module, int, int[])]()                        | 
| [TryReadSpan\<T>(T[], string, int, int[])]()                        | 
| [TryReadSpanCustom(T[], TypeDefinition, int, int, int[])]()         | 
| [TryReadSpanCustom(T[], TypeDefinition, int, IntPtr, int[])]()      | 
| [TryReadSpanCustom(T[], TypeDefinition, int, Module, int, int[])]() | 
| [TryReadSpanCustom(T[], TypeDefinition, int, string, int, int[])]() | 
| [TryReadString(string, int, ReadStringType, int, int[])]()          | 
| [TryReadString(string, int, ReadStringType, IntPtr, int[])]()       | 
| [TryReadString(string, int, ReadStringType, Module, int, int[])]()  | 
| [TryReadString(string, int, ReadStringType, string, int, int[])]()  | 
| [Update()]()                                                        | 
| [Write\<T>(T, int, int[])]()                                        | 
| [Write\<T>(T, IntPtr, int[])]()                                     | 
| [Write\<T>(T, Module, int, int[])]()                                | 
| [Write\<T>(T, string, int, int[])]()                                | 
| [WriteSpan\<T>(IList, int, int[])]()                                | 
| [WriteSpan\<T>(IList, IntPtr, int[])]()                             | 
| [WriteSpan\<T>(IList, Module, int, int[])]()                        | 
| [WriteSpan\<T>(IList, string, int, int[])]()                        |
