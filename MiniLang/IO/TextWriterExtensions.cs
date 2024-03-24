using MiniLang.CodeAnalysis;
using MiniLang.CodeAnalysis.Syntax;
using MiniLang.CodeAnalysis.Text;
using System.CodeDom.Compiler;

namespace MiniLang.IO
{
    public static class TextWriterExtensions
    {
        public static bool IsConsole(this TextWriter writer)
        {
            if (writer == Console.Out)
                return !Console.IsOutputRedirected;

            if (writer == Console.Error)
                return !Console.IsErrorRedirected && !Console.IsOutputRedirected;

            if (writer is IndentedTextWriter iw && iw.InnerWriter.IsConsole())
                return true;

            return false;
        }

        private static void SetForeground(this TextWriter writer, ConsoleColor color)
        {
            if (writer.IsConsole())
                Console.ForegroundColor = color;
        }

        private static void ResetColor(this TextWriter writer)
        {
            if (writer.IsConsole())
                Console.ResetColor();
        }

        public static void WriteKeyword(this TextWriter writer, string text)
        {
            writer.SetForeground(ConsoleColor.Blue);
            writer.Write(text);
            writer.ResetColor();
        }
        public static void WriteKeyword(this TextWriter writer, TokenType type)
        {
            writer.WriteKeyword(SyntaxFacts.GetText(type)!);
        }

        public static void WriteType(this TextWriter writer, string text)
        {
            writer.SetForeground(ConsoleColor.Yellow);
            writer.Write(text);
            writer.ResetColor();
        }

        public static void WriteIdentifier(this TextWriter writer, string text)
        {
            writer.SetForeground(ConsoleColor.Cyan);
            writer.Write(text);
            writer.ResetColor();
        }

        public static void WriteNumber(this TextWriter writer, string text)
        {
            writer.SetForeground(ConsoleColor.DarkCyan);
            writer.Write(text);
            writer.ResetColor();
        }

        public static void WriteString(this TextWriter writer, string text)
        {
            writer.SetForeground(ConsoleColor.DarkYellow);
            writer.Write(text);
            writer.ResetColor();
        }

        public static void WritePunctuation(this TextWriter writer, string text)
        {
            writer.SetForeground(ConsoleColor.DarkGray);
            writer.Write(text);
            writer.ResetColor();
        }
        public static void WritePunctuation(this TextWriter writer, TokenType type)
        {
            writer.WritePunctuation(SyntaxFacts.GetText(type)!);
        }

        public static void WriteSpace(this TextWriter writer)
        {
            writer.WritePunctuation(" ");
        }

        public static void WriteDiagnostics(this TextWriter writer, IEnumerable<Diagnostic> diagnostics)
        {
            foreach (Diagnostic diag in diagnostics.OrderBy(d => d.Location.FileName)
                .ThenBy(d => d.Location.Span.Start)
                .ThenBy(d => d.Location.Span.Length))
            {
                TextLocation location = diag.Location;
                SourceText source = location.Source;
                string fileName = location.FileName;
                int startLine = location.StartLine + 1;
                int startColumn = location.StartColumn + 1;
                int endLine = location.EndLine + 1;
                int endColumn = location.EndColumn + 1;

                TextSpan span = location.Span;
                int lineIndex = source.GetLineIndex(span.Start);
                TextLine line = source.Lines[lineIndex];

                TextSpan prefixSpan = TextSpan.FromBounds(line.Span.Start, span.Start);
                TextSpan suffixSpan = TextSpan.FromBounds(span.End, line.Span.End);

                string prefix = source.ToString(prefixSpan);
                string error = source.ToString(span);
                string suffix = source.ToString(suffixSpan);

                writer.WriteLine();

                writer.SetForeground(ConsoleColor.DarkRed);
                writer.WriteLine($"{fileName}({startLine}, {startColumn}; {endLine}, {endColumn}):");
                writer.WriteLine(diag);

                writer.ResetColor();
                writer.Write("  ");
                writer.Write(prefix);
                writer.SetForeground(ConsoleColor.DarkRed);
                writer.Write(error);
                writer.ResetColor();
                writer.WriteLine(suffix);
            }

            writer.WriteLine();
        }
    }
}
