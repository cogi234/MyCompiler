using MiniCompiler.CodeAnalysis.Text;

namespace MiniCompiler.CodeAnalysis.Syntax.SyntaxNodes
{
    public sealed class UnaryExpressionNode : ExpressionNode
    {
        public UnaryExpressionNode(Token operatorToken, ExpressionNode operand)
        {
            OperatorToken = operatorToken;
            Operand = operand;
        }

        public override NodeType Type => NodeType.UnaryExpression;
        public Token OperatorToken { get; }
        public ExpressionNode Operand { get; }

        public override TextSpan Span => TextSpan.FromBounds(OperatorToken.Span.Start, Operand.Span.End);

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Operand;
        }

        public override Token GetLastToken()
        {
            return Operand.GetLastToken();
        }
    }
}
