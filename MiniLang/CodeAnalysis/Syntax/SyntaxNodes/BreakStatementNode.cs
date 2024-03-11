﻿using MiniLang.CodeAnalysis.Text;

namespace MiniLang.CodeAnalysis.Syntax.SyntaxNodes
{
    public sealed class BreakStatementNode : StatementNode
    {
        public BreakStatementNode(SyntaxTree syntaxTree, Token keyword, Token semicolon) : base(syntaxTree)
        {
            Keyword = keyword;
            Semicolon = semicolon;
        }

        public Token Keyword { get; }
        public Token Semicolon { get; }

        public override NodeType Type => NodeType.BreakStatement;
        public override TextSpan Span => TextSpan.FromBounds(Keyword.Span.Start, Semicolon.Span.End);
        public override IEnumerable<SyntaxNode> GetChildren() => Enumerable.Empty<SyntaxNode>();
        public override Token GetLastToken() => Semicolon;
    }
}
