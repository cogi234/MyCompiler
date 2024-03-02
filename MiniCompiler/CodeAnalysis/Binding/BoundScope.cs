using MiniCompiler.CodeAnalysis.Symbols;
using System.Collections.Immutable;

namespace MiniCompiler.CodeAnalysis.Binding
{
    internal sealed class BoundScope
    {
        private Dictionary<string, VariableSymbol> variables = new Dictionary<string, VariableSymbol>();
        private Dictionary<string, FunctionSymbol> functions = new Dictionary<string, FunctionSymbol>();

        public BoundScope(BoundScope? parent)
        {
            Parent = parent;
        }

        public BoundScope? Parent { get; }

        public bool TryDeclareVariable(VariableSymbol variable)
        {
            if (variables.ContainsKey(variable.Name))
                return false;

            variables.Add(variable.Name, variable);
            return true;
        }
        public bool TryLookupVariable(string name, out VariableSymbol? variable)
        {
            if (variables.TryGetValue(name, out variable))
                return true;
            if (Parent != null)
                return Parent.TryLookupVariable(name, out variable);
            return false;
        }
        public ImmutableArray<VariableSymbol> GetDeclaredVariables()
        {
            return variables.Values.ToImmutableArray();
        }

        public bool TryDeclareFunction(FunctionSymbol function)
        {
            if (functions.ContainsKey(function.Name))
                return false;

            functions.Add(function.Name, function);
            return true;
        }
        public bool TryLookupFunction(string name, out FunctionSymbol? function)
        {
            if (functions.TryGetValue(name, out function))
                return true;
            if (Parent != null)
                return Parent.TryLookupFunction(name, out function);
            return false;
        }
        public ImmutableArray<FunctionSymbol> GetDeclaredFunctions()
        {
            return functions.Values.ToImmutableArray();
        }
    }
}
