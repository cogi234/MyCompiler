
namespace MiniCompiler.CodeAnalysis.Binding.BoundNodes
{
    internal sealed class BoundForStatement : BoundStatement
    {
        public BoundForStatement(BoundVariableDeclarationStatement? declaration, BoundExpression condition,
            BoundAssignmentExpression? increment, BoundStatement statement)
        {
            Declaration = declaration;
            Condition = condition;
            Increment = increment;
            Statement = statement;
        }

        public BoundVariableDeclarationStatement? Declaration { get; }
        public BoundExpression Condition { get; }
        public BoundAssignmentExpression? Increment { get; }
        public BoundStatement Statement { get; }

        public override BoundNodeType BoundNodeType => BoundNodeType.ForStatement;

        public override IEnumerable<BoundNode> GetChildren()
        {
            if (Declaration != null)
                yield return Declaration;
            yield return Condition;
            if (Increment != null)
                yield return Increment;
            yield return Statement;
        }
    }
}
