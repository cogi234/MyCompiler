namespace MiniCompiler.CodeAnalysis.Binding
{
    internal sealed class LabelSymbol
    {
        public LabelSymbol(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
