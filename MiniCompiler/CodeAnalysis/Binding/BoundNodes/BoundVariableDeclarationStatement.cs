
namespace MiniCompiler.CodeAnalysis.Binding.BoundNodes
{
    internal sealed class BoundVariableDeclarationStatement : BoundStatement
    {
        public BoundVariableDeclarationStatement(VariableSymbol variable, BoundExpression? initializer)
        {
            Variable = variable;
            Initializer = initializer;
        }

        public VariableSymbol Variable { get; }
        public BoundExpression? Initializer { get; }
        public override BoundNodeType BoundNodeType => BoundNodeType.VariableDeclarationStatement;

        public override IEnumerable<BoundNode> GetChildren()
        {
            if (Initializer == null)
                yield break;
            else 
                yield return Initializer;
        }
    }
}