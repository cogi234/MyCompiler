namespace MiniCompiler.CodeAnalysis.Text
{
    public sealed class TextLine
    {
        private readonly int lineBreakWidth;

        public TextLine(SourceText sourceText, TextSpan span, int lineBreakWidth)
        {
            SourceText = sourceText;
            Span = span;
            this.lineBreakWidth = lineBreakWidth;
        }

        public SourceText SourceText { get; }
        public TextSpan Span { get; }
        public TextSpan SpanWithLineBreak => new TextSpan(Span.Start, Span.Length + lineBreakWidth);

        public override string ToString() => SourceText.ToString(Span);
    }
}
