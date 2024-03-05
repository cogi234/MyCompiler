using MiniCompiler.IO;

namespace MiniCompiler.CodeAnalysis.Binding.BoundNodes
{
    enum BoundNodeType
    {
        //Statements
        BlockStatement,
        ExpressionStatement,
        VariableDeclarationStatement,
        LabelStatement,
        GotoStatement,
        ConditionalGotoStatement,
        IfStatement,
        WhileStatement,
        DoWhileStatement,
        ForStatement,

        //Expressions
        ErrorExpression,
        LiteralExpression,
        VariableExpression,
        AssignmentExpression,
        CallExpression,
        ConversionExpression,
        UnaryExpression,
        BinaryExpression,
    }

    internal abstract class BoundNode
    {
        public abstract BoundNodeType BoundNodeType { get; }
        public abstract IEnumerable<BoundNode> GetChildren();

        public override string ToString()
        {
            using (StringWriter writer = new StringWriter())
            {
                this.WriteTo(writer);
                return writer.ToString();
            }
        }
    }
}
