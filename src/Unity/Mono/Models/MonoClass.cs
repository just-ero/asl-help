using AslHelp.Collections;
using AslHelp.MemUtils.Exceptions;

namespace AslHelp.Mono.Models;

public class MonoClass : CachedEnumerable<string, MonoField>
{
    internal MonoClass(nint address)
    {
        Address = address;
    }

    internal MonoClass(nint address, nint staticFields)
    {
        Address = address;
        _static = staticFields;
    }

    public nint Address { get; }

    private string _name;
    public string Name => _name ??= Unity.Manager.ClassName(Address).ToValidIdentifierUnity();

    private string _namespace;
    public string Namespace => _namespace ??= Unity.Manager.ClassNamespace(Address).ToValidIdentifierUnity();

    private nint? _static;
    public nint Static => _static ??= Unity.Manager.ClassStaticFields(Address);

    public new int this[string fieldName]
    {
        get
        {
            if (base.TryGetValue(fieldName, out MonoField monoField))
            {
                return monoField.Offset;
            }
            else
            {
                string msg =
                    $"Field '{fieldName}' was not present in '{Name}'. " +
                    $"Ensure correct spelling. Names are case sensitive.";

                throw new NotFoundException(msg);
            }
        }
    }

    public override IEnumerator<MonoField> GetEnumerator()
    {
        foreach (MonoField field in Unity.Manager.EnumerateFields(Address))
        {
            if (Unity.Instance.LoadCanceled)
            {
                yield break;
            }

            yield return field;
        }
    }

    protected override string GetKey(MonoField monoField)
    {
        return monoField.Name;
    }

    internal void DebugAllFields()
    {
        if (!this.Any())
        {
            Debug.Info("    => No fields.");
        }
        else
        {
            IOrderedEnumerable<MonoField> fields =
                this
                .Where(f => !f.IsConst)
                .OrderByDescending(f => f.IsStatic)
                .ThenBy(f => f.Offset);

            foreach (MonoField field in fields)
            {
                Debug.Info($"    => {field}");
            }
        }
    }
}
