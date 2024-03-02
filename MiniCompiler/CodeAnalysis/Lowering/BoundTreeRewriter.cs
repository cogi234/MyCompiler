using MiniCompiler.CodeAnalysis.Binding.BoundNodes;
using System.Collections.Immutable;

namespace MiniCompiler.CodeAnalysis.Lowering
{
    internal abstract class BoundTreeRewriter
    {
        #region Statements
        public virtual BoundStatement RewriteStatement(BoundStatement node)
        {
            switch (node.BoundNodeType)
            {
                case BoundNodeType.BlockStatement:
                    return RewriteBlockStatement((BoundBlockStatement)node);
                case BoundNodeType.ExpressionStatement:
                    return RewriteExpressionStatement((BoundExpressionStatement)node);
                case BoundNodeType.VariableDeclarationStatement:
                    return RewriteVariableDeclarationStatement((BoundVariableDeclarationStatement)node);
                case BoundNodeType.LabelStatement:
                    return RewriteLabelStatement((BoundLabelStatement)node);
                case BoundNodeType.GotoStatement:
                    return RewriteGotoStatement((BoundGotoStatement)node);
                case BoundNodeType.ConditionalGotoStatement:
                    return RewriteConditionalGotoStatement((BoundConditionalGotoStatement)node);
                case BoundNodeType.IfStatement:
                    return RewriteIfStatement((BoundIfStatement)node);
                case BoundNodeType.WhileStatement:
                    return RewriteWhileStatement((BoundWhileStatement)node);
                case BoundNodeType.ForStatement:
                    return RewriteForStatement((BoundForStatement)node);
                default:
                    return node;
            }
        }

        protected virtual BoundStatement RewriteForStatement(BoundForStatement node)
        {
            BoundVariableDeclarationStatement? declaration = null;
            if (node.Declaration != null)
                declaration = (BoundVariableDeclarationStatement)RewriteVariableDeclarationStatement(node.Declaration);
            BoundExpression condition = RewriteExpression(node.Condition);
            BoundAssignmentExpression? increment = null;
            if (node.Increment != null)
                increment = (BoundAssignmentExpression)RewriteAssignmentExpression(node.Increment);
            BoundStatement body = RewriteStatement(node.Body);

            if (declaration == node.Declaration && condition == node.Condition &&
                increment == node.Increment && body == node.Body)
                return node;

            return new BoundForStatement(declaration, condition, increment, body);
        }

        protected virtual BoundStatement RewriteWhileStatement(BoundWhileStatement node)
        {
            BoundExpression condition = RewriteExpression(node.Condition);
            BoundStatement body = RewriteStatement(node.Body);

            if (condition == node.Condition && body == node.Body)
                return node;

            return new BoundWhileStatement(condition, body);
        }

        protected virtual BoundStatement RewriteIfStatement(BoundIfStatement node)
        {
            BoundExpression condition = RewriteExpression(node.Condition);
            BoundStatement thenStatement = RewriteStatement(node.ThenStatement);
            BoundStatement? elseStatement = null;
            if (node.ElseStatement != null)
                elseStatement = RewriteStatement(node.ElseStatement);

            if (condition == node.Condition && thenStatement == node.ThenStatement && elseStatement == node.ElseStatement)
                return node;

            return new BoundIfStatement(condition, thenStatement, elseStatement);
        }

        protected virtual BoundStatement RewriteConditionalGotoStatement(BoundConditionalGotoStatement node)
        {
            BoundExpression condition = RewriteExpression(node.Condition);

            if (condition == node.Condition)
                return node;

            return new BoundConditionalGotoStatement(node.Label, condition, node.JumpIfTrue);
        }

        protected virtual BoundStatement RewriteGotoStatement(BoundGotoStatement node)
        {
            return node;
        }

        protected virtual BoundStatement RewriteLabelStatement(BoundLabelStatement node)
        {
            return node;
        }

        protected virtual BoundStatement RewriteVariableDeclarationStatement(BoundVariableDeclarationStatement node)
        {
            if (node.Initializer != null)
            {
                BoundExpression initializer = RewriteExpression(node.Initializer);

                if (initializer == node.Initializer)
                    return node;

                return new BoundVariableDeclarationStatement(node.Variable, initializer);
            }

            return node;
        }

        protected virtual BoundStatement RewriteExpressionStatement(BoundExpressionStatement node)
        {
            BoundExpression expression = RewriteExpression(node.Expression);

            if (expression == node.Expression)
                return node;

            return new BoundExpressionStatement(expression);
        }

        protected virtual BoundStatement RewriteBlockStatement(BoundBlockStatement node)
        {
            ImmutableArray<BoundStatement>.Builder? builder = null;

            for (int i = 0; i < node.Statements.Length; i++)
            {
                BoundStatement oldStatement = node.Statements[i];
                BoundStatement newStatement = RewriteStatement(oldStatement);
                if (oldStatement != newStatement)
                {
                    if (builder == null)
                    {
                        builder = ImmutableArray.CreateBuilder<BoundStatement>(node.Statements.Length);

                        for (int j = 0; j < i; j++)
                            builder.Add(node.Statements[j]);
                    }
                }

                if (builder != null)
                    builder.Add(newStatement);
            }

            if (builder == null)
                return node;

            return new BoundBlockStatement(builder.MoveToImmutable());
        }
        #endregion Statements

        #region Expressions
        public virtual BoundExpression RewriteExpression(BoundExpression node)
        {
            switch (node.BoundNodeType)
            {
                case BoundNodeType.ErrorExpression:
                    return RewriteErrorExpression((BoundErrorExpression)node);
                case BoundNodeType.LiteralExpression:
                    return RewriteLiteralExpression((BoundLiteralExpression)node);
                case BoundNodeType.VariableExpression:
                    return RewriteVariableExpression((BoundVariableExpression)node);
                case BoundNodeType.AssignmentExpression:
                    return RewriteAssignmentExpression((BoundAssignmentExpression)node);
                case BoundNodeType.CallExpression:
                    return RewriteCallExpression((BoundCallExpression)node);
                case BoundNodeType.ConversionExpression:
                    return RewriteConversionExpression((BoundConversionExpression)node);
                case BoundNodeType.UnaryExpression:
                    return RewriteUnaryExpression((BoundUnaryExpression)node);
                case BoundNodeType.BinaryExpression:
                    return RewriteBinaryExpression((BoundBinaryExpression)node);
                default:
                    return node;
            }
        }

        protected virtual BoundExpression RewriteBinaryExpression(BoundBinaryExpression node)
        {
            BoundExpression left = RewriteExpression(node.Left);
            BoundExpression right = RewriteExpression(node.Right);

            if (left == node.Left && right == node.Right)
                return node;

            return new BoundBinaryExpression(left, node.BinaryOperator, right);
        }

        protected virtual BoundExpression RewriteUnaryExpression(BoundUnaryExpression node)
        {
            BoundExpression operand = RewriteExpression(node.Operand);

            if (operand == node.Operand)
                return node;

            return new BoundUnaryExpression(node.UnaryOperator, operand);
        }

        protected virtual BoundExpression RewriteCallExpression(BoundCallExpression node)
        {
            ImmutableArray<BoundExpression>.Builder? builder = null;

            for (int i = 0; i < node.Arguments.Length; i++)
            {
                BoundExpression oldExpression = node.Arguments[i];
                BoundExpression newExpression = RewriteExpression(oldExpression);
                if (oldExpression != newExpression)
                {
                    if (builder == null)
                    {
                        builder = ImmutableArray.CreateBuilder<BoundExpression>(node.Arguments.Length);

                        for (int j = 0; j < i; j++)
                            builder.Add(node.Arguments[j]);
                    }
                }

                if (builder != null)
                    builder.Add(newExpression);
            }

            if (builder == null)
                return node;

            return new BoundCallExpression(node.Function, builder.ToImmutable());
        }

        protected BoundExpression RewriteConversionExpression(BoundConversionExpression node)
        {
            BoundExpression expression = RewriteExpression(node.Expression);

            if (expression == node.Expression)
                return node;

            return new BoundConversionExpression(node.Type, expression);
        }

        protected virtual BoundExpression RewriteAssignmentExpression(BoundAssignmentExpression node)
        {
            BoundExpression expression = RewriteExpression(node.Expression);

            if (expression == node.Expression)
                return node;

            return new BoundAssignmentExpression(node.Variable, expression);
        }
        protected virtual BoundExpression RewriteVariableExpression(BoundVariableExpression node)
        {
            return node;
        }

        protected virtual BoundExpression RewriteLiteralExpression(BoundLiteralExpression node)
        {
            return node;
        }

        protected virtual BoundExpression RewriteErrorExpression(BoundErrorExpression node)
        {
            return node;
        }
        #endregion Expressions
    }
}
