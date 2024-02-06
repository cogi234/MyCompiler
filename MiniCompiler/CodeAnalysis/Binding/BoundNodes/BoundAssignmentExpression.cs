namespace MiniCompiler.CodeAnalysis.Binding.BoundNodes
{
    internal class BoundAssignmentExpression : BoundExpression
    {
        public BoundAssignmentExpression(string name, BoundExpression boundExpression)
        {
            Name = name;
            Expression = boundExpression;
        }

        public string Name { get; }
        public BoundExpression Expression { get; }
        public override Type Type => Expression.Type;
        public override BoundNodeType BoundNodeType => BoundNodeType.AssignmentExpression;


        public override IEnumerable<BoundNode> GetChildren()
        {
            yield return Expression;
        }
    }
}