using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniCompiler.CodeAnalysis.Text;

namespace MiniCompiler.CodeAnalysis.Syntax
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
        Bang,
        BangEqual,
        Equal,
        EqualEqual,
        AmpersandAmpersand,
        PipePipe,

        //Keywords
        FalseKeyword,
        TrueKeyword,
    }
    public sealed class Token
    {
        public Token(TokenType type, TextSpan span, string? text, object? value)
        {
            Type = type;
            Span = span;
            Text = text;
            Value = value;
        }

        public TokenType Type { get; }
        public TextSpan Span { get; }
        public string? Text { get; }
        public object? Value { get; }
    }
}
