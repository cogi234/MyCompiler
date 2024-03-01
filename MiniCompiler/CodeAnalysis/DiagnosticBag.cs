using MiniCompiler.CodeAnalysis.Symbols;
using MiniCompiler.CodeAnalysis.Syntax;
using MiniCompiler.CodeAnalysis.Syntax.SyntaxNodes;
using MiniCompiler.CodeAnalysis.Text;
using System;
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

        public void ReportUnexpectedNode(TextSpan span, NodeType expectedNode, NodeType actualNode)
        {
            string message = $"Expected <{expectedNode}>, got <{actualNode}>.";
            Report(span, message);
        }
        #endregion
        #region BinderErrors
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

        public void ReportUndefinedName(TextSpan span, string name)
        {
            string message = $"Variable '{name}' doesn't exist.";
            Report(span, message);
        }

        public void ReportCannotConvert(TextSpan span, TypeSymbol fromType, TypeSymbol toType)
        {
            string message = $"Cannot convert type {fromType} to {toType}.";
            Report(span, message);
        }

        public void ReportAlreadyExistingVariable(TextSpan span, string name)
        {
            string message = $"Variable '{name}' already exists.";
            Report(span, message);
        }

        public void ReportCannotAssign(TextSpan span, string name)
        {
            string message = $"Cannot assign to variable '{name}', it is read only.";
            Report(span, message);
        }
        #endregion
    }
}
