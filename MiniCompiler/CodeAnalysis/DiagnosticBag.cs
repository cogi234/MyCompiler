using MiniCompiler.CodeAnalysis.Syntax;
using MiniCompiler.CodeAnalysis.Text;
using System.Collections;

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

        public void ReportInvalidNumber(TextSpan span, string text, Type type)
        {
            string message = $"The number {text} isn't a valid {type}.";
            Report(span, message);
        }

        public void ReportBadCharacter(TextSpan span, char character)
        {
            string message = $"Bad character input: '{character}'.";
            Report(span, message);
        }

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

        public void ReportUndefinedUnaryOperator(TextSpan span, string operatorText, Type operandType)
        {
            string message = $"Unary operator '{operatorText}' is not defined for type {operandType}.";
            Report(span, message);
        }
        public void ReportUndefinedBinaryOperator(TextSpan span, string operatorText, Type leftType, Type rightType)
        {
            string message = $"Binary operator '{operatorText}' is not defined for for types {leftType} and {rightType}.";
            Report(span, message);
        }

        public void ReportUndefinedName(TextSpan span, string name)
        {
            string message = $"Variable '{name}' doesn't exist.";
            Report(span, message);
        }

        internal void ReportCannotConvert(TextSpan span, Type fromType, Type toType)
        {
            string message = $"Cannot convert type {fromType} to {toType}.";
            Report(span, message);
        }
    }
}
