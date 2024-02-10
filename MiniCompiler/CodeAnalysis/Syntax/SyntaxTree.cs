using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniCompiler.CodeAnalysis.Syntax.SyntaxNodes;
using MiniCompiler.CodeAnalysis.Text;

namespace MiniCompiler.CodeAnalysis.Syntax
{
    public sealed class SyntaxTree
    {
        public SyntaxTree(ImmutableArray<Diagnostic> diagnostics, ExpressionNode root, SourceText sourceText)
        {
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
            Parser parser = new Parser(text);
            return parser.Parse();
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
        public ExpressionNode Root { get; }
        public SourceText SourceText { get; }
    }
}
