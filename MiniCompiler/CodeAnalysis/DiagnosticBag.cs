﻿using MiniCompiler.CodeAnalysis.Syntax;
using System.Collections;
using System.Xml.Linq;

namespace MiniCompiler.CodeAnalysis
{
    public sealed class DiagnosticBag: IEnumerable<Diagnostic>
    {
        private readonly List<Diagnostic> diagnostics = new List<Diagnostic>();

        public IEnumerator<Diagnostic> GetEnumerator() => diagnostics.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void AddRange(DiagnosticBag diagnostics)
        {
            this.diagnostics.AddRange(diagnostics.diagnostics);
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

        public void ReportUnexpectedToken(TextSpan span, TokenType expected, TokenType found)
        {
            string message = $"Unexpected token. Expected <{expected}>, found <{found}>.";
            Report(span, message);
        }

        public void ReportUndefinedUnaryOperator(TextSpan span, string operatorText, Type operandType)
        {
            string message = $"Unary operator '{operatorText}' is not defined for type {operandType}";
            Report(span, message);
        }
        public void ReportUndefinedBinaryOperator(TextSpan span, string operatorText, Type leftType, Type rightType)
        {
            string message = $"Unary operator '{operatorText}' is not defined for for types {leftType} and {rightType}";
            Report(span, message);
        }
    }
}
