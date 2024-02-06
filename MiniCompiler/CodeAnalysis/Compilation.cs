using MiniCompiler.CodeAnalysis.Binding;
using MiniCompiler.CodeAnalysis.Syntax;

namespace MiniCompiler.CodeAnalysis
{
    public sealed class Compilation
    {
        public Compilation(SyntaxTree syntaxTree)
        {
            SyntaxTree = syntaxTree;
        }

        public SyntaxTree SyntaxTree { get; }

        public EvaluationResult Evaluate()
        {
            BoundTree boundTree = BoundTree.Bind(SyntaxTree);

            Diagnostic[] diagnostics = boundTree.Diagnostics.ToArray();

            if (diagnostics.Any())
            {
                return new EvaluationResult(diagnostics, null);
            }
            else
            {
                Evaluator evaluator = new Evaluator(boundTree.Root);
                object value = evaluator.Evaluate();

                return new EvaluationResult(diagnostics, value);
            }
        }
    }
}
