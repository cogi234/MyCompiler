using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniCompiler.CodeAnalysis.Syntax.SyntaxNodes;

namespace MiniCompiler.CodeAnalysis.Syntax
{
    public sealed class SyntaxTree
    {
        public SyntaxTree(IEnumerable<Diagnostic> diagnostics, ExpressionNode root)
        {
            Diagnostics = diagnostics.ToArray();
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


        public IReadOnlyList<Diagnostic> Diagnostics { get; }
        public ExpressionNode Root { get; }
        public IReadOnlyList<Token> Tokens { get; }
    }
}
