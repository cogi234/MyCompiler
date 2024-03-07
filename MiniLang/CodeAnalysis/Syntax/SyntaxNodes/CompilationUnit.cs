using MiniCompiler.CodeAnalysis.Text;
using System.Collections.Immutable;

namespace MiniCompiler.CodeAnalysis.Syntax.SyntaxNodes
{
    public sealed class CompilationUnit : SyntaxNode
    {
        public CompilationUnit(ImmutableArray<MemberNode> members)
        {
            Members = members;
        }

        public ImmutableArray<MemberNode> Members { get; }

        public override NodeType Type => NodeType.CompilationUnit;

        public override TextSpan Span => TextSpan.FromBounds(Members.First().Span.Start, Members.Last().Span.End);


        public override IEnumerable<SyntaxNode> GetChildren()
        {
            foreach (MemberNode member in Members)
            {
                yield return member;
            }
        }

        public override Token GetLastToken()
        {
            return Members.Last().GetLastToken();
        }
    }
}
