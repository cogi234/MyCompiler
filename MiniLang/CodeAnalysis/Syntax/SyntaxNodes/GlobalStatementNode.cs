using MiniCompiler.CodeAnalysis.Text;

namespace MiniCompiler.CodeAnalysis.Syntax.SyntaxNodes
{
    public sealed class GlobalStatementNode : MemberNode
    {
        public GlobalStatementNode(StatementNode statement)
        {
            Statement = statement;
        }

        public StatementNode Statement { get; }

        public override NodeType Type => NodeType.GlobalStatement;
        public override TextSpan Span => Statement.Span;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Statement;
        }

        public override Token GetLastToken() => Statement.GetLastToken();
    }
}
