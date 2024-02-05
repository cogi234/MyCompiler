using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompiler
{
    enum NodeType
    {
        Number,
        BinaryExpression,
        ParenthesizedExpression
    }

    abstract class SyntaxNode
    {
        public abstract NodeType Type { get; }

        public abstract IEnumerable<SyntaxNode> GetChildren();
    }

    abstract class ExpressionNode : SyntaxNode
    {
    }

    sealed class BinaryExpressionNode : ExpressionNode
    {
        public BinaryExpressionNode(ExpressionNode left, Token operatorToken, ExpressionNode right)
        {
            Left = left;
            OperatorToken = operatorToken;
            Right = right;
        }

        public override NodeType Type => NodeType.BinaryExpression;
        public ExpressionNode Left { get; }
        public Token OperatorToken { get; }
        public ExpressionNode Right { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Left;
            yield return Right;
        }
    }

    sealed class ParenthesizedExpressionNode : ExpressionNode
    {
        public ParenthesizedExpressionNode(Token openParenthesis, ExpressionNode expression, Token closeParenthesis)
        {
            OpenParenthesis = openParenthesis;
            Expression = expression;
            CloseParenthesis = closeParenthesis;
        }

        public Token OpenParenthesis { get; }
        public ExpressionNode Expression { get; }
        public Token CloseParenthesis { get; }

        public override NodeType Type => NodeType.ParenthesizedExpression;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Expression;
        }
    }

    sealed class NumberNode : ExpressionNode
    {
        public NumberNode(Token numberToken)
        {
            NumberToken = numberToken;
        }

        public override NodeType Type => NodeType.Number;
        public Token NumberToken { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return Enumerable.Empty<SyntaxNode>();
        }
    }
}
