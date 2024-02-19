using MiniCompiler.CodeAnalysis.Binding.BoundNodes;
using MiniCompiler.CodeAnalysis.Syntax;
using MiniCompiler.CodeAnalysis.Syntax.SyntaxNodes;
using System.Collections.Immutable;

namespace MiniCompiler.CodeAnalysis.Binding
{
    internal sealed class Binder
    {
        private readonly DiagnosticBag diagnostics = new DiagnosticBag();

        private BoundScope scope;

        public DiagnosticBag Diagnostics => diagnostics;


        public Binder(BoundScope? parent)
        {
            scope = new BoundScope(parent);
        }

        public static BoundGlobalScope BindGlobalScope(CompilationUnit compilationUnit)
        {
            Binder binder = new Binder(null);
            BoundExpression expression = binder.BindExpression(compilationUnit.Expression);
            ImmutableArray<VariableSymbol> variables = binder.scope.GetDeclaredVariables();
            ImmutableArray<Diagnostic> diagnostics = binder.Diagnostics.ToImmutableArray();
            return new BoundGlobalScope(null, diagnostics, variables, expression);
        }

        public BoundExpression BindExpression(ExpressionNode node)
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

            if (scope.TryLookup(name, out VariableSymbol variable))
                return new BoundVariableExpression(variable);
            else
            {
                diagnostics.ReportUndefinedName(node.IdentifierToken.Span, name);
                return new BoundLiteralExpression(0);
            }
        }

        private BoundExpression BindAssignmentExpression(AssignmentExpressionNode node)
        {
            string name = node.IdentifierToken.Text;
            BoundExpression boundExpression = BindExpression(node.Expression);

            VariableSymbol assignmentVariable = new VariableSymbol(name, boundExpression.Type);

            if (scope.TryLookup(name, out VariableSymbol? existingVariable))
            {
                if (existingVariable.Type == assignmentVariable.Type)
                    return new BoundAssignmentExpression(existingVariable, boundExpression);
                else
                {
                    diagnostics.ReportWrongTypeAssigment(node.Span, name, existingVariable.Type, assignmentVariable.Type);
                    return new BoundLiteralExpression(0);
                }
            } else
            {
                scope.TryDeclare(assignmentVariable);
                return new BoundAssignmentExpression(assignmentVariable, boundExpression);
            }
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
