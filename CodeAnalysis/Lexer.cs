using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompiler
{
    internal sealed class Lexer
    {
        private readonly string text;
        private int position = 0;
        private List<string> diagnostics = new List<string>();

        public IEnumerable<string> Diagnostics => diagnostics;

        public Lexer(string text) {
            this.text = text;
        }

        public Token NextToken()
        {
            // <number>
            // + - * / ( )
            // <whitespace>

            if (position >= text.Length)
                return new Token(TokenType.EndOfFile, position, "\0", null);

            if (char.IsDigit(Current))
            {
                int start = position;

                while (char.IsDigit(Current))
                    Next();

                int length = position - start;
                string text = this.text.Substring(start, length);
                if (!int.TryParse(text, out int value))
                {
                    diagnostics.Add($"ERROR ({position}): The number {text} cannot be represented by an Int32");
                }
                return new Token(TokenType.Number, start, text, value);
            }

            if (char.IsWhiteSpace(Current))
            {
                int start = position;

                while (char.IsWhiteSpace(Current))
                    Next();

                int length = position - start;
                string text = this.text.Substring(start, length);
                return new Token(TokenType.WhiteSpace, start, text, null);
            }

            //Single character tokens
            switch (Current)
            {
                case '+':
                    return new Token(TokenType.Plus, position++, "+", null);
                case '-':
                    return new Token(TokenType.Minus, position++, "-", null);
                case '*':
                    return new Token(TokenType.Star, position++, "*", null);
                case '/':
                    return new Token(TokenType.ForwardSlash, position++, "/", null);
                case '(':
                    return new Token(TokenType.OpenParenthesis, position++, "(", null);
                case ')':
                    return new Token(TokenType.CloseParenthesis, position++, ")", null);
                case '!':
                    return new Token(TokenType.ExclamationMark, position++, "!", null);
            }

            diagnostics.Add($"ERROR ({position}): bad character input: '{Current}'");
            return new Token(TokenType.BadToken, position++, text.Substring(position - 1, 1), null);
        }

        private char Current
        {
            get
            {
                if (position >= text.Length)
                    return '\0';
                return text[position];
            }
        }
        private void Next()
        {
            position++;
        }


    }
}
