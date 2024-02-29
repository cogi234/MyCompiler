﻿using MiniCompiler.CodeAnalysis.Text;

namespace MiniCompiler.CodeAnalysis.Syntax.SyntaxNodes
{
    public sealed class ExpressionStatementNode : StatementNode
    {
        public ExpressionStatementNode(ExpressionNode expression, Token semicolon)
        {
            Expression = expression;
            Semicolon = semicolon;
        }

        public ExpressionNode Expression { get; }
        public Token Semicolon { get; }

        public override NodeType Type => NodeType.ExpressionStatement;

        public override TextSpan Span => TextSpan.FromBounds(Expression.Span.Start, Semicolon.Span.End);

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Expression;
        }

        public override Token GetLastToken()
        {
            return Semicolon;
        }
    }
}
