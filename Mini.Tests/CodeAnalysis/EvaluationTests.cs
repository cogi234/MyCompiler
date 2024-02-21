using MiniCompiler.CodeAnalysis;
using MiniCompiler.CodeAnalysis.Binding;
using MiniCompiler.CodeAnalysis.Syntax;

namespace Mini.Tests.CodeAnalysis
{
    public class EvaluationTests
    {
        [Theory]
        [InlineData("1", 1), InlineData("-1", -1), InlineData("+1", 1), InlineData("(10)", 10)]
        [InlineData("1 + 2", 3), InlineData("1 - 2", -1), InlineData("2 * 3", 6), InlineData("6 / 2", 3)]
        [InlineData("false", false), InlineData("true", true), InlineData("!true", false), InlineData("!false", true)]
        [InlineData("true || false", true), InlineData("false || false", false), InlineData("true && false", false), InlineData("true && true", true)]
        [InlineData("1 == 1", true), InlineData("2 == 10", false), InlineData("1 != 1", false), InlineData("1 != 11", true)]
        [InlineData("false == false", true), InlineData("true == false", false), InlineData("true != true", false), InlineData("false != true", true)]
        [InlineData("(a = 10) * a", 100)]
        public void EvaluatesCorrectly(string text, object expectedValue)
        {
            SyntaxTree syntaxTree = SyntaxTree.Parse(text + ";");
            Compilation compilation = new Compilation(syntaxTree);
            Dictionary<VariableSymbol, object> variables = new Dictionary<VariableSymbol, object>();
            EvaluationResult result = compilation.Evaluate(variables);

            Assert.Empty(result.Diagnostics);
            Assert.Equal(result.Value, expectedValue);
        }
    }
}
