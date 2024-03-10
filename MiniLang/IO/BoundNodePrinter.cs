using MiniLang.CodeAnalysis.Binding.BoundNodes;
using MiniLang.CodeAnalysis.Symbols;
using MiniLang.CodeAnalysis.Syntax;
using System.CodeDom.Compiler;

namespace MiniLang.IO
{
    internal static class BoundNodePrinter
    {
        public static void WriteTo(this BoundNode node, TextWriter writer)
        {
            if (writer is IndentedTextWriter iw)
                WriteTo(node, iw);
            else
                WriteTo(node, new IndentedTextWriter(writer));
        }

        public static void WriteTo(this BoundNode node, IndentedTextWriter writer)
        {
            switch (node.BoundNodeType)
            {
                case BoundNodeType.BlockStatement:
                    WriteBlockStatement((BoundBlockStatement)node, writer);
                    break;
                case BoundNodeType.ExpressionStatement:
                    WriteExpressionStatement((BoundExpressionStatement)node, writer);
                    break;
                case BoundNodeType.VariableDeclarationStatement:
                    WriteVariableDeclarationStatement((BoundVariableDeclarationStatement)node, writer);
                    break;
                case BoundNodeType.LabelStatement:
                    WriteLabelStatement((BoundLabelStatement)node, writer);
                    break;
                case BoundNodeType.GotoStatement:
                    WriteGotoStatement((BoundGotoStatement)node, writer);
                    break;
                case BoundNodeType.ConditionalGotoStatement:
                    WriteConditionalGotoStatement((BoundConditionalGotoStatement)node, writer);
                    break;
                case BoundNodeType.IfStatement:
                    WriteIfStatement((BoundIfStatement)node, writer);
                    break;
                case BoundNodeType.WhileStatement:
                    WriteWhileStatement((BoundWhileStatement)node, writer);
                    break;
                case BoundNodeType.DoWhileStatement:
                    WriteDoWhileStatement((BoundDoWhileStatement)node, writer);
                    break;
                case BoundNodeType.ForStatement:
                    WriteForStatement((BoundForStatement)node, writer);
                    break;
                case BoundNodeType.ReturnStatement:
                    WriteReturnStatement((BoundReturnStatement)node, writer);
                    break;
                case BoundNodeType.ErrorExpression:
                    WriteErrorExpression((BoundErrorExpression)node, writer);
                    break;
                case BoundNodeType.LiteralExpression:
                    WriteLiteralExpression((BoundLiteralExpression)node, writer);
                    break;
                case BoundNodeType.VariableExpression:
                    WriteVariableExpression((BoundVariableExpression)node, writer);
                    break;
                case BoundNodeType.AssignmentExpression:
                    WriteAssignmentExpression((BoundAssignmentExpression)node, writer);
                    break;
                case BoundNodeType.CallExpression:
                    WriteCallExpression((BoundCallExpression)node, writer);
                    break;
                case BoundNodeType.ConversionExpression:
                    WriteConversionExpression((BoundConversionExpression)node, writer);
                    break;
                case BoundNodeType.UnaryExpression:
                    WriteUnaryExpression((BoundUnaryExpression)node, writer);
                    break;
                case BoundNodeType.BinaryExpression:
                    WriteBinaryExpression((BoundBinaryExpression)node, writer);
                    break;
                default:
                    throw new Exception($"Unexpected node {node.BoundNodeType}");
            }
        }

        #region Statements
        private static void WriteBlockStatement(BoundBlockStatement node, IndentedTextWriter writer)
        {
            writer.WritePunctuation(TokenType.OpenBrace);
            writer.WriteLine();

            writer.Indent++;
            foreach (BoundStatement statement in node.Statements)
                statement.WriteTo(writer);
            writer.Indent--;

            writer.WritePunctuation(TokenType.CloseBrace);
            writer.WriteLine();
        }

        private static void WriteExpressionStatement(BoundExpressionStatement node, IndentedTextWriter writer)
        {
            node.Expression.WriteTo(writer);
            writer.WritePunctuation(TokenType.Semicolon);
            writer.WriteLine();
        }

        private static void WriteVariableDeclarationStatement(BoundVariableDeclarationStatement node, IndentedTextWriter writer, bool newLine = true)
        {
            node.Variable.WriteTo(writer);
            if (node.Initializer != null)
            {
                writer.WriteSpace();
                writer.WritePunctuation(TokenType.Equal);
                writer.WriteSpace();
                node.Initializer.WriteTo(writer);
            }
            if (newLine)
            {
                writer.WritePunctuation(TokenType.Semicolon);
                writer.WriteLine();
            }
        }

        private static void WriteLabelStatement(BoundLabelStatement node, IndentedTextWriter writer)
        {
            int previousIndent = writer.Indent;
            writer.Indent = 0;

            writer.WritePunctuation(node.Label.Name);
            writer.WritePunctuation(TokenType.Colon);
            writer.WriteLine();

            writer.Indent = previousIndent;
        }

        private static void WriteGotoStatement(BoundGotoStatement node, IndentedTextWriter writer)
        {
            writer.WriteKeyword("goto ");
            writer.WriteIdentifier(node.Label.Name);
            writer.WritePunctuation(TokenType.Semicolon);
            writer.WriteLine();
        }

        private static void WriteConditionalGotoStatement(BoundConditionalGotoStatement node, IndentedTextWriter writer)
        {
            writer.WriteKeyword("goto ");
            writer.WriteIdentifier(node.Label.Name);
            writer.WriteKeyword(node.JumpIfTrue ? " if " : " unless ");
            node.Condition.WriteTo(writer);
            writer.WritePunctuation(TokenType.Semicolon);
            writer.WriteLine();
        }

        private static void WriteIfStatement(BoundIfStatement node, IndentedTextWriter writer)
        {
            writer.WriteKeyword(TokenType.IfKeyword);
            writer.WriteSpace();
            writer.WritePunctuation(TokenType.OpenParenthesis);
            node.Condition.WriteTo(writer);
            writer.WritePunctuation(TokenType.CloseParenthesis);
            writer.WriteLine();
            writer.WriteNestedStatement(node.ThenStatement);

            if (node.ElseStatement != null)
            {
                writer.WriteKeyword(TokenType.ElseKeyword);
                writer.WriteLine();
                writer.WriteNestedStatement(node.ElseStatement);
            }
        }

        private static void WriteWhileStatement(BoundWhileStatement node, IndentedTextWriter writer)
        {
            writer.WriteKeyword(TokenType.WhileKeyword);
            writer.WriteSpace();
            writer.WritePunctuation(TokenType.OpenParenthesis);
            node.Condition.WriteTo(writer);
            writer.WritePunctuation(TokenType.CloseParenthesis);
            writer.WriteLine();
            writer.WriteNestedStatement(node.Body);
        }

        private static void WriteDoWhileStatement(BoundDoWhileStatement node, IndentedTextWriter writer)
        {
            writer.WriteKeyword(TokenType.DoKeyword);
            writer.WriteLine();
            writer.WriteNestedStatement(node.Body);
            writer.WriteKeyword(TokenType.WhileKeyword);
            writer.WriteSpace();
            writer.WritePunctuation(TokenType.OpenParenthesis);
            node.Condition.WriteTo(writer);
            writer.WritePunctuation(TokenType.CloseParenthesis);
            writer.WriteLine();
        }

        private static void WriteForStatement(BoundForStatement node, IndentedTextWriter writer)
        {
            writer.WriteKeyword(TokenType.ForKeyword);
            writer.WriteSpace();
            writer.WritePunctuation(TokenType.OpenParenthesis);
            node.Declaration?.WriteTo(writer);
            writer.WritePunctuation(TokenType.Semicolon);
            writer.WriteSpace();
            node.Condition.WriteTo(writer);
            writer.WritePunctuation(TokenType.Semicolon);
            writer.WriteSpace();
            node.Increment?.WriteTo(writer);
            writer.WritePunctuation(TokenType.CloseParenthesis);
            writer.WriteNestedStatement(node.Body);
        }

        private static void WriteReturnStatement(BoundReturnStatement node, IndentedTextWriter writer)
        {
            writer.WriteKeyword(TokenType.ReturnKeyword);
            if (node.Expression != null)
            {
                writer.WriteSpace();
                node.Expression.WriteTo(writer);
            }
            writer.WriteLine();
        }
        #endregion
        #region Expressions
        private static void WriteErrorExpression(BoundErrorExpression node, IndentedTextWriter writer)
        {
            writer.WriteKeyword("?");
        }

        private static void WriteLiteralExpression(BoundLiteralExpression node, IndentedTextWriter writer)
        {
            string value = node.Value.ToString() ?? "";

            if (node.Type == TypeSymbol.Bool)
                writer.WriteKeyword(value);
            else if (node.Type == TypeSymbol.Int)
                writer.WriteNumber(value);
            else if (node.Type == TypeSymbol.String)
            {
                value = "\"" + value.Replace("\"", "\"\"") + "\"";
                writer.WriteString(value);
            }
            else
                throw new Exception($"Unexpected type {node.Type}");
        }

        private static void WriteVariableExpression(BoundVariableExpression node, IndentedTextWriter writer)
        {
            writer.WriteIdentifier(node.Variable.Name);
        }

        private static void WriteAssignmentExpression(BoundAssignmentExpression node, IndentedTextWriter writer)
        {
            writer.WriteIdentifier(node.Variable.Name);
            writer.WriteSpace();
            writer.WritePunctuation(TokenType.Equal);
            writer.WriteSpace();
            node.Expression.WriteTo(writer);
        }

        private static void WriteCallExpression(BoundCallExpression node, IndentedTextWriter writer)
        {
            writer.WriteIdentifier(node.Function.Name);
            writer.WritePunctuation(TokenType.OpenParenthesis);

            bool isFirst = true;
            foreach (BoundExpression argument in node.Arguments)
            {
                if (isFirst)
                    isFirst = false;
                else
                {
                    writer.WritePunctuation(TokenType.Comma);
                    writer.WriteSpace();
                }

                argument.WriteTo(writer);
            }

            writer.WritePunctuation(TokenType.CloseParenthesis);
        }

        private static void WriteConversionExpression(BoundConversionExpression node, IndentedTextWriter writer)
        {
            writer.WriteIdentifier(node.Type.Name);
            writer.WritePunctuation(TokenType.OpenParenthesis);
            node.Expression.WriteTo(writer);
            writer.WritePunctuation(TokenType.CloseParenthesis);
        }

        private static void WriteUnaryExpression(BoundUnaryExpression node, IndentedTextWriter writer)
        {
            int precedence = SyntaxFacts.GetUnaryOperatorPrecedence(node.UnaryOperator.TokenType);

            writer.WritePunctuation(node.UnaryOperator.TokenType);
            writer.WriteNestedExpression(node.Operand, precedence);
        }

        private static void WriteBinaryExpression(BoundBinaryExpression node, IndentedTextWriter writer)
        {
            int precedence = SyntaxFacts.GetBinaryOperatorPrecedence(node.BinaryOperator.TokenType);

            writer.WriteNestedExpression(node.Left, precedence);
            writer.WriteSpace();
            writer.WritePunctuation(node.BinaryOperator.TokenType);
            writer.WriteSpace();
            writer.WriteNestedExpression(node.Right, precedence);
        }
        #endregion

        private static void WriteNestedStatement(this IndentedTextWriter writer, BoundStatement node)
        {
            bool needsIndentation = !(node is BoundBlockStatement);

            if (needsIndentation)
                writer.Indent++;

            node.WriteTo(writer);

            if (needsIndentation)
                writer.Indent--;
        }

        private static void WriteNestedExpression(this IndentedTextWriter writer, BoundExpression node, int parentPrecedence)
        {
            if (node is BoundUnaryExpression unary)
                writer.WriteNestedExpression(unary, parentPrecedence, SyntaxFacts.GetUnaryOperatorPrecedence(unary.UnaryOperator.TokenType));
            else if (node is BoundBinaryExpression binary)
                writer.WriteNestedExpression(binary, parentPrecedence, SyntaxFacts.GetBinaryOperatorPrecedence(binary.BinaryOperator.TokenType));
            else
                node.WriteTo(writer);
        }

        private static void WriteNestedExpression(this IndentedTextWriter writer, BoundExpression node, int parentPrecedence, int currentPrecedence)
        {
            bool needsParenthesis = parentPrecedence >= currentPrecedence;

            if (needsParenthesis)
                writer.WritePunctuation(TokenType.OpenParenthesis);

            node.WriteTo(writer);

            if (needsParenthesis)
                writer.WritePunctuation(TokenType.CloseParenthesis);
        }
    }
}
