namespace MiniCompiler.CodeAnalysis.Symbols
{
    public enum SymbolType
    {
        Variable,
        Type,
        Function,
        Parameter,
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
