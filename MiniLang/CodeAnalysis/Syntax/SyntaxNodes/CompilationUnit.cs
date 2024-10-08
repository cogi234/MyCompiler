﻿using MiniLang.CodeAnalysis.Text;
using System.Collections.Immutable;

namespace MiniLang.CodeAnalysis.Syntax.SyntaxNodes
{
    public sealed class CompilationUnit : SyntaxNode
    {
        public CompilationUnit(SyntaxTree syntaxTree, ImmutableArray<MemberNode> members) : base(syntaxTree)
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
