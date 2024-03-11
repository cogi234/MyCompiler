using MiniLang.CodeAnalysis.Text;

namespace MiniLang.CodeAnalysis.Syntax.SyntaxNodes
{
    public sealed class FunctionDeclarationNode : MemberNode
    {
        public FunctionDeclarationNode(SyntaxTree syntaxTree, Token typeKeyword, Token identifier, Token openParenthesis,
            SeparatedNodeList<ParameterNode> parameters, Token closeParenthesis, StatementNode body) : base(syntaxTree)
        {
            TypeKeyword = typeKeyword;
            Identifier = identifier;
            OpenParenthesis = openParenthesis;
            Parameters = parameters;
            CloseParenthesis = closeParenthesis;
            Body = body;
        }

        public Token TypeKeyword { get; }
        public Token Identifier { get; }
        public Token OpenParenthesis { get; }
        public SeparatedNodeList<ParameterNode> Parameters { get; }
        public Token CloseParenthesis { get; }
        public StatementNode Body { get; }

        public override NodeType Type => NodeType.FunctionDeclaration;
        public override TextSpan Span => TextSpan.FromBounds(TypeKeyword.Span.Start, Body.Span.End);
        public override IEnumerable<SyntaxNode> GetChildren()
        {
            foreach (ParameterNode parameter in Parameters)
            {
                yield return parameter;
            }
            yield return Body;
        }

        public override Token GetLastToken() => Body.GetLastToken();
    }
}
