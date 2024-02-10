using static System.Net.Mime.MediaTypeNames;

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
        public string ToString(int start, int length) => SourceText.ToString(Span.Start + start, length);
        public string ToString(int start) => SourceText.ToString(Span.Start + start, Span.Length - start);
        public string ToString(TextSpan span) => SourceText.ToString(Span.Start + span.Start, span.Length);
    }
}
