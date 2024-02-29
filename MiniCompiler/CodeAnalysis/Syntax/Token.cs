using MiniCompiler.CodeAnalysis.Syntax.SyntaxNodes;
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
        String,
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
        Ampersand,
        AmpersandAmpersand,
        Pipe,
        PipePipe,
        Caret,
        Tilde,
        Semicolon,
        LessThan,
        LessThanEqual,
        GreaterThan,
        GreaterThanEqual,

        //Keywords
        FalseKeyword,
        TrueKeyword,
        VarKeyword,
        LetKeyword,
        IfKeyword,
        ElseKeyword,
        WhileKeyword,
        ForKeyword,
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
        /// <summary>
        /// A token is missing if it was inserted by the parser and isn't in the source
        /// </summary>
        public bool IsMissing => Text == null;
    }
}
