using MiniLang.IO;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reflection;
using System.Text;

namespace mi
{
    internal abstract class Repl
    {
        private readonly List<MetaCommand> metaCommands = new List<MetaCommand>();
        private readonly List<string> submissionHistory = new List<string>();
        private int submissionHistoryIndex;

        private bool done;

        protected Repl()
        {
            InitializeMetaCommands();
        }

        public void Run()
        {
            while (true)
            {
                string? text = EditSubmission();
                if (string.IsNullOrWhiteSpace(text))
                    continue;
                if (text.ToLower() == "#exit")
                    return;

                if (!text.Contains(Environment.NewLine) && text.StartsWith("#"))
                    EvaluateMetaCommand(text);
                else
                    EvaluateSubmission(text);

                submissionHistory.Add(text);
                submissionHistoryIndex = 0;
            }
        }

        private string EditSubmission()
        {
            done = false;

            ObservableCollection<string> document = new ObservableCollection<string>() { "" };
            SubmissionView view = new SubmissionView(RenderLine, document);

            while (!done)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                HandleKey(key, document, view);
            }

            view.CurrentLine = document.Count - 1;
            view.CurrentColumn = document[view.CurrentLine].Length;
            Console.WriteLine();

            return string.Join(Environment.NewLine, document);
        }

        #region KeyHandling
        private void HandleKey(ConsoleKeyInfo key, ObservableCollection<string> document, SubmissionView view)
        {
            if (key.Modifiers == default)
            {
                switch (key.Key)
                {
                    case ConsoleKey.Escape:
                        HandleEscape(document, view);
                        break;
                    case ConsoleKey.Enter:
                        HandleEnter(document, view);
                        break;
                    case ConsoleKey.LeftArrow:
                        HandleLeftArrow(document, view);
                        break;
                    case ConsoleKey.RightArrow:
                        HandleRightArrow(document, view);
                        break;
                    case ConsoleKey.UpArrow:
                        HandleUpArrow(document, view);
                        break;
                    case ConsoleKey.DownArrow:
                        HandleDownArrow(document, view);
                        break;
                    case ConsoleKey.Backspace:
                        HandleBackspace(document, view);
                        break;
                    case ConsoleKey.Delete:
                        HandleDelete(document, view);
                        break;
                    case ConsoleKey.Home:
                        HandleHome(document, view);
                        break;
                    case ConsoleKey.End:
                        HandleEnd(document, view);
                        break;
                    case ConsoleKey.Tab:
                        HandleTab(document, view);
                        break;
                    case ConsoleKey.PageUp:
                        HandlePageUp(document, view);
                        break;
                    case ConsoleKey.PageDown:
                        HandlePageDown(document, view);
                        break;
                }
            }
            else if (key.Modifiers == ConsoleModifiers.Control)
            {
                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        HandleControlEnter(document, view);
                        break;
                }
            }
            else if (key.Modifiers == ConsoleModifiers.Shift)
            {
                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        HandleShiftEnter(document, view);
                        break;
                }
            }

            if (key.Key != ConsoleKey.Backspace && key.KeyChar >= ' ')
                HandleTyping(document, view, key.KeyChar.ToString());
        }

        private void HandleEscape(ObservableCollection<string> document, SubmissionView view)
        {
            document.Clear();
            document.Add("");
            view.CurrentColumn = 0;
            view.CurrentLine = 0;
        }

        private void HandleEnter(ObservableCollection<string> document, SubmissionView view)
        {
            string submissionText = string.Join(Environment.NewLine, document);
            if (submissionText.StartsWith("#") || IsCompleteSubmission(submissionText))
            {
                done = true;
                return;
            }

            InsertLine(document, view);
        }

        private void HandleControlEnter(ObservableCollection<string> document, SubmissionView view)
        {
            done = true;
            return;
        }

        private void HandleShiftEnter(ObservableCollection<string> document, SubmissionView view)
        {
            InsertLine(document, view);
        }

        private void HandleLeftArrow(ObservableCollection<string> document, SubmissionView view)
        {
            if (view.CurrentColumn > 0)
                view.CurrentColumn--;
            else if (view.CurrentLine > 0)
            {
                view.CurrentLine--;
                view.CurrentColumn = document[view.CurrentLine].Length;
            }
        }

        private void HandleRightArrow(ObservableCollection<string> document, SubmissionView view)
        {
            string line = document[view.CurrentLine];
            if (view.CurrentColumn < line.Length)
                view.CurrentColumn++;
            else if (view.CurrentLine < document.Count - 1)
            {
                view.CurrentLine++;
                view.CurrentColumn = 0;
            }
        }

        private void HandleUpArrow(ObservableCollection<string> document, SubmissionView view)
        {
            if (view.CurrentLine > 0)
                view.CurrentLine--;
        }

        private void HandleDownArrow(ObservableCollection<string> document, SubmissionView view)
        {
            if (view.CurrentLine < document.Count - 1)
                view.CurrentLine++;
        }

        private void HandleBackspace(ObservableCollection<string> document, SubmissionView view)
        {
            int start = view.CurrentColumn;
            if (start == 0)
            {
                if (view.CurrentLine == 0)
                    return;

                string currentLine = document[view.CurrentLine];
                string previousLine = document[view.CurrentLine - 1];
                document.RemoveAt(view.CurrentLine);
                view.CurrentLine--;
                document[view.CurrentLine] = previousLine + currentLine;
                view.CurrentColumn = previousLine.Length;
            }
            else
            {
                string line = document[view.CurrentLine];
                string before = line.Substring(0, start - 1);
                string after = line.Substring(start);
                document[view.CurrentLine] = before + after;
                view.CurrentColumn--;
            }
        }

        private void HandleDelete(ObservableCollection<string> document, SubmissionView view)
        {
            string line = document[view.CurrentLine];
            if (view.CurrentColumn >= line.Length)
            {
                if (view.CurrentLine == document.Count - 1)
                    return;

                string currentLine = document[view.CurrentLine];
                string nextLine = document[view.CurrentLine + 1];
                document.RemoveAt(view.CurrentLine + 1);
                document[view.CurrentLine] = currentLine + nextLine;
            }
            else
            {
                string before = line.Substring(0, view.CurrentColumn);
                string after = line.Substring(view.CurrentColumn + 1);
                document[view.CurrentLine] = before + after;
            }
        }

        private void HandleHome(ObservableCollection<string> document, SubmissionView view)
        {
            view.CurrentColumn = 0;
        }

        private void HandleEnd(ObservableCollection<string> document, SubmissionView view)
        {
            view.CurrentColumn = document[view.CurrentLine].Length;
        }

        private void HandleTab(ObservableCollection<string> document, SubmissionView view)
        {
            const int TabWidth = 4;
            int start = view.CurrentColumn;
            int remainingSpaces = TabWidth - start % TabWidth;
            string line = document[view.CurrentLine];
            document[view.CurrentLine] = line.Insert(start, new string(' ', remainingSpaces));
            view.CurrentColumn += remainingSpaces;
        }

        private void HandlePageUp(ObservableCollection<string> document, SubmissionView view)
        {
            if (submissionHistory.Count > 0)
            {
                submissionHistoryIndex--;
                if (submissionHistoryIndex < 0)
                    submissionHistoryIndex = submissionHistory.Count - 1;
                UpdateDocumentFromHistory(document, view);
            }
        }

        private void HandlePageDown(ObservableCollection<string> document, SubmissionView view)
        {
            if (submissionHistory.Count > 0)
            {
                submissionHistoryIndex++;
                if (submissionHistoryIndex > submissionHistory.Count - 1)
                    submissionHistoryIndex = 0;
                UpdateDocumentFromHistory(document, view);
            }
        }

        private void HandleTyping(ObservableCollection<string> document, SubmissionView view, string text)
        {
            document[view.CurrentLine] = document[view.CurrentLine].Insert(view.CurrentColumn, text);
            view.CurrentColumn += text.Length;
        }
        #endregion

        private void UpdateDocumentFromHistory(ObservableCollection<string> document, SubmissionView view)
        {
            if (submissionHistory.Count == 0)
                return;

            document.Clear();

            string historyItem = submissionHistory[submissionHistoryIndex];
            string[] lines = historyItem.Split(Environment.NewLine);
            foreach (string line in lines)
                document.Add(line);

            view.CurrentLine = document.Count - 1;
            view.CurrentColumn = document[view.CurrentLine].Length;
        }

        protected void ClearHistory()
        {
            submissionHistory.Clear();
        }

        protected virtual void RenderLine(string line)
        {
            Console.Write(line);
        }

        protected abstract bool IsCompleteSubmission(string text);
        protected abstract void EvaluateSubmission(string text);

        private static void InsertLine(ObservableCollection<string> document, SubmissionView view)
        {
            string remainder = document[view.CurrentLine].Substring(view.CurrentColumn);
            document[view.CurrentLine] = document[view.CurrentLine].Substring(0, view.CurrentColumn);

            int lineIndex = view.CurrentLine + 1;
            document.Insert(lineIndex, remainder);
            view.CurrentColumn = 0;
            view.CurrentLine = lineIndex;
        }

        private sealed class SubmissionView
        {
            private readonly Action<string> lineRenderer;
            private readonly ObservableCollection<string> submissionDocument;
            private int cursorTop;
            private int renderedLineCount;
            private int currentLine;
            private int currentColumn;

            public SubmissionView(Action<string> lineRenderer, ObservableCollection<string> submissionDocument)
            {
                this.lineRenderer = lineRenderer;
                this.submissionDocument = submissionDocument;
                this.submissionDocument.CollectionChanged += SubmissionDocumentChanged;
                cursorTop = Console.CursorTop;
                Render();
            }

            private void SubmissionDocumentChanged(object? sender, NotifyCollectionChangedEventArgs e)
            {
                Render();
            }

            private void Render()
            {
                Console.CursorVisible = false;

                int lineCount = 0;
                for (; lineCount < submissionDocument.Count; lineCount++)
                {
                    string line = submissionDocument[lineCount];

                    if (cursorTop + lineCount >= Console.WindowHeight - 1)
                    {
                        Console.SetCursorPosition(0, Math.Min(cursorTop + lineCount, Console.WindowHeight - 1));
                        Console.WriteLine(line);
                        if (cursorTop > 0)
                            cursorTop--;
                    }

                    Console.SetCursorPosition(0, cursorTop + lineCount);

                    Console.ForegroundColor = ConsoleColor.Green;
                    if (lineCount == 0)
                        Console.Write("» ");
                    else if (lineCount == submissionDocument.Count - 1)
                        Console.Write("« ");
                    else
                        Console.Write("· ");
                    Console.ResetColor();

                    lineRenderer(line);

                    Console.WriteLine(new string(' ', Console.WindowWidth - line.Length - 2));
                }

                int numberOfBlankLines = renderedLineCount - lineCount;
                if (numberOfBlankLines > 0)
                {
                    string blankLine = new string(' ', Console.WindowWidth);
                    for (int i = 0; i < numberOfBlankLines; i++)
                    {
                        Console.SetCursorPosition(0, cursorTop + lineCount + i);
                        Console.WriteLine(blankLine);
                    }
                }

                renderedLineCount = lineCount;
                Console.CursorVisible = true;
                UpdateCursorPosition();
            }

            private void UpdateCursorPosition()
            {
                Console.CursorTop = cursorTop + currentLine;
                Console.CursorLeft = 2 + currentColumn;
            }

            public int CurrentLine
            {
                get => currentLine;
                set
                {
                    if (currentLine != value)
                    {
                        currentLine = value;
                        currentColumn = Math.Min(submissionDocument[currentLine].Length, currentColumn);
                        UpdateCursorPosition();
                    }
                }
            }
            public int CurrentColumn
            {
                get => currentColumn;
                set
                {
                    if (currentColumn != value)
                    {
                        currentColumn = value;
                        UpdateCursorPosition();
                    }
                }
            }
        }

        [AttributeUsage(AttributeTargets.Method)]
        protected sealed class MetaCommandAttribute : Attribute
        {
            public MetaCommandAttribute(string name, string description)
            {
                Name = name;
                Description = description;
            }

            public string Name { get; }
            public string Description { get; }
        }
        private class MetaCommand
        {
            public MetaCommand(string name, string description, MethodInfo methodInfo)
            {
                Name = name;
                Description = description;
                Method = methodInfo;
            }

            public string Name { get; }
            public string Description { get; }
            public MethodInfo Method { get; }
        }

        private void InitializeMetaCommands()
        {
            MethodInfo[] methods = GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            foreach (MethodInfo method in methods)
            {
                MetaCommandAttribute? attribute = (MetaCommandAttribute?)method.GetCustomAttribute(typeof(MetaCommandAttribute));
                if (attribute == null)
                    continue;

                MetaCommand metaCommand = new MetaCommand(attribute.Name, attribute.Description, method);
                metaCommands.Add(metaCommand);
            }
        }
        private void EvaluateMetaCommand(string input)
        {
            // Parse arguments
            List<string> arguments = new List<string>();
            bool inQuotes = false;
            int position = 1;
            StringBuilder sb = new StringBuilder();
            void CommitPendingArgument()
            {
                string arg = sb.ToString();
                if (!string.IsNullOrWhiteSpace(arg))
                    arguments.Add(arg);
                sb.Clear();
            }

            while (position < input.Length)
            {
                char current = input[position];
                char next = position + 1 >= input.Length ? '\0' : input[position + 1];

                if (char.IsWhiteSpace(current))
                {
                    if (!inQuotes)
                        CommitPendingArgument();
                    else
                        sb.Append(current);
                } else if (current == '\"')
                {
                    if (!inQuotes)
                        inQuotes = true;
                    else if (next == '\"')
                    {
                        sb.Append(current);
                        position++;
                    } else
                        inQuotes = false;
                } else
                {
                    sb.Append(current);
                }

                position++;
            }
            CommitPendingArgument();

            string? commandName = arguments.FirstOrDefault();
            if (arguments.Count > 0)
                arguments.RemoveAt(0);

            MetaCommand? command = metaCommands.SingleOrDefault(mc => mc.Name == commandName);
            if (command == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Invalid command {input}.");
                Console.ResetColor();
                return;
            }

            ParameterInfo[] parameters = command.Method.GetParameters();

            if (arguments.Count != parameters.Length)
            {
                string parameterNames = string.Join(" ", parameters.Select(p => $"<{p.Name}>"));
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"error: invalid number of arguments");
                Console.WriteLine($"usage: #{commandName} {parameterNames}");
                Console.ResetColor();
                return;
            }

            command.Method.Invoke(this, arguments.ToArray());
        }
        [MetaCommand("help", "Describes available meta commands.")]
        protected void EvaluateHelp()
        {
            int maxNameLength = metaCommands.Max(m => m.Name.Length);
            foreach (MetaCommand metaCommand in metaCommands.OrderBy(mc => mc.Name))
            {
                string paddedName = metaCommand.Name.PadRight(maxNameLength);
                Console.Out.WritePunctuation("#");
                Console.Out.WriteIdentifier(paddedName);
                Console.Out.WriteSpace();
                Console.Out.WritePunctuation(":");
                Console.Out.WriteSpace();
                Console.Out.WritePunctuation(metaCommand.Description);
                Console.Out.WriteLine();
            }
        }
        [MetaCommand("clear", "Clears the screen.")]
        protected void EvaluateClear()
        {
            Console.Clear();
        }
    }
}
