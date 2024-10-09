using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using AslHelp.Collections.Errors;
using AslHelp.Shared.Results;

using InputField = (string TypeName, string Name, uint Alignment);
using InputStruct = (string Name, string? Super, (string Type, string Name, uint Alignment)[] Fields);

namespace AslHelp.Memory.NativeStructs;

internal sealed partial class NativeStructCollectionInitializer
{
    private readonly Dictionary<string, InputField[]> _genericDefinitions = [];
    private readonly Dictionary<string, NativeStruct> _structCache = [];

    private static readonly Regex _genericRegex = new(@"^(\w+)<(?:(\w+(?:<.+?>)?\*?)(?:,\s?)?)+>$", RegexOptions.Compiled);
    private static readonly Regex _arrayRegex = new(@"^(\w+\*?)\[(\d*)\]$", RegexOptions.Compiled);
    private static readonly Regex _bitFieldRegex = new(@"^(\w+):(\d*)$", RegexOptions.Compiled);

    private Result<(uint Size, uint Align, uint BitfieldSize)> GetTypeSize(string type)
    {
        if (IsNativeType(type, out uint size))
        {
            return (size, size, 0);
        }

        if (_structCache.TryGetValue(type, out var s))
        {
            return (s.Size, s.Alignment, 0);
        }

        if (_genericRegex.Match(type) is { Groups: [_, { Value: string gName }, { Captures: CaptureCollection gParams }] })
        {
            if (!_genericDefinitions.TryGetValue(gName, out var fields))
            {
                return NativeStructInitializationError.GenericDefinitionNotFound(gName);
            }

            var map = gParams.Cast<Capture>()
                .Select((c, i) => (Index: i, Name: c.Value))
                .ToDictionary(t => $"T{t.Index}", t => t.Name);

            return
                GenerateStruct(gName, null, fields, map)
                .Map<(uint, uint, uint)>(static s => (s.Size, s.Alignment, 0));
        }

        if (_arrayRegex.Match(type) is { Groups: [_, { Value: string arrayType }, { Value: string strLength }] })
        {
            if (!uint.TryParse(strLength, out uint length))
            {
                return NativeStructInitializationError.ArrayLengthMustBeUnsignedInteger(type, strLength);
            }

            return
                GetTypeSize(arrayType)
                .Map<(uint, uint, uint)>(r => (r.Size * length, r.Align, 0));
        }

        if (_bitFieldRegex.Match(type) is { Groups: [_, { Value: string bitfieldType }, { Value: string strBitfieldSize }] })
        {
            if (!IsIntegerType(bitfieldType, out size))
            {
                return NativeStructInitializationError.BitfieldTypeMustBeInteger(bitfieldType);
            }

            if (!uint.TryParse(strBitfieldSize, out uint bitfieldSize))
            {
                return NativeStructInitializationError.BitfieldSizeMustBeUnsignedInteger(type, strBitfieldSize);
            }

            return (size, size, bitfieldSize);
        }

        return (_pointerSize, _pointerSize, 0);
    }

    private bool IsNativeType(string type, out uint size)
    {
        size = type switch
        {
            "byte" or "sbyte" or "bool" => 0x01,
            "ushort" or "short" or "char" => 0x02,
            "uint" or "int" or "float" => 0x04,
            "ulong" or "long" or "double" => 0x08,
            "decimal" => 0x10,
            "nint" or "nuint" => _pointerSize,
            _ => 0
        };

        return size != 0;
    }

    private bool IsIntegerType(string type, out uint size)
    {
        size = type switch
        {
            "byte" or "sbyte" => 0x01,
            "ushort" or "short" => 0x02,
            "uint" or "int" => 0x04,
            "ulong" or "long" => 0x08,
            _ => 0
        };

        return size != 0;
    }

    public static uint Align(uint offset, uint alignment)
    {
        if (alignment <= 0)
        {
            return 0;
        }

        uint mod = offset % alignment;
        if (mod > 0)
        {
            offset += alignment - mod;
        }

        return offset;
    }
}
