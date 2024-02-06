using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompiler.CodeAnalysis.Syntax
{
    public enum NodeType
    {
        LiteralExpression,
        UnaryExpression,
        BinaryExpression,
        ParenthesizedExpression
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
        public UnaryExpressionNode(Token operatorToken, ExpressionNode operand)
        {
            OperatorToken = operatorToken;
            Operand = operand;
        }

        public override NodeType Type => NodeType.UnaryExpression;
        public Token OperatorToken { get; }
        public ExpressionNode Operand { get; }


        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Operand;
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
        public LiteralExpressionNode(Token literalToken) : this(literalToken, literalToken.Value) { }

        public LiteralExpressionNode(Token literalToken, object value)
        {
            LiteralToken = literalToken;
            Value = value;
        }

        public override NodeType Type => NodeType.LiteralExpression;
        public Token LiteralToken { get; }
        public object Value { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return Enumerable.Empty<SyntaxNode>();
        }
    }
}
