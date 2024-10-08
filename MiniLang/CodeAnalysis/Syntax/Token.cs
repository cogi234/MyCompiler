﻿using MiniLang.CodeAnalysis.Text;

namespace MiniLang.CodeAnalysis.Syntax
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
        Type,
        Plus,
        Minus,
        Star,
        ForwardSlash,
        Percent,
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
        Period,
        Comma,
        Colon,
        Semicolon,
        LessThan,
        LessThanEqual,
        GreaterThan,
        GreaterThanEqual,

        //Keywords
        FalseKeyword,
        TrueKeyword,
        VarKeyword,
        IfKeyword,
        ElseKeyword,
        DoKeyword,
        WhileKeyword,
        ForKeyword,
        ContinueKeyword,
        BreakKeyword,
        ReturnKeyword,
    }
    public sealed class Token
    {
        public Token(SyntaxTree syntaxTree, TokenType type, TextSpan span, string? text, object? value, bool isFake = false)
        {
            SyntaxTree = syntaxTree;
            Type = type;
            Span = span;
            Text = text;
            Value = value;
            IsFake = isFake;
        }

        public SyntaxTree SyntaxTree { get; }
        public TokenType Type { get; }
        public TextSpan Span { get; }
        public string? Text { get; }
        public object? Value { get; }
        public bool IsFake { get; }
        public TextLocation Location => new TextLocation(SyntaxTree.SourceText, Span);
    }
}
