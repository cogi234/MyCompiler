using MiniCompiler.CodeAnalysis.Binding.BoundNodes;
using MiniCompiler.CodeAnalysis.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
