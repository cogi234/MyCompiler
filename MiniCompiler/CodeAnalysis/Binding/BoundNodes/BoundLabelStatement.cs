namespace MiniCompiler.CodeAnalysis.Binding.BoundNodes
{
    internal sealed class BoundLabelStatement : BoundStatement
    {
        public BoundLabelStatement(BoundLabel label)
        {
            Label = label;
        }

        public BoundLabel Label { get; }

        public override BoundNodeType BoundNodeType => BoundNodeType.LabelStatement;

        public override IEnumerable<BoundNode> GetChildren()
        {
            return Enumerable.Empty<BoundNode>();
        }
    }
}
