using MiniCompiler.CodeAnalysis.Binding.BoundNodes;
using MiniCompiler.CodeAnalysis.Syntax;
using MiniCompiler.CodeAnalysis.Syntax.SyntaxNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace MiniCompiler.CodeAnalysis.Binding
{
    internal sealed class Binder
    {
        private readonly DiagnosticBag diagnostics = new DiagnosticBag();
        private readonly Dictionary<string, object> variables;

        public DiagnosticBag Diagnostics => diagnostics;
        public Dictionary<string, object> Variables => variables;
        public SyntaxTree SyntaxTree { get; }


        public Binder(SyntaxTree syntaxTree, Dictionary<string, object> variables)
        {
            SyntaxTree = syntaxTree;
            this.variables = variables;
            diagnostics.AddRange(syntaxTree.Diagnostics);
        }

        public BoundTree Bind()
        {
            BoundExpression boundExpression = BindExpression(SyntaxTree.Root);
            return new BoundTree(diagnostics, boundExpression);
        }

        private BoundExpression BindExpression(ExpressionNode node)
        {
            switch (node.Type)
            {
                case NodeType.ParenthesizedExpression:
                    return BindExpression(((ParenthesizedExpressionNode)node).Expression);
                case NodeType.LiteralExpression:
                    return BindLiteralExpression((LiteralExpressionNode)node);
                case NodeType.NameExpression:
                    return BindNameExpression((NameExpressionNode)node);
                case NodeType.AssignmentExpression:
                    return BindAssignmentExpression((AssignmentExpressionNode)node);
                case NodeType.UnaryExpression:
                    return BindUnaryExpression((UnaryExpressionNode)node);
                case NodeType.BinaryExpression:
                    return BindBinaryExpression((BinaryExpressionNode)node);
                default:
                    throw new Exception($"Unexpected syntax node {node.Type}");
            }
        }

        private BoundExpression BindLiteralExpression(LiteralExpressionNode node)
        {
            object value = node.Value ?? 0;
            return new BoundLiteralExpression(value);
        }

        private BoundExpression BindNameExpression(NameExpressionNode node)
        {
            string name = node.IdentifierToken.Text;
            if (!variables.TryGetValue(name, out object? value))
            {
                diagnostics.ReportUndefinedName(node.IdentifierToken.Span, name);
                return new BoundLiteralExpression(0);
            }
            Type type = value?.GetType() ?? typeof(object);
            return new BoundVariableExpression(name, type);
        }

        private BoundExpression BindAssignmentExpression(AssignmentExpressionNode node)
        {
            string name = node.IdentifierToken.Text;
            BoundExpression boundExpression = BindExpression(node.Expression);

            object? defaultValue =
                boundExpression.Type == typeof(int)
                ? 0
                : boundExpression.Type == typeof(bool)
                ? false
                : null;

            if (defaultValue == null)
                throw new Exception($"Unsupported variable type: {boundExpression.Type}");

            variables[name] = defaultValue;
            return new BoundAssignmentExpression(name, boundExpression);
        }

        private BoundExpression BindUnaryExpression(UnaryExpressionNode node)
        {
            BoundExpression boundOperand = BindExpression(node.Operand);
            BoundUnaryOperator? boundOperator = BoundUnaryOperator.Bind(node.OperatorToken.Type, boundOperand.Type);

            if (boundOperator == null)
            {
                diagnostics.ReportUndefinedUnaryOperator(node.OperatorToken.Span, node.OperatorToken.Text ?? node.OperatorToken.Type.ToString(), boundOperand.Type);
                return boundOperand;
            }

            return new BoundUnaryExpression(boundOperator, boundOperand);
        }

        private BoundExpression BindBinaryExpression(BinaryExpressionNode node)
        {
            BoundExpression boundLeft = BindExpression(node.Left);
            BoundExpression boundRight = BindExpression(node.Right);
            BoundBinaryOperator? boundOperator = BoundBinaryOperator.Bind(node.OperatorToken.Type, boundLeft.Type, boundRight.Type);

            if (boundOperator == null)
            {
                diagnostics.ReportUndefinedBinaryOperator(node.OperatorToken.Span, node.OperatorToken.Text ?? node.OperatorToken.Type.ToString(), boundLeft.Type, boundRight.Type);
                return boundLeft;
            }

            return new BoundBinaryExpression(boundLeft, boundOperator, boundRight);
        }
    }
}
