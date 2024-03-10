
namespace MiniLang.CodeAnalysis.Binding.BoundNodes
{
    internal sealed class BoundIfStatement : BoundStatement
    {
        public BoundIfStatement(BoundExpression condition, BoundStatement thenStatement, BoundStatement? elseStatement)
        {
            Condition = condition;
            ThenStatement = thenStatement;
            ElseStatement = elseStatement;
        }

        public BoundExpression Condition { get; }
        public BoundStatement ThenStatement { get; }
        public BoundStatement? ElseStatement { get; }

        public override BoundNodeType BoundNodeType => BoundNodeType.IfStatement;

        public override IEnumerable<BoundNode> GetChildren()
        {
            yield return Condition;
            yield return ThenStatement;
            if (ElseStatement != null)
                yield return ElseStatement;
        }
    }
}
