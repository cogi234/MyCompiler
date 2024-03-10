using MiniLang.CodeAnalysis.Syntax.SyntaxNodes;
using System.Collections.Immutable;

namespace MiniLang.CodeAnalysis.Symbols
{
    public sealed class FunctionSymbol : Symbol
    {
        public FunctionSymbol(string name, ImmutableArray<ParameterSymbol> parameters, TypeSymbol returnType, FunctionDeclarationNode? declaration = null)
            : base(name)
        {
            Parameters = parameters;
            ReturnType = returnType;
            Declaration = declaration;
        }

        public override SymbolType SymbolType => SymbolType.Function;

        public ImmutableArray<ParameterSymbol> Parameters { get; }
        public TypeSymbol ReturnType { get; }
        public FunctionDeclarationNode? Declaration { get; }
    }
}
