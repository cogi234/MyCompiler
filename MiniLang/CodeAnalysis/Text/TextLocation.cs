namespace MiniLang.CodeAnalysis.Text
{
    public struct TextLocation
    {
        public TextLocation(SourceText source, TextSpan span)
        {
            Source = source;
            Span = span;
        }

        public SourceText Source { get; }
        public TextSpan Span { get; }

        public string FileName => Source.FileName;
        public int StartLine => Source.GetLineIndex(Span.Start);
        public int StartColumn => Span.Start - Source.Lines[StartLine].Span.Start;
        public int EndLine => Source.GetLineIndex(Span.End);
        public int EndColumn => Span.End - Source.Lines[EndLine].Span.Start;
    }
}
