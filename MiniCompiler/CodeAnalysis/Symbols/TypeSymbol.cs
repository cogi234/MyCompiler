namespace MiniCompiler.CodeAnalysis.Symbols
{
    public sealed class TypeSymbol : Symbol
    {
        public static readonly TypeSymbol Error = new TypeSymbol("?", null);
        public static readonly TypeSymbol Void = new TypeSymbol("void", null);
        public static readonly TypeSymbol Null = new TypeSymbol("null", null);
        public static readonly TypeSymbol Bool = new TypeSymbol("bool", false);
        public static readonly TypeSymbol Int = new TypeSymbol("int", 0);
        public static readonly TypeSymbol String = new TypeSymbol("string", "");

        private TypeSymbol(string name, object? defaultValue) : base(name)
        {
            DefaultValue = defaultValue;
        }

        public object? DefaultValue { get; }

        public override SymbolType SymbolType => SymbolType.Type;

        public static TypeSymbol? Lookup(string name)
        {
            switch (name)
            {
                case "void": return Void;
                case "null": return Null;
                case "bool": return Bool;
                case "int": return Int;
                case "string": return String;
                default:
                    return null;
            }
        }
    }
}
