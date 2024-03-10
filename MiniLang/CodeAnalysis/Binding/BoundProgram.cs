﻿using MiniLang.CodeAnalysis.Binding.BoundNodes;
using MiniLang.CodeAnalysis.Symbols;
using System.Collections.Immutable;

namespace MiniLang.CodeAnalysis.Binding
{
    internal sealed class BoundProgram
    {
        public BoundProgram(ImmutableArray<Diagnostic> diagnostics,
            ImmutableDictionary<FunctionSymbol, BoundBlockStatement> functions, BoundBlockStatement statement)
        {
            Diagnostics = diagnostics;
            Functions = functions;
            Statement = statement;
        }

        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public ImmutableDictionary<FunctionSymbol, BoundBlockStatement> Functions { get; }
        public BoundBlockStatement Statement { get; }
    }
}
