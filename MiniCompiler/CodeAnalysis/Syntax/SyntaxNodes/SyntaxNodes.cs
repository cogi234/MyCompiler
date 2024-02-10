using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MiniCompiler.CodeAnalysis.Syntax.SyntaxNodes
{
    public enum NodeType
    {
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
            // ├ ─ └ │

            string marker = isLast ? "└──" : "├──";

            writer.Write(indent);
            writer.Write(marker);
            writer.Write(this.Type);

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
            }

            writer.WriteLine();

            indent += isLast ? "   " : "│  ";

            SyntaxNode lastChild = GetChildren().LastOrDefault();
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
