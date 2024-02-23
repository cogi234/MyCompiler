
namespace MiniCompiler.CodeAnalysis.Binding.BoundNodes
{
    internal sealed class BoundWhileStatement : BoundStatement
    {
        public BoundWhileStatement(BoundExpression condition, BoundStatement statement)
        {
            Condition = condition;
            Statement = statement;
        }

        public BoundExpression Condition { get; }
        public BoundStatement Statement { get; }

        public override BoundNodeType BoundNodeType => BoundNodeType.WhileStatement;

        public override IEnumerable<BoundNode> GetChildren()
        {
            yield return Condition;
            yield return Statement;
        }
    }
}
