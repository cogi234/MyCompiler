using MiniCompiler.CodeAnalysis.Text;

namespace MiniCompiler.CodeAnalysis.Syntax.SyntaxNodes
{
    public enum NodeType
    {
        CompilationUnit,

        //Statements
        BlockStatement,
        ExpressionStatement,
        VariableDeclarationStatement,
        IfStatement,
        ElseClause,
        WhileStatement,

        //Expressions
        LiteralExpression,
        NameExpression,
        UnaryExpression,
        BinaryExpression,
        ParenthesizedExpression,
        AssignmentExpression,
    }

    public abstract class SyntaxNode
    {
        public abstract NodeType Type { get; }
        public abstract TextSpan Span { get; }
        public abstract IEnumerable<SyntaxNode> GetChildren();

        public void PrettyPrint(TextWriter writer, string indent = "", bool isLast = true)
        {
            string marker = isLast ? "└──" : "├──";

            writer.Write(indent);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            writer.Write(marker);
            Console.ForegroundColor = ConsoleColor.Cyan;
            writer.Write(Type);

            Console.ForegroundColor = ConsoleColor.Blue;
            switch (Type)
            {
                case NodeType.LiteralExpression:
                    writer.Write($" {((LiteralExpressionNode)this).Value}");
                    break;
                case NodeType.BinaryExpression:
                    writer.Write($" {((BinaryExpressionNode)this).OperatorToken.Text}");
                    break;
                case NodeType.UnaryExpression:
                    writer.Write($" {((UnaryExpressionNode)this).OperatorToken.Text}");
                    break;
                case NodeType.AssignmentExpression:
                    writer.Write($" {((AssignmentExpressionNode)this).IdentifierToken.Text}");
                    break;
            }
            Console.ResetColor();

            writer.WriteLine();

            indent += isLast ? "   " : "│  ";

            SyntaxNode? lastChild = GetChildren().LastOrDefault();
            foreach (SyntaxNode child in GetChildren())
            {
                child.PrettyPrint(writer, indent, child == lastChild);
            }
        }

        public override string ToString()
        {
            using (StringWriter writer = new StringWriter())
            {
                PrettyPrint(writer);
                return writer.ToString();
            }
        }
    }
}
