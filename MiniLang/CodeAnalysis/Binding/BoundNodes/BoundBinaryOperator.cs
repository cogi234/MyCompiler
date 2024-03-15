using MiniLang.CodeAnalysis.Symbols;
using MiniLang.CodeAnalysis.Syntax;

namespace MiniLang.CodeAnalysis.Binding.BoundNodes
{
    internal sealed class BoundBinaryOperator
    {
        private BoundBinaryOperator(BoundBinaryOperationType operationType, TokenType tokenType, TypeSymbol type)
            : this(operationType, tokenType, type, type, type) { }
        private BoundBinaryOperator(BoundBinaryOperationType operationType, TokenType tokenType, TypeSymbol operandType, TypeSymbol resultType)
            : this(operationType, tokenType, operandType, operandType, resultType) { }
        private BoundBinaryOperator(BoundBinaryOperationType operationType, TokenType tokenType, TypeSymbol leftType, TypeSymbol rightType, TypeSymbol resultType)
        {
            OperationType = operationType;
            TokenType = tokenType;
            LeftType = leftType;
            RightType = rightType;
            ResultType = resultType;
        }

        public BoundBinaryOperationType OperationType { get; }
        public TokenType TokenType { get; }
        public TypeSymbol LeftType { get; }
        public TypeSymbol RightType { get; }
        public TypeSymbol ResultType { get; }

        private static readonly BoundBinaryOperator[] operators =
        {
            //Numbers
            new BoundBinaryOperator(BoundBinaryOperationType.Addition, TokenType.Plus, TypeSymbol.Int),
            new BoundBinaryOperator(BoundBinaryOperationType.Subtraction, TokenType.Minus, TypeSymbol.Int),
            new BoundBinaryOperator(BoundBinaryOperationType.Multiplication, TokenType.Star, TypeSymbol.Int),
            new BoundBinaryOperator(BoundBinaryOperationType.Division, TokenType.ForwardSlash, TypeSymbol.Int),
            new BoundBinaryOperator(BoundBinaryOperationType.Modulo, TokenType.Percent, TypeSymbol.Int),
            new BoundBinaryOperator(BoundBinaryOperationType.BitwiseAnd, TokenType.Ampersand, TypeSymbol.Int),
            new BoundBinaryOperator(BoundBinaryOperationType.BitwiseOr, TokenType.Pipe, TypeSymbol.Int),
            new BoundBinaryOperator(BoundBinaryOperationType.BitwiseXor, TokenType.Caret, TypeSymbol.Int),
            //Booleans
            new BoundBinaryOperator(BoundBinaryOperationType.LogicalAnd, TokenType.AmpersandAmpersand, TypeSymbol.Bool),
            new BoundBinaryOperator(BoundBinaryOperationType.LogicalOr, TokenType.PipePipe, TypeSymbol.Bool),
            new BoundBinaryOperator(BoundBinaryOperationType.BitwiseAnd, TokenType.Ampersand, TypeSymbol.Bool),
            new BoundBinaryOperator(BoundBinaryOperationType.BitwiseOr, TokenType.Pipe, TypeSymbol.Bool),
            new BoundBinaryOperator(BoundBinaryOperationType.BitwiseXor, TokenType.Caret, TypeSymbol.Bool),
            //Strings
            new BoundBinaryOperator(BoundBinaryOperationType.Concatenation, TokenType.Plus, TypeSymbol.String),
            //Comparisons
            new BoundBinaryOperator(BoundBinaryOperationType.Equality, TokenType.EqualEqual, TypeSymbol.Bool),
            new BoundBinaryOperator(BoundBinaryOperationType.Unequality, TokenType.BangEqual, TypeSymbol.Bool),
            new BoundBinaryOperator(BoundBinaryOperationType.Equality, TokenType.EqualEqual, TypeSymbol.Int, TypeSymbol.Bool),
            new BoundBinaryOperator(BoundBinaryOperationType.Unequality, TokenType.BangEqual, TypeSymbol.Int, TypeSymbol.Bool),
            new BoundBinaryOperator(BoundBinaryOperationType.LesserThan, TokenType.LessThan, TypeSymbol.Int, TypeSymbol.Bool),
            new BoundBinaryOperator(BoundBinaryOperationType.LesserThanOrEqual, TokenType.LessThanEqual, TypeSymbol.Int, TypeSymbol.Bool),
            new BoundBinaryOperator(BoundBinaryOperationType.GreaterThan, TokenType.GreaterThan, TypeSymbol.Int, TypeSymbol.Bool),
            new BoundBinaryOperator(BoundBinaryOperationType.GreaterThanOrEqual, TokenType.GreaterThanEqual, TypeSymbol.Int, TypeSymbol.Bool),
            new BoundBinaryOperator(BoundBinaryOperationType.Equality, TokenType.EqualEqual, TypeSymbol.String, TypeSymbol.Bool),
            new BoundBinaryOperator(BoundBinaryOperationType.Unequality, TokenType.BangEqual, TypeSymbol.String, TypeSymbol.Bool),
        };

        public static BoundBinaryOperator? Bind(TokenType tokenType, TypeSymbol leftType, TypeSymbol rightType)
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
