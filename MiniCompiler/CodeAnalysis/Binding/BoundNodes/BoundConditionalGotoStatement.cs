namespace MiniCompiler.CodeAnalysis.Binding.BoundNodes
{
    internal sealed class BoundConditionalGotoStatement : BoundStatement
    {
        public BoundConditionalGotoStatement(LabelSymbol label, BoundExpression condition, bool invertCondition = false)
        {
            Label = label;
            Condition = condition;
            InvertCondition = invertCondition;
        }

        public LabelSymbol Label { get; }
        public BoundExpression Condition { get; }
        public bool InvertCondition { get; }

        public override BoundNodeType BoundNodeType => BoundNodeType.ConditionalGotoStatement;

        public override IEnumerable<BoundNode> GetChildren()
        {
            yield return Condition;
        }
    }
}
