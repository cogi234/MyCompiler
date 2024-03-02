using MiniCompiler.CodeAnalysis.Binding.BoundNodes;
using MiniCompiler.CodeAnalysis.Syntax.SyntaxNodes;
using MiniCompiler.CodeAnalysis.Text;
using System.Collections.Immutable;

namespace MiniCompiler.CodeAnalysis.Syntax
{
    internal sealed class Parser
    {
        private readonly ImmutableArray<Token> tokens;
        private readonly DiagnosticBag diagnostics = new DiagnosticBag();
        public DiagnosticBag Diagnostics => diagnostics;

        private int position;

        public Parser(SourceText text)
        {
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

        public CompilationUnit ParseCompilationUnit()
        {
            ImmutableArray<StatementNode>.Builder statements = ImmutableArray.CreateBuilder<StatementNode>();
            while (Current.Type != TokenType.EndOfFile)
            {
                Token startToken = Current;
                statements.Add(ParseStatement());

                //If we didn't consume any tokens, we skip this one
                if (Current == startToken)
                    NextToken();
            }

            ExpectToken(TokenType.EndOfFile);

            return new CompilationUnit(statements.ToImmutable());
        }

        #region Statements
        private StatementNode ParseStatement()
        {
            switch (Current.Type)
            {
                case TokenType.OpenBrace:
                    return ParseBlockStatement();
                case TokenType.LetKeyword:
                case TokenType.VarKeyword:
                    return ParseVariableDeclarationStatement();
                case TokenType.IfKeyword:
                    return ParseIfStatement();
                case TokenType.WhileKeyword:
                    return ParseWhileStatement();
                case TokenType.DoKeyword:
                    return ParseDoWhileStatement();
                case TokenType.ForKeyword:
                    return ParseForStatement();
                default:
                    return ParseExpressionStatement();
            }
        }

        // for (var i = 0; i < 10; i = i + 1)
        private StatementNode ParseForStatement()
        {
            bool isValid = true;
            Token forKeyword = ExpectToken(TokenType.ForKeyword);
            Token openParenthesis = ExpectToken(TokenType.OpenParenthesis);

            VariableDeclarationStatementNode? declaration = null;
            if (Current.Type != TokenType.Semicolon)
                declaration = ParseVariableDeclarationStatement(false);

            ExpectToken(TokenType.Semicolon);

            ExpressionNode condition = ParseExpression();
            ExpectToken(TokenType.Semicolon);

            AssignmentExpressionNode? increment = null;
            if (Current.Type != TokenType.CloseParenthesis)
            {
                ExpressionNode expression = ParseAssignmentExpression();
                if (expression.Type != NodeType.AssignmentExpression)
                {
                    diagnostics.ReportUnexpectedNode(expression.Span, expression.Type, NodeType.AssignmentExpression);
                    isValid = false;
                }
                else
                    increment = (AssignmentExpressionNode)expression;
            }

            Token closeParenthesis = ExpectToken(TokenType.CloseParenthesis);

            StatementNode statement = ParseStatement();

            if (isValid)
                return new ForStatementNode(forKeyword, openParenthesis, declaration, condition, increment, closeParenthesis, statement);
            else
                return statement;
        }

        private DoWhileStatementNode ParseDoWhileStatement()
        {
            Token doKeyword = ExpectToken(TokenType.DoKeyword);
            StatementNode statement = ParseStatement();
            Token whileKeyword = ExpectToken(TokenType.WhileKeyword);
            Token openParenthesis = ExpectToken(TokenType.OpenParenthesis);
            ExpressionNode condition = ParseExpression();
            Token closeParenthesis = ExpectToken(TokenType.CloseParenthesis);
            Token semicolon = ExpectToken(TokenType.Semicolon);
            return new DoWhileStatementNode(doKeyword, statement, whileKeyword, openParenthesis, condition, closeParenthesis, semicolon);
        }

        private WhileStatementNode ParseWhileStatement()
        {
            Token whileKeyword = ExpectToken(TokenType.WhileKeyword);
            Token openParenthesis = ExpectToken(TokenType.OpenParenthesis);
            ExpressionNode condition = ParseExpression();
            Token closeParenthesis = ExpectToken(TokenType.CloseParenthesis);
            StatementNode statement = ParseStatement();
            return new WhileStatementNode(whileKeyword, openParenthesis, condition, closeParenthesis, statement);
        }

        private IfStatementNode ParseIfStatement()
        {
            Token ifKeyword = ExpectToken(TokenType.IfKeyword);
            Token openParenthesis = ExpectToken(TokenType.OpenParenthesis);
            ExpressionNode condition = ParseExpression();
            Token closeParenthesis = ExpectToken(TokenType.CloseParenthesis);
            StatementNode ifStatement = ParseStatement();

            //The else is optional
            ElseClauseNode? elseStatement = null;
            if (Current.Type == TokenType.ElseKeyword)
            {
                Token elseKeyword = ExpectToken(TokenType.ElseKeyword);
                StatementNode statement = ParseStatement();
                elseStatement = new ElseClauseNode(elseKeyword, statement);
            }

            return new IfStatementNode(ifKeyword, openParenthesis, condition, closeParenthesis, ifStatement, elseStatement);
        }

        private VariableDeclarationStatementNode ParseVariableDeclarationStatement(bool takeSemicolon = true)
        {
            Token keyword = ExpectTokens(TokenType.VarKeyword, TokenType.LetKeyword);
            Token identifier = ExpectToken(TokenType.Identifier);

            //The initializer is optional
            Token? equal = null;
            ExpressionNode? initializer = null;
            //if (Current.Type == TokenType.Equal)
            //{   Commented until typed variable declaration is implemented
            equal = ExpectToken(TokenType.Equal);
            initializer = ParseExpression();
            //}

            Token? semicolon = takeSemicolon ? ExpectToken(TokenType.Semicolon) : null;
            return new VariableDeclarationStatementNode(keyword, identifier, equal, initializer, semicolon);
        }

        private ExpressionStatementNode ParseExpressionStatement()
        {
            ExpressionNode expression = ParseExpression();
            Token semicolon = ExpectToken(TokenType.Semicolon);
            return new ExpressionStatementNode(expression, semicolon);
        }

        private BlockStatementNode ParseBlockStatement()
        {
            Token openToken = ExpectToken(TokenType.OpenBrace);

            ImmutableArray<StatementNode>.Builder statements = ImmutableArray.CreateBuilder<StatementNode>();
            while (Current.Type != TokenType.CloseBrace && Current.Type != TokenType.EndOfFile)
            {
                Token startToken = Current;
                statements.Add(ParseStatement());

                //If we didn't consume any tokens, we skip this one
                if (Current == startToken)
                    NextToken();
            }

            Token closeToken = ExpectToken(TokenType.CloseBrace);
            return new BlockStatementNode(openToken, statements.ToImmutable(), closeToken);
        }
        #endregion Statements

        #region Expressions
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
                case TokenType.String:
                    return ParseStringLiteral();
                case TokenType.Identifier:
                case TokenType.Type:
                default:
                    return ParseNameOrCallExpression();
            }
        }

        private ParenthesizedExpressionNode ParseParenthesizedExpression()
        {
            Token openToken = ExpectToken(TokenType.OpenParenthesis);
            ExpressionNode expression = ParseExpression();
            Token closeToken = ExpectToken(TokenType.CloseParenthesis);
            return new ParenthesizedExpressionNode(openToken, expression, closeToken);
        }

        private LiteralExpressionNode ParseBooleanLiteral()
        {
            Token token = ExpectTokens(TokenType.TrueKeyword, TokenType.FalseKeyword);
            bool value = token.Type == TokenType.TrueKeyword;
            return new LiteralExpressionNode(token, value);
        }

        private LiteralExpressionNode ParseNumberLiteral()
        {
            Token numberToken = ExpectToken(TokenType.Number);
            return new LiteralExpressionNode(numberToken, numberToken.Value ?? 0);
        }

        private LiteralExpressionNode ParseStringLiteral()
        {
            Token stringToken = ExpectToken(TokenType.String);
            return new LiteralExpressionNode(stringToken, stringToken.Value ?? "");
        }

        private ExpressionNode ParseNameOrCallExpression()
        {
            if ((Current.Type == TokenType.Identifier || Current.Type == TokenType.Type) 
                && Peek(1).Type == TokenType.OpenParenthesis)
                return ParseCallExpression();

            return ParseNameExpression();
        }

        private CallExpressionNode ParseCallExpression()
        {
            Token identifier = ExpectTokens(TokenType.Identifier, TokenType.Type);
            Token openParenthesis = ExpectToken(TokenType.OpenParenthesis);
            SeparatedNodeList<ExpressionNode> arguments = ParseArguments();
            Token closeParenthesis = ExpectToken(TokenType.CloseParenthesis);

            return new CallExpressionNode(identifier, openParenthesis, arguments, closeParenthesis);
        }

        private SeparatedNodeList<ExpressionNode> ParseArguments()
        {
            ImmutableArray<SyntaxNode>.Builder arguments = ImmutableArray.CreateBuilder<SyntaxNode>();
            ImmutableArray<Token>.Builder separators = ImmutableArray.CreateBuilder<Token>();
            while (Current.Type != TokenType.CloseParenthesis && Current.Type != TokenType.Semicolon && Current.Type != TokenType.EndOfFile)
            {
                Token startToken = Current;

                arguments.Add(ParseExpression());
                //If we're not at the end, we want a comma to separate the arguments.
                if (Current.Type != TokenType.CloseParenthesis && Current.Type != TokenType.Semicolon && Current.Type != TokenType.EndOfFile)
                    separators.Add(ExpectToken(TokenType.Comma));
                else
                    break;

                //If we didn't consume any tokens, we skip this one
                if (Current == startToken)
                    NextToken();
            }
            return new SeparatedNodeList<ExpressionNode>(arguments.ToImmutable(), separators.ToImmutable());
        }

        private VariableExpressionNode ParseNameExpression()
        {
            Token identifier = ExpectToken(TokenType.Identifier);
            return new VariableExpressionNode(identifier);
        }
        #endregion Expressions

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
            return new Token(expectedType, new TextSpan(Current.Span.Start, 0), null, null, true);
        }

        private Token ExpectTokens(params TokenType[] expectedTypes)
        {
            if (expectedTypes.Contains(Current.Type))
                return NextToken();


            diagnostics.ReportUnexpectedToken(Current.Span, Current.Type, expectedTypes);
            return new Token(expectedTypes[0], new TextSpan(Current.Span.Start, 0), null, null, true);
        }
    }
}
