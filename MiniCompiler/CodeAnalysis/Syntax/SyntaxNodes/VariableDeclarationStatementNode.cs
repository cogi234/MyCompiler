using MiniCompiler.CodeAnalysis.Text;

namespace MiniCompiler.CodeAnalysis.Syntax.SyntaxNodes
{
    public sealed class VariableDeclarationStatementNode : StatementNode
    {
        public VariableDeclarationStatementNode(Token keyword, Token identifier, Token? equal, ExpressionNode? initializer, Token semicolon)
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
        public Token Semicolon { get; }

        public override NodeType Type => NodeType.VariableDeclarationStatement;

        public override TextSpan Span => TextSpan.FromBounds(Keyword.Span.Start, Semicolon.Span.End);

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            if (Initializer == null)
                yield break;
            else
                yield return Initializer;
        }
    }
}
