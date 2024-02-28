using MiniCompiler.CodeAnalysis.Syntax.SyntaxNodes;

namespace MiniCompiler.CodeAnalysis.Binding.BoundNodes
{
    enum BoundNodeType
    {
        //Statements
        BlockStatement,
        ExpressionStatement,
        VariableDeclarationStatement,
        IfStatement,
        WhileStatement,
        ForStatement,

        //Expressions
        LiteralExpression,
        VariableExpression,
        AssignmentExpression,
        UnaryExpression,
        BinaryExpression,
    }

    internal abstract class BoundNode
    {
        public abstract BoundNodeType BoundNodeType { get; }
        public abstract IEnumerable<BoundNode> GetChildren();

        public void PrettyPrint(TextWriter writer, string indent = "", bool isLast = true)
        {
            string marker = isLast ? "└──" : "├──";

            writer.Write(indent);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            writer.Write(marker);

            WriteNode(writer);

            writer.WriteLine();

            indent += isLast ? "   " : "│  ";

            BoundNode? lastChild = GetChildren().LastOrDefault();
            foreach (BoundNode child in GetChildren())
            {
                child.PrettyPrint(writer, indent, child == lastChild);
            }
        }

        private void WriteNode(TextWriter writer)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            writer.Write(BoundNodeType);

            Console.ForegroundColor = ConsoleColor.Blue;
            if (this is BoundExpression)
                writer.Write($" {((BoundExpression)this).Type}");
            switch (BoundNodeType)
            {
                case BoundNodeType.LiteralExpression:
                    writer.Write($" {((BoundLiteralExpression)this).Value}");
                    break;
                case BoundNodeType.BinaryExpression:
                    writer.Write($" {((BoundBinaryExpression)this).BinaryOperator.OperationType}");
                    break;
                case BoundNodeType.UnaryExpression:
                    writer.Write($" {((BoundUnaryExpression)this).UnaryOperator.OperationType}");
                    break;
                case BoundNodeType.AssignmentExpression:
                    writer.Write($" {((BoundAssignmentExpression)this).Variable.Name}");
                    break;
                case BoundNodeType.VariableExpression:
                    writer.Write($" {((BoundVariableExpression)this).Variable.Name}");
                    break;
                case BoundNodeType.VariableDeclarationStatement:
                    writer.Write($" {((BoundVariableDeclarationStatement)this).Variable.Name}");
                    break;
            }
            Console.ResetColor();
        }
    }
}
