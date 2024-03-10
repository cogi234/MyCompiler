using MiniLang.CodeAnalysis.Text;

namespace MiniLang.CodeAnalysis.Syntax.SyntaxNodes
{
    public sealed class ReturnStatementNode : StatementNode
    {
        public ReturnStatementNode(Token keyword, ExpressionNode? expression, Token semicolon)
        {
            Keyword = keyword;
            Expression = expression;
            Semicolon = semicolon;
        }

        public Token Keyword { get; }
        public ExpressionNode? Expression { get; }
        public Token Semicolon { get; }

        public override NodeType Type => NodeType.ReturnStatement;
        public override TextSpan Span => TextSpan.FromBounds(Keyword.Span.Start, Semicolon.Span.End);
        public override IEnumerable<SyntaxNode> GetChildren()
        {
            if (Expression != null)
                yield return Expression;
            else
                yield break;
        }
        public override Token GetLastToken() => Semicolon;
    }
}
