namespace MiniLang.CodeAnalysis.Binding.BoundNodes
{
    internal class BoundReturnStatement : BoundStatement
    {
        public BoundReturnStatement(BoundExpression? expression)
        {
            Expression = expression;
        }

        public BoundExpression? Expression { get; }

        public override BoundNodeType BoundNodeType => BoundNodeType.ReturnStatement;
        public override IEnumerable<BoundNode> GetChildren()
        {
            if (Expression != null)
                yield return Expression;
            else
                yield break;
        }
    }
}