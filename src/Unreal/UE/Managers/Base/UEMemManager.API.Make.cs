using AslHelp.MemUtils.Exceptions;
using AslHelp.MemUtils.Pointers;
using AslHelp.UE.Models;

namespace AslHelp.UE.Managers;

public abstract partial class UEMemManager
{
    public Pointer<T> Make<T>(string baseObjectName, params object[] fields) where T : unmanaged
    {
        (nint baseAddress, int[] offsets) = ResolveOffsets(true, baseObjectName, fields);
        return new(baseAddress, offsets);
    }

    public Pointer<T> Make<T>(nint baseAddress, string baseObjectName, params object[] fields) where T : unmanaged
    {
        (nint _, int[] offsets) = ResolveOffsets(false, baseObjectName, fields);
        return new(baseAddress, offsets);
    }

    public UEStringPointer MakeString(string baseObjectName, params object[] fields)
    {
        (nint baseAddress, int[] offsets) = ResolveOffsets(true, baseObjectName, fields);
        return new(128, baseAddress, offsets);
    }

    public UEStringPointer MakeString(nint baseAddress, string baseObjectName, params object[] fields)
    {
        (nint _, int[] offsets) = ResolveOffsets(false, baseObjectName, fields);
        return new(128, baseAddress, offsets);
    }

    public UEStringPointer MakeString(int length, string baseObjectName, params object[] fields)
    {
        (nint baseAddress, int[] offsets) = ResolveOffsets(true, baseObjectName, fields);
        return new(length, baseAddress, offsets);
    }

    public UEStringPointer MakeString(int length, nint baseAddress, string baseObjectName, params object[] fields)
    {
        (nint _, int[] offsets) = ResolveOffsets(true, baseObjectName, fields);
        return new(length, baseAddress, offsets);
    }

    public TArrayPointer<T> MakeTArray<T>(string baseObjectName, params object[] fields) where T : unmanaged
    {
        (nint baseAddress, int[] offsets) = ResolveOffsets(true, baseObjectName, fields);

        return new(baseAddress, offsets);
    }

    public TArrayPointer<T> MakeTArray<T>(nint baseAddress, string baseObjectName, params object[] fields) where T : unmanaged
    {
        (nint _, int[] offsets) = ResolveOffsets(false, baseObjectName, fields);

        return new(baseAddress, offsets);
    }

    private (nint, int[]) ResolveOffsets(bool getAddress, string baseObjectName, params object[] fields)
    {
        fields.ThrowIfNullOrEmpty("At least one field name or offset must be provided.");

        if (!UObjects.TryGetValue(baseObjectName, out UObject baseObject))
        {
            string msg =
                $"UObject '{baseObjectName}' could not be found. " +
                $"Ensure correct spelling. Names are case sensitive.";

            throw new NotFoundException(msg);
        }

        nint baseAddress = default;

        if (getAddress)
        {
            baseAddress = GetBaseAddress(baseObject);

            if (baseAddress == 0)
            {
                throw new InvalidAddressException($"Base address was 0.");
            }
        }

        int[] offsets = new int[fields.Length];

        for (int i = 0; i < offsets.Length; i++)
        {
            object arg = fields[i];

            if (arg is int offset)
            {
                offsets[i] = offset;
                continue;
            }

            if (arg is not string fieldName)
            {
                string msg = "Arguments for UnrealManager.Make must consist only of strings or integers.";
                throw new ArgumentException(msg);
            }

            if (!baseObject.TryGetValue(fieldName, out UField next))
            {
                string msg =
                    $"Field '{fieldName}' was not present in '{baseObject.FName.Name}'. " +
                    $"Ensure correct spelling. Names are case sensitive.";

                throw new NotFoundException(msg);
            }

            offsets[i] = next.Offset;
            baseObject = next.Class;
        }

        return (baseAddress, offsets);
    }

    private nint GetBaseAddress(UObject baseObject)
    {
        switch (baseObject.FName.Name)
        {
            case "World":
            {
                if (GWorld == 0)
                {
                    string msg = "Base class was World, but GWorld was not found.";
                    throw new InvalidAddressException(msg);
                }

                return GWorld;
            }

            case "Engine" or "GameEngine":
            {
                if (GEngine == 0)
                {
                    string msg = "Base class was Engine, but GEngine was not found.";
                    throw new InvalidAddressException(msg);
                }

                return GEngine;
            }

            default:
            {
                return baseObject.Address;
            }
        }
    }
}
