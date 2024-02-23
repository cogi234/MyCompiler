using MiniCompiler.CodeAnalysis.Syntax;

namespace MiniCompiler.CodeAnalysis.Binding.BoundNodes
{
    internal sealed class BoundUnaryOperator
    {
        private BoundUnaryOperator(BoundUnaryOperationType operationType, TokenType tokenType, Type operandType, Type resultType)
        {
            OperationType = operationType;
            TokenType = tokenType;
            OperandType = operandType;
            ResultType = resultType;
        }

        private BoundUnaryOperator(BoundUnaryOperationType operationType, TokenType tokenType, Type operandType)
            : this(operationType, tokenType, operandType, operandType)
        {
        }

        public BoundUnaryOperationType OperationType { get; }
        public TokenType TokenType { get; }
        public Type OperandType { get; }
        public Type ResultType { get; }

        private static BoundUnaryOperator[] operators =
        {
            //Boolean
            new BoundUnaryOperator(BoundUnaryOperationType.LogicalNegation, TokenType.Bang, typeof(bool)),
            //Number
            new BoundUnaryOperator(BoundUnaryOperationType.Identity, TokenType.Plus, typeof(int)),
            new BoundUnaryOperator(BoundUnaryOperationType.Negation, TokenType.Minus, typeof(int)),
        };

        public static BoundUnaryOperator? Bind(TokenType tokenType, Type operandType)
        {
            foreach (BoundUnaryOperator op in operators)
            {
                if (op.TokenType == tokenType && op.OperandType == operandType)
                    return op;
            }
            return null;
        }
    }
}
