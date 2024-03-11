using MiniLang.CodeAnalysis.Symbols;
using MiniLang.CodeAnalysis.Syntax;
using MiniLang.CodeAnalysis.Syntax.SyntaxNodes;
using MiniLang.CodeAnalysis.Text;
using System.Collections;
using System.Text;

namespace MiniLang.CodeAnalysis
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

        private void Report(TextLocation location, string message)
        {
            Diagnostic diagnostic = new Diagnostic(location, message);
            diagnostics.Add(diagnostic);
        }

        #region LexerErrors
        public void ReportInvalidNumber(TextLocation location, string text, TypeSymbol type)
        {
            string message = $"The number {text} isn't a valid {type}.";
            Report(location, message);
        }

        public void ReportBadCharacter(TextLocation location, char character)
        {
            string message = $"Bad character input: '{character}'.";
            Report(location, message);
        }

        public void ReportUnterminatedString(TextLocation location)
        {
            string message = $"Unterminated string literal.";
            Report(location, message);
        }
        #endregion
        #region ParserErrors
        public void ReportUnexpectedToken(TextLocation location, TokenType found, params TokenType[] expected)
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
            Report(location, message);
        }

        public void ReportUnexpectedNode(TextLocation location, NodeType actualNode, params NodeType[] expectedNode)
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

            Report(location, message.ToString());
        }
        #endregion
        #region BinderErrors

        public void ReportCannotConvert(TextLocation location, TypeSymbol fromType, TypeSymbol toType)
        {
            string message = $"Cannot convert type {fromType} to {toType}.";
            Report(location, message);
        }

        public void ReportUndefinedUnaryOperator(TextLocation location, string operatorText, TypeSymbol operandType)
        {
            string message = $"Unary operator '{operatorText}' is not defined for type {operandType}.";
            Report(location, message);
        }
        public void ReportUndefinedBinaryOperator(TextLocation location, string operatorText, TypeSymbol leftType, TypeSymbol rightType)
        {
            string message = $"Binary operator '{operatorText}' is not defined for types {leftType} and {rightType}.";
            Report(location, message);
        }

        public void ReportSymbolAlreadyDeclared(TextLocation location, string name)
        {
            string message = $"'{name}' already exists.";
            Report(location, message);
        }

        public void ReportUndefinedVariable(TextLocation location, string name)
        {
            string message = $"Variable '{name}' doesn't exist.";
            Report(location, message);
        }
        public void ReportNotAVariable(TextLocation location, string name)
        {
            string message = $"'{name}' is not a variable.";
            Report(location, message);
        }
        public void ReportCannotAssign(TextLocation location, string name)
        {
            string message = $"Cannot assign to variable '{name}', it is read only.";
            Report(location, message);
        }

        public void ReportUndefinedFunction(TextLocation location, string? name)
        {
            string message = $"Function '{name}' doesn't exist.";
            Report(location, message);
        }
        public void ReportNotAFunction(TextLocation location, string name)
        {
            string message = $"'{name}' is not a function.";
            Report(location, message);
        }
        public void ReportWrongArgumentCount(TextLocation location, string name, int count)
        {
            string message = $"Function '{name}' doesn't handle {count} arguments";
            Report(location, message);
        }
        public void ReportParameterAlreadyDeclared(TextLocation location, string name)
        {
            string message = $"A parameter with the name '{name}' already exists.";
            Report(location, message);
        }

        public void ReportNullExpression(TextLocation location)
        {
            string message = "Expression must have a non-null value.";
            Report(location, message);
        }

        public void ReportInvalidBreakOrContinue(TextLocation location, string? text)
        {
            string message = $"{text} statement must be inside a loop.";
            Report(location, message);
        }

        public void ReportInvalidReturn(TextLocation location)
        {
            string message = "The 'return' keyword can only be used inside a funtion.";
            Report(location, message);
        }
        public void ReportInvalidReturnExpression(TextLocation location, string functionName)
        {
            string message = $"Since the function '{functionName}' does not return a value," +
                " the 'return' keyword cannot be followed by an expression.";
            Report(location, message);
        }
        public void ReportMissingReturnExpression(TextLocation location, TypeSymbol returnType)
        {
            string message = $"An expression of type '{returnType}' expected.";
            Report(location, message);
        }

        public void ReportAllPathsMustReturn(TextLocation location)
        {
            string message = "Not all code paths return a value.";
            Report(location, message);
        }
        #endregion
    }
}
