using MiniLang.CodeAnalysis.Symbols;

namespace MiniLang.CodeAnalysis.Binding.BoundNodes
{
    internal abstract class BoundExpression : BoundNode
    {
        public abstract TypeSymbol Type { get; }
    }
}
