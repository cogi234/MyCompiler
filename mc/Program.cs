using MiniLang.CodeAnalysis;
using MiniLang.CodeAnalysis.Symbols;
using MiniLang.CodeAnalysis.Syntax;
using MiniLang.IO;

namespace mc
{
    internal class Program
    {
        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.Error.WriteLine("usage: mc <source-paths>");
                return 1;
            }

            IEnumerable<string> paths = GetFilePaths(args);
            List<SyntaxTree> syntaxTrees = new List<SyntaxTree>();
            bool hasErrors = false;

            foreach (string path in paths)
            {
                if (!File.Exists(path))
                {
                    Console.Error.WriteLine($"error: file '{path}' doesn't exist");
                    hasErrors = true;
                    continue;
                }

                SyntaxTree syntaxTree = SyntaxTree.Load( path);
                syntaxTrees.Add(syntaxTree);
            }

            if (hasErrors)
                return 1;

            Compilation compilation = new Compilation(syntaxTrees.ToArray());
            EvaluationResult result = compilation.Evaluate(new Dictionary<VariableSymbol, object?>());

            if (!result.Diagnostics.Any())
            {
                if (result.Value != null)
                    Console.WriteLine(result.Value);
            }
            else
            {
                Console.Error.WriteDiagnostics(result.Diagnostics);
                return 1;
            }

            return 0;
        }

        private static IEnumerable<string> GetFilePaths(IEnumerable<string> paths)
        {
            SortedSet<string> result = new SortedSet<string>();

            foreach (string path in paths)
            {
                string absolutePath = Path.GetFullPath(path);
                if (Directory.Exists(absolutePath))
                    result.UnionWith(Directory.EnumerateFiles(absolutePath, "*.ml", SearchOption.AllDirectories));
                else
                    result.Add(absolutePath);
            }

            return result;
        }
    }
}
