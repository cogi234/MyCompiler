using MyCompiler.CodeAnalysis.Syntax;

namespace MyCompiler.CodeAnalysis.Binding.BoundNodes
{
    internal sealed class BoundBinaryOperator
    {
        private BoundBinaryOperator(BoundBinaryOperationType operationType, TokenType tokenType, Type type)
            : this(operationType, tokenType, type, type, type) { }
        private BoundBinaryOperator(BoundBinaryOperationType operationType, TokenType tokenType, Type operandType, Type resultType)
            : this(operationType, tokenType, operandType, operandType, resultType) { }
        private BoundBinaryOperator(BoundBinaryOperationType operationType, TokenType tokenType, Type leftType, Type rightType, Type resultType)
        {
            OperationType = operationType;
            TokenType = tokenType;
            LeftType = leftType;
            RightType = rightType;
            ResultType = resultType;
        }

        public BoundBinaryOperationType OperationType { get; }
        public TokenType TokenType { get; }
        public Type LeftType { get; }
        public Type RightType { get; }
        public Type ResultType { get; }

        private static BoundBinaryOperator[] operators =
        {
            new BoundBinaryOperator(BoundBinaryOperationType.LogicalAnd, TokenType.AmpersandAmpersand, typeof(bool)),
            new BoundBinaryOperator(BoundBinaryOperationType.LogicalOr, TokenType.PipePipe, typeof(bool)),
            new BoundBinaryOperator(BoundBinaryOperationType.Equality, TokenType.EqualEqual, typeof(bool)),
            new BoundBinaryOperator(BoundBinaryOperationType.Unequality, TokenType.BangEqual, typeof(bool)),

            new BoundBinaryOperator(BoundBinaryOperationType.Addition, TokenType.Plus, typeof(int)),
            new BoundBinaryOperator(BoundBinaryOperationType.Subtraction, TokenType.Minus, typeof(int)),
            new BoundBinaryOperator(BoundBinaryOperationType.Multiplication, TokenType.Star, typeof(int)),
            new BoundBinaryOperator(BoundBinaryOperationType.Division, TokenType.ForwardSlash, typeof(int)),
            new BoundBinaryOperator(BoundBinaryOperationType.Equality, TokenType.EqualEqual, typeof(int), typeof(bool)),
            new BoundBinaryOperator(BoundBinaryOperationType.Unequality, TokenType.BangEqual, typeof(int), typeof(bool)),
        };

        public static BoundBinaryOperator? Bind(TokenType tokenType, Type leftType, Type rightType)
        {
            foreach (BoundBinaryOperator op in operators)
            {
                if (op.TokenType == tokenType && op.LeftType == leftType && op.RightType == rightType)
                    return op;
            }
            return null;
        }
    }
}
