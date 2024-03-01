using MiniCompiler.CodeAnalysis.Symbols;

namespace MiniCompiler.CodeAnalysis.Binding.BoundNodes
{
    internal sealed class BoundLabelStatement : BoundStatement
    {
        public BoundLabelStatement(LabelSymbol label)
        {
            Label = label;
        }

        public LabelSymbol Label { get; }

        public override BoundNodeType BoundNodeType => BoundNodeType.LabelStatement;

        public override IEnumerable<BoundNode> GetChildren()
        {
            return Enumerable.Empty<BoundNode>();
        }
    }
}
