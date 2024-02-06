using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MyCompiler.CodeAnalysis.Binding;
using MyCompiler.CodeAnalysis.Syntax;

namespace MyCompiler.CodeAnalysis
{
    internal sealed class Evaluator
    {
        BoundExpression root;
        public Evaluator(BoundExpression root)
        {
            this.root = root;
        }

        public int Evaluate()
        {
            return EvaluateExpression(root);
        }

        private int EvaluateExpression(BoundExpression expression)
        {
            switch (expression.BoundNodeType)
            {
                case BoundNodeType.LiteralExpression:
                    return (int)((BoundLiteralExpression)expression).Value;
                case BoundNodeType.UnaryExpression:
                    {
                        BoundUnaryExpression unaryExpression = (BoundUnaryExpression)expression;
                        int operand = EvaluateExpression(unaryExpression.Operand);
                        switch (unaryExpression.OperationType)
                        {
                            case BoundUnaryOperationType.Identity:
                                return operand;
                            case BoundUnaryOperationType.Negation:
                                return -operand;
                            default:
                                throw new Exception($"Unhandled unary operation {unaryExpression.OperationType}");
                        }
                    }
                case BoundNodeType.BinaryExpression:
                    {
                        BoundBinaryExpression binaryExpression = (BoundBinaryExpression)expression;
                        int left = EvaluateExpression(binaryExpression.Left);
                        int right = EvaluateExpression(binaryExpression.Right);
                        switch (binaryExpression.OperationType)
                        {
                            case BoundBinaryOperationType.Addition:
                                return left + right;
                            case BoundBinaryOperationType.Subtraction:
                                return left - right;
                            case BoundBinaryOperationType.Multiplication:
                                return left * right;
                            case BoundBinaryOperationType.Division:
                                return left / right;
                            default:
                                throw new Exception($"Unhandled binary operation {binaryExpression.OperationType}");
                        }
                    }
                default:
                    throw new Exception($"Unexpected node {expression.BoundNodeType}");
            }
        }
    }
}
