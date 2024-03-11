using MiniLang.CodeAnalysis.Text;

namespace MiniLang.CodeAnalysis.Syntax.SyntaxNodes
{
    public sealed class ExpressionStatementNode : StatementNode
    {
        public ExpressionStatementNode(SyntaxTree syntaxTree, ExpressionNode expression, Token semicolon) : base(syntaxTree)
        {
            Expression = expression;
            Semicolon = semicolon;
        }

        public ExpressionNode Expression { get; }
        public Token Semicolon { get; }

        public override NodeType Type => NodeType.ExpressionStatement;

        public override TextSpan Span => TextSpan.FromBounds(Expression.Span.Start, Semicolon.Span.End);

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Expression;
        }

        public override Token GetLastToken()
        {
            return Semicolon;
        }
    }
}
