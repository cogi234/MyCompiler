﻿namespace MiniCompiler.CodeAnalysis.Syntax
{
    public static class SyntaxFacts
    {
        public static int GetUnaryOperatorPrecedence(this TokenType type)
        {
            switch (type)
            {
                case TokenType.Plus:
                case TokenType.Minus:
                    return 6;

                case TokenType.Bang:
                    return 6;

                default:
                    return 0;
            }
        }

        public static int GetBinaryOperatorPrecedence(this TokenType type)
        {
            switch (type)
            {
                case TokenType.Star:
                case TokenType.ForwardSlash:
                    return 5;

                case TokenType.Plus:
                case TokenType.Minus:
                    return 4;

                case TokenType.EqualEqual:
                case TokenType.BangEqual:
                    return 3;

                case TokenType.AmpersandAmpersand:
                    return 2;
                case TokenType.PipePipe:
                    return 1;

                default:
                    return 0;
            }
        }

        public static TokenType GetKeywordType(string text)
        {
            switch (text)
            {
                case "false":
                    return TokenType.FalseKeyword;
                case "true":
                    return TokenType.TrueKeyword;
                case "var":
                    return TokenType.VarKeyword;
                case "let":
                    return TokenType.LetKeyword;
                default:
                    return TokenType.Identifier;
            }
        }

        public static IEnumerable<TokenType> GetUnaryOperatorTypes()
        {
            TokenType[] types = (TokenType[])Enum.GetValues(typeof(TokenType));
            foreach (TokenType type in types)
            {
                if (GetUnaryOperatorPrecedence(type) > 0)
                    yield return type;
            }
        }

        public static IEnumerable<TokenType> GetBinaryOperatorTypes()
        {
            TokenType[] types = (TokenType[])Enum.GetValues(typeof(TokenType));
            foreach (TokenType type in types)
            {
                if (GetBinaryOperatorPrecedence(type) > 0)
                    yield return type;
            }
        }

        public static string? GetText(TokenType tokenType)
        {
            switch (tokenType)
            {
                case TokenType.Plus:
                    return "+";
                case TokenType.Minus:
                    return "-";
                case TokenType.Star:
                    return "*";
                case TokenType.ForwardSlash:
                    return "/";
                case TokenType.OpenParenthesis:
                    return "(";
                case TokenType.CloseParenthesis:
                    return ")";
                case TokenType.OpenBrace:
                    return "{";
                case TokenType.CloseBrace:
                    return "}";
                case TokenType.Semicolon:
                    return ";";
                case TokenType.Bang:
                    return "!";
                case TokenType.BangEqual:
                    return "!=";
                case TokenType.Equal:
                    return "=";
                case TokenType.EqualEqual:
                    return "==";
                case TokenType.AmpersandAmpersand:
                    return "&&";
                case TokenType.PipePipe:
                    return "||";
                case TokenType.FalseKeyword:
                    return "false";
                case TokenType.TrueKeyword:
                    return "true";
                case TokenType.VarKeyword:
                    return "var";
                case TokenType.LetKeyword:
                    return "let";
                default:
                    return null;
            }
        }

    }
}
