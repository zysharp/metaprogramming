using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;

using ZySharp.Validation;

namespace ZySharp.Metaprogramming.Extensions.Expressions
{
    public static partial class ExprExtensions
    {
        /// <summary>
        /// Checks if the input expression tree contains one- ore more of the given <paramref name="nodes"/>.
        /// </summary>
        /// <param name="expression">The input expression tree.</param>
        /// <param name="nodes">The expression nodes to search for.</param>
        /// <returns><c>True</c> if the input expression tree contains one- ore more of the given nodes.</returns>
        [Pure]
        public static bool Contains(this Expression expression, params Expression[] nodes)
        {
            ValidateArgument.For(expression, nameof(expression), v => v.NotNull());
            ValidateArgument.For(nodes, nameof(nodes), v => v.NotNull()
                .ForEach(x => x, v => v.NotNull()));

            var visitor = new ContainsVisitor(nodes);
            visitor.Visit(expression);

            return visitor.Found;
        }

        #region Visitor

        private sealed class ContainsVisitor :
            ExpressionVisitor
        {
            private readonly ISet<Expression> _expressions = new HashSet<Expression>();

            public bool Found { get; private set; }

            public ContainsVisitor(params Expression[] nodes)
            {
                ValidateArgument.For(nodes, nameof(nodes), v => v.NotNullOrEmpty()
                    .ForEach(x => x, v => v.NotNull()));

                foreach (var expression in nodes)
                {
                    _expressions.Add(expression);
                }
            }

            public override Expression? Visit(Expression? node)
            {
                if ((node is not null) && _expressions.Contains(node))
                {
                    Found = true;
                    return node;
                }

                return base.Visit(node);
            }
        }

        #endregion Visitor
    }
}