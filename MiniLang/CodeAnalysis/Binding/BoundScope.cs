using MiniCompiler.CodeAnalysis.Symbols;
using System.Collections.Immutable;

namespace MiniCompiler.CodeAnalysis.Binding
{
    internal sealed class BoundScope
    {
        private readonly Dictionary<string, Symbol> symbols = new Dictionary<string, Symbol>();

        public BoundScope(BoundScope? parent)
        {
            Parent = parent;
        }

        public BoundScope? Parent { get; }

        public bool TryDeclareSymbol<TSymbol>(TSymbol symbol) where TSymbol : Symbol
        {
            if (symbols.ContainsKey(symbol.Name))
                return false;

            symbols.Add(symbol.Name, symbol);
            return true;
        }
        public bool TryDeclareVariable(VariableSymbol variable) => TryDeclareSymbol(variable);
        public bool TryDeclareFunction(FunctionSymbol function) => TryDeclareSymbol(function);

        public Symbol? TryLookupSymbol(string name)
        {
            if (symbols.TryGetValue(name, out Symbol? symbol))
                return symbol;
            return Parent?.TryLookupSymbol(name);
        }

        public ImmutableArray<TSymbol> GetDeclaredSymbols<TSymbol>() where TSymbol : Symbol
        {
            return symbols.Values.OfType<TSymbol>().ToImmutableArray();
        }
        public ImmutableArray<VariableSymbol> GetDeclaredVariables()
            => GetDeclaredSymbols<VariableSymbol>();
        public ImmutableArray<FunctionSymbol> GetDeclaredFunctions()
            => GetDeclaredSymbols<FunctionSymbol>();
    }
}
