using MiniLang.CodeAnalysis.Text;

namespace MiniLang.CodeAnalysis.Syntax.SyntaxNodes
{
    public sealed class DoWhileStatementNode : StatementNode
    {
        public DoWhileStatementNode(SyntaxTree syntaxTree, Token doKeyword, StatementNode statement, Token whileKeyword,
            Token openParenthesis, ExpressionNode condition, Token closeParenthesis, Token semiColon) : base(syntaxTree)
        {
            DoKeyword = doKeyword;
            Statement = statement;
            WhileKeyword = whileKeyword;
            OpenParenthesis = openParenthesis;
            Condition = condition;
            CloseParenthesis = closeParenthesis;
            SemiColon = semiColon;
        }

        public Token DoKeyword { get; }
        public StatementNode Statement { get; }
        public Token WhileKeyword { get; }
        public Token OpenParenthesis { get; }
        public ExpressionNode Condition { get; }
        public Token CloseParenthesis { get; }
        public Token SemiColon { get; }

        public override NodeType Type => NodeType.DoWhileStatement;

        public override TextSpan Span => TextSpan.FromBounds(DoKeyword.Span.Start, CloseParenthesis.Span.End);

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Statement;
            yield return Condition;
        }

        public override Token GetLastToken() => SemiColon;
    }
    public sealed class WhileStatementNode : StatementNode
    {
        public WhileStatementNode(SyntaxTree syntaxTree, Token whileKeyword, Token openParenthesis, ExpressionNode condition,
            Token closeParenthesis, StatementNode statement) : base(syntaxTree)
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
