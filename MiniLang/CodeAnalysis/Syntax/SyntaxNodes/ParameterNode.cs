using MiniLang.CodeAnalysis.Text;

namespace MiniLang.CodeAnalysis.Syntax.SyntaxNodes
{
    public sealed class ParameterNode : SyntaxNode
    {
        public ParameterNode(Token typeKeyword, Token identifier)
        {
            TypeKeyword = typeKeyword;
            Identifier = identifier;
        }

        public Token TypeKeyword { get; }
        public Token Identifier { get; }

        public override NodeType Type => NodeType.Parameter;
        public override TextSpan Span => TextSpan.FromBounds(TypeKeyword.Span.Start, Identifier.Span.End);
        public override IEnumerable<SyntaxNode> GetChildren() => Enumerable.Empty<SyntaxNode>();

        public override Token GetLastToken() => Identifier;
    }
}
