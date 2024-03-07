namespace MiniCompiler.CodeAnalysis.Symbols
{
    public sealed class GlobalVariableSymbol : VariableSymbol
    {
        public GlobalVariableSymbol(string name, bool isReadOnly, TypeSymbol type) : base(name, isReadOnly, type)
        {
        }

        public override SymbolType SymbolType => SymbolType.GlobalVariable;
    }
}
