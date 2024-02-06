using MiniCompiler.CodeAnalysis;
using MiniCompiler.CodeAnalysis.Binding;
using MiniCompiler.CodeAnalysis.Syntax;
using MiniCompiler.CodeAnalysis.Syntax.SyntaxNodes;

namespace MyCompiler
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool tokenOutput = false;
            bool syntaxTreeOutput = false;

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
                EvaluationResult result = compilation.Evaluate();

                IReadOnlyList<Diagnostic> diagnostics = result.Diagnostics;

                Console.ForegroundColor = ConsoleColor.DarkRed;

                foreach (Diagnostic error in diagnostics)
                {
                    Console.WriteLine(error);
                }

                Console.ResetColor();

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
