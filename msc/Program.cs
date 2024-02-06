using MiniCompiler.CodeAnalysis;
using MiniCompiler.CodeAnalysis.Binding;
using MiniCompiler.CodeAnalysis.Syntax;
using MiniCompiler.CodeAnalysis.Syntax.SyntaxNodes;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace MyCompiler
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool tokenOutput = false;
            bool syntaxTreeOutput = false;

            Dictionary<string, object> variables = new Dictionary<string, object>();

            PrintHelp();
            while (true)
            {
                Console.Write("> ");
                string line = Console.ReadLine() ?? "";

                if (string.IsNullOrEmpty(line))
                    continue;
                else if (line.ToLower() == "#exit")
                    return;
                else if (line.ToLower() == "#clear")
                {
                    Console.Clear();
                    continue;
                }
                else if (line.ToLower() == "#token")
                {
                    tokenOutput = !tokenOutput;
                    Console.WriteLine(tokenOutput ? "Toggled tokens on" : "Toggled tokens off");
                    continue;
                }
                else if (line.ToLower() == "#syntax")
                {
                    syntaxTreeOutput = !syntaxTreeOutput;
                    Console.WriteLine(syntaxTreeOutput ? "Toggled syntax tree on" : "Toggled syntax tree off");
                    continue;
                }
                else if (line.ToLower() == "#help")
                {
                    PrintHelp();
                    continue;
                }

                SyntaxTree syntaxTree = SyntaxTree.Parse(line);

                if (tokenOutput)
                {
                    IReadOnlyList<Token> tokens = syntaxTree.Tokens;
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

                    PrettyPrintSyntaxNode(syntaxTree.Root);

                    Console.ResetColor();
                }

                Compilation compilation = new Compilation(syntaxTree);
                EvaluationResult result = compilation.Evaluate(variables);

                PrintDiagnostics(result.Diagnostics, line);

                if (result.Value != null)
                    Console.WriteLine(result.Value);
            }
        }

        static void PrintHelp()
        {
            Console.WriteLine("#exit: exit the program");
            Console.WriteLine("#help: view this");
            Console.WriteLine("#clear: clear the screen");
            Console.WriteLine("#token: toggle token display");
            Console.WriteLine("#syntax: toggle syntax tree");
        }

        static void PrintDiagnostics(IReadOnlyList<Diagnostic> diagnostics, string line)
        {
            foreach (Diagnostic diag in diagnostics)
            {
                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(diag);
                Console.ResetColor();

                string prefix = line.Substring(0, diag.Span.Start);
                string error = line.Substring(diag.Span.Start, diag.Span.Length);
                string suffix = line.Substring(diag.Span.End);

                Console.Write("  ");
                Console.Write(prefix);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(error);
                Console.ResetColor();
                Console.WriteLine(suffix);
            }

            if (diagnostics.Any())
                Console.WriteLine();
        }

        static void PrettyPrintSyntaxNode(SyntaxNode node, string indent = "", bool isLast = true)
        {
            // ├ ─ └ │

            string marker = isLast ? "└──" : "├──";

            Console.Write(indent);
            Console.Write(marker);
            Console.Write(node.Type);

            switch (node.Type)
            {
                case NodeType.LiteralExpression:
                    Console.Write($" {((LiteralExpressionNode)node).Value}");
                    break;
                case NodeType.BinaryExpression:
                    Console.Write($" {((BinaryExpressionNode)node).OperatorToken.Text}");
                    break;
                case NodeType.UnaryExpression:
                    Console.Write($" {((UnaryExpressionNode)node).OperatorToken.Text}");
                    break;
            }

            Console.WriteLine();

            indent += isLast ? "   " : "│  ";

            SyntaxNode lastChild = node.GetChildren().LastOrDefault();
            foreach (SyntaxNode child in node.GetChildren())
            {
                PrettyPrintSyntaxNode(child, indent, child == lastChild);
            }
        }
    }
}
