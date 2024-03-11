using MiniLang.CodeAnalysis.Text;

namespace MiniLang.CodeAnalysis.Syntax.SyntaxNodes
{
    public sealed class VariableDeclarationStatementNode : StatementNode
    {
        public VariableDeclarationStatementNode(SyntaxTree syntaxTree, Token keyword, Token identifier, Token? equal,
            ExpressionNode? initializer, Token? semicolon) : base(syntaxTree)
        {
            Keyword = keyword;
            Identifier = identifier;
            Equal = equal;
            Initializer = initializer;
            Semicolon = semicolon;
        }

        public Token Keyword { get; }
        public Token Identifier { get; }
        public Token? Equal { get; }
        public ExpressionNode? Initializer { get; }
        public Token? Semicolon { get; }

        public override NodeType Type => NodeType.VariableDeclarationStatement;

        public override TextSpan Span
        {
            get
            {
                if (Semicolon != null)
                    return TextSpan.FromBounds(Keyword.Span.Start, Semicolon.Span.End);
                if (Initializer != null)
                    return TextSpan.FromBounds(Keyword.Span.Start, Initializer.Span.End);
                return TextSpan.FromBounds(Keyword.Span.Start, Identifier.Span.End);
            }
        }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            if (Initializer == null)
                yield break;
            else
                yield return Initializer;
        }

        public override Token GetLastToken()
        {
            if (Semicolon != null)
                return Semicolon;
            if (Initializer != null)
                return Initializer.GetLastToken();
            return Identifier;
        }
    }
}
