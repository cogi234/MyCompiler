using MiniCompiler.CodeAnalysis.Syntax;

namespace Mini.Tests.CodeAnalysis.Syntax
{
    public class SyntactFactTests
    {
        [Theory]
        [MemberData(nameof(GetTokenTypeData))]
        public void GetTextRoundTrips(TokenType tokenType)
        {
            string text = SyntaxFacts.GetText(tokenType);
            if (text == null)
                return;

            IEnumerable<Token> tokens = SyntaxTree.ParseTokens(text);
            Token token = Assert.Single(tokens);

            Assert.Equal(tokenType, token.Type);
            Assert.Equal(text, token.Text);
        }

        public static IEnumerable<object[]> GetTokenTypeData()
        {
            TokenType[] types = (TokenType[])Enum.GetValues(typeof(TokenType));
            foreach (TokenType type in types)
            {
                yield return new object[] { type };
            }
        }
    }
}
