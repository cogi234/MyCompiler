using MiniLang.CodeAnalysis.Text;

namespace MiniLang.CodeAnalysis.Syntax.SyntaxNodes
{
    public enum NodeType
    {
        CompilationUnit,
        GlobalStatement,
        FunctionDeclaration,

        //Statements
        BlockStatement,
        ExpressionStatement,
        VariableDeclarationStatement,
        Parameter,
        IfStatement,
        ElseClause,
        WhileStatement,
        DoWhileStatement,
        ForStatement,
        BreakStatement,
        ContinueStatement,
        ReturnStatement,

        //Expressions
        LiteralExpression,
        NameExpression,
        UnaryExpression,
        BinaryExpression,
        ParenthesizedExpression,
        AssignmentExpression,
        CallExpression,
    }

    public abstract class SyntaxNode
    {
        protected SyntaxNode(SyntaxTree syntaxTree)
        {
            SyntaxTree = syntaxTree;
        }

        public abstract NodeType Type { get; }
        public abstract TextSpan Span { get; }
        public SyntaxTree SyntaxTree { get; }
        public TextLocation Location => new TextLocation(SyntaxTree.SourceText, Span);
        public SyntaxNode Parent => SyntaxTree.ChildToParent[this];

        public abstract IEnumerable<SyntaxNode> GetChildren();
        public abstract Token GetLastToken();

        public void PrettyPrint(TextWriter writer, string indent = "", bool isLast = true)
        {
            string marker = isLast ? "└──" : "├──";

            writer.Write(indent);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            writer.Write(marker);

            WriteNode(writer);

            writer.WriteLine();

            indent += isLast ? "   " : "│  ";

            SyntaxNode? lastChild = GetChildren().LastOrDefault();
            foreach (SyntaxNode child in GetChildren())
            {
                child.PrettyPrint(writer, indent, child == lastChild);
            }
        }

        private void WriteNode(TextWriter writer)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            writer.Write(Type);

            Console.ForegroundColor = ConsoleColor.Blue;
            switch (Type)
            {
                case NodeType.LiteralExpression:
                    writer.Write($" Value:{((LiteralExpressionNode)this).Value}");
                    break;
                case NodeType.BinaryExpression:
                    writer.Write($" Operator:{((BinaryExpressionNode)this).OperatorToken.Text}");
                    break;
                case NodeType.UnaryExpression:
                    writer.Write($" Operator:{((UnaryExpressionNode)this).OperatorToken.Text}");
                    break;
                case NodeType.AssignmentExpression:
                    writer.Write($" Identifier:{((AssignmentExpressionNode)this).Identifier.Text}");
                    break;
                case NodeType.NameExpression:
                    writer.Write($" Identifier:{((VariableExpressionNode)this).Identifier.Text}");
                    break;
                case NodeType.VariableDeclarationStatement:
                    writer.Write($" Identifier::{((VariableDeclarationStatementNode)this).Identifier.Text} Type:{((VariableDeclarationStatementNode)this).Keyword.Text}");
                    break;
                case NodeType.CallExpression:
                    writer.Write($" {((CallExpressionNode)this).Identifier.Text}");
                    break;
            }
            Console.ResetColor();
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
