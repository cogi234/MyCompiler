using MiniLang.CodeAnalysis.Symbols;
using MiniLang.CodeAnalysis.Syntax;

namespace MiniLang.IO
{
    public static class SymbolPrinter
    {
        public static void WriteTo(this Symbol symbol, TextWriter writer)
        {
            switch (symbol.SymbolType)
            {
                case SymbolType.Type:
                    WriteTypeTo((TypeSymbol)symbol, writer);
                    break;
                case SymbolType.Function:
                    WriteFunctionTo((FunctionSymbol)symbol, writer);
                    break;
                case SymbolType.Parameter:
                    WriteParameterTo((ParameterSymbol)symbol, writer);
                    break;
                case SymbolType.GlobalVariable:
                    WriteGlobalTo((GlobalVariableSymbol)symbol, writer);
                    break;
                case SymbolType.LocalVariable:
                    WriteLocalTo((LocalVariableSymbol)symbol, writer);
                    break;
                default:
                    throw new Exception($"Unexpected symbol: {symbol.SymbolType}");
            }
        }

        private static void WriteTypeTo(TypeSymbol symbol, TextWriter writer)
        {
            writer.WriteType(symbol.Name);
        }

        private static void WriteFunctionTo(FunctionSymbol symbol, TextWriter writer)
        {
            symbol.ReturnType.WriteTo(writer);
            writer.WriteSpace();
            writer.WriteIdentifier(symbol.Name);
            writer.WritePunctuation(TokenType.OpenParenthesis);

            for (int i = 0; i < symbol.Parameters.Length; i++)
            {
                if (i > 0)
                {
                    writer.WritePunctuation(TokenType.Comma);
                    writer.WriteSpace();
                }

                symbol.Parameters[i].WriteTo(writer);
            }

            writer.WritePunctuation(TokenType.CloseParenthesis);
        }

        private static void WriteParameterTo(ParameterSymbol symbol, TextWriter writer)
        {
            symbol.Type.WriteTo(writer);
            writer.WriteSpace();
            writer.WriteIdentifier(symbol.Name);
        }

        private static void WriteGlobalTo(GlobalVariableSymbol symbol, TextWriter writer)
        {
            symbol.Type.WriteTo(writer);
            writer.WriteSpace();
            writer.WriteIdentifier(symbol.Name);
        }

        private static void WriteLocalTo(LocalVariableSymbol symbol, TextWriter writer)
        {
            symbol.Type.WriteTo(writer);
            writer.WriteSpace();
            writer.WriteIdentifier(symbol.Name);
        }
    }
}
