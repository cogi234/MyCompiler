using System.Collections.Immutable;

namespace MiniCompiler.CodeAnalysis.Symbols
{
    public sealed class FunctionSymbol : Symbol
    {
        public FunctionSymbol(string name, ImmutableArray<ParameterSymbol> parameters, TypeSymbol returnType) : base(name)
        {
            Parameters = parameters;
            ReturnType = returnType;
        }

        public override SymbolType SymbolType => SymbolType.Function;

        public ImmutableArray<ParameterSymbol> Parameters { get; }
        public TypeSymbol ReturnType { get; }
    }
}
