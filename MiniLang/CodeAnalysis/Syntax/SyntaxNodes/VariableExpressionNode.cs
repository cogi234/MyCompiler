using MiniLang.CodeAnalysis.Text;

namespace MiniLang.CodeAnalysis.Syntax.SyntaxNodes
{
    public sealed class VariableExpressionNode : ExpressionNode
    {
        public VariableExpressionNode(Token identifier)
        {
            Identifier = identifier;
        }

        public Token Identifier { get; }

        public override NodeType Type => NodeType.NameExpression;

        public override TextSpan Span => Identifier.Span;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return Enumerable.Empty<SyntaxNode>();
        }

        public override Token GetLastToken()
        {
            return Identifier;
        }
    }
}
