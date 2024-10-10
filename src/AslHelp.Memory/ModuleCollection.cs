using System;
using System.Collections.Generic;
using System.Diagnostics;

using AslHelp.Collections;
using AslHelp.Memory.Extensions;

namespace AslHelp.Memory;

public sealed class ModuleCollection : KeyedCollection<string, Module>
{
    private readonly Process _process;

    public ModuleCollection(Process process)
        : base(StringComparer.InvariantCultureIgnoreCase)
    {
        _process = process;
    }

    public override IEnumerator<Module> GetEnumerator()
    {
        return _process.GetModules().GetEnumerator();
    }

    protected override string GetKey(Module value)
    {
        return value.Name;
    }

    protected override string KeyNotFoundMessage(string key)
    {
        return $"The given module '{key}' was not present in the target process.";
    }
}
