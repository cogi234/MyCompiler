namespace MiniLang.CodeAnalysis.Syntax.SyntaxNodes
{
    public abstract class ExpressionNode : SyntaxNode
    {
        protected ExpressionNode(SyntaxTree syntaxTree) : base(syntaxTree)
        {
        }
    }
}
