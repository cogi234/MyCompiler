namespace MiniCompiler.CodeAnalysis.Symbols
{
    public sealed class TypeSymbol : Symbol
    {
        public static readonly TypeSymbol Error = new TypeSymbol("?");
        public static readonly TypeSymbol Void = new TypeSymbol("void");
        public static readonly TypeSymbol Bool = new TypeSymbol("bool");
        public static readonly TypeSymbol Int = new TypeSymbol("int");
        public static readonly TypeSymbol String = new TypeSymbol("string");

        private TypeSymbol(string name) : base(name)
        {
        }

        public override SymbolType SymbolType => SymbolType.Type;

        public static TypeSymbol? Lookup(string name)
        {
            switch (name)
            {
                case "void": return Void;
                case "bool": return Bool;
                case "int": return Int;
                case "string": return String;
                default:
                    return null;
            }
        }
    }
}
