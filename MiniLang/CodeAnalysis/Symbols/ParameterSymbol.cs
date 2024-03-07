namespace MiniCompiler.CodeAnalysis.Symbols
{
    public sealed class ParameterSymbol : LocalVariableSymbol
    {
        internal ParameterSymbol(string name, TypeSymbol type, object? defaultValue = null) : base(name, isReadOnly: true, type)
        {
            DefaultValue = defaultValue;
        }

        public object? DefaultValue { get; }

        public override SymbolType SymbolType => SymbolType.Parameter;
    }
}
