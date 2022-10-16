namespace AslHelp.Data.AutoSplitter;

internal static partial class Actions
{
    public record Action
    {
        public Action(string name)
            : this("", name, 0) { }

        public Action(string body, string name, int line)
        {
            Body = body;
            Name = name;
            Line = line;
        }

        public string Body { get; private set; }
        public string Name { get; }
        public int Line { get; }

        public void Append(string content)
        {
            Body = $"{Body}{content}";
            Update();

            Debug.Info($"    => Added the following code to the end of {Name}:");
            Debug.Info($"       `{content}`");
        }

        public void Prepend(string content)
        {
            Body = $"{content}{Body}";
            Update();

            Debug.Info($"    => Added the following code to the beginning of {Name}:");
            Debug.Info($"       `{content}`");
        }

        private void Update()
        {
            object method = Activator.CreateInstance(_aslMethodType, Body, Name, Line);
            _aslMethods.SetFieldValue(Name, method, ReflectionExt.PUBLIC_INSTANCE);
        }
    }
}
