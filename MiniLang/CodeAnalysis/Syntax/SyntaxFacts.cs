using MiniLang.CodeAnalysis.Symbols;

namespace MiniLang.CodeAnalysis.Syntax
{
    public static class SyntaxFacts
    {
        public static int GetUnaryOperatorPrecedence(this TokenType type)
        {
            switch (type)
            {
                case TokenType.Plus:
                case TokenType.Minus:
                case TokenType.Bang:
                case TokenType.Tilde:
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
                case TokenType.LessThan:
                case TokenType.LessThanEqual:
                case TokenType.GreaterThan:
                case TokenType.GreaterThanEqual:
                    return 3;

                case TokenType.Ampersand:
                case TokenType.AmpersandAmpersand:
                    return 2;

                case TokenType.Pipe:
                case TokenType.PipePipe:
                case TokenType.Caret:
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
                case "if":
                    return TokenType.IfKeyword;
                case "else":
                    return TokenType.ElseKeyword;
                case "do":
                    return TokenType.DoKeyword;
                case "while":
                    return TokenType.WhileKeyword;
                case "for":
                    return TokenType.ForKeyword;
                case "continue":
                    return TokenType.ContinueKeyword;
                case "break":
                    return TokenType.BreakKeyword;
                case "return":
                    return TokenType.ReturnKeyword;
                default:
                    if (TypeSymbol.Lookup(text) == null)
                        return TokenType.Identifier;
                    return TokenType.Type;
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
                case TokenType.Period:
                    return ".";
                case TokenType.Comma:
                    return ",";
                case TokenType.Colon:
                    return ":";
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
                case TokenType.LessThan:
                    return "<";
                case TokenType.LessThanEqual:
                    return "<=";
                case TokenType.GreaterThan:
                    return ">";
                case TokenType.GreaterThanEqual:
                    return ">=";
                case TokenType.Ampersand:
                    return "&";
                case TokenType.AmpersandAmpersand:
                    return "&&";
                case TokenType.Pipe:
                    return "|";
                case TokenType.PipePipe:
                    return "||";
                case TokenType.Caret:
                    return "^";
                case TokenType.Tilde:
                    return "~";
                case TokenType.FalseKeyword:
                    return "false";
                case TokenType.TrueKeyword:
                    return "true";
                case TokenType.VarKeyword:
                    return "var";
                case TokenType.IfKeyword:
                    return "if";
                case TokenType.ElseKeyword:
                    return "else";
                case TokenType.DoKeyword:
                    return "do";
                case TokenType.WhileKeyword:
                    return "while";
                case TokenType.ForKeyword:
                    return "for";
                case TokenType.ContinueKeyword:
                    return "continue";
                case TokenType.BreakKeyword:
                    return "break";
                case TokenType.ReturnKeyword:
                    return "return";
                default:
                    return null;
            }
        }

    }
}
