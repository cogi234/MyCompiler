using MiniCompiler.CodeAnalysis.Text;

namespace MiniCompiler.CodeAnalysis.Syntax.SyntaxNodes
{
    public sealed class CompilationUnit : SyntaxNode
    {
        public CompilationUnit(ExpressionNode expression)
        {
            Expression = expression;
        }

        public ExpressionNode Expression { get; }

        public override NodeType Type => NodeType.CompilationUnit;

        public override TextSpan Span => Expression.Span;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Expression;
        }
    }
}
