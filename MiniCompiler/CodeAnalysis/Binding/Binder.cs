using MiniCompiler.CodeAnalysis.Binding.BoundNodes;
using MiniCompiler.CodeAnalysis.Lowering;
using MiniCompiler.CodeAnalysis.Symbols;
using MiniCompiler.CodeAnalysis.Syntax;
using MiniCompiler.CodeAnalysis.Syntax.SyntaxNodes;
using MiniCompiler.CodeAnalysis.Text;
using System.Collections.Immutable;

namespace MiniCompiler.CodeAnalysis.Binding
{
    internal sealed class Binder
    {
        private readonly DiagnosticBag diagnostics = new DiagnosticBag();
        public DiagnosticBag Diagnostics => diagnostics;

        private BoundScope scope;
        private FunctionSymbol? function;
        private Stack<(BoundLabel BreakLabel, BoundLabel ContinueLabel)> loopStack =
            new Stack<(BoundLabel BreakLabel, BoundLabel ContinueLabel)>();
        private int loopLabelCounter;

        public Binder(BoundScope? parent, FunctionSymbol? function)
        {
            scope = new BoundScope(parent);
            this.function = function;

            if (function != null)
            {
                foreach (ParameterSymbol parameter in function.Parameters)
                    scope.TryDeclareVariable(parameter);
            }
        }

        private static BoundScope CreateParentScope(BoundGlobalScope? previous)
        {
            Stack<BoundGlobalScope> stack = new Stack<BoundGlobalScope>();
            while (previous != null)
            {
                stack.Push(previous);
                previous = previous.Previous;
            }

            BoundScope createdScope = CreateRootScope();

            while (stack.Count > 0)
            {
                BoundGlobalScope current = stack.Pop();
                BoundScope scope = new BoundScope(createdScope);

                foreach (FunctionSymbol v in current.Functions)
                    scope.TryDeclareFunction(v);

                foreach (VariableSymbol v in current.Variables)
                    scope.TryDeclareVariable(v);

                createdScope = scope;
            }

            return createdScope;
        }
        private static BoundScope CreateRootScope()
        {
            BoundScope rootScope = new BoundScope(null);

            foreach (FunctionSymbol f in BuiltInFunctions.GetAll())
                rootScope.TryDeclareFunction(f);

            return rootScope;
        }

        public static BoundGlobalScope BindGlobalScope(BoundGlobalScope? previousGlobalScope, CompilationUnit compilationUnit)
        {
            BoundScope parentScope = CreateParentScope(previousGlobalScope);
            Binder binder = new Binder(parentScope, null);

            foreach (FunctionDeclarationNode function in compilationUnit.Members.OfType<FunctionDeclarationNode>())
                binder.BindFunctionDeclaration(function);

            ImmutableArray<BoundStatement>.Builder statements = ImmutableArray.CreateBuilder<BoundStatement>();
            foreach (GlobalStatementNode globalStatement in compilationUnit.Members.OfType<GlobalStatementNode>())
            {
                BoundStatement statement = binder.BindStatement(globalStatement.Statement);
                statements.Add(statement);
            }

            ImmutableArray<FunctionSymbol> functions = binder.scope.GetDeclaredFunctions();
            ImmutableArray<VariableSymbol> variables = binder.scope.GetDeclaredVariables();
            ImmutableArray<Diagnostic> diagnostics = binder.Diagnostics.ToImmutableArray();

            if (previousGlobalScope != null)
                diagnostics = diagnostics.InsertRange(0, previousGlobalScope.Diagnostics);

            return new BoundGlobalScope(previousGlobalScope, diagnostics, functions, variables, statements.ToImmutable());
        }

        public static BoundProgram BindProgram(BoundGlobalScope globalScope)
        {
            BoundScope parentScope = CreateParentScope(globalScope);
            ImmutableDictionary<FunctionSymbol, BoundBlockStatement>.Builder functionBodies = ImmutableDictionary.CreateBuilder<FunctionSymbol, BoundBlockStatement>();
            ImmutableArray<Diagnostic>.Builder diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();
            BoundGlobalScope? scope = globalScope;

            while (scope != null)
            {
                foreach (FunctionSymbol function in scope.Functions)
                {
                    Binder binder = new Binder(parentScope, function);
                    BoundStatement body = binder.BindStatement(function.Declaration!.Body);
                    BoundBlockStatement loweredBody = Lowerer.Lower(body);
                    functionBodies.Add(function, loweredBody);

                    diagnostics.AddRange(binder.diagnostics);
                }
                scope = scope.Previous;
            }

            BoundBlockStatement statement = Lowerer.Lower(new BoundBlockStatement(globalScope.Statements));

            return new BoundProgram(diagnostics.ToImmutable(), functionBodies.ToImmutable(), statement);
        }

        private void BindFunctionDeclaration(FunctionDeclarationNode node)
        {
            scope = new BoundScope(scope);

            TypeSymbol returnType = TypeSymbol.Lookup(node.TypeKeyword.Text!)!;

            ImmutableArray<ParameterSymbol>.Builder parameters = ImmutableArray.CreateBuilder<ParameterSymbol>();
            HashSet<string> seenParameterNames = new HashSet<string>();

            foreach (ParameterNode parameter in node.Parameters)
            {
                string name = parameter.Identifier.Text!;
                TypeSymbol type = TypeSymbol.Lookup(parameter.TypeKeyword.Text!)!;

                if (!seenParameterNames.Add(name))
                    diagnostics.ReportParameterAlreadyDeclared(parameter.Identifier.Span, name);
                else
                    parameters.Add(new ParameterSymbol(name, type));
            }
            ImmutableArray<ParameterSymbol> boundParameters = parameters.ToImmutable();

            scope = scope.Parent!;

            BindFunction(node.Identifier, boundParameters, returnType, node);
        }

        #region Statements
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
                case NodeType.DoWhileStatement:
                    return BindDoWhileStatement((DoWhileStatementNode)node);
                case NodeType.ForStatement:
                    return BindForStatement((ForStatementNode)node);
                case NodeType.BreakStatement:
                    return BindBreakStatement((BreakStatementNode)node);
                case NodeType.ContinueStatement:
                    return BindContinueStatement((ContinueStatementNode)node);
                case NodeType.ReturnStatement:
                    return BindReturnStatement((ReturnStatementNode)node);
                default:
                    throw new Exception($"Unexpected syntax node {node.Type}");
            }
        }

        private BoundStatement BindReturnStatement(ReturnStatementNode node)
        {
            BoundExpression? expression = null;

            if (function == null)
                diagnostics.ReportInvalidReturn(node.Keyword.Span);
            else
            {
                if (function.ReturnType == TypeSymbol.Void)
                {
                    if (node.Expression != null)
                        diagnostics.ReportInvalidReturnExpression(node.Expression!.Span, function.Name);
                } else
                {
                    if (node.Expression == null)
                        diagnostics.ReportMissingReturnExpression(node.Keyword.Span, function.ReturnType);
                    else
                        expression = BindExpression(node.Expression, function.ReturnType);
                }
            }

            return new BoundReturnStatement(expression);
        }

        private BoundStatement BindContinueStatement(ContinueStatementNode node)
        {
            if (loopStack.Count == 0)
            {
                diagnostics.ReportInvalidBreakOrContinue(node.Keyword.Span, node.Keyword.Text);
                return new BoundExpressionStatement(new BoundErrorExpression());
            }
            BoundLabel continueLabel = loopStack.Peek().ContinueLabel;
            return new BoundGotoStatement(continueLabel);
        }

        private BoundStatement BindBreakStatement(BreakStatementNode node)
        {
            if (loopStack.Count == 0)
            {
                diagnostics.ReportInvalidBreakOrContinue(node.Keyword.Span, node.Keyword.Text);
                return new BoundExpressionStatement(new BoundErrorExpression());
            }
            BoundLabel breakLabel = loopStack.Peek().BreakLabel;
            return new BoundGotoStatement(breakLabel);
        }

        private BoundForStatement BindForStatement(ForStatementNode node)
        {
            scope = new BoundScope(scope);

            BoundVariableDeclarationStatement? declaration = null;
            if (node.Declaration != null)
                declaration = BindVariableDeclarationStatement(node.Declaration);

            BoundExpression condition = BindExpression(node.Condition, TypeSymbol.Bool);

            BoundAssignmentExpression? increment = null;
            if (node.Increment != null)
                increment = (BoundAssignmentExpression)BindAssignmentExpression(node.Increment);

            BoundStatement statement = BindLoopBody(node.Statement, out BoundLabel breakLabel, out BoundLabel continueLabel);

            scope = scope.Parent!;

            return new BoundForStatement(declaration, condition, increment, statement, breakLabel, continueLabel);
        }

        private BoundDoWhileStatement BindDoWhileStatement(DoWhileStatementNode node)
        {
            scope = new BoundScope(scope);
            BoundStatement statement = BindLoopBody(node.Statement, out BoundLabel breakLabel, out BoundLabel continueLabel);
            scope = scope.Parent!;
            BoundExpression condition = BindExpression(node.Condition, TypeSymbol.Bool);

            return new BoundDoWhileStatement(statement, condition, breakLabel, continueLabel);
        }

        private BoundWhileStatement BindWhileStatement(WhileStatementNode node)
        {
            BoundExpression condition = BindExpression(node.Condition, TypeSymbol.Bool);
            scope = new BoundScope(scope);
            BoundStatement statement = BindLoopBody(node.Statement, out BoundLabel breakLabel, out BoundLabel continueLabel);
            scope = scope.Parent!;

            return new BoundWhileStatement(condition, statement, breakLabel, continueLabel);
        }

        private BoundStatement BindLoopBody(StatementNode body, out BoundLabel breakLabel, out BoundLabel continueLabel)
        {
            breakLabel = new BoundLabel($"break{loopLabelCounter}");
            continueLabel = new BoundLabel($"continue{loopLabelCounter}");
            loopLabelCounter++;

            loopStack.Push((breakLabel, continueLabel));
            BoundStatement boundBody = BindStatement(body);
            loopStack.Pop();

            return boundBody;
        }

        private BoundIfStatement BindIfStatement(IfStatementNode node)
        {
            BoundExpression condition = BindExpression(node.Condition, TypeSymbol.Bool);
            scope = new BoundScope(scope);
            BoundStatement ifStatement = BindStatement(node.Body);
            scope = scope.Parent!;

            //The else is optional
            BoundStatement? elseStatement = node.ElseClause == null ? null : BindStatement(node.ElseClause.Statement);

            return new BoundIfStatement(condition, ifStatement, elseStatement);
        }

        private BoundVariableDeclarationStatement BindVariableDeclarationStatement(VariableDeclarationStatementNode node,
            bool isReadOnly = false)
        {
            VariableSymbol variable;
            BoundExpression? initializer = null;
            if (node.Keyword.Type == TokenType.Type)
            {
                TypeSymbol type = TypeSymbol.Lookup(node.Keyword.Text!)!;
                variable = BindVariable(node.Identifier, isReadOnly, type);
                if (node.Initializer != null)
                    initializer = BindExpression(node.Initializer, type);
            }
            else
            {
                initializer = BindExpression(node.Initializer!);
                variable = BindVariable(node.Identifier, isReadOnly, initializer!.Type);
            }

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
            BoundExpression expression = BindExpression(node.Expression, true);
            return new BoundExpressionStatement(expression);
        }
        #endregion Statements

        #region Expressions
        private BoundExpression BindExpression(ExpressionNode node, bool canBeVoid = false)
        {
            BoundExpression result = BindExpressionInternal(node);
            if (!canBeVoid && result.Type == TypeSymbol.Void)
            {
                diagnostics.ReportNullExpression(node.Span);
                return new BoundErrorExpression();
            }
            return result;
        }

        private BoundExpression BindExpression(ExpressionNode node, TypeSymbol expectedType)
        {
            BoundExpression boundExpression = BindExpression(node);
            if (boundExpression.Type == TypeSymbol.Error || expectedType == TypeSymbol.Error)
                return new BoundErrorExpression();

            return BindConversion(node, expectedType);
        }

        private BoundExpression BindExpressionInternal(ExpressionNode node)
        {
            switch (node.Type)
            {
                case NodeType.ParenthesizedExpression:
                    return BindExpression(((ParenthesizedExpressionNode)node).Expression);
                case NodeType.LiteralExpression:
                    return BindLiteralExpression((LiteralExpressionNode)node);
                case NodeType.NameExpression:
                    return BindVariableExpression((VariableExpressionNode)node);
                case NodeType.AssignmentExpression:
                    return BindAssignmentExpression((AssignmentExpressionNode)node);
                case NodeType.CallExpression:
                    return BindCallExpression((CallExpressionNode)node);
                case NodeType.UnaryExpression:
                    return BindUnaryExpression((UnaryExpressionNode)node);
                case NodeType.BinaryExpression:
                    return BindBinaryExpression((BinaryExpressionNode)node);
                default:
                    diagnostics.ReportUnexpectedNode(node.Span, node.Type);
                    return new BoundErrorExpression();
            }
        }

        private BoundLiteralExpression BindLiteralExpression(LiteralExpressionNode node)
        {
            object value = node.Value;
            return new BoundLiteralExpression(value);
        }

        private BoundExpression BindVariableExpression(VariableExpressionNode node)
        {
            string? name = node.Identifier.Text;
            if (node.Identifier.IsFake)
                return new BoundErrorExpression();

            if (!scope.TryLookupVariable(name!, out VariableSymbol? variable))
            {
                diagnostics.ReportUndefinedVariable(node.Identifier.Span, name!);
                return new BoundErrorExpression();
            }

            return new BoundVariableExpression(variable!);
        }

        private BoundExpression BindAssignmentExpression(AssignmentExpressionNode node)
        {
            string name = node.Identifier.Text!;

            if (!scope.TryLookupVariable(name, out VariableSymbol? variable))
            {
                diagnostics.ReportUndefinedVariable(node.Identifier.Span, name);
                return new BoundErrorExpression();
            }

            BoundExpression boundExpression = BindExpression(node.Expression, variable!.Type);

            if (boundExpression.Type == TypeSymbol.Error || variable!.Type == TypeSymbol.Error)
                return new BoundErrorExpression();

            if (variable.IsReadOnly)
            {
                diagnostics.ReportCannotAssign(node.Identifier.Span, name);
                return new BoundErrorExpression();
            }

            return new BoundAssignmentExpression(variable, boundExpression);
        }

        private BoundExpression BindCallExpression(CallExpressionNode node)
        {
            string name = node.Identifier.Text!;

            if (node.Arguments.Count == 1 && node.Identifier.Type == TokenType.Type)
                return BindConversion(node.Arguments[0], TypeSymbol.Lookup(name)!, true);
            else
            {
                if (!scope.TryLookupFunction(name, out FunctionSymbol? function))
                {
                    diagnostics.ReportUndefinedFunction(node.Identifier.Span, name);
                    return new BoundErrorExpression();
                }
                if (node.Arguments.Count != function!.Parameters.Length)
                {
                    TextSpan span;

                    if (node.Arguments.Count > function!.Parameters.Length)
                    {
                        TextSpan firstExceedingSpan;
                        if (function.Parameters.Length > 0)
                            firstExceedingSpan = node.Arguments.GetSeparator(function.Parameters.Length - 1).Span;
                        else
                            firstExceedingSpan = node.Arguments[0].Span;

                        TextSpan lastExceedingSpan = node.Arguments[node.Arguments.Count - 1].Span;
                        span = TextSpan.FromBounds(firstExceedingSpan.Start, lastExceedingSpan.End);
                    } else
                        span = node.CloseParenthesis.Span;

                    diagnostics.ReportWrongArgumentCount(span, function.Name, node.Arguments.Count);
                    return new BoundErrorExpression();
                }

                ImmutableArray<BoundExpression> arguments = BindArguments(node.Arguments, function!.Parameters);

                //If there's an error in the arguments, we return an error expression
                if (arguments.Any(a => a.Type == TypeSymbol.Error))
                    return new BoundErrorExpression();

                return new BoundCallExpression(function, arguments);
            }
        }

        private ImmutableArray<BoundExpression> BindArguments(SeparatedNodeList<ExpressionNode> arguments, ImmutableArray<ParameterSymbol> parameters)
        {
            ImmutableArray<BoundExpression>.Builder boundArguments = ImmutableArray.CreateBuilder<BoundExpression>();

            for (int i = 0; i < arguments.Count; i++)
            {
                boundArguments.Add(BindExpression(arguments[i], parameters[i].Type));
            }

            return boundArguments.ToImmutable();
        }

        private BoundExpression BindConversion(ExpressionNode node, TypeSymbol toType, bool allowExplicit = false)
        {
            BoundExpression expression = BindExpression(node);

            if (expression.Type == TypeSymbol.Error || toType == TypeSymbol.Error)
                return new BoundErrorExpression();

            Conversion conversion = Conversion.Classify(expression.Type, toType);

            if (!conversion.Exists || (!allowExplicit && !conversion.IsImplicit))
            {
                diagnostics.ReportCannotConvert(node.Span, expression.Type, toType);
                return new BoundErrorExpression();
            }

            if (conversion.IsIdentity)
                return expression;

            return new BoundConversionExpression(toType, expression);
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

        private FunctionSymbol BindFunction(Token identifier, ImmutableArray<ParameterSymbol> parameters,
            TypeSymbol returnType, FunctionDeclarationNode declaration)
        {
            string name = identifier.Text!;
            FunctionSymbol function = new FunctionSymbol(name, parameters, returnType, declaration);

            if (returnType != TypeSymbol.Error && !identifier.IsFake && !scope.TryDeclareFunction(function))
                diagnostics.ReportSymbolAlreadyDeclared(identifier.Span, name);

            return function;
        }
        private VariableSymbol BindVariable(Token identifier, bool isReadOnly, TypeSymbol type)
        {
            string name = identifier.Text!;
            VariableSymbol variable = function == null
                ? new GlobalVariableSymbol(name, isReadOnly, type)
                : new LocalVariableSymbol(name, isReadOnly, type);

            if (type != TypeSymbol.Error && !identifier.IsFake && !scope.TryDeclareVariable(variable))
                diagnostics.ReportSymbolAlreadyDeclared(identifier.Span, name);

            return variable;
        }
    }
}
