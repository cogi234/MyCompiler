using MiniLang.CodeAnalysis.Syntax.SyntaxNodes;
using MiniLang.CodeAnalysis.Text;
using System.Collections.Immutable;

namespace MiniLang.CodeAnalysis.Syntax
{
    public sealed class SyntaxTree
    {
        private delegate void ParseHandler(SyntaxTree syntaxTree, out CompilationUnit? root,
            out ImmutableArray<Diagnostic> diagnostics);

        private SyntaxTree(SourceText sourceText, ParseHandler handler)
        {
            SourceText = sourceText;

            handler(this, out CompilationUnit? root, out ImmutableArray<Diagnostic> diagnostics);

            Diagnostics = diagnostics;
            Root = root!;
        }

        public static SyntaxTree Load(string fileName)
        {
            string text = File.ReadAllText(fileName);
            SourceText sourceText = SourceText.From(text, fileName);
            return Parse(sourceText);
        }

        public static SyntaxTree Parse(string text)
        {
            SourceText sourceText = SourceText.From(text);
            return Parse(sourceText);
        }
        public static SyntaxTree Parse(SourceText text)
        {
            return new SyntaxTree(text, Parse);
        }
        private static void Parse(SyntaxTree syntaxTree, out CompilationUnit root, out ImmutableArray<Diagnostic> diagnostics)
        {
            Parser parser = new Parser(syntaxTree);
            root = parser.ParseCompilationUnit();
            diagnostics = parser.Diagnostics.ToImmutableArray();
        }

        public static ImmutableArray<Token> ParseTokens(string text)
        {
            SourceText source = SourceText.From(text);
            return ParseTokens(source);
        }
        public static ImmutableArray<Token> ParseTokens(string text, out ImmutableArray<Diagnostic> diagnostics)
        {
            SourceText source = SourceText.From(text);
            return ParseTokens(source, out diagnostics);
        }
        public static ImmutableArray<Token> ParseTokens(SourceText source)
        {
            return ParseTokens(source, out _);
        }
        public static ImmutableArray<Token> ParseTokens(SourceText sourceText, out ImmutableArray<Diagnostic> diagnostics)
        {
            List<Token> tokens = new List<Token>();

            void ParseTokens(SyntaxTree syntaxTree, out CompilationUnit? root, out ImmutableArray<Diagnostic> diagnostics)
            {
                root = null;
                Lexer lexer = new Lexer(syntaxTree);

                while (true)
                {
                    Token token = lexer.NextToken();
                    if (token.Type == TokenType.EndOfFile)
                    {
                        root = new CompilationUnit(syntaxTree, ImmutableArray<MemberNode>.Empty);
                        break;
                    }

                    tokens.Add(token);
                }

                diagnostics = lexer.Diagnostics.ToImmutableArray();
            }

            SyntaxTree syntaxTree = new SyntaxTree(sourceText, ParseTokens);
            diagnostics = syntaxTree.Diagnostics.ToImmutableArray();
            return tokens.ToImmutableArray();
        }


        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public CompilationUnit Root { get; }
        public SourceText SourceText { get; }
    }
}
