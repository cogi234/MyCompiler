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
                    position++;
                    tokenType = TokenType.Plus;
                    break;
                case '-':
                    position++;
                    tokenType = TokenType.Minus;
                    break;
                case '*':
                    position++;
                    tokenType = TokenType.Star;
                    break;
                case '/':
                    position++;
                    tokenType = TokenType.ForwardSlash;
                    break;
                case '(':
                    position++;
                    tokenType = TokenType.OpenParenthesis;
                    break;
                case ')':
                    position++;
                    tokenType = TokenType.CloseParenthesis;
                    break;
                case '{':
                    position++;
                    tokenType = TokenType.OpenBrace;
                    break;
                case '}':
                    position++;
                    tokenType = TokenType.CloseBrace;
                    break;
                case '.':
                    position++;
                    tokenType = TokenType.Period;
                    break;
                case ',':
                    position++;
                    tokenType = TokenType.Comma;
                    break;
                case ':':
                    position++;
                    tokenType = TokenType.Colon;
                    break;
                case ';':
                    position++;
                    tokenType = TokenType.Semicolon;
                    break;
                case '!':
                    position++;
                    if (Current != '=')
                        tokenType = TokenType.Bang;
                    else
                    {
                        tokenType = TokenType.BangEqual;
                        position++;
                    }
                    break;
                case '&':
                    position++;
                    if (Current != '&')
                        tokenType = TokenType.Ampersand;
                    else
                    {
                        tokenType = TokenType.AmpersandAmpersand;
                        position++;
                    }
                    break;
                case '|':
                    position++;
                    if (Current != '|')
                        tokenType = TokenType.Pipe;
                    else
                    {
                        tokenType = TokenType.PipePipe;
                        position++;
                    }
                    break;
                case '^':
                    position++;
                    tokenType = TokenType.Caret;
                    break;
                case '~':
                    position++;
                    tokenType = TokenType.Tilde;
                    break;
                case '<':
                    position++;
                    if (Current != '=')
                        tokenType = TokenType.LessThan;
                    else
                    {
                        tokenType = TokenType.LessThanEqual;
                        position++;
                    }
                    break;
                case '>':
                    position++;
                    if (Current != '=')
                        tokenType = TokenType.GreaterThan;
                    else
                    {
                        tokenType = TokenType.GreaterThanEqual;
                        position++;
                    }
                    break;
                case '=':
                    position++;
                    if (Current != '=')
                        tokenType = TokenType.Equal;
                    else
                    {
                        tokenType = TokenType.EqualEqual;
                        position++;
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
                        position++;
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
            Next();

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
                            Next();
                            Next();
                        }
                        else
                        {
                            Next();
                            done = true;
                        }
                        break;
                    default:
                        builder.Append(Current);
                        Next();
                        break;
                }
            }

            tokenType = TokenType.String;
            tokenValue = builder.ToString();
        }

        private void ReadNumber()
        {
            while (char.IsDigit(Current))
                Next();

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
                Next();

            tokenType = TokenType.WhiteSpace;
        }

        private void ReadIdentifierOrKeyword()
        {
            while (char.IsLetterOrDigit(Current))
                Next();

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
        private void Next()
        {
            position++;
        }
    }
}
