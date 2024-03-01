using MiniCompiler.CodeAnalysis.Symbols;
using MiniCompiler.CodeAnalysis.Syntax;

namespace MiniCompiler.CodeAnalysis.Binding.BoundNodes
{
    internal sealed class BoundUnaryOperator
    {
        private BoundUnaryOperator(BoundUnaryOperationType operationType, TokenType tokenType, TypeSymbol operandType, TypeSymbol resultType)
        {
            OperationType = operationType;
            TokenType = tokenType;
            OperandType = operandType;
            ResultType = resultType;
        }

        private BoundUnaryOperator(BoundUnaryOperationType operationType, TokenType tokenType, TypeSymbol operandType)
            : this(operationType, tokenType, operandType, operandType)
        {
        }

        public BoundUnaryOperationType OperationType { get; }
        public TokenType TokenType { get; }
        public TypeSymbol OperandType { get; }
        public TypeSymbol ResultType { get; }

        private static BoundUnaryOperator[] operators =
        {
            //Boolean
            new BoundUnaryOperator(BoundUnaryOperationType.LogicalNegation, TokenType.Bang, TypeSymbol.Bool),
            //Number
            new BoundUnaryOperator(BoundUnaryOperationType.Identity, TokenType.Plus, TypeSymbol.Int),
            new BoundUnaryOperator(BoundUnaryOperationType.Negation, TokenType.Minus, TypeSymbol.Int),
            new BoundUnaryOperator(BoundUnaryOperationType.BitwiseNegation, TokenType.Tilde, TypeSymbol.Int),
        };

        public static BoundUnaryOperator? Bind(TokenType tokenType, TypeSymbol operandType)
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
