using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompiler
{
    class Evaluator
    {
        ExpressionNode root;
        public Evaluator(ExpressionNode root)
        {
            this.root = root;
        }

        public int Evaluate()
        {
            return EvaluateExpression(root);
        }

        private int EvaluateExpression(ExpressionNode expression)
        {
            switch (expression.Type)
            {
                case NodeType.Number:
                    return (int)(expression as NumberNode).NumberToken.Value;
                case NodeType.BinaryExpression:
                    BinaryExpressionNode binaryExpression = (BinaryExpressionNode)expression;
                    int left = EvaluateExpression(binaryExpression.Left);
                    int right = EvaluateExpression(binaryExpression.Right);
                    if (binaryExpression.OperatorToken.Type == TokenType.Plus)
                        return left + right;
                    if (binaryExpression.OperatorToken.Type == TokenType.Minus)
                        return left - right;
                    if (binaryExpression.OperatorToken.Type == TokenType.Star)
                        return left * right;
                    if (binaryExpression.OperatorToken.Type == TokenType.ForwardSlash)
                        return left / right;
                    throw new Exception($"Unexpected binary operator {binaryExpression.OperatorToken.Type}");
                case NodeType.ParenthesizedExpression:
                    return EvaluateExpression((expression as ParenthesizedExpressionNode).Expression);
                default:
                    throw new Exception($"Unexpected node {expression.Type}");
            }
        }
    }
}
