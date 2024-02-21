using MiniCompiler.CodeAnalysis.Text;

namespace MiniCompiler.CodeAnalysis.Syntax.SyntaxNodes
{
    public sealed class NameExpressionNode : ExpressionNode
    {
        public NameExpressionNode(Token identifier)
        {
            Identifier = identifier;
        }

        public Token Identifier { get; }

        public override NodeType Type => NodeType.NameExpression;

        public override TextSpan Span => Identifier.Span;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return Enumerable.Empty<SyntaxNode>();
        }
    }
}
