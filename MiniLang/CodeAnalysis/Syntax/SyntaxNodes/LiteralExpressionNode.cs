using MiniLang.CodeAnalysis.Text;

namespace MiniLang.CodeAnalysis.Syntax.SyntaxNodes
{
    public sealed class LiteralExpressionNode : ExpressionNode
    {
        public LiteralExpressionNode(SyntaxTree syntaxTree, Token literalToken)
            : this(syntaxTree, literalToken, literalToken.Value!) { }

        public LiteralExpressionNode(SyntaxTree syntaxTree, Token literalToken, object value) : base(syntaxTree)
        {
            LiteralToken = literalToken;
            Value = value;
        }

        public override NodeType Type => NodeType.LiteralExpression;
        public Token LiteralToken { get; }
        public object Value { get; }

        public override TextSpan Span => LiteralToken.Span;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return Enumerable.Empty<SyntaxNode>();
        }

        public override Token GetLastToken()
        {
            return LiteralToken;
        }
    }
}
