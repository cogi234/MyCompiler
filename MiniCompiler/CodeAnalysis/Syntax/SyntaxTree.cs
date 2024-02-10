using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniCompiler.CodeAnalysis.Syntax.SyntaxNodes;

namespace MiniCompiler.CodeAnalysis.Syntax
{
    public sealed class SyntaxTree
    {
        public SyntaxTree(ImmutableArray<Diagnostic> diagnostics, ExpressionNode root)
        {
            Diagnostics = diagnostics;
            Root = root;
        }

        public static SyntaxTree Parse(string text)
        {
            Parser parser = new Parser(text);
            return parser.Parse();
        }

        public static IEnumerable<Token> ParseTokens(string text)
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
    }
}
