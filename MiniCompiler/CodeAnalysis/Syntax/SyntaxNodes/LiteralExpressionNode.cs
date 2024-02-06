namespace MiniCompiler.CodeAnalysis.Syntax.SyntaxNodes
{
    public sealed class LiteralExpressionNode : ExpressionNode
    {
        public LiteralExpressionNode(Token literalToken) : this(literalToken, literalToken.Value) { }

        public LiteralExpressionNode(Token literalToken, object value)
        {
            LiteralToken = literalToken;
            Value = value;
        }

        public override NodeType Type => NodeType.LiteralExpression;
        public Token LiteralToken { get; }
        public object Value { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return Enumerable.Empty<SyntaxNode>();
        }
    }
}
