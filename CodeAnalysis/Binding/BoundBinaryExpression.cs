namespace MyCompiler.CodeAnalysis.Binding
{
    internal enum BoundBinaryOperationType
    {
        Addition,
        Subtraction,
        Multiplication,
        Division,

        LogicalAnd,
        LogicalOr,
        Equality,
        Unequality
    }
    internal sealed class BoundBinaryExpression : BoundExpression
    {
        public BoundBinaryExpression(BoundExpression left, BoundBinaryOperator binaryOperator, BoundExpression right)
        {
            Left = left;
            BinaryOperator = binaryOperator;
            Right = right;
        }

        public override Type Type => Right.Type;

        public override BoundNodeType BoundNodeType => BoundNodeType.BinaryExpression;

        public BoundExpression Left { get; }
        public BoundBinaryOperator BinaryOperator { get; }
        public BoundExpression Right { get; }

        public override IEnumerable<BoundNode> GetChildren()
        {
            yield return Left;
            yield return Right;
        }
    }
}
