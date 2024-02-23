using MiniCompiler.CodeAnalysis.Text;

namespace MiniCompiler.CodeAnalysis.Syntax.SyntaxNodes
{
    public sealed class ElseStatementNode : StatementNode
    {
        public ElseStatementNode(Token elseKeyword, StatementNode statement)
        {
            ElseKeyword = elseKeyword;
            Statement = statement;
        }

        public Token ElseKeyword { get; }
        public StatementNode Statement { get; }

        public override NodeType Type => NodeType.ElseStatement;

        public override TextSpan Span => TextSpan.FromBounds(ElseKeyword.Span.Start, Statement.Span.End);

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Statement;
        }
    }
}
