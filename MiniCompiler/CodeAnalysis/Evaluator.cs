using MiniCompiler.CodeAnalysis.Binding;
using MiniCompiler.CodeAnalysis.Binding.BoundNodes;
using System.ComponentModel.Design;
using System.Linq.Expressions;

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
                case BoundNodeType.VariableDeclarationStatement:
                    EvaluateVariableDeclarationStatement((BoundVariableDeclarationStatement)statement);
                    break;
                case BoundNodeType.IfStatement:
                    EvaluateIfStatement((BoundIfStatement)statement);
                    break;
                case BoundNodeType.WhileStatement:
                    EvaluateWhileStatement((BoundWhileStatement)statement);
                    break;
                case BoundNodeType.ForStatement:
                    EvaluateForStatement((BoundForStatement)statement);
                    break;
                default:
                    throw new Exception($"Unexpected node {statement.BoundNodeType}");
            }
        }

        private void EvaluateForStatement(BoundForStatement statement)
        {
            if (statement.Declaration != null)
                EvaluateVariableDeclarationStatement(statement.Declaration);

            while ((bool)EvaluateExpression(statement.Condition))
            {
                EvaluateStatement(statement.Statement);
                if (statement.Increment != null)
                    EvaluateAssignmentExpression(statement.Increment);
            }
        }

        private void EvaluateWhileStatement(BoundWhileStatement statement)
        {
            while ((bool)EvaluateExpression(statement.Condition))
            {
                EvaluateStatement(statement.Statement);
            }
        }

        private void EvaluateIfStatement(BoundIfStatement statement)
        {
            bool condition = (bool)EvaluateExpression(statement.Condition);
            if (condition)
                EvaluateStatement(statement.IfStatement);
            else if (statement.ElseStatement != null)
                EvaluateStatement(statement.ElseStatement);
        }

        private void EvaluateVariableDeclarationStatement(BoundVariableDeclarationStatement statement)
        {
            object value = EvaluateExpression(statement.Initializer);
            variables[statement.Variable] = value;
            lastValue = value;
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
                    return EvaluateLiteralExpression((BoundLiteralExpression)expression);
                case BoundNodeType.VariableExpression:
                    return EvaluateVariableExpression((BoundVariableExpression)expression);
                case BoundNodeType.AssignmentExpression:
                    return EvaluateAssignmentExpression((BoundAssignmentExpression)expression);
                case BoundNodeType.UnaryExpression:
                    return EvaluateUnaryExpression((BoundUnaryExpression)expression);
                case BoundNodeType.BinaryExpression:
                    return EvaluateBinaryExpression((BoundBinaryExpression)expression);
                default:
                    throw new Exception($"Unexpected node {expression.BoundNodeType}");
            }
        }

        private static object EvaluateLiteralExpression(BoundLiteralExpression expression)
        {
            return expression.Value;
        }

        private object EvaluateVariableExpression(BoundVariableExpression expression)
        {
            object value = variables[expression.Variable];
            return value;
        }

        private object EvaluateAssignmentExpression(BoundAssignmentExpression expression)
        {
            object value = EvaluateExpression(expression.Expression);
            variables[expression.Variable] = value;
            return value;
        }

        private object EvaluateUnaryExpression(BoundUnaryExpression expression)
        {
            object operand = EvaluateExpression(expression.Operand);
            switch (expression.UnaryOperator.OperationType)
            {
                case BoundUnaryOperationType.Identity:
                    return (int)operand;
                case BoundUnaryOperationType.Negation:
                    return -(int)operand;
                case BoundUnaryOperationType.LogicalNegation:
                    return !(bool)operand;
                default:
                    throw new Exception($"Unhandled unary operation {expression.UnaryOperator.OperationType}");
            }
        }

        private object EvaluateBinaryExpression(BoundBinaryExpression expression)
        {
            object left = EvaluateExpression(expression.Left);
            object right = EvaluateExpression(expression.Right);
            switch (expression.BinaryOperator.OperationType)
            {
                //Numbers
                case BoundBinaryOperationType.Addition:
                    return (int)left + (int)right;
                case BoundBinaryOperationType.Subtraction:
                    return (int)left - (int)right;
                case BoundBinaryOperationType.Multiplication:
                    return (int)left * (int)right;
                case BoundBinaryOperationType.Division:
                    return (int)left / (int)right;
                //Booleans
                case BoundBinaryOperationType.LogicalAnd:
                    return (bool)left && (bool)right;
                case BoundBinaryOperationType.LogicalOr:
                    return (bool)left || (bool)right;
                //Comparisons
                case BoundBinaryOperationType.Equality:
                    return Equals(left, right);
                case BoundBinaryOperationType.Unequality:
                    return !Equals(left, right);
                case BoundBinaryOperationType.LesserThan:
                    return (int)left < (int)right;
                case BoundBinaryOperationType.LesserThanOrEqual:
                    return (int)left <= (int)right;
                case BoundBinaryOperationType.GreaterThan:
                    return (int)left > (int)right;
                case BoundBinaryOperationType.GreaterThanOrEqual:
                    return (int)left >= (int)right;
                default:
                    throw new Exception($"Unhandled binary operation {expression.BinaryOperator.OperationType}");
            }
        }
    }
}
