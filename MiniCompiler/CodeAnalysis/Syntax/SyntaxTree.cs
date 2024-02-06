using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniCompiler.CodeAnalysis.Syntax.SyntaxNodes;

namespace MiniCompiler.CodeAnalysis.Syntax
{
    public sealed class SyntaxTree
    {
        public SyntaxTree(DiagnosticBag diagnostics, ExpressionNode root, Token[] tokens)
        {
            Diagnostics = diagnostics;
            Root = root;
            Tokens = tokens;
        }

        public static SyntaxTree Parse(string text)
        {
            Parser parser = new Parser(text);
            return parser.Parse();
        }

        public DiagnosticBag Diagnostics { get; }
        public ExpressionNode Root { get; }
        public IReadOnlyList<Token> Tokens { get; }
    }
}
