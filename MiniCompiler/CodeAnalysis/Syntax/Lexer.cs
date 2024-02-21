using MiniCompiler.CodeAnalysis.Text;

namespace MiniCompiler.CodeAnalysis.Syntax
{
    internal sealed class Lexer
    {
        private readonly SourceText text;
        private readonly DiagnosticBag diagnostics = new DiagnosticBag();
        public DiagnosticBag Diagnostics => diagnostics;

        private int position = 0;
        private int start;
        private TokenType tokenType;
        private object? tokenValue;
        private string? tokenText;

        public Lexer(SourceText text)
        {
            this.text = text;
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
                    tokenType = TokenType.EndOfFile;
                    position++;
                    break;
                case '+':
                    tokenType = TokenType.Plus;
                    position++;
                    break;
                case '-':
                    tokenType = TokenType.Minus;
                    position++;
                    break;
                case '*':
                    tokenType = TokenType.Star;
                    position++;
                    break;
                case '/':
                    tokenType = TokenType.ForwardSlash;
                    position++;
                    break;
                case '(':
                    tokenType = TokenType.OpenParenthesis;
                    position++;
                    break;
                case ')':
                    tokenType = TokenType.CloseParenthesis;
                    position++;
                    break;
                case '{':
                    tokenType = TokenType.OpenBrace;
                    position++;
                    break;
                case '}':
                    tokenType = TokenType.CloseBrace;
                    position++;
                    break;
                case ';':
                    tokenType = TokenType.Semicolon;
                    position++;
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
                    if (Current == '&')
                    {
                        tokenType = TokenType.AmpersandAmpersand;
                        position++;
                    }
                    break;
                case '|':
                    position++;
                    if (Current == '|')
                    {
                        tokenType = TokenType.PipePipe;
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
                        diagnostics.ReportBadCharacter(new TextSpan(start, 1), Current);
                        position++;
                    }
                    break;
            }

            int length = position - start;
            if (tokenType == TokenType.EndOfFile)
                length = 0;
            if (tokenText == null && tokenType != TokenType.EndOfFile)
                tokenText = SyntaxFacts.GetText(tokenType);
            if (tokenText == null && tokenType != TokenType.EndOfFile)
                tokenText = text.ToString(start, length);

            return new Token(tokenType, new TextSpan(start, length), tokenText, tokenValue);
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
            tokenText = text.ToString(start, length);
            tokenType = SyntaxFacts.GetKeywordType(tokenText);
        }

        private void ReadNumber()
        {
            while (char.IsDigit(Current))
                Next();

            int length = position - start;
            tokenText = text.ToString(start, length);

            if (!int.TryParse(tokenText, out int number))
                diagnostics.ReportInvalidNumber(new TextSpan(start, length), tokenText, typeof(int));

            tokenType = TokenType.Number;
            tokenValue = number;
        }

        private char Current => Peek(0);

        private char Peek(int offset)
        {
            int index = position + offset;

            if (index >= text.Length)
                return '\0';
            return text[index];
        }
        private void Next()
        {
            position++;
        }
    }
}
