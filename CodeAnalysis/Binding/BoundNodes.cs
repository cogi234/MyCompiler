using MyCompiler.CodeAnalysis.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompiler.CodeAnalysis.Binding
{
    enum BoundNodeType
    {
        LiteralExpression,
        UnaryExpression,
        BinaryExpression
    }
    internal enum BoundUnaryOperationType
    {
        Identity,
        Negation,
        LogicalNegation
    }
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

    internal abstract class BoundNode
    {
        public abstract BoundNodeType BoundNodeType { get; }
        public abstract IEnumerable<BoundNode> GetChildren();
    }

    internal abstract class BoundExpression : BoundNode
    {
        public abstract Type Type { get; }
    }

    internal sealed class BoundLiteralExpression : BoundExpression
    {
        public BoundLiteralExpression(object value)
        {
            Value = value;
        }

        public override Type Type => Value.GetType();

        public override BoundNodeType BoundNodeType => BoundNodeType.LiteralExpression;

        public object Value { get; }

        public override IEnumerable<BoundNode> GetChildren()
        {
            return Enumerable.Empty<BoundNode>();
        }
    }

    internal sealed class BoundUnaryOperator
    {
        private BoundUnaryOperator(BoundUnaryOperationType operationType, TokenType tokenType, Type operandType, Type resultType)
        {
            OperationType = operationType;
            TokenType = tokenType;
            OperandType = operandType;
            ResultType = resultType;
        }

        private BoundUnaryOperator(BoundUnaryOperationType operationType, TokenType tokenType, Type operandType) 
            : this(operationType, tokenType, operandType, operandType)
        {
        }

        public BoundUnaryOperationType OperationType { get; }
        public TokenType TokenType { get; }
        public Type OperandType { get; }
        public Type ResultType { get; }

        private static BoundUnaryOperator[] operators =
        {
            new BoundUnaryOperator(BoundUnaryOperationType.LogicalNegation, TokenType.Bang, typeof(bool)),

            new BoundUnaryOperator(BoundUnaryOperationType.Identity, TokenType.Plus, typeof(int)),
            new BoundUnaryOperator(BoundUnaryOperationType.Negation, TokenType.Minus, typeof(int)),
        };

        public static BoundUnaryOperator? Bind(TokenType tokenType, Type operandType)
        {
            foreach (BoundUnaryOperator op in operators)
            {
                if (op.TokenType == tokenType && op.OperandType == operandType)
                    return op;
            }
            return null;
        }
    }
    internal sealed class BoundUnaryExpression : BoundExpression
    {
        public BoundUnaryExpression(BoundUnaryOperator unaryOperator, BoundExpression operand)
        {
            UnaryOperator = unaryOperator;
            Operand = operand;
        }

        public override Type Type => Operand.Type;

        public override BoundNodeType BoundNodeType => BoundNodeType.UnaryExpression;

        public BoundUnaryOperator UnaryOperator { get; }
        public BoundExpression Operand { get; }

        public override IEnumerable<BoundNode> GetChildren()
        {
            yield return Operand;
        }
    }

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
            new BoundBinaryOperator(BoundBinaryOperationType.Equality, TokenType.EqualEqual, typeof(int), typeof(bool)),
            new BoundBinaryOperator(BoundBinaryOperationType.Unequality, TokenType.BangEqual, typeof(int), typeof(bool)),

            new BoundBinaryOperator(BoundBinaryOperationType.Addition, TokenType.Plus, typeof(int)),
            new BoundBinaryOperator(BoundBinaryOperationType.Subtraction, TokenType.Minus, typeof(int)),
            new BoundBinaryOperator(BoundBinaryOperationType.Multiplication, TokenType.Star, typeof(int)),
            new BoundBinaryOperator(BoundBinaryOperationType.Division, TokenType.ForwardSlash, typeof(int)),
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
