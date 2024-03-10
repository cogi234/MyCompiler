using MiniLang.CodeAnalysis.Symbols;

namespace MiniLang.CodeAnalysis.Binding
{
    internal sealed class Conversion
    {
        public static readonly Conversion None = new Conversion(false, false, false);
        public static readonly Conversion Identity = new Conversion(true, true, true);
        public static readonly Conversion Implicit = new Conversion(true, false, true);
        public static readonly Conversion Explicit = new Conversion(true, false, false);

        private Conversion(bool exists, bool isIdentity, bool isImplicit)
        {
            Exists = exists;
            IsIdentity = isIdentity;
            IsImplicit = isImplicit;
        }

        public bool Exists { get; }
        public bool IsIdentity { get; }
        public bool IsImplicit { get; }


        public static Conversion Classify(TypeSymbol from, TypeSymbol to)
        {
            //Identity
            if (from == to)
                return Identity;

            //Implicit conversions
            if (from == TypeSymbol.Bool || from == TypeSymbol.Int)
            {
                if (to == TypeSymbol.String)
                    return Implicit;
            }

            //Explicit conversions
            if (from == TypeSymbol.String)
            {
                if (to == TypeSymbol.Bool)
                    return Explicit;
            }
            else if (from == TypeSymbol.String)
            {
                if (to == TypeSymbol.Int)
                    return Explicit;
            }



            return None;
        }
    }
}
