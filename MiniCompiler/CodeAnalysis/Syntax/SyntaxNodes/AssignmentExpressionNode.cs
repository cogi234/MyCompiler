namespace MiniCompiler.CodeAnalysis.Syntax.SyntaxNodes
{
    public sealed class AssignmentExpressionNode : ExpressionNode
    {
        public AssignmentExpressionNode(NameExpressionNode left, Token equalsToken, ExpressionNode right)
        {
            Left = left;
            EqualsToken = equalsToken;
            Right = right;
        }

        public override NodeType Type => NodeType.AssignmentExpression;
        public NameExpressionNode Left { get; }
        public Token EqualsToken { get; }
        public ExpressionNode Right { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Left;
            yield return Right;
        }
    }
}
