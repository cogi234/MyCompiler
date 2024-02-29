using MiniCompiler.CodeAnalysis;
using MiniCompiler.CodeAnalysis.Syntax;
using MiniCompiler.CodeAnalysis.Text;

namespace MyCompiler
{
    internal sealed class MiniRepl : Repl
    {
        private Compilation? previousCompilation = null;
        private Dictionary<VariableSymbol, object> variables = new Dictionary<VariableSymbol, object>();

        private bool tokenOutput = false;
        private bool syntaxTreeOutput = false;
        private bool boundTreeOutput = false;

        protected override void EvaluateSubmission(string text)
        {
            SyntaxTree syntaxTree = SyntaxTree.Parse(text);

            Compilation compilation = previousCompilation == null
                ? new Compilation(syntaxTree)
                : previousCompilation.ContinueWith(syntaxTree);

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
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkGray;

                syntaxTree.Root.PrettyPrint(Console.Out);

                Console.ResetColor();
            }

            if (boundTreeOutput)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkGray;

                compilation.EmitTree(Console.Out);

                Console.ResetColor();
            }

            EvaluationResult result = compilation.Evaluate(variables);

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

        protected override bool IsCompleteSubmission(string text)
        {
            if (string.IsNullOrEmpty(text))
                return false;

            SyntaxTree syntaxTree = SyntaxTree.Parse(text);

            if (syntaxTree.Diagnostics.Any())
                return false;

            return true;
        }

        protected override void RenderLine(string line)
        {
            IEnumerable<Token> tokens = SyntaxTree.ParseTokens(line);

            foreach (Token token in tokens)
            {
                bool isKeyword = token.Type.ToString().EndsWith("Keyword");
                bool isOperator = SyntaxFacts.GetUnaryOperatorPrecedence(token.Type) > 0 ||
                    SyntaxFacts.GetBinaryOperatorPrecedence(token.Type) > 0;
                if (isKeyword)
                    Console.ForegroundColor = ConsoleColor.Blue;
                else if (isOperator)
                    Console.ForegroundColor = ConsoleColor.Gray;
                else if (token.Type == TokenType.Number)
                    Console.ForegroundColor = ConsoleColor.White;
                else if (token.Type == TokenType.Identifier)
                    Console.ForegroundColor = ConsoleColor.Cyan;
                else
                    Console.ForegroundColor = ConsoleColor.DarkGray;

                Console.Write(token.Text);

                Console.ResetColor();
            }
        }

        protected override void EvaluateMetaCommand(string input)
        {
            string lowerInput = input.ToLower();
            switch (lowerInput)
            {
                case "#clear":
                    Console.Clear();
                    break;
                case "#token":
                    tokenOutput = !tokenOutput;
                    Console.WriteLine(tokenOutput ? "Toggled tokens on" : "Toggled tokens off");
                    break;
                case "#showtree":
                    syntaxTreeOutput = !syntaxTreeOutput;
                    Console.WriteLine(syntaxTreeOutput ? "Toggled syntax tree on" : "Toggled syntax tree off");
                    break;
                case "#showprogram":
                    boundTreeOutput = !boundTreeOutput;
                    Console.WriteLine(boundTreeOutput ? "Toggled bound tree on" : "Toggled bound tree off");
                    break;
                case "#reset":
                    previousCompilation = null;
                    variables = new Dictionary<VariableSymbol, object>();
                    Console.WriteLine("Reset the context");
                    break;
                case "#help":
                    PrintHelp();
                    break;
                default:
                    base.EvaluateMetaCommand(input);
                    break;
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
    }
}
