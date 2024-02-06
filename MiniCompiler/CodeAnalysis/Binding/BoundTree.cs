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
        public BoundTree(DiagnosticBag diagnostics, BoundExpression root)
        {
            Diagnostics = diagnostics;
            Root = root;
        }

        public static BoundTree Bind(SyntaxTree syntaxTree)
        {
            Binder binder = new Binder(syntaxTree);
            return binder.Bind();
        }

        public DiagnosticBag Diagnostics { get; }
        public BoundExpression Root { get; }

    }
}
