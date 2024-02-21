using MiniCompiler.CodeAnalysis.Text;
using System.Collections.Immutable;

namespace MiniCompiler.CodeAnalysis.Syntax.SyntaxNodes
{
    public sealed class BlockStatementNode : StatementNode
    {
        public BlockStatementNode(Token openBrace, ImmutableArray<StatementNode> statements, Token closeBrace)
        {
            OpenBrace = openBrace;
            Statements = statements;
            CloseBrace = closeBrace;
        }

        public Token OpenBrace { get; }
        public ImmutableArray<StatementNode> Statements { get; }
        public Token CloseBrace { get; }

        public override NodeType Type => NodeType.BlockStatement;

        public override TextSpan Span => TextSpan.FromBounds(OpenBrace.Span.Start, CloseBrace.Span.End);

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            foreach (StatementNode statement in Statements)
            {
                yield return statement;
            }
        }
    }
}
