using MiniCompiler.CodeAnalysis.Syntax.SyntaxNodes;
using MiniCompiler.CodeAnalysis.Text;
using System.Collections.Immutable;

namespace MiniCompiler.CodeAnalysis.Syntax
{
    internal sealed class Parser
    {
        private readonly ImmutableArray<Token> tokens;
        private readonly SourceText sourceText;
        private readonly DiagnosticBag diagnostics = new DiagnosticBag();
        public DiagnosticBag Diagnostics => diagnostics;

        private int position;

        public Parser(SourceText text)
        {
            sourceText = text;
            List<Token> tokens = new List<Token>();
            Lexer lexer = new Lexer(text);
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

            this.tokens = tokens.ToImmutableArray();

            diagnostics.AddRange(lexer.Diagnostics);
        }

        public SyntaxTree Parse()
        {
            ExpressionNode expression = ParseExpression();
            ExpectToken(TokenType.EndOfFile);
            return new SyntaxTree(diagnostics.ToImmutableArray(), expression, sourceText);
        }

        private ExpressionNode ParseExpression(int parentPrecedence = 0)
        {
            return ParseAssignmentExpression(parentPrecedence);
        }

        private ExpressionNode ParseAssignmentExpression(int parentPrecedence = 0)
        {
            if (Current.Type == TokenType.Identifier &&
                Peek(1).Type == TokenType.Equal)
            {
                Token identifierToken = ExpectToken(TokenType.Identifier);
                Token operatorToken = ExpectToken(TokenType.Equal);
                ExpressionNode right = ParseExpression(parentPrecedence);
                return new AssignmentExpressionNode(identifierToken, operatorToken, right);
            }

            return ParseMathematicalExpression(parentPrecedence);
        }

        private ExpressionNode ParseMathematicalExpression(int parentPrecedence = 0)
        {
            ExpressionNode left;

            //Unary expressions
            int unaryOperatorPrecedence = Current.Type.GetUnaryOperatorPrecedence();
            if (unaryOperatorPrecedence != 0 && unaryOperatorPrecedence >= parentPrecedence)
            {
                Token operatorToken = ExpectTokens(SyntaxFacts.GetUnaryOperatorTypes().ToArray());
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

                Token operatorToken = ExpectTokens(SyntaxFacts.GetBinaryOperatorTypes().ToArray());
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
                    return ParseParenthesizedExpression();
                case TokenType.TrueKeyword:
                case TokenType.FalseKeyword:
                    return ParseBooleanLiteral();
                case TokenType.Number:
                    return ParseNumberLiteral();
                case TokenType.Identifier:
                default:
                    return ParseNameExpression();
            }
        }

        private ExpressionNode ParseParenthesizedExpression()
        {
            Token openToken = ExpectToken(TokenType.OpenParenthesis);
            ExpressionNode expression = ParseExpression();
            Token closeToken = ExpectToken(TokenType.CloseParenthesis);
            return new ParenthesizedExpressionNode(openToken, expression, closeToken);
        }

        private ExpressionNode ParseBooleanLiteral()
        {
            Token token = ExpectTokens(TokenType.TrueKeyword, TokenType.FalseKeyword);
            bool value = token.Type == TokenType.TrueKeyword;
            return new LiteralExpressionNode(token, value);
        }

        private ExpressionNode ParseNumberLiteral()
        {
            Token numberToken = ExpectToken(TokenType.Number);
            return new LiteralExpressionNode(numberToken, numberToken.Value);
        }

        private ExpressionNode ParseNameExpression()
        {
            Token token = ExpectToken(TokenType.Identifier);
            return new NameExpressionNode(token);
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
        private Token ExpectToken(TokenType expectedType)
        {
            if (Current.Type == expectedType)
                return NextToken();

            diagnostics.ReportUnexpectedToken(Current.Span, Current.Type, expectedType);
            return new Token(expectedType, new TextSpan(Current.Span.Start, 0), null, null);
        }

        private Token ExpectTokens(params TokenType[] expectedTypes)
        {
            if (expectedTypes.Contains(Current.Type))
                return NextToken();


            diagnostics.ReportUnexpectedToken(Current.Span, Current.Type, expectedTypes);
            return new Token(expectedTypes[0], new TextSpan(Current.Span.Start, 0), null, null);
        }
    }
}
