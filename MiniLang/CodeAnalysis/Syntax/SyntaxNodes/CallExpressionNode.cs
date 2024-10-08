﻿using MiniLang.CodeAnalysis.Text;

namespace MiniLang.CodeAnalysis.Syntax.SyntaxNodes
{
    public sealed class CallExpressionNode : ExpressionNode
    {
        public CallExpressionNode(SyntaxTree syntaxTree, Token identifier, Token openParenthesis,
            SeparatedNodeList<ExpressionNode> arguments, Token closeParenthesis) : base(syntaxTree)
        {
            Identifier = identifier;
            OpenParenthesis = openParenthesis;
            Arguments = arguments;
            CloseParenthesis = closeParenthesis;
        }
        public Token Identifier { get; }
        public Token OpenParenthesis { get; }
        public SeparatedNodeList<ExpressionNode> Arguments { get; }
        public Token CloseParenthesis { get; }

        public override NodeType Type => NodeType.CallExpression;
        public override TextSpan Span => TextSpan.FromBounds(Identifier.Span.Start, CloseParenthesis.Span.End);


        public override IEnumerable<SyntaxNode> GetChildren() => Arguments;
        public override Token GetLastToken() => CloseParenthesis;
    }
}
