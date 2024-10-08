﻿using MiniLang.CodeAnalysis;
using MiniLang.CodeAnalysis.Symbols;
using MiniLang.CodeAnalysis.Syntax;

namespace Mini.Tests.CodeAnalysis
{

    public class EvaluationTests
    {
        [Theory]
        #region InlineData
        //Ints
        [InlineData("1;", 1)]
        [InlineData("+1;", 1)]
        [InlineData("-1;", -1)]
        [InlineData("~1;", -2)]
        [InlineData("(10);", 10)]
        [InlineData("1 + 2;", 3)]
        [InlineData("1 - 2;", -1)]
        [InlineData("2 * 3;", 6)]
        [InlineData("6 / 2;", 3)]
        [InlineData("1 | 2;", 3)]
        [InlineData("1 | 0;", 1)]
        [InlineData("3 & 2;", 2)]
        [InlineData("3 & 0;", 0)]
        [InlineData("1 ^ 0;", 1)]
        [InlineData("1 ^ 3;", 2)]
        //Bools
        [InlineData("false;", false)]
        [InlineData("true;", true)]
        [InlineData("!true;", false)]
        [InlineData("!false;", true)]
        [InlineData("true || false;", true)]
        [InlineData("false || false;", false)]
        [InlineData("true && false;", false)]
        [InlineData("true && true;", true)]
        [InlineData("true | false;", true)]
        [InlineData("false | false;", false)]
        [InlineData("true & false;", false)]
        [InlineData("true & true;", true)]
        [InlineData("true ^ false;", true)]
        [InlineData("true ^ true;", false)]
        [InlineData("false ^ false;", false)]
        //Strings
        [InlineData("\"Hello\" + \" World!\";", "Hello World!")]
        //Comparisons
        [InlineData("1 == 1;", true)]
        [InlineData("2 == 10;", false)]
        [InlineData("1 != 1;", false)]
        [InlineData("1 != 11;", true)]
        [InlineData("10 > 2;", true)]
        [InlineData("2 > 10;", false)]
        [InlineData("1 >= 1;", true)]
        [InlineData("2 >= 10;", false)]
        [InlineData("1 < 10;", true)]
        [InlineData("20 < 10;", false)]
        [InlineData("1 <= 1;", true)]
        [InlineData("20 <= 10;", false)]

        [InlineData("false == false;", true)]
        [InlineData("true == false;", false)]
        [InlineData("true != true;", false)]
        [InlineData("false != true;", true)]

        [InlineData("var a = 10; a * a;", 100)]
        [InlineData("var a = 1;if (a == 1) a = 10; a;", 10)]
        [InlineData("var a = 1;if (a == 2) a = 10; a;", 1)]
        [InlineData("if (true) 1; else 2;", 1)]
        [InlineData("if (false) 1; else 2;", 2)]
        [InlineData("var i = 0; while(i < 10) i = i + 1;", 10)]
        [InlineData("var i = 0; do i = i + 1; while(i < 10);", 10)]
        [InlineData("for(var i = 0; i < 10; i = i + 1) i;", 10)]
        //Fibonnaci
        [InlineData("var f1 = 0; var f2 = 1; for (var i = 0; i < 10; i = i + 1){var result = f1 + f2; f1 = f2; f2 = result;} f2;", 89)]
        #endregion InlineData
        public void EvaluatesCorrectly(string text, object expectedValue)
        {
            SyntaxTree syntaxTree = SyntaxTree.Parse(text);
            Compilation compilation = new Compilation(syntaxTree);
            Dictionary<VariableSymbol, object?> variables = new Dictionary<VariableSymbol, object?>();
            EvaluationResult result = compilation.Evaluate(variables);

            Assert.Empty(result.Diagnostics);
            Assert.Equal(expectedValue, result.Value);
        }

        [Fact]
        public void VariableDeclarationReportsRedeclaration()
        {
            AssertDiagnostics(@"
                {
                    var x = 10;
                    var y = 100;
                    {
                        var x = 15;
                    }
                    var [x] = 5;
                }
            ", @"
                'x' already exists.
            ");
        }
        [Fact]
        public void AssignmentReportsUndefinedName()
        {
            AssertDiagnostics(@"
                [z] = true;
            ", @"
                Variable 'z' doesn't exist.
            ");
        }
        [Fact]
        public void AssignmentReportsCannotConvert()
        {
            AssertDiagnostics(@"
                {
                    var x = false;
                    x = [10];
                }
            ", @"
                Cannot convert type int to bool.
            ");
        }
        [Fact]
        public void IfStatementReportsCannotConvert()
        {
            AssertDiagnostics(@"
                {
                    if ([1])
                        10;
                }
            ", @"
                Cannot convert type int to bool.
            ");
        }
        [Fact]
        public void WhileStatementReportsCannotConvert()
        {
            AssertDiagnostics(@"
                {
                    while ([10])
                        1;
                }
            ", @"
                Cannot convert type int to bool.
            ");
        }
        [Fact]
        public void ForStatementReportsCannotConvert()
        {
            AssertDiagnostics(@"
                {
                    for (;[1];)
                        10;
                }
            ", @"
                Cannot convert type int to bool.
            ");
        }
        [Fact]
        public void ExpressionReportsUndefinedUnary()
        {
            AssertDiagnostics(@"
                [-]true;
            ", @"
                Unary operator '-' is not defined for type bool.
            ");
        }
        [Fact]
        public void ExpressionReportsUndefinedBinary()
        {
            AssertDiagnostics(@"
                1 [&&] true;
            ", @"
                Binary operator '&&' is not defined for types int and bool.
            ");
        }
        [Fact]
        public void UnterminatedStringGetsReported()
        {
            AssertDiagnostics(@"
                [""]asd""""asd[]
            ", @"
                Unterminated string literal.
                Unexpected token. Expected <Semicolon>, found <EndOfFile>.
            ");
        }

        private void AssertDiagnostics(string text, string diagnosticText)
        {
            AnnotatedText annotatedText = AnnotatedText.Parse(text);
            SyntaxTree syntaxTree = SyntaxTree.Parse(annotatedText.Text);
            Compilation compilation = new Compilation(syntaxTree);
            EvaluationResult result = compilation.Evaluate(new Dictionary<VariableSymbol, object?>());

            string[] expectedDiagnostics = AnnotatedText.UnindentLines(diagnosticText);

            if (annotatedText.Spans.Length != expectedDiagnostics.Length)
                throw new Exception("Must mark as many expected error spans with [] as expected diagnostics");

            Assert.Equal(result.Diagnostics.Length, expectedDiagnostics.Length);

            for (int i = 0; i < expectedDiagnostics.Length; i++)
            {
                string expectedMessage = expectedDiagnostics[i];
                string actualMessage = result.Diagnostics[i].Message;
                Assert.Equal(expectedMessage, actualMessage);

                MiniLang.CodeAnalysis.Text.TextSpan expectedSpan = annotatedText.Spans[i];
                MiniLang.CodeAnalysis.Text.TextSpan actualSpan = result.Diagnostics[i].Location.Span;
                Assert.Equal(expectedSpan, actualSpan);
            }
        }
    }
}
