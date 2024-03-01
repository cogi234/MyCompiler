namespace MiniCompiler.CodeAnalysis.Symbols
{
    public enum SymbolType
    {
        Variable,
        Type,
    }
    public abstract class Symbol
    {
        private protected Symbol(string name) {
            Name = name;
        }

        public string Name { get; }
        public override string? ToString() => Name;

        public abstract SymbolType SymbolType { get; }
    }
}
