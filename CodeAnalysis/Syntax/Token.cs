using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompiler.CodeAnalysis.Syntax
{
    public enum TokenType
    {
        //Technical tokens
        BadToken,
        WhiteSpace,
        EndOfFile,

        //Tokens
        Number,
        Identifier,
        Plus,
        Minus,
        Star,
        ForwardSlash,
        OpenParenthesis,
        CloseParenthesis,
        ExclamationMark,

        //Keywords
        FalseKeyword,
        TrueKeyword
    }
    public sealed class Token
    {
        public Token(TokenType type, int position, string? text, object? value)
        {
            Type = type;
            Position = position;
            Text = text;
            Value = value;
        }

        public TokenType Type { get; }
        public int Position { get; }
        public string? Text { get; }
        public object? Value { get; }
    }
}
