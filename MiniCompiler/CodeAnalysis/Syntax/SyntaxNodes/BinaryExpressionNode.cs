namespace MiniCompiler.CodeAnalysis.Syntax.SyntaxNodes
{
    public sealed class BinaryExpressionNode : ExpressionNode
    {
        public BinaryExpressionNode(ExpressionNode left, Token operatorToken, ExpressionNode right)
        {
            Left = left;
            OperatorToken = operatorToken;
            Right = right;
        }

        public override NodeType Type => NodeType.BinaryExpression;
        public ExpressionNode Left { get; }
        public Token OperatorToken { get; }
        public ExpressionNode Right { get; }

        public override TextSpan Span => TextSpan.FromBounds(Left.Span.Start, Right.Span.End);

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Left;
            yield return Right;
        }
    }
}
