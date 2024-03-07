using MiniCompiler.CodeAnalysis.Symbols;

namespace MiniCompiler.CodeAnalysis.Binding.BoundNodes
{
    internal enum BoundBinaryOperationType
    {
        Addition,
        Subtraction,
        Multiplication,
        Division,
        BitwiseOr,
        BitwiseAnd,
        BitwiseXor,

        LogicalAnd,
        LogicalOr,

        Equality,
        Unequality,
        GreaterThan,
        GreaterThanOrEqual,
        LesserThan,
        LesserThanOrEqual,

        Concatenation,
    }
    internal sealed class BoundBinaryExpression : BoundExpression
    {
        public BoundBinaryExpression(BoundExpression left, BoundBinaryOperator binaryOperator, BoundExpression right)
        {
            Left = left;
            BinaryOperator = binaryOperator;
            Right = right;
        }

        public override TypeSymbol Type => BinaryOperator.ResultType;

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
