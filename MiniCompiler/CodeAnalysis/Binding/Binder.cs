﻿using MiniCompiler.CodeAnalysis.Binding.BoundNodes;
using MiniCompiler.CodeAnalysis.Symbols;
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

        public static BoundGlobalScope BindGlobalScope(BoundGlobalScope? previousGlobalScope, CompilationUnit compilationUnit)
        {
            BoundScope parentScope = CreateParentScope(previousGlobalScope);
            Binder binder = new Binder(parentScope);
            BoundStatement statement = binder.BindCompilationUnit(compilationUnit);
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

            BoundScope createdScope = new BoundScope(null);

            while (stack.Count > 0)
            {
                BoundGlobalScope current = stack.Pop();
                BoundScope scope = new BoundScope(createdScope);

                foreach (VariableSymbol v in current.Variables)
                    scope.TryDeclare(v);

                createdScope = scope;
            }

            return createdScope;
        }

        #region Statements
        private BoundBlockStatement BindCompilationUnit(CompilationUnit node)
        {
            ImmutableArray<BoundStatement>.Builder statements = ImmutableArray.CreateBuilder<BoundStatement>();

            foreach (StatementNode statement in node.Statements)
            {
                statements.Add(BindStatement(statement));
            }

            return new BoundBlockStatement(statements.ToImmutable());
        }
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
                case NodeType.WhileStatement:
                    return BindWhileStatement((WhileStatementNode)node);
                case NodeType.ForStatement:
                    return BindForStatement((ForStatementNode)node);
                default:
                    throw new Exception($"Unexpected syntax node {node.Type}");
            }
        }

        private BoundStatement BindForStatement(ForStatementNode node)
        {
            scope = new BoundScope(scope);

            BoundVariableDeclarationStatement? declaration = null;
            if (node.Declaration != null)
                declaration = BindVariableDeclarationStatement(node.Declaration);

            BoundExpression condition = BindExpression(node.Condition, TypeSymbol.Bool);

            BoundAssignmentExpression? increment = null;
            if (node.Increment != null)
                increment = (BoundAssignmentExpression)BindAssignmentExpression(node.Increment);

            BoundStatement statement = BindStatement(node.Statement);

            scope = scope.Parent!;

            return new BoundForStatement(declaration, condition, increment, statement);
        }

        private BoundWhileStatement BindWhileStatement(WhileStatementNode node)
        {
            BoundExpression condition = BindExpression(node.Condition, TypeSymbol.Bool);
            scope = new BoundScope(scope);
            BoundStatement statement = BindStatement(node.Statement);
            scope = scope.Parent!;

            return new BoundWhileStatement(condition, statement);
        }

        private BoundIfStatement BindIfStatement(IfStatementNode node)
        {
            BoundExpression condition = BindExpression(node.Condition, TypeSymbol.Bool);
            scope = new BoundScope(scope);
            BoundStatement ifStatement = BindStatement(node.Statement);
            scope = scope.Parent!;

            //The else is optional
            BoundStatement? elseStatement = node.ElseClause == null ? null : BindStatement(node.ElseClause.Statement);

            return new BoundIfStatement(condition, ifStatement, elseStatement);
        }

        private BoundVariableDeclarationStatement BindVariableDeclarationStatement(VariableDeclarationStatementNode node)
        {
            bool isReadOnly = node.Keyword.Type == TokenType.LetKeyword;
            //For now, we don't handle declarations without initializer
            BoundExpression initializer = BindExpression(node.Initializer);
            VariableSymbol variable = BindVariable(node.Identifier, isReadOnly, initializer.Type);

            return new BoundVariableDeclarationStatement(variable, initializer);
        }

        private BoundBlockStatement BindBlockStatement(BlockStatementNode node)
        {
            ImmutableArray<BoundStatement>.Builder statements = ImmutableArray.CreateBuilder<BoundStatement>();
            scope = new BoundScope(scope);

            foreach (StatementNode statement in node.Statements)
            {
                statements.Add(BindStatement(statement));
            }

            scope = scope.Parent!;

            return new BoundBlockStatement(statements.ToImmutable());
        }

        private BoundExpressionStatement BindExpressionStatement(ExpressionStatementNode node)
        {
            BoundExpression expression = BindExpression(node.Expression);
            return new BoundExpressionStatement(expression);
        }
        #endregion Statements

        #region Expressions
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

        private BoundExpression BindExpression(ExpressionNode node, TypeSymbol expectedType)
        {
            BoundExpression boundExpression = BindExpression(node);
            if (boundExpression.Type != expectedType)
            {
                diagnostics.ReportCannotConvert(node.Span, boundExpression.Type, expectedType);
                return new BoundErrorExpression();
            }
            return boundExpression;
        }

        private BoundLiteralExpression BindLiteralExpression(LiteralExpressionNode node)
        {
            object value = node.Value;
            return new BoundLiteralExpression(value);
        }

        private BoundExpression BindNameExpression(NameExpressionNode node)
        {
            string? name = node.Identifier.Text;
            if (string.IsNullOrEmpty(name))
            {
                return new BoundErrorExpression();
            }

            if (!scope.TryLookup(name, out VariableSymbol? variable))
            {
                diagnostics.ReportUndefinedName(node.Identifier.Span, name);
                return new BoundErrorExpression();
            }

            return new BoundVariableExpression(variable!);
        }

        private BoundExpression BindAssignmentExpression(AssignmentExpressionNode node)
        {
            string? name = node.Identifier.Text;
            BoundExpression boundExpression = BindExpression(node.Expression);

            if (!scope.TryLookup(name, out VariableSymbol? variable))
            {
                diagnostics.ReportUndefinedName(node.Identifier.Span, name);
                return new BoundErrorExpression();
            }

            if (boundExpression.Type == TypeSymbol.Error || variable.Type == TypeSymbol.Error)
                return new BoundErrorExpression();

            if (variable!.Type != boundExpression.Type)
            {
                diagnostics.ReportCannotConvert(node.Expression.Span, boundExpression.Type, variable.Type);
                return new BoundErrorExpression();
            }

            if (variable.IsReadOnly)
            {
                diagnostics.ReportCannotAssign(node.Identifier.Span, name);
                return new BoundErrorExpression();
            }

            return new BoundAssignmentExpression(variable, boundExpression);
        }

        private BoundExpression BindUnaryExpression(UnaryExpressionNode node)
        {
            BoundExpression boundOperand = BindExpression(node.Operand);

            if (boundOperand.Type == TypeSymbol.Error)
                return new BoundErrorExpression();

            BoundUnaryOperator? boundOperator = BoundUnaryOperator.Bind(node.OperatorToken.Type, boundOperand.Type);

            if (boundOperator == null)
            {
                diagnostics.ReportUndefinedUnaryOperator(node.OperatorToken.Span, node.OperatorToken.Text ??
                    node.OperatorToken.Type.ToString(), boundOperand.Type);
                return new BoundErrorExpression();
            }

            return new BoundUnaryExpression(boundOperator, boundOperand);
        }

        private BoundExpression BindBinaryExpression(BinaryExpressionNode node)
        {
            BoundExpression boundLeft = BindExpression(node.Left);
            BoundExpression boundRight = BindExpression(node.Right);

            if (boundLeft.Type == TypeSymbol.Error || boundRight.Type == TypeSymbol.Error)
                return new BoundErrorExpression();

            BoundBinaryOperator? boundOperator = BoundBinaryOperator.Bind(node.OperatorToken.Type, boundLeft.Type,
                boundRight.Type);

            if (boundOperator == null)
            {
                diagnostics.ReportUndefinedBinaryOperator(node.OperatorToken.Span, node.OperatorToken.Text ??
                    node.OperatorToken.Type.ToString(), boundLeft.Type, boundRight.Type);
                return new BoundErrorExpression();
            }

            return new BoundBinaryExpression(boundLeft, boundOperator, boundRight);
        }
        #endregion Expressions

        private VariableSymbol BindVariable(Token identifier, bool isReadOnly, TypeSymbol type)
        {
            string name = identifier.Text ?? "?";
            VariableSymbol variable = new VariableSymbol(name, isReadOnly, type);

            if (!identifier.IsFake && !scope.TryDeclare(variable))
                diagnostics.ReportAlreadyExistingVariable(identifier.Span, name);

            return variable;
        }
    }
}
