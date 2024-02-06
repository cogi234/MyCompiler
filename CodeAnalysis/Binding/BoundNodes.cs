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
        Negation
    }
    internal enum BoundBinaryOperationType
    {
        Addition,
        Subtraction,
        Multiplication,
        Division
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

    internal sealed class BoundUnaryExpression : BoundExpression
    {
        public BoundUnaryExpression(BoundUnaryOperationType operationType, BoundExpression operand)
        {
            OperationType = operationType;
            Operand = operand;
        }

        public override Type Type => Operand.Type;

        public override BoundNodeType BoundNodeType => BoundNodeType.UnaryExpression;

        public BoundUnaryOperationType OperationType { get; }
        public BoundExpression Operand { get; }

        public override IEnumerable<BoundNode> GetChildren()
        {
            yield return Operand;
        }
    }

    internal sealed class BoundBinaryExpression : BoundExpression
    {
        public BoundBinaryExpression(BoundExpression left, BoundBinaryOperationType operationType, BoundExpression right)
        {
            Left = left;
            OperationType = operationType;
            Right = right;
        }

        public override Type Type => Right.Type;

        public override BoundNodeType BoundNodeType => BoundNodeType.BinaryExpression;

        public BoundExpression Left { get; }
        public BoundBinaryOperationType OperationType { get; }
        public BoundExpression Right { get; }

        public override IEnumerable<BoundNode> GetChildren()
        {
            yield return Left;
            yield return Right;
        }
    }
}
