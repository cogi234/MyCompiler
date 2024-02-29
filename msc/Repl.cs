using MiniCompiler.CodeAnalysis;
using MiniCompiler.CodeAnalysis.Syntax;
using MiniCompiler.CodeAnalysis.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;

namespace MyCompiler
{
    internal abstract class Repl
    {
        private List<string> submissionHistory = new List<string>();
        private int submissionHistoryIndex;

        private bool done;

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
            SubmissionView view = new SubmissionView(document);

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
            } else if (key.Modifiers == ConsoleModifiers.Control)
            {
                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        HandleControlEnter(document, view);
                        break;
                }
            } else if (key.Modifiers == ConsoleModifiers.Shift)
            {
                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        HandleShiftEnter(document, view);
                        break;
                }
            }

            if (key.KeyChar >= ' ')
                HandleTyping(document, view, key.KeyChar.ToString());
        }

        private void HandleEscape(ObservableCollection<string> document, SubmissionView view)
        {
            document[view.CurrentLine] = string.Empty;
            view.CurrentColumn = 0;
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
            } else
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

        protected virtual void EvaluateMetaCommand(string input)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Invalid command {input}.");
            Console.ResetColor();
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
            private readonly ObservableCollection<string> submissionDocument;
            private readonly int cursorTop;
            private int renderedLineCount;
            private int currentLine;
            private int currentColumn;

            public SubmissionView(ObservableCollection<string> submissionDocument)
            {
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

                    Console.SetCursorPosition(0, cursorTop + lineCount);

                    Console.ForegroundColor = ConsoleColor.Green;
                    if (lineCount == 0)
                        Console.Write("» ");
                    else if (lineCount == submissionDocument.Count - 1)
                        Console.Write("« ");
                    else
                        Console.Write("· ");
                    Console.ResetColor();

                    Console.Write(line);

                    int remainder = Console.WindowWidth - line.Length - 2;
                    Console.WriteLine(new string(' ', remainder));
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
    }
}
