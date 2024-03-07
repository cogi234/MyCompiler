using MiniCompiler.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text;

namespace Mini.Tests.CodeAnalysis
{
    internal sealed class AnnotatedText
    {
        public AnnotatedText(string text, ImmutableArray<TextSpan> spans)
        {
            Text = text;
            Spans = spans;
        }

        public string Text { get; }
        public ImmutableArray<TextSpan> Spans { get; }

        public static AnnotatedText Parse(string text)
        {
            text = Unindent(text);

            StringBuilder textBuilder = new StringBuilder();
            ImmutableArray<TextSpan>.Builder spanBuilder = ImmutableArray.CreateBuilder<TextSpan>();
            Stack<int> startStack = new Stack<int>();

            int position = 0;
            foreach (char c in text)
            {
                switch (c)
                {
                    case '[':
                        startStack.Push(position);
                        break;
                    case ']':
                        if (startStack.Count == 0)
                            throw new ArgumentException("Too many ']' in text", nameof(text));

                        int start = startStack.Pop();
                        int end = position;
                        TextSpan span = TextSpan.FromBounds(start, end);
                        spanBuilder.Add(span);
                        break;
                    default:
                        textBuilder.Append(c);
                        position++;
                        break;
                }
            }

            if (startStack.Count != 0)
                throw new ArgumentException("Too many '[' in text", nameof(text));

            return new AnnotatedText(textBuilder.ToString(), spanBuilder.ToImmutable());
        }
        public static string Unindent(string text)
        {
            string[] lines = UnindentLines(text);

            return string.Join(Environment.NewLine, lines);
        }

        public static string[] UnindentLines(string text)
        {
            List<string> lines = new List<string>();

            using (StringReader stringReader = new StringReader(text))
            {
                string line;
                while ((line = stringReader.ReadLine()) != null)
                    lines.Add(line);
            }

            int minIndentation = int.MaxValue;
            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];

                //Ignore empty lines
                if (line.Trim().Length == 0)
                {
                    lines[i] = string.Empty;
                    continue;
                }

                int indentation = line.Length - line.TrimStart().Length;
                minIndentation = Math.Min(minIndentation, indentation);
            }

            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].Length == 0)
                    continue;
                lines[i] = lines[i].Substring(minIndentation);
            }

            //Remove leading blank lines
            while (lines.Count > 0 && lines[0].Length == 0)
                lines.RemoveAt(0);
            //Remove trailing blank lines
            while (lines.Count > 0 && lines[lines.Count - 1].Length == 0)
                lines.RemoveAt(lines.Count - 1);

            return lines.ToArray();
        }
    }
}
