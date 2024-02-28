
namespace MiniCompiler.CodeAnalysis.Binding.BoundNodes
{
    internal sealed class BoundWhileStatement : BoundStatement
    {
        public BoundWhileStatement(BoundExpression condition, BoundStatement body)
        {
            Condition = condition;
            Body = body;
        }

        public BoundExpression Condition { get; }
        public BoundStatement Body { get; }

        public override BoundNodeType BoundNodeType => BoundNodeType.WhileStatement;

        public override IEnumerable<BoundNode> GetChildren()
        {
            yield return Condition;
            yield return Body;
        }
    }
}
