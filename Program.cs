namespace MyCompiler
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool tokenOutput = false;
            bool treeOutput = false;
            Console.WriteLine("#help: view this message\n#clear: clear the screen\n#token: toggle token display\n#tree: toggle syntax tree\n#exit: exit");
            while (true)
            {
                Console.Write("> ");
                string line = Console.ReadLine();

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
                else if (line.ToLower() == "#tree")
                {
                    treeOutput = !treeOutput;
                    Console.WriteLine(treeOutput ? "Toggled tree on" : "Toggled tree off");
                    continue;
                }
                else if (line.ToLower() == "#help")
                {
                    Console.WriteLine("#help: view this\n#clear: clear the screen\n#token: toggle token display\n#tree: toggle syntax tree");
                    continue;
                }

                if (tokenOutput)
                {
                    Lexer lexer = new Lexer(line);
                    while (true)
                    {
                        Token token = lexer.NextToken();
                        Console.Write($"{token.Type}: '{token.Text}'");
                        if (token.Value != null)
                            Console.Write($" {token.Value}");
                        Console.WriteLine();

                        if (token.Type == TokenType.EndOfFile)
                            break;
                    }
                    Console.WriteLine() ;
                }

                SyntaxTree syntaxTree = SyntaxTree.Parse(line);
                if (treeOutput)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;

                    PrettyPrintSyntaxNode(syntaxTree.Root);

                    Console.ResetColor();
                }

                if (!syntaxTree.Diagnostics.Any())
                {
                    Evaluator evaluator = new Evaluator(syntaxTree.Root);
                    int result = evaluator.Evaluate();
                    Console.WriteLine(result);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;

                    foreach (string error in syntaxTree.Diagnostics)
                    {
                        Console.WriteLine(error);
                    }

                    Console.ResetColor();
                }
            }
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
                    Console.Write($" {(node as LiteralExpressionNode).LiteralToken.Value}");
                    break;
                case NodeType.BinaryExpression:
                    Console.Write($" {(node as BinaryExpressionNode).OperatorToken.Text}");
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
