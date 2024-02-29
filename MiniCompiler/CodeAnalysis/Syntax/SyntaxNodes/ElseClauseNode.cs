using MiniCompiler.CodeAnalysis.Text;

namespace MiniCompiler.CodeAnalysis.Syntax.SyntaxNodes
{
    public sealed class ElseClauseNode : StatementNode
    {
        public ElseClauseNode(Token elseKeyword, StatementNode statement)
        {
            ElseKeyword = elseKeyword;
            Statement = statement;
        }

        public Token ElseKeyword { get; }
        public StatementNode Statement { get; }

        public override NodeType Type => NodeType.ElseClause;

        public override TextSpan Span => TextSpan.FromBounds(ElseKeyword.Span.Start, Statement.Span.End);

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Statement;
        }

        public override Token GetLastToken()
        {
            return Statement.GetLastToken();
        }
    }
}
