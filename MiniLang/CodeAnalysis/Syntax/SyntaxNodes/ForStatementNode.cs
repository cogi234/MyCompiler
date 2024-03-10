using MiniLang.CodeAnalysis.Text;

namespace MiniLang.CodeAnalysis.Syntax.SyntaxNodes
{
    // for (var i = 0; i < 10; i = i + 1)
    public sealed class ForStatementNode : StatementNode
    {
        public ForStatementNode(Token forKeyword, Token openParenthesis, VariableDeclarationStatementNode? declaration,
            ExpressionNode condition, AssignmentExpressionNode? increment, Token closeParenthesis, StatementNode statement)
        {
            ForKeyword = forKeyword;
            OpenParenthesis = openParenthesis;
            Declaration = declaration;
            Condition = condition;
            Increment = increment;
            CloseParenthesis = closeParenthesis;
            Statement = statement;
        }

        public Token ForKeyword { get; }
        public Token OpenParenthesis { get; }
        public VariableDeclarationStatementNode? Declaration { get; }
        public ExpressionNode Condition { get; }
        public AssignmentExpressionNode? Increment { get; }
        public Token CloseParenthesis { get; }
        public StatementNode Statement { get; }

        public override NodeType Type => NodeType.ForStatement;

        public override TextSpan Span => TextSpan.FromBounds(ForKeyword.Span.Start, Statement.Span.End);

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            if (Declaration != null)
                yield return Declaration;
            yield return Condition;
            if (Increment != null)
                yield return Increment;
            yield return Statement;
        }

        public override Token GetLastToken()
        {
            return Statement.GetLastToken();
        }
    }
}
