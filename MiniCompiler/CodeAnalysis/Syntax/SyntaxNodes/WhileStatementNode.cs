using MiniCompiler.CodeAnalysis.Text;

namespace MiniCompiler.CodeAnalysis.Syntax.SyntaxNodes
{
    public sealed class WhileStatementNode : StatementNode
    {
        public WhileStatementNode(Token whileKeyword, Token openParenthesis, ExpressionNode condition,
            Token closeParenthesis, StatementNode statement)
        {
            WhileKeyword = whileKeyword;
            OpenParenthesis = openParenthesis;
            Condition = condition;
            CloseParenthesis = closeParenthesis;
            Statement = statement;
        }

        public Token WhileKeyword { get; }
        public Token OpenParenthesis { get; }
        public ExpressionNode Condition { get; }
        public Token CloseParenthesis { get; }
        public StatementNode Statement { get; }

        public override NodeType Type => NodeType.WhileStatement;

        public override TextSpan Span => TextSpan.FromBounds(WhileKeyword.Span.Start, Statement.Span.End);

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Condition;
            yield return Statement;
        }

        public override Token GetLastToken()
        {
            return Statement.GetLastToken();
        }
    }
}
