using MiniCompiler.CodeAnalysis.Binding.BoundNodes;
using MiniCompiler.CodeAnalysis.Symbols;
using MiniCompiler.CodeAnalysis.Syntax;
using MiniCompiler.IO;
using System.CodeDom.Compiler;

namespace MiniCompiler.CodeAnalysis.Binding
{
    internal sealed class ControlFlowGraph
    {
        public ControlFlowGraph(BasicBlock start, BasicBlock end, List<BasicBlock> blocks, List<BasicBlockBranch> branches)
        {
            Start = start;
            End = end;
            Blocks = blocks;
            Branches = branches;
        }

        public BasicBlock Start { get; }
        public BasicBlock End { get; }
        public List<BasicBlock> Blocks { get; }
        public List<BasicBlockBranch> Branches { get; }

        public void WriteTo(TextWriter writer)
        {
            string Quote(string text)
            {
                return "\"" + text.TrimEnd().Replace("\\", "\\\\").Replace("\"", "\\\"").
                    Replace(Environment.NewLine, "\\l") + "\"";
            }
            Dictionary<BasicBlock, string> blockIds = new Dictionary<BasicBlock, string>();

            writer.WriteLine("digraph G {");

            for (int i = 0; i < Blocks.Count; i++)
            {
                string id = $"N{i}";
                blockIds.Add(Blocks[i], id);
            }

            foreach (BasicBlock block in Blocks)
            {
                string id = blockIds[block];
                string label = Quote(block.ToString());
                writer.WriteLine($"    {id} [label = {label} shape = box]");
            }

            foreach (BasicBlockBranch branch in Branches)
            {
                string fromId = blockIds[branch.From];
                string toId = blockIds[branch.To];
                string label = Quote(branch.ToString());
                writer.WriteLine($"    {fromId} -> {toId} [label = {label}]");
            }

            writer.WriteLine("}");
        }

        public static ControlFlowGraph Create(BoundBlockStatement body)
        {
            BasicBlockBuilder basicBlockBuilder = new BasicBlockBuilder();
            List<BasicBlock> blocks = basicBlockBuilder.Build(body);
            GraphBuilder graphBuilder = new GraphBuilder();
            return graphBuilder.Build(blocks);
        }

        public static bool AllPathsReturn(BoundBlockStatement body)
        {
            ControlFlowGraph graph = Create(body);

            foreach (BasicBlockBranch branch in graph.End.Incoming)
            {
                BoundStatement? lastStatement = branch.From.Statements.LastOrDefault();
                if (lastStatement == null || lastStatement.BoundNodeType != BoundNodeType.ReturnStatement)
                    return false;
            }

            return true;
        }

        public sealed class BasicBlock
        {
            public BasicBlock()
            {
            }
            public BasicBlock(bool isStart)
            {
                IsStart = isStart;
                IsEnd = !isStart;
            }

            public bool IsStart { get; }
            public bool IsEnd { get; }
            public List<BoundStatement> Statements { get; } = new List<BoundStatement>();
            public List<BasicBlockBranch> Incoming { get; } = new List<BasicBlockBranch>();
            public List<BasicBlockBranch> Outgoing { get; } = new List<BasicBlockBranch>();

            public override string ToString()
            {
                if (IsStart)
                    return "<Start>";

                if (IsEnd)
                    return "<End>";

                using (StringWriter writer = new StringWriter())
                using (IndentedTextWriter indentedWriter = new IndentedTextWriter(writer))
                {
                    foreach (BoundStatement statement in Statements)
                        statement.WriteTo(indentedWriter);

                    return writer.ToString();
                }
            }
        }
        public sealed class BasicBlockBranch
        {
            public BasicBlockBranch(BasicBlock from, BasicBlock to, BoundExpression? condition)
            {
                From = from;
                To = to;
                Condition = condition;
            }

            public BasicBlock From { get; }
            public BasicBlock To { get; }
            public BoundExpression? Condition { get; }

            public override string ToString()
            {
                if (Condition == null)
                    return string.Empty;

                return Condition.ToString();
            }
        }
        public sealed class BasicBlockBuilder
        {
            private List<BoundStatement> statements = new List<BoundStatement>();
            private List<BasicBlock> blocks = new List<BasicBlock>();

            public List<BasicBlock> Build(BoundBlockStatement block)
            {
                foreach (BoundStatement statement in block.Statements)
                {
                    switch (statement.BoundNodeType)
                    {
                        case BoundNodeType.LabelStatement:
                            StartBLock();
                            statements.Add(statement);
                            break;
                        case BoundNodeType.GotoStatement:
                        case BoundNodeType.ConditionalGotoStatement:
                        case BoundNodeType.ReturnStatement:
                            statements.Add(statement);
                            StartBLock();
                            break;
                        case BoundNodeType.VariableDeclarationStatement:
                        case BoundNodeType.ExpressionStatement:
                            statements.Add(statement);
                            break;
                        default:
                            throw new Exception($"Unexpected statement: {statement.BoundNodeType}");
                    }
                }

                EndBlock();

                return blocks.ToList();
            }

            private void StartBLock()
            {
                EndBlock();
            }

            private void EndBlock()
            {
                if (statements.Count > 0)
                {
                    BasicBlock block = new BasicBlock();
                    block.Statements.AddRange(statements);
                    blocks.Add(block);
                    statements.Clear();
                }
            }
        }
        public sealed class GraphBuilder
        {
            private Dictionary<BoundStatement, BasicBlock> blockFromStatement = new Dictionary<BoundStatement, BasicBlock>();
            private Dictionary<BoundLabel, BasicBlock> blockFromLabel = new Dictionary<BoundLabel, BasicBlock>();
            private List<BasicBlockBranch> branches = new List<BasicBlockBranch>();
            private BasicBlock start = new BasicBlock(true);
            private BasicBlock end = new BasicBlock(false);

            public ControlFlowGraph Build(List<BasicBlock> blocks)
            {
                if (!blocks.Any())
                    Connect(start, end);
                else
                    Connect(start, blocks.First());

                foreach (BasicBlock block in blocks)
                {
                    foreach (BoundStatement statement in block.Statements)
                    {
                        blockFromStatement.Add(statement, block);
                        if (statement is BoundLabelStatement labelStatement)
                            blockFromLabel.Add(labelStatement.Label, block);
                    }
                }

                for (int i = 0; i < blocks.Count; i++)
                {
                    BasicBlock current = blocks[i];
                    BasicBlock next = i == blocks.Count - 1 ? end : blocks[i + 1];

                    foreach (BoundStatement statement in current.Statements)
                    {
                        bool isLastStatementInBlock = statement == current.Statements.Last();
                        switch (statement.BoundNodeType)
                        {
                            case BoundNodeType.GotoStatement:
                                BoundGotoStatement gs = (BoundGotoStatement)statement;
                                BasicBlock toBlock = blockFromLabel[gs.Label];
                                Connect(current, toBlock);
                                break;
                            case BoundNodeType.ConditionalGotoStatement:
                                BoundConditionalGotoStatement cgs = (BoundConditionalGotoStatement)statement;
                                BasicBlock thenBlock = blockFromLabel[cgs.Label];
                                BasicBlock elseBlock = next;
                                BoundExpression negatedCondition = Negate(cgs.Condition);
                                BoundExpression thenCondition = cgs.JumpIfTrue ? cgs.Condition : negatedCondition;
                                BoundExpression elseCondition = cgs.JumpIfTrue ? negatedCondition : cgs.Condition;
                                Connect(current, thenBlock, thenCondition);
                                Connect(current, elseBlock, elseCondition);
                                break;
                            case BoundNodeType.ReturnStatement:
                                Connect(current, end);
                                break;
                            case BoundNodeType.VariableDeclarationStatement:
                            case BoundNodeType.LabelStatement:
                            case BoundNodeType.ExpressionStatement:
                                if (isLastStatementInBlock)
                                    Connect(current, next);
                                break;
                            default:
                                throw new Exception($"Unexpected statement: {statement.BoundNodeType}");
                        }
                    }
                }

            ScanAgain:
                foreach (BasicBlock block in blocks)
                {
                    if (!block.Incoming.Any())
                    {
                        RemoveBlock(blocks, block);
                        goto ScanAgain;
                    }
                }

                blocks.Insert(0, start);
                blocks.Add(end);

                return new ControlFlowGraph(start, end, blocks, branches);
            }

            private void Connect(BasicBlock from, BasicBlock to, BoundExpression? condition = null)
            {
                if (condition is BoundLiteralExpression l)
                {
                    bool value = (bool)l.Value;
                    if (value)
                        condition = null;
                    else
                        return;
                }

                BasicBlockBranch branch = new BasicBlockBranch(from, to, condition);
                from.Outgoing.Add(branch);
                to.Incoming.Add(branch);
                branches.Add(branch);
            }

            private void RemoveBlock(List<BasicBlock> blocks, BasicBlock block)
            {
                foreach (BasicBlockBranch branch in block.Incoming)
                {
                    branch.From.Outgoing.Remove(branch);
                    branches.Remove(branch);
                }

                foreach (BasicBlockBranch branch in block.Outgoing)
                {
                    branch.To.Incoming.Remove(branch);
                    branches.Remove(branch);
                }

                blocks.Remove(block);
            }

            private BoundExpression Negate(BoundExpression condition)
            {
                if (condition is BoundLiteralExpression literal)
                {
                    bool value = (bool)literal.Value;
                    return new BoundLiteralExpression(!value);
                }

                BoundUnaryOperator op = BoundUnaryOperator.Bind(TokenType.Bang, TypeSymbol.Bool)!;
                return new BoundUnaryExpression(op, condition);
                ;
            }
        }
    }
}
