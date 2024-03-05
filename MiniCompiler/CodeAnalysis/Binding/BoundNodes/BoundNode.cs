namespace MiniCompiler.CodeAnalysis.Binding.BoundNodes
{
    enum BoundNodeType
    {
        //Statements
        BlockStatement,
        ExpressionStatement,
        VariableDeclarationStatement,
        LabelStatement,
        GotoStatement,
        ConditionalGotoStatement,
        IfStatement,
        WhileStatement,
        DoWhileStatement,
        ForStatement,

        //Expressions
        ErrorExpression,
        LiteralExpression,
        VariableExpression,
        AssignmentExpression,
        CallExpression,
        ConversionExpression,
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
            if (this is BoundExpression expression)
                writer.Write($" Type:<{expression.Type}>");
            switch (BoundNodeType)
            {
                case BoundNodeType.LiteralExpression:
                    writer.Write($" Value:{((BoundLiteralExpression)this).Value}");
                    break;
                case BoundNodeType.BinaryExpression:
                    writer.Write($" Operation:{((BoundBinaryExpression)this).BinaryOperator.OperationType}");
                    break;
                case BoundNodeType.UnaryExpression:
                    writer.Write($" Operation:{((BoundUnaryExpression)this).UnaryOperator.OperationType}");
                    break;
                case BoundNodeType.VariableExpression:
                    writer.Write($" Variable:{((BoundVariableExpression)this).Variable}");
                    break;
                case BoundNodeType.AssignmentExpression:
                    writer.Write($" Variable:{((BoundAssignmentExpression)this).Variable}");
                    break;
                case BoundNodeType.VariableDeclarationStatement:
                    writer.Write($" Type:{((BoundVariableDeclarationStatement)this).Variable.Type} Variable:{((BoundVariableDeclarationStatement)this).Variable}");
                    break;
                case BoundNodeType.LabelStatement:
                    writer.Write($" Label:{((BoundLabelStatement)this).Label}");
                    break;
                case BoundNodeType.GotoStatement:
                    writer.Write($" Label:{((BoundGotoStatement)this).Label}");
                    break;
                case BoundNodeType.ConditionalGotoStatement:
                    writer.Write($" on {((BoundConditionalGotoStatement)this).JumpIfTrue}, go to {((BoundConditionalGotoStatement)this).Label} ");
                    break;
            }
            Console.ResetColor();
        }
    }
}
