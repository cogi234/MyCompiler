using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompiler.CodeAnalysis.Syntax
{
    internal static class SyntaxFacts
    {
        public static int GetBinaryOperatorPrecedence(this TokenType type)
        {
            switch (type)
            {
                case TokenType.Plus:
                case TokenType.Minus:
                    return 1;

                case TokenType.Star:
                case TokenType.ForwardSlash:
                    return 2;

                default:
                    return 0;
            }
        }

        public static int GetUnaryOperatorPrecedence(this TokenType type)
        {
            switch (type)
            {
                case TokenType.Plus:
                case TokenType.Minus:
                    return 3;

                default:
                    return 0;
            }
        }

        internal static TokenType GetKeywordType(string text)
        {
            switch (text)
            {
                case "false":
                    return TokenType.FalseKeyword;
                case "true":
                    return TokenType.TrueKeyword;
                default:
                    return TokenType.Identifier;
            }
        }
    }
}
