using MiniCompiler.CodeAnalysis.Syntax.SyntaxNodes;
using MiniCompiler.CodeAnalysis.Text;
using System.Collections.Immutable;

namespace MiniCompiler.CodeAnalysis.Syntax
{
    public sealed class SyntaxTree
    {
        private SyntaxTree(SourceText sourceText)
        {
            Parser parser = new Parser(sourceText);
            var root = parser.ParseCompilationUnit();
            var diagnostics = parser.Diagnostics.ToImmutableArray();

            Diagnostics = diagnostics;
            Root = root;
            SourceText = sourceText;
        }

        public static SyntaxTree Parse(string text)
        {
            SourceText sourceText = new SourceText(text);
            return Parse(sourceText);
        }
        public static SyntaxTree Parse(SourceText text)
        {
            return new SyntaxTree(text);
        }

        public static IEnumerable<Token> ParseTokens(string text)
        {
            SourceText source = new SourceText(text);
            return ParseTokens(source);
        }
        public static IEnumerable<Token> ParseTokens(SourceText text)
        {
            Lexer lexer = new Lexer(text);
            while (true)
            {
                Token token = lexer.NextToken();

                if (token.Type == TokenType.EndOfFile)
                    break;

                yield return token;
            }
        }


        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public CompilationUnit Root { get; }
        public SourceText SourceText { get; }
    }
}
