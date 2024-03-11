using System.Collections.Immutable;

namespace MiniLang.CodeAnalysis.Text
{
    public sealed class SourceText
    {
        private readonly string text;

        private SourceText(string text, string fileName)
        {
            this.text = text;
            FileName = fileName;
            Lines = ParseLines();
        }

        public string FileName { get; }

        public static SourceText From(string text, string fileName = "")
        {
            return new SourceText(text, fileName);
        }

        public ImmutableArray<TextLine> Lines { get; }

        public char this[int index] => text[index];
        public int Length => text.Length;


        public int GetLineIndex(int position)
        {
            int lower = 0;
            int upper = Lines.Length - 1;

            while (lower <= upper)
            {
                int index = lower + (upper - lower) / 2;
                int start = Lines[index].SpanWithLineBreak.Start;

                //We found the line
                if (position == start)
                    return index;

                //Current line is before our position
                if (start > position)
                    upper = index - 1;
                else //Current line is after our position
                    lower = index + 1;
            }

            return lower - 1;
        }

        private ImmutableArray<TextLine> ParseLines()
        {
            ImmutableArray<TextLine>.Builder result = ImmutableArray.CreateBuilder<TextLine>();

            int lineStart = 0;
            int position = 0;

            while (position < text.Length)
            {
                int lineBreakWidth = GetLineBreakWidth(position);

                if (lineBreakWidth == 0)
                    position++;
                else
                {
                    AddLine(result, lineStart, position, lineBreakWidth);
                    position += lineBreakWidth;
                    lineStart = position;
                }
            }

            if (position >= lineStart)
                AddLine(result, lineStart, position, 0);

            return result.ToImmutable();
        }

        private void AddLine(ImmutableArray<TextLine>.Builder result, int lineStart, int position, int lineBreakWidth)
        {
            TextLine line = new TextLine(this, TextSpan.FromBounds(lineStart, position), lineBreakWidth);
            result.Add(line);
        }

        private int GetLineBreakWidth(int position)
        {
            char current = text[position];
            char next = position + 1 >= text.Length ? '\0' : text[position + 1];

            if (current == '\r' && next == '\n')
                return 2;
            if (current == '\r' || current == '\n')
                return 1;
            return 0;
        }


        public override string ToString() => text;
        public string ToString(int start, int length)
        {
            if (length == 0)
                return "";
            if (start < 0)
                return "";
            if (start >= text.Length)
                return "";
            if (start + length > text.Length)
                return "";
            return text.Substring(start, length);
        }
        public string ToString(TextSpan span) => ToString(span.Start, span.Length);
    }
}
