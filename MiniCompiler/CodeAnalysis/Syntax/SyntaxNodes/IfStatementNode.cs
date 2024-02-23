using MiniCompiler.CodeAnalysis.Text;

namespace MiniCompiler.CodeAnalysis.Syntax.SyntaxNodes
{
    public sealed class IfStatementNode : StatementNode
    {
        public IfStatementNode(
            Token ifToken, Token openParenthesis, ExpressionNode condition, Token closeParenthesis, StatementNode ifStatement,
            Token? elseToken, StatementNode? elseStatement)
        {
            IfToken = ifToken;
            OpenParenthesis = openParenthesis;
            Condition = condition;
            CloseParenthesis = closeParenthesis;
            IfStatement = ifStatement;
            ElseToken = elseToken;
            ElseStatement = elseStatement;
        }

        public Token IfToken { get; }
        public Token OpenParenthesis { get; }
        public ExpressionNode Condition { get; }
        public Token CloseParenthesis { get; }
        public StatementNode IfStatement { get; }
        public Token? ElseToken { get; }
        public StatementNode? ElseStatement { get; }

        public override NodeType Type => NodeType.IfStatement;

        //If there's no else statement, we take the if statement end
        public override TextSpan Span => TextSpan.FromBounds(IfToken.Span.Start, (ElseStatement ?? IfStatement).Span.End);

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Condition;
            yield return IfStatement;
            if (ElseStatement != null)
                yield return ElseStatement;
        }
    }
}
