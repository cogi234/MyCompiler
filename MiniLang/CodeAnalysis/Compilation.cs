using MiniLang.CodeAnalysis.Binding;
using MiniLang.CodeAnalysis.Binding.BoundNodes;
using MiniLang.CodeAnalysis.Symbols;
using MiniLang.CodeAnalysis.Syntax;
using MiniLang.IO;
using System.Collections.Immutable;

namespace MiniLang.CodeAnalysis
{
    public sealed class Compilation
    {
        private BoundGlobalScope? globalScope;

        private Compilation(Compilation? previous, params SyntaxTree[] syntaxTrees)
        {
            Previous = previous;
            SyntaxTrees = syntaxTrees.ToImmutableArray();
        }
        public Compilation(params SyntaxTree[] syntaxTrees) : this(null, syntaxTrees)
        {
        }

        public Compilation? Previous { get; }
        public ImmutableArray<SyntaxTree> SyntaxTrees { get; }

        internal BoundGlobalScope GlobalScope
        {
            get
            {
                if (globalScope == null)
                {
                    BoundGlobalScope globalScope = Binder.BindGlobalScope(Previous?.GlobalScope, SyntaxTrees);
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
            IEnumerable<Diagnostic> parseDiagnostics = SyntaxTrees.SelectMany(st => st.Diagnostics);

            ImmutableArray<Diagnostic> diagnostics = parseDiagnostics.Concat(GlobalScope.Diagnostics).ToImmutableArray();

            if (diagnostics.Any())
                return new EvaluationResult(diagnostics, null);

            BoundProgram program = Binder.BindProgram(GlobalScope);

            string appPath = Environment.GetCommandLineArgs()[0];
            string appDirectory = Path.GetDirectoryName(appPath)!;
            string cfgPath = Path.Combine(appDirectory, "cfg.dot");
            BoundBlockStatement cfgStatement = !program.Statement.Statements.Any() && program.Functions.Any()
                ? program.Functions.Last().Value
                : program.Statement;
            ControlFlowGraph cfg = ControlFlowGraph.Create(cfgStatement);
            using (StreamWriter streamWriter = new StreamWriter(cfgPath))
                cfg.WriteTo(streamWriter);

            if (program.Diagnostics.Any())
                return new EvaluationResult(program.Diagnostics.ToImmutableArray(), null);

            Evaluator evaluator = new Evaluator(program, variables);
            object? value = evaluator.Evaluate();

            return new EvaluationResult(diagnostics.ToImmutableArray(), value);
        }

        public void EmitTree(TextWriter writer)
        {
            BoundProgram program = Binder.BindProgram(GlobalScope);

            if (program.Statement.Statements.Any())
                program.Statement.WriteTo(writer);
            else
            {
                foreach (KeyValuePair<FunctionSymbol, BoundBlockStatement> function in program.Functions)
                {
                    if (!GlobalScope.Functions.Contains(function.Key))
                        continue;
                    function.Key.WriteTo(writer);
                    function.Value.WriteTo(writer);
                }
            }
        }
    }
}
