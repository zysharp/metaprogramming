using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;

namespace ZySharp.Metaprogramming.Extensions.Expressions
{
    public static partial class ExprExtensions
    {
        /// <summary>
        /// Performs a "depth-first" traversal of the given expression tree and returns a list of nodes and their
        /// levels in the original tree.
        /// </summary>
        /// <param name="expression">The expression tree.</param>
        /// <returns>A list of all individual nodes in the expression tree.</returns>
        /// <remarks>
        /// This method preserves the original order in which the nodes are traversed by a
        /// <see cref="ExpressionVisitor"/>.
        /// </remarks>
        [Pure]
        public static IEnumerable<(Expression? Node, int Id, int Level)> Flatten(this Expression? expression)
        {
            var visitor = new FlatteningVisitor();
            visitor.Visit(expression);

            return visitor.Nodes;
        }

        #region Internal

        private sealed class FlatteningVisitor :
            ExpressionVisitor
        {
            private readonly Dictionary<Expression, int> _ids = new();
            private int _lastId;
            private int _level;

            public List<(Expression? Node, int Id, int Level)> Nodes { get; } = new();

            public override Expression? Visit(Expression? node)
            {
                if (node is null)
                {
                    return base.Visit(node);
                }

                if (!_ids.TryGetValue(node, out var id))
                {
                    id = _lastId++;
                    _ids[node] = id;
                }

                Nodes.Add((node, id, _level));

                ++_level;
                var result = base.Visit(node);
                --_level;

                return result;
            }

            protected override Expression VisitExtension(Expression node)
            {
                // Prevent reduction of extension expressions..
                return node;
            }
        }

        #endregion Internal
    }
}