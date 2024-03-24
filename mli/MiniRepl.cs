using MiniLang.CodeAnalysis;
using MiniLang.CodeAnalysis.Symbols;
using MiniLang.CodeAnalysis.Syntax;
using MiniLang.CodeAnalysis.Text;
using MiniLang.IO;

namespace mi
{
    internal sealed class MiniRepl : Repl
    {
        private static bool loadingSubmissions;

        private Compilation? previousCompilation = null;
        private Dictionary<VariableSymbol, object?> variables = new Dictionary<VariableSymbol, object?>();

        private bool tokenOutput = false;
        private bool syntaxTreeOutput = false;
        private bool boundTreeOutput = false;

        public MiniRepl()
        {
            LoadSubmissions();
        }

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
                Console.ForegroundColor = ConsoleColor.DarkGray;

                syntaxTree.Root.PrettyPrint(Console.Out);

                Console.ResetColor();
            }

            if (boundTreeOutput)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;

                compilation.EmitTree(Console.Out);

                Console.ResetColor();
            }

            EvaluationResult result = compilation.Evaluate(variables);

            if (result.Diagnostics.Any())
            {
                Console.Out.WriteDiagnostics(result.Diagnostics);
            }
            else
            {
                previousCompilation = compilation;

                SaveSubmission(text);
            }
        }

        protected override bool IsCompleteSubmission(string text)
        {
            if (string.IsNullOrEmpty(text))
                return true;

            bool lastTwoLinesAreBlank = text.Split(Environment.NewLine)
                .Reverse()
                .TakeWhile(string.IsNullOrWhiteSpace).Count() >= 2;
            if (lastTwoLinesAreBlank)
                return true;

            SyntaxTree syntaxTree = SyntaxTree.Parse(text);

            if (syntaxTree.Root.GetLastToken().IsFake)
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
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                else if (token.Type == TokenType.String)
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                else if (token.Type == TokenType.Identifier)
                    Console.ForegroundColor = ConsoleColor.Cyan;
                else if (token.Type == TokenType.Type)
                    Console.ForegroundColor = ConsoleColor.Yellow;
                else
                    Console.ForegroundColor = ConsoleColor.DarkGray;

                Console.Write(token.Text);

                Console.ResetColor();
            }
        }

        [MetaCommand("reset", "Resets the context.")]
        private void EvaluateReset()
        {
            previousCompilation = null;
            variables = new Dictionary<VariableSymbol, object?>();
            ClearSubmissions();
        }
        [MetaCommand("showTokens", "Displays tokens.")]
        private void EvaluateShowTokens()
        {
            tokenOutput = !tokenOutput;
            Console.WriteLine(tokenOutput ? "Toggled tokens on" : "Toggled tokens off");
        }
        [MetaCommand("showTree", "Displays the syntax tree.")]
        private void EvaluateShowTree()
        {
            syntaxTreeOutput = !syntaxTreeOutput;
            Console.WriteLine(syntaxTreeOutput ? "Toggled syntax tree on" : "Toggled syntax tree off");
        }
        [MetaCommand("showProgram", "Displays the program's bound tree.")]
        private void EvaluateShowProgram()
        {
            boundTreeOutput = !boundTreeOutput;
            Console.WriteLine(boundTreeOutput ? "Toggled bound tree on" : "Toggled bound tree off");
        }
        [MetaCommand("load", "Loads a script file.")]
        private void EvaluateLoad(string path)
        {
            path = Path.GetFullPath(path);

            if (!File.Exists(path))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"error: file does not exist '{path}'");
                Console.ResetColor();
                return;
            }

            string text = File.ReadAllText(path);
            textToEdit = text;
        }
        [MetaCommand("dump", "Shows bound tree of a given function.")]
        private void EvaluateDump(string functionName)
        {
            if (previousCompilation == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"error: function '{functionName}' does not exist");
                Console.ResetColor();
                return;
            }

            var function = previousCompilation.GetSymbols().OfType<FunctionSymbol>().
                SingleOrDefault(f => f.Name == functionName);
            if (function == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"error: function '{functionName}' does not exist");
                Console.ResetColor();
                return;
            }

            previousCompilation.EmitTree(Console.Out, function);
        }
        [MetaCommand("symbols", "Lists all symbols.")]
        private void EvaluateSymbols()
        {
            if (previousCompilation == null)
                return;

            IOrderedEnumerable<Symbol> symbols = previousCompilation.GetSymbols().OrderBy(s => s.SymbolType)
                .ThenBy(s => s.Name);
            foreach (Symbol symbol in symbols)
            {
                symbol.WriteTo(Console.Out);
                Console.WriteLine();
            }
        }

        private static string GetSubmissionsDirectory()
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string submissionsDirectory = Path.Combine(localAppData, "MiniLang", "Submissions");
            return submissionsDirectory;
        }

        private void LoadSubmissions()
        {
            string submissionsDirectory = GetSubmissionsDirectory();
            if (!Directory.Exists(submissionsDirectory))
                return;

            var files = Directory.GetFiles(submissionsDirectory).OrderBy(f => f).ToArray();
            if (files.Length == 0)
                return;

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"Loaded {files.Length} submission(s).");
            Console.ResetColor();

            loadingSubmissions = true;

            foreach (string file in files)
            {
                string text = File.ReadAllText(file);
                EvaluateSubmission(text);
            }

            loadingSubmissions = false;
        }

        private static void ClearSubmissions()
        {
            Directory.Delete(GetSubmissionsDirectory(), true);
        }

        private static void SaveSubmission(string text)
        {
            if (loadingSubmissions)
                return;

            string submissionDirectory = GetSubmissionsDirectory();
            Directory.CreateDirectory(submissionDirectory);
            int count = Directory.GetFiles(submissionDirectory).Length;
            string name = $"submission{count:0000}";
            string fileName = Path.Combine(submissionDirectory, name);
            File.WriteAllText(fileName, text);
        }
    }
}
