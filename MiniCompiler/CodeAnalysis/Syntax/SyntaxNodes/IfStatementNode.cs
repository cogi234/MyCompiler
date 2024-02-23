using MiniCompiler.CodeAnalysis.Text;

namespace MiniCompiler.CodeAnalysis.Syntax.SyntaxNodes
{
    public sealed class IfStatementNode : StatementNode
    {
        public IfStatementNode(
            Token ifKeyword, Token openParenthesis, ExpressionNode condition, Token closeParenthesis,
            StatementNode statement, ElseClauseNode? elseStatement)
        {
            IfKeyword = ifKeyword;
            OpenParenthesis = openParenthesis;
            Condition = condition;
            CloseParenthesis = closeParenthesis;
            Statement = statement;
            ElseStatement = elseStatement;
        }

        public Token IfKeyword { get; }
        public Token OpenParenthesis { get; }
        public ExpressionNode Condition { get; }
        public Token CloseParenthesis { get; }
        public StatementNode Statement { get; }
        public ElseClauseNode? ElseStatement { get; }

        public override NodeType Type => NodeType.IfStatement;

        //If there's no else statement, we take the if statement end
        public override TextSpan Span => TextSpan.FromBounds(IfKeyword.Span.Start, (ElseStatement ?? Statement).Span.End);

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Condition;
            yield return Statement;
            if (ElseStatement != null)
                yield return ElseStatement;
        }
    }
}
