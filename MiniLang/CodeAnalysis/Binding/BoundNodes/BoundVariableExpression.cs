using MiniLang.CodeAnalysis.Symbols;

namespace MiniLang.CodeAnalysis.Binding.BoundNodes
{
    internal sealed class BoundVariableExpression : BoundExpression
    {
        public BoundVariableExpression(VariableSymbol variable)
        {
            Variable = variable;
        }

        public override BoundNodeType BoundNodeType => BoundNodeType.VariableExpression;
        public VariableSymbol Variable { get; }

        public override TypeSymbol Type => Variable.Type;

        public override IEnumerable<BoundNode> GetChildren()
        {
            return Enumerable.Empty<BoundNode>();
        }
    }
}