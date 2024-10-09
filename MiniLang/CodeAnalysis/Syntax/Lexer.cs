using MiniLang.CodeAnalysis.Symbols;
using MiniLang.CodeAnalysis.Text;
using System.Text;

namespace MiniLang.CodeAnalysis.Syntax
{
    internal sealed class Lexer
    {
        private readonly SourceText source;
        private readonly SyntaxTree syntaxTree;
        private readonly DiagnosticBag diagnostics = new DiagnosticBag();
        public DiagnosticBag Diagnostics => diagnostics;

        private int position = 0;
        private int start;
        private TokenType tokenType;
        private object? tokenValue;
        private string? tokenText;

        public Lexer(SyntaxTree syntaxTree)
        {
            this.syntaxTree = syntaxTree;
            source = syntaxTree.SourceText;
        }

        public Token NextToken()
        {
            start = position;
            tokenType = TokenType.BadToken;
            tokenValue = null;
            tokenText = null;

            switch (Current)
            {
                case '\0':
                    return new Token(syntaxTree, TokenType.EndOfFile, new TextSpan(start, 0), "\0", null);
                case '+':
                    NextCharacter();
                    tokenType = TokenType.Plus;
                    break;
                case '-':
                    NextCharacter();
                    tokenType = TokenType.Minus;
                    break;
                case '*':
                    NextCharacter();
                    tokenType = TokenType.Star;
                    break;
                case '/':
                    NextCharacter();
                    tokenType = TokenType.ForwardSlash;
                    break;
                case '%':
                    NextCharacter();
                    tokenType = TokenType.Percent;
                    break;
                case '(':
                    NextCharacter();
                    tokenType = TokenType.OpenParenthesis;
                    break;
                case ')':
                    NextCharacter();
                    tokenType = TokenType.CloseParenthesis;
                    break;
                case '{':
                    NextCharacter();
                    tokenType = TokenType.OpenBrace;
                    break;
                case '}':
                    NextCharacter();
                    tokenType = TokenType.CloseBrace;
                    break;
                case '.':
                    NextCharacter();
                    tokenType = TokenType.Period;
                    break;
                case ',':
                    NextCharacter();
                    tokenType = TokenType.Comma;
                    break;
                case ':':
                    NextCharacter();
                    tokenType = TokenType.Colon;
                    break;
                case ';':
                    NextCharacter();
                    tokenType = TokenType.Semicolon;
                    break;
                case '!':
                    NextCharacter();
                    if (Current != '=')
                        tokenType = TokenType.Bang;
                    else
                    {
                        tokenType = TokenType.BangEqual;
                        NextCharacter();
                    }
                    break;
                case '&':
                    NextCharacter();
                    if (Current != '&')
                        tokenType = TokenType.Ampersand;
                    else
                    {
                        tokenType = TokenType.AmpersandAmpersand;
                        NextCharacter();
                    }
                    break;
                case '|':
                    NextCharacter();
                    if (Current != '|')
                        tokenType = TokenType.Pipe;
                    else
                    {
                        tokenType = TokenType.PipePipe;
                        NextCharacter();
                    }
                    break;
                case '^':
                    NextCharacter();
                    tokenType = TokenType.Caret;
                    break;
                case '~':
                    NextCharacter();
                    tokenType = TokenType.Tilde;
                    break;
                case '<':
                    NextCharacter();
                    if (Current != '=')
                        tokenType = TokenType.LessThan;
                    else
                    {
                        tokenType = TokenType.LessThanEqual;
                        NextCharacter();
                    }
                    break;
                case '>':
                    NextCharacter();
                    if (Current != '=')
                        tokenType = TokenType.GreaterThan;
                    else
                    {
                        tokenType = TokenType.GreaterThanEqual;
                        NextCharacter();
                    }
                    break;
                case '=':
                    NextCharacter();
                    if (Current != '=')
                        tokenType = TokenType.Equal;
                    else
                    {
                        tokenType = TokenType.EqualEqual;
                        NextCharacter();
                    }
                    break;
                case '"':
                    ReadString();
                    break;
                default:
                    if (char.IsDigit(Current)) //Number tokens
                    {
                        ReadNumber();
                    }
                    else if (char.IsWhiteSpace(Current)) //Whitespace tokens
                    {
                        ReadWhitespace();
                    }
                    else if (char.IsLetter(Current)) // Word tokens
                    {
                        ReadIdentifierOrKeyword();
                    }
                    else
                    {
                        TextSpan span = new TextSpan(start, 1);
                        TextLocation location = new TextLocation(source, span);
                        diagnostics.ReportBadCharacter(location, Current);
                        NextCharacter();
                    }
                    break;
            }

            int length = position - start;
            if (tokenText == null && tokenType != TokenType.EndOfFile)
                tokenText = SyntaxFacts.GetText(tokenType);
            if (tokenText == null && tokenType != TokenType.EndOfFile)
                tokenText = source.ToString(start, length);

            return new Token(syntaxTree, tokenType, new TextSpan(start, length), tokenText, tokenValue);
        }

        private void ReadString()
        {
            //Skip the first quote
            NextCharacter();

            StringBuilder builder = new StringBuilder();

            bool done = false;
            while (!done)
            {
                switch (Current)
                {
                    case '\0':
                    case '\r':
                    case '\n':
                        TextSpan span = new TextSpan(start, 1);
                        TextLocation location = new TextLocation(source, span);
                        diagnostics.ReportUnterminatedString(location);
                        done = true;
                        break;
                    case '"':
                        if (Peek(1) == '"')
                        {
                            builder.Append(Current);
                            NextCharacter();
                            NextCharacter();
                        }
                        else
                        {
                            NextCharacter();
                            done = true;
                        }
                        break;
                    default:
                        builder.Append(Current);
                        NextCharacter();
                        break;
                }
            }

            tokenType = TokenType.String;
            tokenValue = builder.ToString();
        }

        private void ReadNumber()
        {
            while (char.IsDigit(Current))
                NextCharacter();

            int length = position - start;
            tokenText = source.ToString(start, length);

            if (!int.TryParse(tokenText, out int number))
            {
                TextSpan span = new TextSpan(start, length);
                TextLocation location = new TextLocation(source, span);
                diagnostics.ReportInvalidNumber(location, tokenText, TypeSymbol.Int);
            }

            tokenType = TokenType.Number;
            tokenValue = number;
        }

        private void ReadWhitespace()
        {
            while (char.IsWhiteSpace(Current))
                NextCharacter();

            tokenType = TokenType.WhiteSpace;
        }

        private void ReadIdentifierOrKeyword()
        {
            while (char.IsLetterOrDigit(Current) || Current == '_')
                NextCharacter();

            int length = position - start;
            tokenText = source.ToString(start, length);
            tokenType = SyntaxFacts.GetKeywordType(tokenText);
        }

        private char Current => Peek(0);

        private char Peek(int offset)
        {
            int index = position + offset;

            if (index >= source.Length)
                return '\0';
            return source[index];
        }
        private void NextCharacter()
        {
            position++;
        }
    }
}
