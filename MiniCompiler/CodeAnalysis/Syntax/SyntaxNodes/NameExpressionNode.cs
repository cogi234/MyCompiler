using MiniCompiler.CodeAnalysis.Text;

namespace MiniCompiler.CodeAnalysis.Syntax.SyntaxNodes
{
    public sealed class NameExpressionNode : ExpressionNode
    {
        public NameExpressionNode(Token identifierToken)
        {
            IdentifierToken = identifierToken;
        }

        public Token IdentifierToken { get; }

        public override NodeType Type => NodeType.NameExpression;

        public override TextSpan Span => IdentifierToken.Span;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return Enumerable.Empty<SyntaxNode>();
        }
    }
}
