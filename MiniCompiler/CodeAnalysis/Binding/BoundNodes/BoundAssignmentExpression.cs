namespace MiniCompiler.CodeAnalysis.Binding.BoundNodes
{
    internal class BoundAssignmentExpression : BoundExpression
    {
        public BoundAssignmentExpression(VariableSymbol variable, BoundExpression boundExpression)
        {
            Variable = variable;
            Expression = boundExpression;
        }

        public VariableSymbol Variable { get; }
        public BoundExpression Expression { get; }
        public override Type Type => Expression.Type;
        public override BoundNodeType BoundNodeType => BoundNodeType.AssignmentExpression;


        public override IEnumerable<BoundNode> GetChildren()
        {
            yield return Expression;
        }
    }
}