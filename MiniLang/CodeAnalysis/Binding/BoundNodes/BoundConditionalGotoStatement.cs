﻿namespace MiniLang.CodeAnalysis.Binding.BoundNodes
{
    internal sealed class BoundConditionalGotoStatement : BoundStatement
    {
        public BoundConditionalGotoStatement(BoundLabel label, BoundExpression condition, bool jumpIfTrue = true)
        {
            Label = label;
            Condition = condition;
            JumpIfTrue = jumpIfTrue;
        }

        public BoundLabel Label { get; }
        public BoundExpression Condition { get; }
        public bool JumpIfTrue { get; }

        public override BoundNodeType BoundNodeType => BoundNodeType.ConditionalGotoStatement;

        public override IEnumerable<BoundNode> GetChildren()
        {
            yield return Condition;
        }
    }
}
