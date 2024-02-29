using MiniCompiler.CodeAnalysis;
using MiniCompiler.CodeAnalysis.Syntax;

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
    }
}
