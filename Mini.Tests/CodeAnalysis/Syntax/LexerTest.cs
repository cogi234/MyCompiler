using MiniCompiler.CodeAnalysis.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mini.Tests.CodeAnalysis.Syntax
{
    public class LexerTest
    {
        [Theory]
        [MemberData(nameof(GetTokensData))]
        public void LexerParsesToken(TokenType tokenType, string text)
        {
            IEnumerable<Token> tokens = SyntaxTree.ParseTokens(text);

            Token token = Assert.Single(tokens);
            Assert.Equal(tokenType, token.Type);
            Assert.Equal(text, token.Text);
        }

        [Theory]
        [MemberData(nameof(GetTokenPairsData))]
        public void LexerParsesTokenPairs(TokenType type1, string text1, TokenType type2, string text2)
        {
            string text = text1 + text2;
            Token[] tokens = SyntaxTree.ParseTokens(text).ToArray();

            Assert.Equal(2, tokens.Length);

            Assert.Equal(type1, tokens[0].Type);
            Assert.Equal(text1, tokens[0].Text);

            Assert.Equal(type2, tokens[1].Type);
            Assert.Equal(text2, tokens[1].Text);
        }

        [Theory]
        [MemberData(nameof(GetSeparatedTokenPairsData))]
        public void LexerParsesSeparatedTokenPairs(TokenType type1, string text1, TokenType separatorType, string separatorText, TokenType type2, string text2)
        {
            string text = text1 + separatorText + text2;
            Token[] tokens = SyntaxTree.ParseTokens(text).ToArray();

            Assert.Equal(3, tokens.Length);

            Assert.Equal(type1, tokens[0].Type);
            Assert.Equal(text1, tokens[0].Text);

            Assert.Equal(separatorType, tokens[1].Type);
            Assert.Equal(separatorText, tokens[1].Text);

            Assert.Equal(type2, tokens[2].Type);
            Assert.Equal(text2, tokens[2].Text);
        }


        public static IEnumerable<object[]> GetTokensData()
        {
            foreach (var t in GetTokens().Concat(GetSeparators()))
                yield return new object[] { t.type, t.text };
        }

        public static IEnumerable<object[]> GetTokenPairsData()
        {
            foreach (var t in GetTokenPairs())
                yield return new object[] { t.type1, t.text1, t.type2, t.text2 };
        }

        public static IEnumerable<object[]> GetSeparatedTokenPairsData()
        {
            foreach (var t in GetSeparatedTokenPairs())
                yield return new object[] { t.type1, t.text1, t.separatorType, t.separatorText, t.type2, t.text2 };
        }

        private static IEnumerable<(TokenType type, string text)> GetTokens()
        {
            return new[]
            {
                (TokenType.Plus,"+"),
                (TokenType.Minus,"-"),
                (TokenType.Star,"*"),
                (TokenType.ForwardSlash,"/"),
                (TokenType.OpenParenthesis,"("),
                (TokenType.CloseParenthesis,")"),
                (TokenType.Bang,"!"),
                (TokenType.BangEqual,"!="),
                (TokenType.Equal,"="),
                (TokenType.EqualEqual,"=="),
                (TokenType.AmpersandAmpersand,"&&"),
                (TokenType.PipePipe,"||"),
                (TokenType.FalseKeyword,"false"),
                (TokenType.TrueKeyword,"true"),

                (TokenType.Number,"1"),
                (TokenType.Number,"1234567890"),
                (TokenType.Identifier,"a"),
                (TokenType.Identifier,"abcdefghijklmnopqrstuvwxyz"),
            };
        }

        private static IEnumerable<(TokenType type, string text)> GetSeparators()
        {
            return new[]
            {
                (TokenType.WhiteSpace," "),
                (TokenType.WhiteSpace,"  "),
                (TokenType.WhiteSpace,"\r"),
                (TokenType.WhiteSpace,"\n"),
                (TokenType.WhiteSpace,"\n\r"),
            };
        }

        private static bool RequiresSeparator(TokenType t1, TokenType t2)
        {
            bool t1IsWord = t1.ToString().EndsWith("Keyword") || t1 == TokenType.Identifier;
            bool t2IsWord = t2.ToString().EndsWith("Keyword") || t2 == TokenType.Identifier;

            if (t1IsWord && t2IsWord)
                return true;
            if (t1IsWord && t2 == TokenType.Number)
                return true;
            if (t1 == TokenType.Number && t2 == TokenType.Number)
                return true;
            if (t1 == TokenType.Bang && t2 == TokenType.Equal)
                return true;
            if (t1 == TokenType.Equal && t2 == TokenType.Equal)
                return true;
            if (t1 == TokenType.Bang && t2 == TokenType.EqualEqual)
                return true;
            if (t1 == TokenType.Equal && t2 == TokenType.EqualEqual)
                return true;

            return false;
        }

        private static IEnumerable<(TokenType type1, string text1, TokenType type2, string text2)> GetTokenPairs()
        {
            foreach (var t1 in GetTokens())
            {
                foreach (var t2 in GetTokens())
                {
                    if (!RequiresSeparator(t1.type, t2.type))
                        yield return (t1.type, t1.text, t2.type, t2.text);
                }
            }
        }
        private static IEnumerable<(TokenType type1, string text1, TokenType separatorType, string separatorText, TokenType type2, string text2)> GetSeparatedTokenPairs()
        {
            foreach (var t1 in GetTokens())
            {
                foreach (var t2 in GetTokens())
                {
                    if (RequiresSeparator(t1.type, t2.type))
                    {
                        foreach (var s in GetSeparators())
                        {
                            yield return (t1.type, t1.text, s.type, s.text, t2.type, t2.text);
                        }
                    }
                }
            }
        }
    }
}