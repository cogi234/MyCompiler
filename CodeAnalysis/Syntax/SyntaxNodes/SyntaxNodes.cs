using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompiler.CodeAnalysis.Syntax.SyntaxNodes
{
    public enum NodeType
    {
        LiteralExpression,
        UnaryExpression,
        BinaryExpression,
        ParenthesizedExpression
    }

    public abstract class SyntaxNode
    {
        public abstract NodeType Type { get; }
        public abstract IEnumerable<SyntaxNode> GetChildren();
    }
}
