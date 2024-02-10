using MiniCompiler.CodeAnalysis.Binding;
using MiniCompiler.CodeAnalysis.Syntax;
using System.Collections.Immutable;

namespace MiniCompiler.CodeAnalysis
{
    public sealed class Compilation
    {
        public Compilation(SyntaxTree syntaxTree)
        {
            SyntaxTree = syntaxTree;
        }

        public SyntaxTree SyntaxTree { get; }

        public EvaluationResult Evaluate(Dictionary<VariableSymbol, object> variables)
        {
            BoundTree boundTree = BoundTree.Bind(SyntaxTree, variables);

            Diagnostic[] diagnostics = boundTree.Diagnostics.ToArray();

            if (diagnostics.Any())
            {
                return new EvaluationResult(diagnostics.ToImmutableArray(), null);
            }
            else
            {
                Evaluator evaluator = new Evaluator(boundTree.Root, variables);
                object value = evaluator.Evaluate();

                return new EvaluationResult(diagnostics.ToImmutableArray(), value);
            }
        }
    }
}
