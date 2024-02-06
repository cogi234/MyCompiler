using MyCompiler.CodeAnalysis.Binding.BoundNodes;
using MyCompiler.CodeAnalysis.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompiler.CodeAnalysis.Binding
{
    internal sealed class BoundTree
    {
        public BoundTree(IEnumerable<string> diagnostics, BoundExpression root)
        {
            Diagnostics = diagnostics.ToArray();
            Root = root;
        }

        public static BoundTree Bind(SyntaxTree syntaxTree)
        {
            Binder binder = new Binder(syntaxTree);
            return binder.Bind();
        }

        public IReadOnlyList<string> Diagnostics { get; }
        public BoundExpression Root { get; }

    }
}
