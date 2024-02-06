namespace MiniCompiler.CodeAnalysis.Binding.BoundNodes
{
    internal class BoundVariableExpression : BoundExpression
    {
        public BoundVariableExpression(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; }
        public override Type Type { get; }

        public override BoundNodeType BoundNodeType => BoundNodeType.VariableExpression;

        public override IEnumerable<BoundNode> GetChildren()
        {
            return Enumerable.Empty<BoundNode>();
        }
    }
}