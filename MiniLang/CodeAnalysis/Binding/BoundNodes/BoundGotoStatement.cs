namespace MiniLang.CodeAnalysis.Binding.BoundNodes
{
    internal sealed class BoundGotoStatement : BoundStatement
    {
        public BoundGotoStatement(BoundLabel label)
        {
            Label = label;
        }

        public BoundLabel Label { get; }

        public override BoundNodeType BoundNodeType => BoundNodeType.GotoStatement;

        public override IEnumerable<BoundNode> GetChildren()
        {
            return Enumerable.Empty<BoundNode>();
        }
    }
}
