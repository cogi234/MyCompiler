using MiniCompiler.CodeAnalysis.Text;

namespace MiniCompiler.CodeAnalysis.Syntax.SyntaxNodes
{
    public sealed class AssignmentExpressionNode : ExpressionNode
    {
        public AssignmentExpressionNode(Token identifierToken, Token equalsToken, ExpressionNode expression)
        {
            IdentifierToken = identifierToken;
            EqualsToken = equalsToken;
            Expression = expression;
        }

        public override NodeType Type => NodeType.AssignmentExpression;

        public Token IdentifierToken { get; }
        public Token EqualsToken { get; }
        public ExpressionNode Expression { get; }

        public override TextSpan Span => TextSpan.FromBounds(IdentifierToken.Span.Start, Expression.Span.End);

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Expression;
        }
    }
}
