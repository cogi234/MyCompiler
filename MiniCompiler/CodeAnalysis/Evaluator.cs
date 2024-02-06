using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MiniCompiler.CodeAnalysis.Binding.BoundNodes;

namespace MiniCompiler.CodeAnalysis
{

    internal sealed class Evaluator
    {
        private BoundExpression root;
        private readonly Dictionary<string, object> variables;

        public Dictionary<string, object> Variables => variables;
        public Evaluator(BoundExpression root, Dictionary<string, object> variables)
        {
            this.root = root;
            this.variables = variables;
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
                case BoundNodeType.VariableExpression:
                    {
                        BoundVariableExpression variableExpression = (BoundVariableExpression)expression;
                        object value = variables[variableExpression.Name];
                        return value;
                    }
                case BoundNodeType.AssignmentExpression:
                    {
                        BoundAssignmentExpression assignmentExpression = (BoundAssignmentExpression)expression;
                        object value = EvaluateExpression(assignmentExpression.Expression);
                        variables[assignmentExpression.Name] = value;
                        return value;
                    }
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
                            case BoundBinaryOperationType.Equality:
                                return Equals(left, right);
                            case BoundBinaryOperationType.Unequality:
                                return !Equals(left, right);
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
