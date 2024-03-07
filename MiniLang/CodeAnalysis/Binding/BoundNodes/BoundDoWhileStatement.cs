
namespace MiniCompiler.CodeAnalysis.Binding.BoundNodes
{
    internal sealed class BoundDoWhileStatement : BoundLoopStatement
    {
        public BoundDoWhileStatement(BoundStatement body, BoundExpression condition,
            BoundLabel breakLabel, BoundLabel continueLabel) : base(breakLabel, continueLabel)
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
}
