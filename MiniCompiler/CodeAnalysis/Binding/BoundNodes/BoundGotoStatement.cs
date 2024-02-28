namespace MiniCompiler.CodeAnalysis.Binding.BoundNodes
{
    internal sealed class BoundGotoStatement : BoundStatement
    {
        public BoundGotoStatement(LabelSymbol label)
        {
            Label = label;
        }

        public LabelSymbol Label { get; }

        public override BoundNodeType BoundNodeType => BoundNodeType.GotoStatement;

        public override IEnumerable<BoundNode> GetChildren()
        {
            return Enumerable.Empty<BoundNode>();
        }
    }
}
