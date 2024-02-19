using MiniCompiler.CodeAnalysis.Binding;
using MiniCompiler.CodeAnalysis.Binding.BoundNodes;
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
            BoundGlobalScope globalScope = Binder.BindGlobalScope(SyntaxTree.Root);

            ImmutableArray<Diagnostic> diagnostics = SyntaxTree.Diagnostics.Concat(globalScope.Diagnostics).ToImmutableArray();

            if (diagnostics.Any())
            {
                return new EvaluationResult(diagnostics, null);
            }
            else
            {
                Evaluator evaluator = new Evaluator(globalScope.Expression, variables);
                object value = evaluator.Evaluate();

                return new EvaluationResult(diagnostics.ToImmutableArray(), value);
            }
        }
    }
}
