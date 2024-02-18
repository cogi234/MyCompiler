using MiniCompiler.CodeAnalysis.Binding.BoundNodes;
using MiniCompiler.CodeAnalysis.Syntax;

namespace MiniCompiler.CodeAnalysis.Binding
{
    internal sealed class BoundTree
    {
        public BoundTree(IEnumerable<Diagnostic> diagnostics, BoundExpression root)
        {
            Diagnostics = diagnostics.ToArray();
            Root = root;
        }

        public static BoundTree Bind(SyntaxTree syntaxTree, Dictionary<VariableSymbol, object> variables)
        {
            Binder binder = new Binder(syntaxTree, variables);
            return binder.Bind();
        }

        public IReadOnlyList<Diagnostic> Diagnostics { get; }
        public BoundExpression Root { get; }

    }
}
