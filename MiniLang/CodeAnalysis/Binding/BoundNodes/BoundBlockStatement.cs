using System.Collections.Immutable;

namespace MiniLang.CodeAnalysis.Binding.BoundNodes
{
    internal sealed class BoundBlockStatement : BoundStatement
    {
        public BoundBlockStatement(ImmutableArray<BoundStatement> statements)
        {
            Statements = statements;
        }

        public ImmutableArray<BoundStatement> Statements { get; }

        public override BoundNodeType BoundNodeType => BoundNodeType.BlockStatement;

        public override IEnumerable<BoundNode> GetChildren()
        {
            foreach (BoundStatement statement in Statements)
            {
                yield return statement;
            }
        }
    }
}
