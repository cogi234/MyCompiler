using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompiler.CodeAnalysis.Syntax
{
    internal sealed class Parser
    {
        private readonly Token[] tokens;
        private int position;
        private List<string> diagnostics = new List<string>();
        public IEnumerable<string> Diagnostics => diagnostics;

        public Parser(string line)
        {
            List<Token> tokens = new List<Token>();
            Lexer lexer = new Lexer(line);
            Token token;
            do
            {
                token = lexer.NextToken();

                if (token.Type != TokenType.WhiteSpace &&
                    token.Type != TokenType.BadToken)
                {
                    tokens.Add(token);
                }
            } while (token.Type != TokenType.EndOfFile);

            this.tokens = tokens.ToArray();
            diagnostics.AddRange(lexer.Diagnostics);
        }

        public SyntaxTree Parse()
        {
            ExpressionNode expression = ParseExpression();
            MatchToken(TokenType.EndOfFile);
            return new SyntaxTree(diagnostics, expression);
        }

        private ExpressionNode ParseExpression(int parentPrecedence = 0)
        {
            ExpressionNode left;

            //Unary expressions
            int unaryOperatorPrecedence = Current.Type.GetUnaryOperatorPrecedence();
            if (unaryOperatorPrecedence != 0 && unaryOperatorPrecedence >= parentPrecedence)
            {
                Token operatorToken = NextToken();
                ExpressionNode expression = ParseExpression(unaryOperatorPrecedence);
                left = new UnaryExpressionNode(operatorToken, expression);
            }
            else
            {
                left = ParsePrimaryExpression();
            }

            //Binary expressions
            while (true)
            {
                int precedence = Current.Type.GetBinaryOperatorPrecedence();
                if (precedence == 0 || precedence <= parentPrecedence)
                    break;

                Token operatorToken = NextToken();
                ExpressionNode right = ParseExpression(precedence);
                left = new BinaryExpressionNode(left, operatorToken, right);
            }

            return left;
        }

        private ExpressionNode ParsePrimaryExpression()
        {
            switch (Current.Type)
            {
                case TokenType.OpenParenthesis:
                    return new ParenthesizedExpressionNode(NextToken(), ParseExpression(), MatchToken(TokenType.CloseParenthesis));
                case TokenType.TrueKeyword:
                    return new LiteralExpressionNode(NextToken(), true);
                case TokenType.FalseKeyword:
                    return new LiteralExpressionNode(NextToken(), false);
                default:
                    {
                        Token numberToken = MatchToken(TokenType.Number);
                        return new LiteralExpressionNode(numberToken, numberToken.Value);
                    }
            }
        }

        private Token Peek(int offset)
        {
            int index = position + offset;
            if (index >= tokens.Length)
                return tokens[tokens.Length - 1];
            return tokens[index];
        }
        private Token Current => Peek(0);
        private Token NextToken()
        {
            position++;
            return Peek(-1);
        }
        private Token MatchToken(TokenType type)
        {
            if (Current.Type == type)
                return NextToken();

            diagnostics.Add($"ERROR ({Current.Position}): Unexpected token <{Current.Type}>, expected <{type}>");
            return new Token(type, Current.Position, null, null);
        }
    }
}
