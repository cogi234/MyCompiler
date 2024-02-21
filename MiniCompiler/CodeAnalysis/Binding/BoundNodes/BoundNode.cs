namespace MiniCompiler.CodeAnalysis.Binding.BoundNodes
{
    enum BoundNodeType
    {
        //Statements
        BlockStatement,
        ExpressionStatement,

        //Expressions
        LiteralExpression,
        VariableExpression,
        AssignmentExpression,
        UnaryExpression,
        BinaryExpression,
    }

    internal abstract class BoundNode
    {
        public abstract BoundNodeType BoundNodeType { get; }
        public abstract IEnumerable<BoundNode> GetChildren();
    }
}
