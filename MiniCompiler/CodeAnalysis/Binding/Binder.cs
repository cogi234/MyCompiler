﻿using MiniCompiler.CodeAnalysis.Binding.BoundNodes;
using MiniCompiler.CodeAnalysis.Syntax;
using MiniCompiler.CodeAnalysis.Syntax.SyntaxNodes;
using MiniCompiler.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Xml.Linq;

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

        public static BoundGlobalScope BindGlobalScope(BoundGlobalScope? previousGlobalScope, CompilationUnit compilationUnit)
        {
            BoundScope parentScope = CreateParentScope(previousGlobalScope);
            Binder binder = new Binder(parentScope);
            BoundStatement statement = binder.BindStatement(compilationUnit.Statement);
            ImmutableArray<VariableSymbol> variables = binder.scope.GetDeclaredVariables();
            ImmutableArray<Diagnostic> diagnostics = binder.Diagnostics.ToImmutableArray();

            if (previousGlobalScope != null)
                diagnostics = diagnostics.InsertRange(0, previousGlobalScope.Diagnostics);

            return new BoundGlobalScope(previousGlobalScope, diagnostics, variables, statement);
        }

        private static BoundScope CreateParentScope(BoundGlobalScope? previous)
        {
            Stack<BoundGlobalScope> stack = new Stack<BoundGlobalScope>();
            while (previous != null)
            {
                stack.Push(previous);
                previous = previous.Previous;
            }

            BoundScope createdScope = null;

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                var scope = new BoundScope(createdScope);

                foreach (var v in current.Variables)
                    scope.TryDeclare(v);

                createdScope = scope;
            }

            return createdScope;
        }

        //Binding statements

        private BoundStatement BindStatement(StatementNode node)
        {
            switch (node.Type)
            {
                case NodeType.BlockStatement:
                    return BindBlockStatement((BlockStatementNode)node);
                case NodeType.ExpressionStatement:
                    return BindExpressionStatement((ExpressionStatementNode)node);
                case NodeType.VariableDeclarationStatement:
                    return BindVariableDeclarationStatement((VariableDeclarationStatementNode)node);
                case NodeType.IfStatement:
                    return BindIfStatement((IfStatementNode)node);
                default:
                    throw new Exception($"Unexpected syntax node {node.Type}");
            }
        }

        private BoundStatement BindIfStatement(IfStatementNode node)
        {
            BoundExpression condition = BindExpression(node.Condition);
            scope = new BoundScope(scope);
            BoundStatement ifStatement = BindStatement(node.Statement);
            scope = scope.Parent;

            //The else is optional
            BoundElseStatement? elseStatement = null;
            if (node.ElseStatement != null)
                elseStatement = BindElseStatement(node.ElseStatement);

            if (condition.Type != typeof(bool))
            {
                diagnostics.ReportCannotConvert(node.Condition.Span, condition.Type, typeof(bool));
                return ifStatement;
            }

            return new BoundIfStatement(condition, ifStatement, elseStatement);
        }

        private BoundElseStatement BindElseStatement(ElseStatementNode node)
        {
            scope = new BoundScope(scope);
            BoundStatement statement = BindStatement(node.Statement);
            scope = scope.Parent;
            return new BoundElseStatement(statement);
        }

        private BoundVariableDeclarationStatement BindVariableDeclarationStatement(VariableDeclarationStatementNode node)
        {
            bool isReadOnly = node.Keyword.Type == TokenType.LetKeyword;
            string name = node.Identifier.Text ?? "";
            //For now, we don't handle declarations without initializer
            BoundExpression initializer = BindExpression(node.Initializer);

            VariableSymbol assignmentVariable = new VariableSymbol(name, isReadOnly, initializer.Type);

            if (!scope.TryDeclare(assignmentVariable))
                diagnostics.ReportAlreadyExistingVariable(node.Identifier.Span, name);

            return new BoundVariableDeclarationStatement(assignmentVariable, initializer);
        }

        private BoundBlockStatement BindBlockStatement(BlockStatementNode node)
        {
            ImmutableArray<BoundStatement>.Builder statements = ImmutableArray.CreateBuilder<BoundStatement>();
            scope = new BoundScope(scope);

            foreach (StatementNode statement in node.Statements)
            {
                statements.Add(BindStatement(statement));
            }

            scope = scope.Parent;

            return new BoundBlockStatement(statements.ToImmutable());
        }

        private BoundExpressionStatement BindExpressionStatement(ExpressionStatementNode node)
        {
            BoundExpression expression = BindExpression(node.Expression);
            return new BoundExpressionStatement(expression);
        }

        //Binding expressions

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

        private BoundLiteralExpression BindLiteralExpression(LiteralExpressionNode node)
        {
            object value = node.Value ?? 0;
            return new BoundLiteralExpression(value);
        }

        private BoundExpression BindNameExpression(NameExpressionNode node)
        {
            string name = node.Identifier.Text ?? "";

            if (scope.TryLookup(name, out VariableSymbol? variable))
                return new BoundVariableExpression(variable);
            else
            {
                diagnostics.ReportUndefinedName(node.Identifier.Span, name);
                return new BoundLiteralExpression(0);
            }
        }

        private BoundExpression BindAssignmentExpression(AssignmentExpressionNode node)
        {
            string name = node.IdentifierToken.Text ?? "";
            BoundExpression boundExpression = BindExpression(node.Expression);

            if (!scope.TryLookup(name, out VariableSymbol? variable))
            {
                diagnostics.ReportUndefinedName(node.IdentifierToken.Span, name);
                return boundExpression;
            }

            if (variable.Type != boundExpression.Type)
            {
                diagnostics.ReportCannotConvert(node.Expression.Span, boundExpression.Type, variable.Type);
                return boundExpression;
            }

            if (variable.IsReadOnly)
            {
                diagnostics.ReportCannotAssign(node.IdentifierToken.Span, name);
                return boundExpression;
            }

            return new BoundAssignmentExpression(variable, boundExpression);
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
