
namespace MiniLang.CodeAnalysis.Binding.BoundNodes
{
    internal sealed class BoundForStatement : BoundLoopStatement
    {
        // for (var i = 0; i < 10; i = i + 1)
        public BoundForStatement(BoundVariableDeclarationStatement? declaration, BoundExpression condition,
            BoundAssignmentExpression? increment, BoundStatement body, BoundLabel breakLabel, BoundLabel continueLabel)
            : base(breakLabel, continueLabel)
        {
            Declaration = declaration;
            Condition = condition;
            Increment = increment;
            Body = body;
        }

        public BoundVariableDeclarationStatement? Declaration { get; }
        public BoundExpression Condition { get; }
        public BoundAssignmentExpression? Increment { get; }
        public BoundStatement Body { get; }

        public override BoundNodeType BoundNodeType => BoundNodeType.ForStatement;

        public override IEnumerable<BoundNode> GetChildren()
        {
            if (Declaration != null)
                yield return Declaration;
            yield return Condition;
            if (Increment != null)
                yield return Increment;
            yield return Body;
        }
    }
}
