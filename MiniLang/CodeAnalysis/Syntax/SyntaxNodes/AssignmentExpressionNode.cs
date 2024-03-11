using MiniLang.CodeAnalysis.Text;

namespace MiniLang.CodeAnalysis.Syntax.SyntaxNodes
{
    public sealed class AssignmentExpressionNode : ExpressionNode
    {
        public AssignmentExpressionNode(SyntaxTree syntaxTree, Token identifier, Token equalsToken, ExpressionNode expression)
            : base(syntaxTree)
        {
            Identifier = identifier;
            EqualsToken = equalsToken;
            Expression = expression;
        }

        public override NodeType Type => NodeType.AssignmentExpression;

        public Token Identifier { get; }
        public Token EqualsToken { get; }
        public ExpressionNode Expression { get; }

        public override TextSpan Span => TextSpan.FromBounds(Identifier.Span.Start, Expression.Span.End);

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Expression;
        }

        public override Token GetLastToken()
        {
            return Expression.GetLastToken();
        }
    }
}
