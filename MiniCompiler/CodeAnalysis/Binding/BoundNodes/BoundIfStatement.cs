
namespace MiniCompiler.CodeAnalysis.Binding.BoundNodes
{
    internal sealed class BoundElseStatement : BoundStatement
    {
        public BoundElseStatement(BoundStatement statement)
        {
            Statement = statement;
        }

        public BoundStatement Statement { get; }

        public override BoundNodeType BoundNodeType => BoundNodeType.ElseStatement;

        public override IEnumerable<BoundNode> GetChildren()
        {
            yield return Statement;
        }
    }
    internal sealed class BoundIfStatement : BoundStatement
    {
        public BoundIfStatement(BoundExpression condition, BoundStatement ifStatement, BoundElseStatement? elseStatement)
        {
            Condition = condition;
            IfStatement = ifStatement;
            ElseStatement = elseStatement;
        }

        public BoundExpression Condition { get; }
        public BoundStatement IfStatement { get; }
        public BoundElseStatement? ElseStatement { get; }

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
