using MiniCompiler.CodeAnalysis.Binding;
using MiniCompiler.CodeAnalysis.Binding.BoundNodes;

namespace MiniCompiler.CodeAnalysis
{

    internal sealed class Evaluator
    {
        private BoundStatement root;
        private readonly Dictionary<VariableSymbol, object> variables;

        private object? lastValue = null;

        public Evaluator(BoundStatement root, Dictionary<VariableSymbol, object> variables)
        {
            this.root = root;
            this.variables = variables;
        }

        public object? Evaluate()
        {
            EvaluateStatement(root);
            return lastValue;
        }

        //Evaluate statements

        private void EvaluateStatement(BoundStatement statement)
        {
            switch (statement.BoundNodeType)
            {
                case BoundNodeType.BlockStatement:
                    EvaluateBlockStatement((BoundBlockStatement)statement);
                    break;
                case BoundNodeType.ExpressionStatement:
                    EvaluateExpressionStatement((BoundExpressionStatement)statement);
                    break;
                default:
                    throw new Exception($"Unexpected node {statement.BoundNodeType}");
            }
        }

        private void EvaluateBlockStatement(BoundBlockStatement statement)
        {
            foreach (BoundStatement currentStatement in statement.Statements)
                EvaluateStatement(currentStatement);
        }

        private void EvaluateExpressionStatement(BoundExpressionStatement statement)
        {
            lastValue = EvaluateExpression(statement.Expression);
        }

        //Evaluate expressions

        private object EvaluateExpression(BoundExpression expression)
        {
            switch (expression.BoundNodeType)
            {
                case BoundNodeType.LiteralExpression:
                    return EvaluateLiteralExpression(expression);
                case BoundNodeType.VariableExpression:
                    return EvaluateVariableExpression(expression);
                case BoundNodeType.AssignmentExpression:
                    return EvaluateAssignmentExpression(expression);
                case BoundNodeType.UnaryExpression:
                    return EvaluateUnaryExpression(expression);
                case BoundNodeType.BinaryExpression:
                    return EvaluateBinaryExpression(expression);
                default:
                    throw new Exception($"Unexpected node {expression.BoundNodeType}");
            }
        }

        private static object EvaluateLiteralExpression(BoundExpression expression)
        {
            return ((BoundLiteralExpression)expression).Value;
        }

        private object EvaluateVariableExpression(BoundExpression expression)
        {
            BoundVariableExpression variableExpression = (BoundVariableExpression)expression;
            object value = variables[variableExpression.Variable];
            return value;
        }

        private object EvaluateAssignmentExpression(BoundExpression expression)
        {
            BoundAssignmentExpression assignmentExpression = (BoundAssignmentExpression)expression;
            object value = EvaluateExpression(assignmentExpression.Expression);
            variables[assignmentExpression.Variable] = value;
            return value;
        }

        private object EvaluateUnaryExpression(BoundExpression expression)
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

        private object EvaluateBinaryExpression(BoundExpression expression)
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
    }
}
