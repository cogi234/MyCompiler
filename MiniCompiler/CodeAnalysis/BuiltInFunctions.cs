using System.Reflection;
using MiniCompiler.CodeAnalysis.Symbols;

namespace MiniCompiler.CodeAnalysis
{
    internal sealed class BuiltInFunctions
    {
        public static readonly FunctionSymbol Print = new FunctionSymbol(
            "print",
            [new ParameterSymbol("text", TypeSymbol.String)],
            TypeSymbol.Void);
        public static readonly FunctionSymbol Input = new FunctionSymbol(
            "input",
            [],
            TypeSymbol.String);
        public static readonly FunctionSymbol Random = new FunctionSymbol(
            "random",
            [new ParameterSymbol("max", TypeSymbol.Int)],
            TypeSymbol.Int);

        internal static IEnumerable<FunctionSymbol> GetAll() =>
            typeof(BuiltInFunctions).GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.FieldType == typeof(FunctionSymbol))
            .Select(f => (FunctionSymbol)f.GetValue(null)!);
    }
}
