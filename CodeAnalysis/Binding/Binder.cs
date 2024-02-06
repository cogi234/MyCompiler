using MyCompiler.CodeAnalysis.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace MyCompiler.CodeAnalysis.Binding
{
    internal sealed class Binder
    {
        private List<string> diagnostics = new List<string>();

        public IEnumerable<string> Diagnostics => diagnostics;
        public SyntaxTree SyntaxTree { get; }

        public Binder(SyntaxTree syntaxTree)
        {
            SyntaxTree = syntaxTree;
            diagnostics.AddRange(syntaxTree.Diagnostics.ToArray());
        }

        public BoundTree Bind()
        {
            BoundExpression boundExpression = BindExpression(SyntaxTree.Root);
            return new BoundTree(diagnostics, boundExpression);
        }

        private BoundExpression BindExpression(ExpressionNode node)
        {
            switch (node.Type)
            {
                case NodeType.LiteralExpression:
                    return BindLiteralExpression((LiteralExpressionNode)node);
                case NodeType.UnaryExpression:
                    return BindUnaryExpression((UnaryExpressionNode)node);
                case NodeType.BinaryExpression:
                    return BindBinaryExpression((BinaryExpressionNode)node);
                case NodeType.ParenthesizedExpression:
                    return BindExpression(((ParenthesizedExpressionNode)node).Expression);
                default:
                    throw new Exception($"Unexpected syntax node {node.Type}");
            }
        }

        private BoundExpression BindLiteralExpression(LiteralExpressionNode node)
        {
            object value = node.Value ?? 0;
            return new BoundLiteralExpression(value);
        }

        private BoundExpression BindUnaryExpression(UnaryExpressionNode node)
        {
            BoundExpression boundOperand = BindExpression(node.Operand);
            BoundUnaryOperationType? boundOperatorType = BindUnaryOperator(node.OperatorToken.Type, boundOperand.Type);

            if (boundOperatorType == null)
            {
                diagnostics.Add($"ERROR {node.OperatorToken.Position}: Unary operator '{node.OperatorToken.Type}' is not defined for type {boundOperand.Type}");
                return boundOperand;
            }

            return new BoundUnaryExpression(boundOperatorType.Value, boundOperand);
        }

        private BoundExpression BindBinaryExpression(BinaryExpressionNode node)
        {
            BoundExpression boundLeft = BindExpression(node.Left);
            BoundExpression boundRight = BindExpression(node.Right);
            BoundBinaryOperationType? boundOperatorType = BindBinaryOperator(node.OperatorToken.Type, boundLeft.Type, boundRight.Type);

            if (boundOperatorType == null)
            {
                diagnostics.Add($"ERROR {node.OperatorToken.Position}: Binary operator '{node.OperatorToken.Type}' is not defined for types {boundLeft.Type} and {boundRight.Type}");
                return boundLeft;
            }

            return new BoundBinaryExpression(boundLeft, boundOperatorType.Value, boundRight);
        }

        private BoundUnaryOperationType? BindUnaryOperator(TokenType tokenType, Type operandType)
        {
            if (operandType != typeof(int))
                return null;

            switch (tokenType)
            {
                case TokenType.Plus:
                    return BoundUnaryOperationType.Identity;
                case TokenType.Minus:
                    return BoundUnaryOperationType.Negation;
                default:
                    throw new Exception($"Unexpected unary operator {tokenType}");
            }
        }
        private BoundBinaryOperationType? BindBinaryOperator(TokenType tokenType, Type leftType, Type rightType)
        {
            if (leftType != typeof(int) || rightType != typeof(int))
                return null;

            switch (tokenType)
            {
                case TokenType.Plus:
                    return BoundBinaryOperationType.Addition;
                case TokenType.Minus:
                    return BoundBinaryOperationType.Subtraction;
                case TokenType.Star:
                    return BoundBinaryOperationType.Multiplication;
                case TokenType.ForwardSlash:
                    return BoundBinaryOperationType.Division;
                default:
                    throw new Exception($"Unexpected binary operator {tokenType}");
            }
        }
    }
}
