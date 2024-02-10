using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCompiler.CodeAnalysis.Syntax.SyntaxNodes
{
    public enum NodeType
    {
        LiteralExpression,
        NameExpression,
        UnaryExpression,
        BinaryExpression,
        ParenthesizedExpression,
        AssignmentExpression,
    }

    public abstract class SyntaxNode
    {
        public abstract NodeType Type { get; }
        public abstract TextSpan Span { get; }
        public abstract IEnumerable<SyntaxNode> GetChildren();
    }
}
