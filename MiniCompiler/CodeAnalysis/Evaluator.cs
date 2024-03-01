using MiniCompiler.CodeAnalysis.Binding.BoundNodes;
using MiniCompiler.CodeAnalysis.Symbols;

namespace MiniCompiler.CodeAnalysis
{
    internal sealed class Evaluator
    {
        private BoundBlockStatement root;
        private readonly Dictionary<VariableSymbol, object> variables;

        private object? lastValue = null;

        public Evaluator(BoundBlockStatement root, Dictionary<VariableSymbol, object> variables)
        {
            this.root = root;
            this.variables = variables;
        }

        public object? Evaluate()
        {
            Dictionary<LabelSymbol, int> labelIndex = new Dictionary<LabelSymbol, int>();

            for (int i = 0; i < root.Statements.Length; i++)
            {
                if (root.Statements[i].BoundNodeType == BoundNodeType.LabelStatement)
                    labelIndex.Add(((BoundLabelStatement)root.Statements[i]).Label, i);
            }

            int index = 0;
            while (index < root.Statements.Length)
            {
                BoundStatement statement = root.Statements[index];
                switch (statement.BoundNodeType)
                {
                    case BoundNodeType.ExpressionStatement:
                        EvaluateExpressionStatement((BoundExpressionStatement)statement);
                        index++;
                        break;
                    case BoundNodeType.VariableDeclarationStatement:
                        EvaluateVariableDeclarationStatement((BoundVariableDeclarationStatement)statement);
                        index++;
                        break;
                    case BoundNodeType.LabelStatement:
                        index++;
                        break;
                    case BoundNodeType.GotoStatement:
                        BoundGotoStatement gotoStatement = (BoundGotoStatement)statement;
                        index = labelIndex[gotoStatement.Label];
                        break;
                    case BoundNodeType.ConditionalGotoStatement:
                        BoundConditionalGotoStatement conditionalGotoStatement = (BoundConditionalGotoStatement)statement;
                        bool condition = (bool)EvaluateExpression(conditionalGotoStatement.Condition);
                        if (condition == conditionalGotoStatement.JumpIfTrue)
                            index = labelIndex[conditionalGotoStatement.Label];
                        else
                            index++;
                        break;
                    default:
                        throw new Exception($"Unexpected node {statement.BoundNodeType}");
                }
            }

            return lastValue;
        }

        private void EvaluateVariableDeclarationStatement(BoundVariableDeclarationStatement statement)
        {
            object value = EvaluateExpression(statement.Initializer);
            variables[statement.Variable] = value;
            lastValue = value;
        }

        private void EvaluateExpressionStatement(BoundExpressionStatement statement)
        {
            lastValue = EvaluateExpression(statement.Expression);
        }

        #region Expressions
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
                case BoundUnaryOperationType.BitwiseNegation:
                    return ~(int)operand;
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
                case BoundBinaryOperationType.BitwiseAnd:
                    if (expression.BinaryOperator.LeftType == typeof(int))
                        return (int)left & (int)right;
                    else
                        return (bool)left & (bool)right;
                case BoundBinaryOperationType.BitwiseOr:
                    if (expression.BinaryOperator.LeftType == typeof(int))
                        return (int)left | (int)right;
                    else
                        return (bool)left | (bool)right;
                case BoundBinaryOperationType.BitwiseXor:
                    if (expression.BinaryOperator.LeftType == typeof(int))
                        return (int)left ^ (int)right;
                    else
                        return (bool)left ^ (bool)right;
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
        #endregion Expressions
    }
}
