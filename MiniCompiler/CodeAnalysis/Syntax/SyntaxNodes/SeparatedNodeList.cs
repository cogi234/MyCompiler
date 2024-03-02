using System.Collections;
using System.Collections.Immutable;

namespace MiniCompiler.CodeAnalysis.Syntax.SyntaxNodes
{
    public abstract class SeparatedNodeList
    {
        public abstract ImmutableArray<SyntaxNode> GetNodes();
        public abstract ImmutableArray<Token> GetSeparators();
    }
    public sealed class SeparatedNodeList<T> : SeparatedNodeList, IEnumerable<T>
        where T : SyntaxNode
    {
        private readonly ImmutableArray<SyntaxNode> nodes;
        private readonly ImmutableArray<Token> separators;

        public SeparatedNodeList(ImmutableArray<SyntaxNode> nodes, ImmutableArray<Token> separators)
        {
            this.nodes = nodes;
            this.separators = separators;
        }

        public int Count => nodes.Length;
        public T this[int index] => (T)nodes[index];
        public Token GetSeparator(int index) => separators[index];

        public override ImmutableArray<SyntaxNode> GetNodes() => nodes;
        public override ImmutableArray<Token> GetSeparators() => separators;


        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return this[i];
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    }
}
