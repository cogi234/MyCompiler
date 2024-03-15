using MiniLang.CodeAnalysis.Binding;
using MiniLang.CodeAnalysis.Binding.BoundNodes;
using MiniLang.CodeAnalysis.Symbols;
using System.Collections.Immutable;

namespace MiniLang.CodeAnalysis
{
    internal sealed class Evaluator
    {
        private readonly BoundProgram program;
        private readonly Dictionary<VariableSymbol, object?> globals;
        private readonly Stack<Dictionary<VariableSymbol, object?>> locals =
            new Stack<Dictionary<VariableSymbol, object?>>();
        private Random? random;

        private object? lastValue = null;

        public Evaluator(BoundProgram program, Dictionary<VariableSymbol, object?> variables)
        {
            this.program = program;
            globals = variables;
            locals.Push(new Dictionary<VariableSymbol, object?>());
        }

        public object? Evaluate()
        {
            return EvaluateStatement(program.Statement);
        }

        public object? EvaluateStatement(BoundBlockStatement body)
        {
            Dictionary<BoundLabel, int> labelIndex = new Dictionary<BoundLabel, int>();

            for (int i = 0; i < body.Statements.Length; i++)
            {
                if (body.Statements[i].BoundNodeType == BoundNodeType.LabelStatement)
                    labelIndex.Add(((BoundLabelStatement)body.Statements[i]).Label, i);
            }

            int index = 0;
            while (index < body.Statements.Length)
            {
                BoundStatement statement = body.Statements[index];
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
                        bool condition = (bool)EvaluateExpression(conditionalGotoStatement.Condition)!;
                        if (condition == conditionalGotoStatement.JumpIfTrue)
                            index = labelIndex[conditionalGotoStatement.Label];
                        else
                            index++;
                        break;
                    case BoundNodeType.ReturnStatement:
                        BoundReturnStatement returnStatement = (BoundReturnStatement)statement;
                        lastValue = returnStatement.Expression == null ? null
                            : EvaluateExpression(returnStatement.Expression);
                        return lastValue;
                    default:
                        throw new Exception($"Unexpected node {statement.BoundNodeType}");
                }
            }

            return lastValue;
        }

        private void EvaluateVariableDeclarationStatement(BoundVariableDeclarationStatement statement)
        {
            object? value;
            if (statement.Initializer == null)
                value = statement.Variable.Type.DefaultValue;
            else
                value = EvaluateExpression(statement.Initializer);

            lastValue = value;

            Assign(statement.Variable, value);
        }

        private void EvaluateExpressionStatement(BoundExpressionStatement statement)
        {
            lastValue = EvaluateExpression(statement.Expression);
        }

        #region Expressions
        private object? EvaluateExpression(BoundExpression expression)
        {
            switch (expression.BoundNodeType)
            {
                case BoundNodeType.LiteralExpression:
                    return EvaluateLiteralExpression((BoundLiteralExpression)expression);
                case BoundNodeType.VariableExpression:
                    return EvaluateVariableExpression((BoundVariableExpression)expression);
                case BoundNodeType.AssignmentExpression:
                    return EvaluateAssignmentExpression((BoundAssignmentExpression)expression);
                case BoundNodeType.CallExpression:
                    return EvaluateCallExpression((BoundCallExpression)expression);
                case BoundNodeType.ConversionExpression:
                    return EvaluateConversionExpression((BoundConversionExpression)expression);
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

        private object? EvaluateVariableExpression(BoundVariableExpression expression)
        {
            if (expression.Variable.SymbolType == SymbolType.GlobalVariable)
            {
                return globals[expression.Variable];
            }
            else
            {
                Dictionary<VariableSymbol, object?> local = locals.Peek();
                return local[expression.Variable];
            }
        }

        private object? EvaluateAssignmentExpression(BoundAssignmentExpression expression)
        {
            object? value = EvaluateExpression(expression.Expression);
            Assign(expression.Variable, value);
            return value;
        }

        private object? EvaluateCallExpression(BoundCallExpression expression)
        {
            ImmutableArray<object?>.Builder argumentBuilder = ImmutableArray.CreateBuilder<object?>();
            foreach (BoundExpression argument in expression.Arguments)
                argumentBuilder.Add(EvaluateExpression(argument));
            ImmutableArray<object?> arguments = argumentBuilder.ToImmutable();

            if (expression.Function == BuiltInFunctions.Input)
            {
                return Console.ReadLine();
            }
            else if (expression.Function == BuiltInFunctions.Print)
            {
                string? message = (string?)arguments[0];
                Console.WriteLine(message);
                return null;
            }
            else if (expression.Function == BuiltInFunctions.Random)
            {
                int max = (int)(arguments[0] ?? 1);
                if (random == null)
                    random = new Random();
                return random.Next(max);
            }
            else
            {
                Dictionary<VariableSymbol, object?> local = new Dictionary<VariableSymbol, object?>();
                for (int i = 0; i < expression.Arguments.Length; i++)
                {
                    ParameterSymbol parameter = expression.Function.Parameters[i];
                    object? value = EvaluateExpression(expression.Arguments[i]);
                    local.Add(parameter, value);
                }
                locals.Push(local);

                BoundBlockStatement statement = program.Functions[expression.Function];
                object? result = EvaluateStatement(statement);

                locals.Pop();

                return result;
            }
        }

        private object? EvaluateConversionExpression(BoundConversionExpression expression)
        {
            object toConvert = EvaluateExpression(expression.Expression)!;

            if (expression.Expression.Type == TypeSymbol.Bool)
            {
                if (expression.Type == TypeSymbol.String)
                    return Convert.ToString(toConvert);
            }
            else if (expression.Expression.Type == TypeSymbol.Int)
            {
                if (expression.Type == TypeSymbol.String)
                    return Convert.ToString(toConvert);
            }
            else if (expression.Expression.Type == TypeSymbol.String)
            {
                if (expression.Type == TypeSymbol.Int)
                    return int.Parse((string)toConvert);
                if (expression.Type == TypeSymbol.Bool)
                    return bool.Parse((string)toConvert);
            }

            throw new Exception($"Can't convert {expression.Expression.Type} to {expression.Type}");
        }

        private object EvaluateUnaryExpression(BoundUnaryExpression expression)
        {
            object operand = EvaluateExpression(expression.Operand)!;
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
            object left = EvaluateExpression(expression.Left)!;
            object right = EvaluateExpression(expression.Right)!;
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
                case BoundBinaryOperationType.Modulo:
                    return (int)left % (int)right;
                case BoundBinaryOperationType.BitwiseAnd:
                    if (expression.BinaryOperator.LeftType == TypeSymbol.Int)
                        return (int)left & (int)right;
                    else
                        return (bool)left & (bool)right;
                case BoundBinaryOperationType.BitwiseOr:
                    if (expression.BinaryOperator.LeftType == TypeSymbol.Int)
                        return (int)left | (int)right;
                    else
                        return (bool)left | (bool)right;
                case BoundBinaryOperationType.BitwiseXor:
                    if (expression.BinaryOperator.LeftType == TypeSymbol.Int)
                        return (int)left ^ (int)right;
                    else
                        return (bool)left ^ (bool)right;
                //Booleans
                case BoundBinaryOperationType.LogicalAnd:
                    return (bool)left && (bool)right;
                case BoundBinaryOperationType.LogicalOr:
                    return (bool)left || (bool)right;
                //Strings
                case BoundBinaryOperationType.Concatenation:
                    return (string)left + (string)right;
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

        private void Assign(VariableSymbol variable, object? value)
        {
            if (variable.SymbolType == SymbolType.GlobalVariable)
                globals[variable] = value;
            else
                locals.Peek()[variable] = value;
        }
    }
}
