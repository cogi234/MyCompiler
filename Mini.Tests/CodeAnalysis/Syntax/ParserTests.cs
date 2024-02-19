using MiniCompiler.CodeAnalysis.Syntax;
using MiniCompiler.CodeAnalysis.Syntax.SyntaxNodes;

namespace Mini.Tests.CodeAnalysis.Syntax
{

    public class ParserTests
    {
        [Theory]
        [MemberData(nameof(GetBinaryOperatorPairsData))]
        public void BinaryExpressionHonorsPrecedence(TokenType op1, TokenType op2)
        {
            int precedence1 = SyntaxFacts.GetBinaryOperatorPrecedence(op1);
            int precedence2 = SyntaxFacts.GetBinaryOperatorPrecedence(op2);
            string text1 = SyntaxFacts.GetText(op1);
            string text2 = SyntaxFacts.GetText(op2);
            string text = $"a {text1} b {text2} c";

            SyntaxNode expression = SyntaxTree.Parse(text).Root;

            if (precedence1 >= precedence2)
            {
                using (AssertingEnumerator e = new AssertingEnumerator(expression))
                {
                    e.AssertCompilationUnit();
                    e.AssertBinaryExpressionNode(op2, text2);
                    e.AssertBinaryExpressionNode(op1, text1);
                    e.AssertNode(NodeType.NameExpression);
                    e.AssertNode(NodeType.NameExpression);
                    e.AssertNode(NodeType.NameExpression);
                }
            }
            else
            {
                using (AssertingEnumerator e = new AssertingEnumerator(expression))
                {
                    e.AssertCompilationUnit();
                    e.AssertBinaryExpressionNode(op1, text1);
                    e.AssertNode(NodeType.NameExpression);
                    e.AssertBinaryExpressionNode(op2, text2);
                    e.AssertNode(NodeType.NameExpression);
                    e.AssertNode(NodeType.NameExpression);
                }
            }
        }

        [Theory]
        [MemberData(nameof(GetUnaryOperatorPairsData))]
        public void UnaryExpressionHonorsPrecedence(TokenType opUn, TokenType opBin)
        {
            int precedence1 = SyntaxFacts.GetUnaryOperatorPrecedence(opUn);
            int precedence2 = SyntaxFacts.GetBinaryOperatorPrecedence(opBin);
            string text1 = SyntaxFacts.GetText(opUn);
            string text2 = SyntaxFacts.GetText(opBin);
            string text = $"{text1} b {text2} c";

            SyntaxNode expression = SyntaxTree.Parse(text).Root;

            if (precedence1 >= precedence2)
            {
                using (AssertingEnumerator e = new AssertingEnumerator(expression))
                {
                    e.AssertCompilationUnit();
                    e.AssertBinaryExpressionNode(opBin, text2);
                    e.AssertUnaryExpressionNode(opUn, text1);
                    e.AssertNode(NodeType.NameExpression);
                    e.AssertNode(NodeType.NameExpression);
                }
            }
            else
            {
                using (AssertingEnumerator e = new AssertingEnumerator(expression))
                {
                    e.AssertCompilationUnit();
                    e.AssertUnaryExpressionNode(opUn, text1);
                    e.AssertBinaryExpressionNode(opBin, text2);
                    e.AssertNode(NodeType.NameExpression);
                    e.AssertNode(NodeType.NameExpression);
                }
            }
        }

        public static IEnumerable<object[]> GetBinaryOperatorPairsData()
        {
            foreach (TokenType op1 in SyntaxFacts.GetBinaryOperatorTypes())
            {
                foreach (TokenType op2 in SyntaxFacts.GetBinaryOperatorTypes())
                {
                    yield return new object[] { op1, op2 };
                }
            }
        }

        public static IEnumerable<object[]> GetUnaryOperatorPairsData()
        {
            foreach (TokenType op1 in SyntaxFacts.GetUnaryOperatorTypes())
            {
                foreach (TokenType op2 in SyntaxFacts.GetBinaryOperatorTypes())
                {
                    yield return new object[] { op1, op2 };
                }
            }
        }
    }
}
