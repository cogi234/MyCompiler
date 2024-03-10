using MiniLang.CodeAnalysis.Text;

namespace Mini.Tests.CodeAnalysis.Text
{
    public class SourceTextTests
    {
        [Theory]
        [InlineData("-----\n----\n---", 7, 1)]
        public void FindsLineCorrectly(string text, int position, int correctLine)
        {
            SourceText source = new SourceText(text);

            int line = source.GetLineIndex(position);
            Assert.Equal(correctLine, line);
        }

        [Theory]
        [InlineData(".", 1)]
        [InlineData(".\r\n", 2)]
        [InlineData(".\r\n\r\n", 3)]
        public void IncludesLastLine(string text, int expectedLineCount)
        {
            SourceText sourceText = new SourceText(text);
            Assert.Equal(sourceText.Lines.Length, expectedLineCount);
        }
    }
}
