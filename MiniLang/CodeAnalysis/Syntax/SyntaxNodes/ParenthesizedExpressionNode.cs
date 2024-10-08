﻿using MiniLang.CodeAnalysis.Text;

namespace MiniLang.CodeAnalysis.Syntax.SyntaxNodes
{
    public sealed class ParenthesizedExpressionNode : ExpressionNode
    {
        public ParenthesizedExpressionNode(SyntaxTree syntaxTree, Token openParenthesis, ExpressionNode expression,
            Token closeParenthesis) : base(syntaxTree)
        {
            OpenParenthesis = openParenthesis;
            Expression = expression;
            CloseParenthesis = closeParenthesis;
        }

        public Token OpenParenthesis { get; }
        public ExpressionNode Expression { get; }
        public Token CloseParenthesis { get; }

        public override NodeType Type => NodeType.ParenthesizedExpression;

        public override TextSpan Span => TextSpan.FromBounds(OpenParenthesis.Span.Start, CloseParenthesis.Span.End);

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Expression;
        }

        public override Token GetLastToken()
        {
            return CloseParenthesis;
        }
    }
}
