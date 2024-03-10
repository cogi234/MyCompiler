using MiniLang.CodeAnalysis.Text;

namespace MiniLang.CodeAnalysis.Syntax.SyntaxNodes
{
    public sealed class IfStatementNode : StatementNode
    {
        public IfStatementNode(
            Token ifKeyword, Token openParenthesis, ExpressionNode condition, Token closeParenthesis,
            StatementNode body, ElseClauseNode? elseClause)
        {
            IfKeyword = ifKeyword;
            OpenParenthesis = openParenthesis;
            Condition = condition;
            CloseParenthesis = closeParenthesis;
            Body = body;
            ElseClause = elseClause;
        }

        public Token IfKeyword { get; }
        public Token OpenParenthesis { get; }
        public ExpressionNode Condition { get; }
        public Token CloseParenthesis { get; }
        public StatementNode Body { get; }
        public ElseClauseNode? ElseClause { get; }

        public override NodeType Type => NodeType.IfStatement;

        //If there's no else statement, we take the if statement end
        public override TextSpan Span => TextSpan.FromBounds(IfKeyword.Span.Start, (ElseClause ?? Body).Span.End);

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Condition;
            yield return Body;
            if (ElseClause != null)
                yield return ElseClause;
        }

        public override Token GetLastToken()
        {
            if (ElseClause == null)
                return Body.GetLastToken();
            return ElseClause.GetLastToken();
        }
    }
}
