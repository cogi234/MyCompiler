using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompiler.CodeAnalysis
{
    public enum NodeType
    {
        LiteralExpression,
        BinaryExpression,
        ParenthesizedExpression,
        UnaryExpression
    }

    public abstract class SyntaxNode
    {
        public abstract NodeType Type { get; }

        public abstract IEnumerable<SyntaxNode> GetChildren();
    }

    public abstract class ExpressionNode : SyntaxNode
    {
    }

    public sealed class BinaryExpressionNode : ExpressionNode
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

    public sealed class UnaryExpressionNode : ExpressionNode
    {
        public UnaryExpressionNode(Token operatorToken, ExpressionNode expression)
        {
            OperatorToken = operatorToken;
            Expression = expression;
        }

        public override NodeType Type => NodeType.UnaryExpression;
        public Token OperatorToken { get; }
        public ExpressionNode Expression { get; }


        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Expression;
        }
    }

    public sealed class ParenthesizedExpressionNode : ExpressionNode
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

    public sealed class LiteralExpressionNode : ExpressionNode
    {
        public LiteralExpressionNode(Token literalToken)
        {
            LiteralToken = literalToken;
        }

        public override NodeType Type => NodeType.LiteralExpression;
        public Token LiteralToken { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return Enumerable.Empty<SyntaxNode>();
        }
    }
}
