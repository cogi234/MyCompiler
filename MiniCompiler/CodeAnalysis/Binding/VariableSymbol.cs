namespace MiniCompiler.CodeAnalysis.Binding
{
    public sealed class VariableSymbol
    {
        internal VariableSymbol(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; }
        public Type Type { get; }
    }
}
