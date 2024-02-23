
namespace MiniCompiler.CodeAnalysis.Binding.BoundNodes
{
    internal sealed class BoundIfStatement : BoundStatement
    {
        public BoundIfStatement(BoundExpression condition, BoundStatement ifStatement, BoundStatement? elseStatement)
        {
            Condition = condition;
            IfStatement = ifStatement;
            ElseStatement = elseStatement;
        }

        public BoundExpression Condition { get; }
        public BoundStatement IfStatement { get; }
        public BoundStatement? ElseStatement { get; }

        public override BoundNodeType BoundNodeType => BoundNodeType.IfStatement;

        public override IEnumerable<BoundNode> GetChildren()
        {
            yield return Condition;
            yield return IfStatement;
            if (ElseStatement != null)
                yield return ElseStatement;
        }
    }
}
