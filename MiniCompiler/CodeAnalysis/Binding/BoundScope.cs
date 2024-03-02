﻿using MiniCompiler.CodeAnalysis.Symbols;
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

        public bool TryLookupSymbol<TSymbol>(string name, out TSymbol? symbol) where TSymbol : Symbol
        {
            symbol = null;
            if (symbols.TryGetValue(name, out Symbol? declaredSymbol))
            {
                if (declaredSymbol is TSymbol matchingSymbol)
                {
                    symbol = matchingSymbol;
                    return true;
                }
            }

            if (Parent != null)
                return Parent.TryLookupSymbol(name, out symbol);

            return false;
        }
        public bool TryLookupVariable(string name, out VariableSymbol? variable) => TryLookupSymbol(name, out variable);
        public bool TryLookupFunction(string name, out FunctionSymbol? function) => TryLookupSymbol(name, out function);

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
