using MiniCompiler.CodeAnalysis.Syntax;
using MiniCompiler.CodeAnalysis.Syntax.SyntaxNodes;

namespace Mini.Tests.CodeAnalysis.Syntax
{
    internal sealed class AssertingEnumerator : IDisposable
    {
        private readonly IEnumerator<SyntaxNode> enumerator;
        private bool hasErrors;

        public AssertingEnumerator(SyntaxNode node)
        {
            enumerator = Flatten(node).GetEnumerator();
        }

        public void AssertNode(NodeType type)
        {
            try
            {
                Assert.True(enumerator.MoveNext());
                Assert.Equal(type, enumerator.Current.Type);
            }
            catch
            {
                hasErrors = true;
                throw;
            }
        }
        public void AssertBinaryExpressionNode(TokenType operatorType, string operatorText)
        {
            try
            {
                AssertNode(NodeType.BinaryExpression);
                BinaryExpressionNode b = (BinaryExpressionNode)enumerator.Current;
                Assert.Equal(b.OperatorToken.Type, operatorType);
                Assert.Equal(b.OperatorToken.Text, operatorText);
            }
            catch
            {
                hasErrors = true;
                throw;
            }
        }
        public void AssertUnaryExpressionNode(TokenType operatorType, string operatorText)
        {
            try
            {
                AssertNode(NodeType.UnaryExpression);
                UnaryExpressionNode b = (UnaryExpressionNode)enumerator.Current;
                Assert.Equal(b.OperatorToken.Type, operatorType);
                Assert.Equal(b.OperatorToken.Text, operatorText);
            }
            catch
            {
                hasErrors = true;
                throw;
            }
        }

        private static IEnumerable<SyntaxNode> Flatten(SyntaxNode root)
        {
            Stack<SyntaxNode> stack = new Stack<SyntaxNode>();
            stack.Push(root);

            while (stack.Count > 0)
            {
                SyntaxNode node = stack.Pop();

                yield return node;

                foreach (SyntaxNode child in node.GetChildren().Reverse())
                    stack.Push(child);
            }
        }

        public void Dispose()
        {
            if (!hasErrors)
                Assert.False(enumerator.MoveNext());

            enumerator.Dispose();
        }
    }
}
