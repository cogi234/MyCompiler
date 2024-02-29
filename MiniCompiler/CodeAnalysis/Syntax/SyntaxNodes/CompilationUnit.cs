using MiniCompiler.CodeAnalysis.Text;
using System.Collections.Immutable;

namespace MiniCompiler.CodeAnalysis.Syntax.SyntaxNodes
{
    public sealed class CompilationUnit : SyntaxNode
    {
        public CompilationUnit(ImmutableArray<StatementNode> statements)
        {
            Statements = statements;
        }

        public ImmutableArray<StatementNode> Statements { get; }

        public override NodeType Type => NodeType.CompilationUnit;

        public override TextSpan Span => TextSpan.FromBounds(Statements.First().Span.Start, Statements.Last().Span.End);


        public override IEnumerable<SyntaxNode> GetChildren()
        {
            foreach (StatementNode statement in Statements)
            {
                yield return statement;
            }
        }

        public override Token GetLastToken()
        {
            return Statements.Last().GetLastToken();
        }
    }
}
