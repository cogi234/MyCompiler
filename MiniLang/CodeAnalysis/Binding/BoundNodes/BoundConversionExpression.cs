using MiniLang.CodeAnalysis.Symbols;

namespace MiniLang.CodeAnalysis.Binding.BoundNodes
{
    internal class BoundConversionExpression : BoundExpression
    {
        public BoundConversionExpression(TypeSymbol toType, BoundExpression expression)
        {
            Type = toType;
            Expression = expression;
        }

        public BoundExpression Expression { get; }

        public override TypeSymbol Type { get; }
        public override BoundNodeType BoundNodeType => BoundNodeType.ConversionExpression;
        public override IEnumerable<BoundNode> GetChildren()
        {
            yield return Expression;
        }
    }
}