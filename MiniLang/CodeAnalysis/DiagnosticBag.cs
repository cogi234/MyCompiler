using MiniCompiler.CodeAnalysis.Symbols;
using MiniCompiler.CodeAnalysis.Syntax;
using MiniCompiler.CodeAnalysis.Syntax.SyntaxNodes;
using MiniCompiler.CodeAnalysis.Text;
using System.Collections;
using System.Text;

namespace MiniCompiler.CodeAnalysis
{
    internal sealed class DiagnosticBag : IEnumerable<Diagnostic>
    {
        private readonly List<Diagnostic> diagnostics = new List<Diagnostic>();

        public IEnumerator<Diagnostic> GetEnumerator() => diagnostics.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void AddRange(IEnumerable<Diagnostic> diagnostics)
        {
            this.diagnostics.AddRange(diagnostics);
        }

        private void Report(TextSpan span, string message)
        {
            Diagnostic diagnostic = new Diagnostic(span, message);
            diagnostics.Add(diagnostic);
        }

        #region LexerErrors
        public void ReportInvalidNumber(TextSpan span, string text, TypeSymbol type)
        {
            string message = $"The number {text} isn't a valid {type}.";
            Report(span, message);
        }

        public void ReportBadCharacter(TextSpan span, char character)
        {
            string message = $"Bad character input: '{character}'.";
            Report(span, message);
        }

        public void ReportUnterminatedString(TextSpan span)
        {
            string message = $"Unterminated string literal.";
            Report(span, message);
        }
        #endregion
        #region ParserErrors
        public void ReportUnexpectedToken(TextSpan span, TokenType found, params TokenType[] expected)
        {
            string message = $"Unexpected token. Expected ";
            for (int i = 0; i < expected.Length; i++)
            {
                message += $"<{expected[i]}>";

                if (i > 0 && i == expected.Length - 2)
                    message += " or ";
                else
                    message += ", ";
            }
            message += $"found <{found}>.";
            Report(span, message);
        }

        public void ReportUnexpectedNode(TextSpan span, NodeType actualNode, params NodeType[] expectedNode)
        {
            StringBuilder message = new StringBuilder($"Unexpected node <{actualNode}>.");

            if (expectedNode.Length > 0)
            {
                message.Append(" Expected");

                for (int i = 0; i < expectedNode.Length; i++)
                {
                    if (i == expectedNode.Length - 1 && i != 0)
                        message.Append(" or");
                    else if (i != 0)
                        message.Append(",");

                    message.Append($" <{expectedNode[i]}>");
                }

                message.Append(".");
            }

            Report(span, message.ToString());
        }
        #endregion
        #region BinderErrors

        public void ReportCannotConvert(TextSpan span, TypeSymbol fromType, TypeSymbol toType)
        {
            string message = $"Cannot convert type {fromType} to {toType}.";
            Report(span, message);
        }

        public void ReportUndefinedUnaryOperator(TextSpan span, string operatorText, TypeSymbol operandType)
        {
            string message = $"Unary operator '{operatorText}' is not defined for type {operandType}.";
            Report(span, message);
        }
        public void ReportUndefinedBinaryOperator(TextSpan span, string operatorText, TypeSymbol leftType, TypeSymbol rightType)
        {
            string message = $"Binary operator '{operatorText}' is not defined for types {leftType} and {rightType}.";
            Report(span, message);
        }

        public void ReportSymbolAlreadyDeclared(TextSpan span, string name)
        {
            string message = $"'{name}' already exists.";
            Report(span, message);
        }

        public void ReportUndefinedVariable(TextSpan span, string name)
        {
            string message = $"Variable '{name}' doesn't exist.";
            Report(span, message);
        }
        public void ReportNotAVariable(TextSpan span, string name)
        {
            string message = $"'{name}' is not a variable.";
            Report(span, message);
        }
        public void ReportCannotAssign(TextSpan span, string name)
        {
            string message = $"Cannot assign to variable '{name}', it is read only.";
            Report(span, message);
        }

        public void ReportUndefinedFunction(TextSpan span, string? name)
        {
            string message = $"Function '{name}' doesn't exist.";
            Report(span, message);
        }
        public void ReportNotAFunction(TextSpan span, string name)
        {
            string message = $"'{name}' is not a function.";
            Report(span, message);
        }
        public void ReportWrongArgumentCount(TextSpan span, string name, int count)
        {
            string message = $"Function '{name}' doesn't handle {count} arguments";
            Report(span, message);
        }
        public void ReportParameterAlreadyDeclared(TextSpan span, string name)
        {
            string message = $"A parameter with the name '{name}' already exists.";
            Report(span, message);
        }

        public void ReportNullExpression(TextSpan span)
        {
            string message = "Expression must have a non-null value.";
            Report(span, message);
        }

        public void ReportInvalidBreakOrContinue(TextSpan span, string? text)
        {
            string message = $"{text} statement must be inside a loop.";
            Report(span, message);
        }

        public void ReportInvalidReturn(TextSpan span)
        {
            string message = "The 'return' keyword can only be used inside a funtion.";
            Report(span, message);
        }
        public void ReportInvalidReturnExpression(TextSpan span, string functionName)
        {
            string message = $"Since the function '{functionName}' does not return a value," +
                " the 'return' keyword cannot be followed by an expression.";
            Report(span, message);
        }
        public void ReportMissingReturnExpression(TextSpan span, TypeSymbol returnType)
        {
            string message = $"An expression of type '{returnType}' expected.";
            Report(span, message);
        }

        public void ReportAllPathsMustReturn(TextSpan span)
        {
            string message = "Not all code paths return a value.";
            Report(span, message);
        }
        #endregion
    }
}
