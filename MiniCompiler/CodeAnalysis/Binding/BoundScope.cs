﻿using System.Collections.Immutable;
using MiniCompiler.CodeAnalysis.Symbols;

namespace MiniCompiler.CodeAnalysis.Binding
{
    internal sealed class BoundScope
    {
        private Dictionary<string, VariableSymbol> variables = new Dictionary<string, VariableSymbol>();

        public BoundScope(BoundScope? parent)
        {
            Parent = parent;
        }

        public BoundScope? Parent { get; }

        public bool TryDeclare(VariableSymbol variable)
        {
            if (variables.ContainsKey(variable.Name))
                return false;

            variables.Add(variable.Name, variable);
            return true;
        }

        public bool TryLookup(string name, out VariableSymbol? variable)
        {
            if (variables.TryGetValue(name, out variable))
                return true;
            if (Parent != null)
                return Parent.TryLookup(name, out variable);
            return false;
        }

        public ImmutableArray<VariableSymbol> GetDeclaredVariables()
        {
            return variables.Values.ToImmutableArray();
        }
    }
}
