using MiniCompiler.CodeAnalysis;
using MiniCompiler.CodeAnalysis.Binding;
using MiniCompiler.CodeAnalysis.Syntax;
using MiniCompiler.CodeAnalysis.Text;
using System.Text;

namespace MyCompiler
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool tokenOutput = false;
            bool syntaxTreeOutput = false;

            Dictionary<VariableSymbol, object> variables = new Dictionary<VariableSymbol, object>();
            Compilation? previousCompilation = null;
            StringBuilder textBuilder = new StringBuilder();

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
                bool isBlank = string.IsNullOrWhiteSpace(input);

                if (textBuilder.Length == 0)
                {
                    if (input.ToLower() == "#exit")
                        return;
                    else if (input.ToLower() == "#clear")
                    {
                        Console.Clear();
                        continue;
                    }
                    else if (input.ToLower() == "#token")
                    {
                        tokenOutput = !tokenOutput;
                        Console.WriteLine(tokenOutput ? "Toggled tokens on" : "Toggled tokens off");
                        continue;
                    }
                    else if (input.ToLower() == "#tree")
                    {
                        syntaxTreeOutput = !syntaxTreeOutput;
                        Console.WriteLine(syntaxTreeOutput ? "Toggled syntax tree on" : "Toggled syntax tree off");
                        continue;
                    }
                    else if (input.ToLower() == "#reset")
                    {
                        previousCompilation = null;
                        variables = new Dictionary<VariableSymbol, object>();
                        Console.WriteLine("Reset the context");
                        continue;
                    }
                    else if (input.ToLower() == "#help")
                    {
                        PrintHelp();
                        continue;
                    }
                }

                textBuilder.AppendLine(input);
                string text = textBuilder.ToString();

                SyntaxTree syntaxTree = SyntaxTree.Parse(text);

                if ((!isBlank && syntaxTree.Diagnostics.Any()) || isBlank)
                    continue;

                textBuilder.Clear();
                Compilation compilation = previousCompilation == null
                    ? new Compilation(syntaxTree)
                    : previousCompilation.ContinueWith(syntaxTree);
                EvaluationResult result = compilation.Evaluate(variables);

                if (tokenOutput)
                {
                    IEnumerable<Token> tokens = SyntaxTree.ParseTokens(text);
                    foreach (Token token in tokens)
                    {
                        Console.Write($"{token.Type}: '{token.Text}'");
                        if (token.Value != null)
                            Console.Write($" {token.Value}");
                        Console.WriteLine();
                    }
                }

                if (syntaxTreeOutput)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;

                    syntaxTree.Root.PrettyPrint(Console.Out);

                    Console.ResetColor();
                }

                if (result.Diagnostics.Any())
                    PrintDiagnostics(result.Diagnostics, syntaxTree);
                else
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine(result.Value);
                    Console.ResetColor();

                    previousCompilation = compilation;
                }
            }
        }

        static void PrintHelp()
        {
            Console.WriteLine("#exit: exit the program");
            Console.WriteLine("#help: view this");
            Console.WriteLine("#clear: clear the screen");
            Console.WriteLine("#token: toggle token display");
            Console.WriteLine("#tree: toggle syntax tree");
            Console.WriteLine("#reset: reset the context");
        }

        static void PrintDiagnostics(IReadOnlyList<Diagnostic> diagnostics, SyntaxTree syntaxTree)
        {
            foreach (Diagnostic diag in diagnostics)
            {
                int lineIndex = syntaxTree.SourceText.GetLineIndex(diag.Span.Start);
                TextLine line = syntaxTree.SourceText.Lines[lineIndex];
                int column = diag.Span.Start - line.Span.Start;

                var prefixSpan = TextSpan.FromBounds(line.Span.Start, diag.Span.Start);
                var suffixSpan = TextSpan.FromBounds(diag.Span.End, line.Span.End);

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
    }
}
