namespace MiniCompiler.CodeAnalysis.Binding.BoundNodes
{
    internal abstract class BoundExpression : BoundNode
    {
        public abstract Type Type { get; }
    }
}
