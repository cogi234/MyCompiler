using MiniCompiler.CodeAnalysis.Binding.BoundNodes;
using System.Collections.Immutable;

namespace MiniCompiler.CodeAnalysis.Lowering
{
    internal sealed class Lowerer : BoundTreeRewriter
    {
        private Lowerer()
        {
        }

        public static BoundStatement Lower(BoundStatement statement)
        {
            var lowerer = new Lowerer();
            return lowerer.RewriteStatement(statement);
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
                whileStatement = new BoundWhileStatement(node.Condition, node.Body);
            else
            {
                BoundBlockStatement whileBody = new BoundBlockStatement([node.Body, new BoundExpressionStatement(node.Increment)]);
                whileStatement = new BoundWhileStatement(node.Condition, whileBody);
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
    }
}
