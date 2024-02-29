using MiniCompiler.CodeAnalysis;
using MiniCompiler.CodeAnalysis.Syntax;
using MiniCompiler.CodeAnalysis.Text;
using System.Text;

namespace MyCompiler
{
    internal abstract class Repl
    {
        private readonly StringBuilder textBuilder = new StringBuilder();

        public void Run()
        {
            PrintHelp();
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                if (textBuilder.Length == 0)
                    Console.Write("» ");
                else
                    Console.Write("· ");
                Console.ResetColor();

                string input = Console.ReadLine() ?? "";

                if (textBuilder.Length == 0 && input.StartsWith("#"))
                {
                    if (input.ToLower() == "#exit")
                        return;
                    EvaluateMetaCommand(input);
                    continue;
                }

                textBuilder.AppendLine(input);
                string text = textBuilder.ToString();

                if (!IsCompleteSubmission(text))
                    continue;

                EvaluateSubmission(text);

                textBuilder.Clear();
            }
        }

        protected void PrintHelp()
        {
            Console.WriteLine("#exit: exit the program");
            Console.WriteLine("#help: view this");
            Console.WriteLine("#clear: clear the screen");
            Console.WriteLine("#token: toggle token display");
            Console.WriteLine("#showTree: toggle syntax tree");
            Console.WriteLine("#showProgram: toggle bound tree");
            Console.WriteLine("#reset: reset the context");
        }

        protected void PrintDiagnostics(IReadOnlyList<Diagnostic> diagnostics, SyntaxTree syntaxTree)
        {
            foreach (Diagnostic diag in diagnostics)
            {
                int lineIndex = syntaxTree.SourceText.GetLineIndex(diag.Span.Start);
                TextLine line = syntaxTree.SourceText.Lines[lineIndex];
                int column = diag.Span.Start - line.Span.Start;

                TextSpan prefixSpan = TextSpan.FromBounds(line.Span.Start, diag.Span.Start);
                TextSpan suffixSpan = TextSpan.FromBounds(diag.Span.End, line.Span.End);

                string prefix = syntaxTree.SourceText.ToString(prefixSpan);
                string error = syntaxTree.SourceText.ToString(diag.Span);
                string suffix = syntaxTree.SourceText.ToString(suffixSpan);

                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write($"({lineIndex + 1}, {column + 1}): ");
                Console.WriteLine(diag);

                Console.ResetColor();
                Console.Write("  ");
                Console.Write(prefix);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(error);
                Console.ResetColor();
                Console.WriteLine(suffix);
            }
        }

        protected abstract bool IsCompleteSubmission(string text);
        protected abstract void EvaluateSubmission(string text);
        protected virtual void EvaluateMetaCommand(string input)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Invalid command {input}.");
            Console.ResetColor();
        }
    }
}
