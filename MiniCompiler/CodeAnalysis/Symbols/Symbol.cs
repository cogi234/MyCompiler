namespace MiniCompiler.CodeAnalysis.Symbols
{
    public enum SymbolType
    {
        Type,
        Function,
        Parameter,
        GlobalVariable,
        LocalVariable,
    }
    public abstract class Symbol
    {
        internal Symbol(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public override string? ToString() => Name;

        public abstract SymbolType SymbolType { get; }
    }
}
