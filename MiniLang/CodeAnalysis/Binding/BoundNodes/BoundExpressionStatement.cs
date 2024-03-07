
namespace MiniCompiler.CodeAnalysis.Binding.BoundNodes
{
    internal sealed class BoundExpressionStatement : BoundStatement
    {
        public BoundExpressionStatement(BoundExpression expression)
        {
            Expression = expression;
        }

        public BoundExpression Expression { get; }

        public override BoundNodeType BoundNodeType => BoundNodeType.ExpressionStatement;

        public override IEnumerable<BoundNode> GetChildren()
        {
            yield return Expression;
        }
    }
}
