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
        OpenBrace,
        CloseBrace,
        Bang,
        BangEqual,
        Equal,
        EqualEqual,
        AmpersandAmpersand,
        PipePipe,
        Semicolon,

        //Keywords
        FalseKeyword,
        TrueKeyword,
        VarKeyword,
        LetKeyword,
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
