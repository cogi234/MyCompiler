using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCompiler.CodeAnalysis.Syntax
{
    internal sealed class Lexer
    {
        private readonly string text;
        private int position = 0;
        private DiagnosticBag diagnostics = new DiagnosticBag();
        public DiagnosticBag Diagnostics => diagnostics;

        public Lexer(string text)
        {
            this.text = text;
        }

        public Token NextToken()
        {
            if (position >= text.Length)
                return new Token(TokenType.EndOfFile, new TextSpan(position, 1), "\0", null);

            int start = position;

            if (char.IsDigit(Current))
            {

                while (char.IsDigit(Current))
                    Next();

                int length = position - start;
                string text = this.text.Substring(start, length);
                if (!int.TryParse(text, out int value))
                {
                    diagnostics.ReportInvalidNumber(new TextSpan(start, length), text, typeof(int));
                }
                return new Token(TokenType.Number, new TextSpan(start, length), text, value);
            }

            if (char.IsWhiteSpace(Current))
            {
                while (char.IsWhiteSpace(Current))
                    Next();

                int length = position - start;
                string text = this.text.Substring(start, length);
                return new Token(TokenType.WhiteSpace, new TextSpan(start, length), text, null);
            }

            if (char.IsLetter(Current))
            {
                while (char.IsLetterOrDigit(Current))
                    Next();

                int length = position - start;
                string text = this.text.Substring(start, length);
                TokenType type = SyntaxFacts.GetKeywordType(text);
                return new Token(type, new TextSpan(start, length), text, null);
            }

            //Single character tokens
            switch (Current)
            {
                case '+':
                    position++;
                    return new Token(TokenType.Plus, new TextSpan(start, 1), "+", null);
                case '-':
                    position++;
                    return new Token(TokenType.Minus, new TextSpan(start, 1), "-", null);
                case '*':
                    position++;
                    return new Token(TokenType.Star, new TextSpan(start, 1), "*", null);
                case '/':
                    position++;
                    return new Token(TokenType.ForwardSlash, new TextSpan(start, 1), "/", null);
                case '(':
                    position++;
                    return new Token(TokenType.OpenParenthesis, new TextSpan(start, 1), "(", null);
                case ')':
                    position++;
                    return new Token(TokenType.CloseParenthesis, new TextSpan(start, 1), ")", null);
                case '!':
                    if (Peek(1) == '=')
                    {
                        position += 2;
                        return new Token(TokenType.BangEqual, new TextSpan(start, 2), "!=", null);
                    }
                    position++;
                    return new Token(TokenType.Bang, new TextSpan(start, 1), "!", null);
                case '&':
                    if (Peek(1) == '&')
                    {
                        position += 2;
                        return new Token(TokenType.AmpersandAmpersand, new TextSpan(start, 2), "&&", null);
                    }
                    break;
                case '|':
                    if (Peek(1) == '|')
                    {
                        position += 2;
                        return new Token(TokenType.PipePipe, new TextSpan(start, 2), "||", null);
                    }
                    break;
                case '=':
                    if (Peek(1) == '=')
                    {
                        position += 2;
                        return new Token(TokenType.EqualEqual, new TextSpan(start, 2), "==", null);
                    }
                    position++;
                    return new Token(TokenType.Equal, new TextSpan(start, 1), "=", null);
            }

            position++;
            diagnostics.ReportBadCharacter(new TextSpan(start, 1), Current);
            return new Token(TokenType.BadToken, new TextSpan(start, 1), text.Substring(position - 1, 1), null);
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
