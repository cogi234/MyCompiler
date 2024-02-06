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

        public object Evaluate()
        {
            return EvaluateExpression(root);
        }

        private object EvaluateExpression(BoundExpression expression)
        {
            switch (expression.BoundNodeType)
            {
                case BoundNodeType.LiteralExpression:
                    return ((BoundLiteralExpression)expression).Value;
                case BoundNodeType.UnaryExpression:
                    {
                        BoundUnaryExpression unaryExpression = (BoundUnaryExpression)expression;
                        object operand = EvaluateExpression(unaryExpression.Operand);
                        switch (unaryExpression.UnaryOperator.OperationType)
                        {
                            case BoundUnaryOperationType.Identity:
                                return (int)operand;
                            case BoundUnaryOperationType.Negation:
                                return -(int)operand;
                            case BoundUnaryOperationType.LogicalNegation:
                                return !(bool)operand;
                            default:
                                throw new Exception($"Unhandled unary operation {unaryExpression.UnaryOperator.OperationType}");
                        }
                    }
                case BoundNodeType.BinaryExpression:
                    {
                        BoundBinaryExpression binaryExpression = (BoundBinaryExpression)expression;
                        object left = EvaluateExpression(binaryExpression.Left);
                        object right = EvaluateExpression(binaryExpression.Right);
                        switch (binaryExpression.BinaryOperator.OperationType)
                        {
                            case BoundBinaryOperationType.Addition:
                                return (int)left + (int)right;
                            case BoundBinaryOperationType.Subtraction:
                                return (int)left - (int)right;
                            case BoundBinaryOperationType.Multiplication:
                                return (int)left * (int)right;
                            case BoundBinaryOperationType.Division:
                                return (int)left / (int)right;
                            case BoundBinaryOperationType.LogicalAnd:
                                return (bool)left && (bool)right;
                            case BoundBinaryOperationType.LogicalOr:
                                return (bool)left || (bool)right;
                            default:
                                throw new Exception($"Unhandled binary operation {binaryExpression.BinaryOperator.OperationType}");
                        }
                    }
                default:
                    throw new Exception($"Unexpected node {expression.BoundNodeType}");
            }
        }
    }
}
