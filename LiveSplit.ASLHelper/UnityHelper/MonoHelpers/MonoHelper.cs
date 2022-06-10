using ASLHelper.UnityHelper;

namespace ASLHelper
{
    public partial class Unity
    {
        private MonoHelper _helper;
    }

    namespace UnityHelper
    {
        public abstract partial class MonoHelper
        {
            public MonoHelper(string type, string version)
            {
                _engine = Xml.Load(type, version);
            }

            #region Fields
            protected readonly Xml _engine;
            protected IntPtr _loadedImages;
            protected readonly Dictionary<string, MonoImage> _imageCache = new();
            #endregion

            #region Abstract Methods
            protected abstract MonoClass GetClass(MonoImage image, uint classToken, int depth = 0);
            protected abstract MonoClass GetClass(MonoImage image, string className, int depth = 0);
            protected abstract IntPtr ScanForImages();
            protected abstract MonoImage GetImage(string name);
            protected abstract IEnumerable<IntPtr> Classes(MonoImage image);
            protected abstract int ClassFieldCount(IntPtr klass);
            protected abstract IntPtr GetStaticAddress(IntPtr klass);
            #endregion

            #region Methods
            public void ClearImages()
            {
                _imageCache.Clear();
            }

            protected MonoClass MakeClass(IntPtr klass)
            {
                return new()
                {
                    NameSpace = ClassNameSpace(klass),
                    Name = ClassName(klass),
                    Address = klass,
                    Static = GetStaticAddress(klass),
                    Fields = GetAllFields(klass)
                };
            }
            #endregion

            #region Classes
            public MonoClass GetClass(string imageName, uint classToken, int depth = 0)
            {
                return GetClass(GetImage(imageName), classToken, depth);
            }

            public MonoClass GetClass(string imageName, string className, int depth = 0)
            {
                return GetClass(GetImage(imageName), className, depth);
            }

            public MonoClass GetParent(MonoClass monoClass)
            {
                return MakeClass(ReadPtr(monoClass.Address + _engine["MonoClass"]["parent"]));
            }

            protected IntPtr ClassParent(IntPtr klass)
            {
                return ReadPtr(klass + _engine["MonoClass"]["parent"]);
            }

            protected IntPtr ClassFromIndex(IntPtr table, int index)
            {
                return ReadPtr(table + (Data.s_Helper.PtrSize * index));
            }

            protected string ClassName(IntPtr klass)
            {
                return ReadStr(ReadPtr(klass + _engine["MonoClass"]["name"]), 128);
            }

            protected string ClassNameSpace(IntPtr klass)
            {
                return ReadStr(ReadPtr(klass + _engine["MonoClass"]["name_space"]), 256);
            }

            protected bool ClassHasFields(IntPtr klass, out IntPtr fields, out int fieldCount)
            {
                fields = ReadPtr(klass + _engine["MonoClass"]["fields"]);
                fieldCount = ClassFieldCount(klass);
                return fields != IntPtr.Zero && fieldCount > 0;
            }
            #endregion

            #region Fields
            protected List<MonoField> GetAllFields(IntPtr klass)
            {
                var fields = new List<MonoField>();
                foreach (var field in Fields(klass))
                {
                    var attrs = FieldAttrs(field);

                    fields.Add(new()
                    {
                        Name = FieldName(field),
                        Offset = FieldOffset(field),
                        IsConst = attrs.HasFlag(MonoFieldAttribute.MONO_FIELD_ATTR_LITERAL),
                        IsStatic = attrs.HasFlag(MonoFieldAttribute.MONO_FIELD_ATTR_STATIC)
                    });
                }

                foreach (var field in fields.OrderBy(f => f.Offset).Where(f => !f.IsConst))
                    Debug.Log(string.Format("    => 0x{0:X3}: {1,-6} {2}", field.Offset, field.IsStatic ? "static" : "", field.Name));

                return fields;
            }

            protected IEnumerable<IntPtr> Fields(IntPtr klass)
            {
                if (!ClassHasFields(klass, out var fields, out var fieldCount))
                {
                    Debug.Log("  => No fields.");
                    yield break;
                }

                Debug.Log("  => Searching for fields...");

                var fieldSize = _engine["MonoClassField"]["size"];
                for (int i = 0; i < fieldCount; i++)
                    yield return fields + (fieldSize * i);
            }

            protected string FieldName(IntPtr field)
            {
                var name = ReadStr(ReadPtr(field + _engine["MonoClassField"]["name"]), 128);
                var split = name.Split('<', '>');
                if (split.Length == 3 && !string.IsNullOrEmpty(split[1]))
                    name = split[1];

                return name;
            }

            protected int FieldOffset(IntPtr field)
            {
                return ReadI32(field + _engine["MonoClassField"]["offset"]);
            }

            protected MonoFieldAttribute FieldAttrs(IntPtr field)
            {
                var monoType = ReadPtr(field + _engine["MonoClassField"]["type"]);
                return Data.s_Helper.Read<MonoFieldAttribute>(monoType + _engine["MonoType"]["attrs"]);
            }
            #endregion

            #region Helpers
            protected IntPtr ReadRel(IntPtr address)
            {
                return address + 0x4 + Data.s_Helper.Read<int>(address);
            }

            protected IntPtr ReadPtr(IntPtr address)
            {
                return Data.s_Helper.Read<IntPtr>(address);
            }

            protected int ReadI32(IntPtr address)
            {
                return Data.s_Helper.Read<int>(address);
            }

            protected uint ReadU32(IntPtr address)
            {
                return Data.s_Helper.Read<uint>(address);
            }

            protected ushort ReadU16(IntPtr address)
            {
                return Data.s_Helper.Read<ushort>(address);
            }

            protected byte ReadI8(IntPtr address)
            {
                return Data.s_Helper.Read<byte>(address);
            }

            protected string ReadStr(IntPtr address, int length)
            {
                return Data.s_Helper.ReadString(length, ReadStringType.UTF8, address);
            }
            #endregion
        }
    }
}