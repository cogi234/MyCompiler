﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompiler
{
    internal sealed class Parser
    {
        private readonly Token[] tokens;
        private int position;
        private List<string> diagnostics = new List<string>();

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
            ExpressionNode left = ParsePrimaryExpression();

            while (true)
            {
                int precedence = GetBinaryOperatorPrecedence(Current.Type);
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
            if (Current.Type == TokenType.OpenParenthesis)
            {
                Token left = NextToken();
                ExpressionNode expression = ParseExpression();
                Token right = MatchToken(TokenType.CloseParenthesis);
                return new ParenthesizedExpressionNode(left, expression, right);
            }

            if (Current.Type == TokenType.Minus)
            {
                Token operatorToken = NextToken();
                ExpressionNode expression = ParseExpression();
                return new UnaryExpressionNode(operatorToken, expression);
            }

            Token numberToken = MatchToken(TokenType.Number);
            return new LiteralExpressionNode(numberToken);
        }

        private static int GetBinaryOperatorPrecedence(TokenType type)
        {
            switch (type)
            {
                case TokenType.Plus:
                case TokenType.Minus:
                    return 1;

                case TokenType.Star:
                case TokenType.ForwardSlash:
                    return 2;

                default:
                    return 0;
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
