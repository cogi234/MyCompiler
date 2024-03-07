using MiniCompiler.CodeAnalysis.Symbols;
using System.Collections.Immutable;

namespace MiniCompiler.CodeAnalysis.Binding.BoundNodes
{
    internal class BoundCallExpression : BoundExpression
    {
        public BoundCallExpression(FunctionSymbol function, ImmutableArray<BoundExpression> arguments)
        {
            Function = function;
            Arguments = arguments;
        }

        public FunctionSymbol Function { get; }
        public ImmutableArray<BoundExpression> Arguments { get; }


        public override TypeSymbol Type => Function.ReturnType;
        public override BoundNodeType BoundNodeType => BoundNodeType.CallExpression;
        public override IEnumerable<BoundNode> GetChildren() => Arguments;
    }
}