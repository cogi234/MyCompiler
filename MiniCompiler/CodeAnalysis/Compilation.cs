using MiniCompiler.CodeAnalysis.Binding;
using MiniCompiler.CodeAnalysis.Binding.BoundNodes;
using MiniCompiler.CodeAnalysis.Lowering;
using MiniCompiler.CodeAnalysis.Symbols;
using MiniCompiler.CodeAnalysis.Syntax;
using System.Collections.Immutable;

namespace MiniCompiler.CodeAnalysis
{
    public sealed class Compilation
    {
        private BoundGlobalScope? globalScope;

        private Compilation(Compilation? previous, SyntaxTree syntaxTree)
        {
            Previous = previous;
            SyntaxTree = syntaxTree;
        }
        public Compilation(SyntaxTree syntaxTree) : this(null, syntaxTree)
        {
        }

        public Compilation? Previous { get; }
        public SyntaxTree SyntaxTree { get; }

        internal BoundGlobalScope GlobalScope
        {
            get
            {
                if (globalScope == null)
                {
                    BoundGlobalScope globalScope = Binder.BindGlobalScope(Previous?.GlobalScope, SyntaxTree.Root);
                    Interlocked.CompareExchange(ref this.globalScope, globalScope, null);
                }

                return globalScope;
            }
        }

        public Compilation ContinueWith(SyntaxTree syntaxTree)
        {
            return new Compilation(this, syntaxTree);
        }

        public EvaluationResult Evaluate(Dictionary<VariableSymbol, object?> variables)
        {
            ImmutableArray<Diagnostic> diagnostics = SyntaxTree.Diagnostics.Concat(GlobalScope.Diagnostics).ToImmutableArray();

            if (diagnostics.Any())
            {
                return new EvaluationResult(diagnostics, null);
            }
            else
            {
                Evaluator evaluator = new Evaluator(GetStatement(), variables);
                object? value = evaluator.Evaluate();

                return new EvaluationResult(diagnostics.ToImmutableArray(), value);
            }
        }

        public void EmitTree(TextWriter writer)
        {
            GetStatement().PrettyPrint(writer);
        }

        private BoundBlockStatement GetStatement()
        {
            BoundStatement initialStatement = GlobalScope.Statement;
            return Lowerer.Lower(initialStatement);
        }
    }
}
