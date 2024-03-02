
namespace MiniCompiler.CodeAnalysis.Binding.BoundNodes
{
    internal sealed class BoundDoWhileStatement : BoundStatement
    {
        public BoundDoWhileStatement(BoundStatement body, BoundExpression condition)
        {
            Body = body;
            Condition = condition;
        }

        public BoundStatement Body { get; }
        public BoundExpression Condition { get; }

        public override BoundNodeType BoundNodeType => BoundNodeType.DoWhileStatement;

        public override IEnumerable<BoundNode> GetChildren()
        {
            yield return Body;
            yield return Condition;
        }
    }
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
