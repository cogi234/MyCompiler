namespace MiniCompiler.CodeAnalysis.Symbols
{
    public sealed class ParameterSymbol : VariableSymbol
    {
        internal ParameterSymbol(string name, TypeSymbol type) : base(name, isReadOnly: true, type)
        {
        }

        public override SymbolType SymbolType => SymbolType.Parameter;
    }
}
