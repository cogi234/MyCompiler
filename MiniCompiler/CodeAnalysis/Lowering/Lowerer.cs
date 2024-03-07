using MiniCompiler.CodeAnalysis.Binding.BoundNodes;
using System.Collections.Immutable;

namespace MiniCompiler.CodeAnalysis.Lowering
{
    internal sealed class Lowerer : BoundTreeRewriter
    {
        private int labelCount = 0;
        private Lowerer()
        {
        }

        public static BoundBlockStatement Lower(BoundStatement statement)
        {
            Lowerer lowerer = new Lowerer();
            BoundStatement result = lowerer.RewriteStatement(statement);
            return Flatten(result);
        }

        private static BoundBlockStatement Flatten(BoundStatement statement)
        {
            ImmutableArray<BoundStatement>.Builder builder = ImmutableArray.CreateBuilder<BoundStatement>();
            Stack<BoundStatement> stack = new Stack<BoundStatement>();
            stack.Push(statement);

            while (stack.Count > 0)
            {
                BoundStatement current = stack.Pop();

                if (current.BoundNodeType == BoundNodeType.BlockStatement)
                {
                    BoundBlockStatement block = (BoundBlockStatement)current;
                    foreach (BoundStatement s in block.Statements.Reverse())
                        stack.Push(s);
                }
                else
                    builder.Add(current);
            }

            return new BoundBlockStatement(builder.ToImmutable());
        }

        protected override BoundStatement RewriteIfStatement(BoundIfStatement node)
        {
            ImmutableArray<BoundStatement>.Builder builder = ImmutableArray.CreateBuilder<BoundStatement>();
            if (node.ElseStatement == null)
            {
                BoundLabel endLabel = GenerateLabel();
                //gotoFalse <condition> end
                builder.Add(new BoundConditionalGotoStatement(endLabel, node.Condition, false));
                //then
                builder.Add(node.ThenStatement);
                //end:
                builder.Add(new BoundLabelStatement(endLabel));
            }
            else
            {
                BoundLabel elseLabel = GenerateLabel();
                BoundLabel endLabel = GenerateLabel();
                //gotoFalse <condition> else
                builder.Add(new BoundConditionalGotoStatement(elseLabel, node.Condition, false));
                //then
                builder.Add(node.ThenStatement);
                //goto end
                builder.Add(new BoundGotoStatement(endLabel));
                //else
                builder.Add(new BoundLabelStatement(elseLabel));
                builder.Add(node.ElseStatement);
                //end:
                builder.Add(new BoundLabelStatement(endLabel));
            }

            return new BoundBlockStatement(builder.ToImmutable());
        }

        protected override BoundStatement RewriteWhileStatement(BoundWhileStatement node)
        {
            ImmutableArray<BoundStatement>.Builder builder = ImmutableArray.CreateBuilder<BoundStatement>();
            BoundLabel bodyLabel = GenerateLabel();
            BoundLabel continueLabel = node.ContinueLabel;
            BoundLabel breakLabel = node.BreakLabel;
            //goto continue
            builder.Add(new BoundGotoStatement(continueLabel));
            //body:
            builder.Add(new BoundLabelStatement(bodyLabel));
            //body
            builder.Add(node.Body);
            //continue:
            builder.Add(new BoundLabelStatement(continueLabel));
            //gotoTrue <condition> continue
            builder.Add(new BoundConditionalGotoStatement(bodyLabel, node.Condition, true));
            //break:
            builder.Add(new BoundLabelStatement(breakLabel));

            return RewriteStatement(new BoundBlockStatement(builder.ToImmutable()));
        }

        protected override BoundStatement RewriteDoWhileStatement(BoundDoWhileStatement node)
        {
            ImmutableArray<BoundStatement>.Builder builder = ImmutableArray.CreateBuilder<BoundStatement>();
            BoundLabel bodyLabel = GenerateLabel();
            BoundLabel continueLabel = node.ContinueLabel;
            BoundLabel breakLabel = node.BreakLabel;
            //body:
            builder.Add(new BoundLabelStatement(bodyLabel));
            //body
            builder.Add(node.Body);
            //continue:
            builder.Add(new BoundLabelStatement(continueLabel));
            //gotoTrue <condition> continue
            builder.Add(new BoundConditionalGotoStatement(bodyLabel, node.Condition, true));
            //break:
            builder.Add(new BoundLabelStatement(breakLabel));

            return RewriteStatement(new BoundBlockStatement(builder.ToImmutable()));
        }

        protected override BoundStatement RewriteForStatement(BoundForStatement node)
        {
            // for (<declaration> <condition> <increment>)
            //      <body>
            // ---->
            // {
            //      <declaration>
            //      while (<condition>)
            //      {
            //          <body>
            //          <increment>
            //      }
            // }

            //Generate the while statement
            BoundWhileStatement whileStatement;
            if (node.Increment == null)
                whileStatement = new BoundWhileStatement(node.Condition, node.Body, node.BreakLabel, node.ContinueLabel);
            else
            {
                BoundBlockStatement whileBody;
                //If the body is already a block, we just add the increment at the end of the block
                if (node.Body.BoundNodeType == BoundNodeType.BlockStatement)
                    whileBody = new BoundBlockStatement(((BoundBlockStatement)node.Body).Statements.
                        Concat([new BoundLabelStatement(node.ContinueLabel), new BoundExpressionStatement(node.Increment)]).ToImmutableArray());
                else
                    whileBody = new BoundBlockStatement([node.Body, new BoundLabelStatement(node.ContinueLabel),
                        new BoundExpressionStatement(node.Increment)]);
                whileStatement = new BoundWhileStatement(node.Condition, whileBody, node.BreakLabel, GenerateLabel());
            }

            //Generate an enclosing block for the declaration if there is one
            BoundStatement result;
            if (node.Declaration == null)
                result = whileStatement;
            else
            {
                BoundBlockStatement enclosingBlock = new BoundBlockStatement([node.Declaration, whileStatement]);
                result = enclosingBlock;
            }

            return RewriteStatement(result);
        }

        private BoundLabel GenerateLabel()
        {
            string name = $"label{labelCount}";
            labelCount++;
            return new BoundLabel(name);
        }
    }
}
