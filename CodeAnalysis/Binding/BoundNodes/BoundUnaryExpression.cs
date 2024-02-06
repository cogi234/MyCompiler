namespace MyCompiler.CodeAnalysis.Binding.BoundNodes
{
    internal enum BoundUnaryOperationType
    {
        Identity,
        Negation,
        LogicalNegation
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
}
