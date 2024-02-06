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


        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Operand;
        }
    }
}
