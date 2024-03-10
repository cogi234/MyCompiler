using MiniLang.CodeAnalysis.Symbols;

namespace MiniLang.CodeAnalysis.Binding.BoundNodes
{
    internal sealed class BoundErrorExpression : BoundExpression
    {
        public override TypeSymbol Type => TypeSymbol.Error;
        public override BoundNodeType BoundNodeType => BoundNodeType.ErrorExpression;

        public override IEnumerable<BoundNode> GetChildren()
        {
            return Enumerable.Empty<BoundNode>();
        }
    }
}