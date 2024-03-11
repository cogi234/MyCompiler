namespace MiniLang.CodeAnalysis.Syntax.SyntaxNodes
{
    public abstract class StatementNode : SyntaxNode
    {
        protected StatementNode(SyntaxTree syntaxTree) : base(syntaxTree)
        {
        }
    }
}
