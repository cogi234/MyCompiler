using MiniCompiler.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mini.Tests.CodeAnalysis.Text
{
    public class SourceTextTests
    {
        [Theory]
        [InlineData("-----\n----\n---", 7, 1, 1)]
        public void FindsLineCorrectly(string text, int position, int correctLine, int correctColumn)
        {
            SourceText source = new SourceText(text);

            (int line, int column) = source.GetLineIndex(position);
            Assert.Equal(correctLine, line);
            Assert.Equal(correctColumn, column);
        }
    }
}
