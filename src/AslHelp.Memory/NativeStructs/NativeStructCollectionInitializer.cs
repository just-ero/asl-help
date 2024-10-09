using System;
using System.Collections.Generic;

using AslHelp.Shared.Results;

namespace AslHelp.Memory.NativeStructs;

internal sealed partial class NativeStructCollectionInitializer
{
    private readonly byte _pointerSize;

    private uint _offset, _bitOffset;
    private uint _pAlignment;

    private NativeStructCollectionInitializer(bool is64Bit)
    {
        _pointerSize = (byte)(is64Bit ? 0x8 : 0x4);
    }

    public static Result<NativeStructCollection> Generate(CollectedInput input, bool is64Bit)
    {
        NativeStructCollectionInitializer initializer = new(is64Bit);
        Result<NativeStructCollection> reflection = new NativeStructCollection();

        foreach (InputStruct inStruct in input)
        {
            reflection = reflection
                .AndThen(r =>
                {
                    return
                        initializer.GenerateStruct(inStruct.Name, inStruct.Super, inStruct.Fields)
                        .Map(s =>
                        {
                            initializer._structCache[s.Name] = s;
                            r[s.Name] = s;

                            return r;
                        });
                });
        }

        return reflection;
    }

    private Result<NativeStruct> GenerateStruct(
        string name,
        string? super,
        InputField[] fields,
        IReadOnlyDictionary<string, string>? genericMap = default)
    {
        _pAlignment = 0;
        _offset = 0;
        _bitOffset = uint.MaxValue;

        Result<NativeStruct> @struct = new NativeStruct(name, super);

        foreach (InputField inField in fields)
        {
            @struct = @struct
                .AndThen(@struct =>
                {
                    (string typeName, string name, uint forcedAlignment) = inField;

                    if (genericMap?.TryGetValue(typeName, out string? mappedType) is true)
                    {
                        typeName = mappedType;
                    }

                    return
                        GenerateField(typeName, name, forcedAlignment)
                        .Map(f =>
                        {
                            @struct[name] = f;

                            return @struct;
                        });
                });
        }

        return @struct;
    }

    private Result<NativeField> GenerateField(string type, string name, uint forcedAlign)
    {
        return
            GetTypeSize(type)
            .Map<NativeField>(r =>
            {
                uint offset = _offset, size = r.Size, align = forcedAlign > 0 ? forcedAlign : r.Align;
                uint bitfieldSize = r.BitfieldSize, bitOffset = _bitOffset;

                (uint, uint)? bitfield = default;

                if (bitfieldSize > 0)
                {
                    if (bitOffset == uint.MaxValue || align != _pAlignment)
                    {
                        bitOffset = 0;
                        offset = Align(offset, Math.Max(align, _pAlignment));
                    }

                    bitfield = new(bitfieldSize, bitOffset);

                    bitOffset += bitfieldSize;
                    size = bitOffset / 8;
                    bitOffset %= 8;
                }
                else
                {
                    bitOffset = uint.MaxValue;
                    offset = Align(offset, Math.Max(align, _pAlignment));
                }

                _offset = offset + size;
                _pAlignment = align;
                _bitOffset = bitOffset;

                return new(type, name, offset, size, align, bitfield);
            });
    }
}
