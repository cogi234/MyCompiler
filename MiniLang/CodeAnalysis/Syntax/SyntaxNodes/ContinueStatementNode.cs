using MiniLang.CodeAnalysis.Text;

namespace MiniLang.CodeAnalysis.Syntax.SyntaxNodes
{
    public sealed class ContinueStatementNode : StatementNode
    {
        public ContinueStatementNode(Token keyword, Token semicolon)
        {
            Keyword = keyword;
            Semicolon = semicolon;
        }

        public Token Keyword { get; }
        public Token Semicolon { get; }

        public override NodeType Type => NodeType.ContinueStatement;
        public override TextSpan Span => TextSpan.FromBounds(Keyword.Span.Start, Semicolon.Span.End);
        public override IEnumerable<SyntaxNode> GetChildren() => Enumerable.Empty<SyntaxNode>();
        public override Token GetLastToken() => Semicolon;
    }
}
