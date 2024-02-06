using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompiler.CodeAnalysis.Binding
{
    enum BoundNodeType
    {
        LiteralExpression,
        UnaryExpression,
        BinaryExpression
    }

    internal abstract class BoundNode
    {
        public abstract BoundNodeType BoundNodeType { get; }
        public abstract IEnumerable<BoundNode> GetChildren();
    }
}
