using MiniCompiler.CodeAnalysis.Text;

namespace MiniCompiler.CodeAnalysis.Syntax.SyntaxNodes
{
    public sealed class CompilationUnit : SyntaxNode
    {
        public CompilationUnit(StatementNode statement)
        {
            Statement = statement;
        }

        public StatementNode Statement { get; }

        public override NodeType Type => NodeType.CompilationUnit;

        public override TextSpan Span => Statement.Span;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Statement;
        }
    }
}
