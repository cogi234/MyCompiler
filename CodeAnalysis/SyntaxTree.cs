using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompiler
{
    internal sealed class SyntaxTree
    {
        public SyntaxTree(IEnumerable<string> diagnostics, ExpressionNode root)
        {
            Diagnostics = diagnostics.ToArray();
            Root = root;
        }

        public static SyntaxTree Parse(string text)
        {
            Parser parser = new Parser(text);
            return parser.Parse();
        }

        public IReadOnlyList<string> Diagnostics { get; }
        public ExpressionNode Root { get; }

    }
}
