namespace MiniCompiler.CodeAnalysis.Binding.BoundNodes
{
    internal class BoundVariableExpression : BoundExpression
    {
        public BoundVariableExpression(VariableSymbol variable)
        {
            Variable = variable;
        }

        public override BoundNodeType BoundNodeType => BoundNodeType.VariableExpression;
        public VariableSymbol Variable { get; }

        public override Type Type => Variable.Type;

        public override IEnumerable<BoundNode> GetChildren()
        {
            return Enumerable.Empty<BoundNode>();
        }
    }
}