using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;

using ZySharp.Validation;

namespace ZySharp.Metaprogramming.Extensions.Expressions
{
    public static partial class ExprExtensions
    {
        /// <summary>
        /// Replaces <paramref name="oldNode"/> with <paramref name="newNode"/> in the given input expression tree.
        /// </summary>
        /// <typeparam name="TDelegate">The type of the lambda delegate.</typeparam>
        /// <param name="expression">The input expression tree.</param>
        /// <param name="oldNode">The old sub-expression.</param>
        /// <param name="newNode">The new sub-expression.</param>
        /// <returns>The transformed expression tree.</returns>
        [Pure]
        [return: NotNullIfNotNull("expression")]
        public static Expression<TDelegate>? Replace<TDelegate>(this Expression<TDelegate>? expression,
            Expression oldNode, Expression newNode)
        {
            return Replace((Expression?)expression, oldNode, newNode) as Expression<TDelegate>;
        }

        /// <inheritdoc cref="Replace{TDelegate}(Expression{TDelegate}?,Expression,Expression)"/>
        [Pure]
        [return: NotNullIfNotNull("expression")]
        public static Expression? Replace(this Expression? expression, Expression oldNode, Expression newNode)
        {
            ValidateArgument.For(oldNode, nameof(oldNode), v => v.NotNull());
            ValidateArgument.For(newNode, nameof(newNode), v => v.NotNull());

            var visitor = new ReplaceVisitor(oldNode, newNode);

            return visitor.Visit(expression);
        }

        /// <summary>
        /// Replaces multiple nodes in the given input expression tree.
        /// </summary>
        /// <typeparam name="TDelegate">The type of the lambda delegate.</typeparam>
        /// <param name="expression">The input expression tree.</param>
        /// <param name="map">A map that maps each expression nodes to their respective replacement nodes.</param>
        /// <returns>The resulting expression tree.</returns>
        [Pure]
        [return: NotNullIfNotNull("expression")]
        public static Expression<TDelegate>? Replace<TDelegate>(this Expression<TDelegate>? expression,
            IDictionary<Expression, Expression> map)
        {
            return Replace((Expression?)expression, map) as Expression<TDelegate>;
        }

        /// <inheritdoc cref="Replace{TDelegate}(Expression{TDelegate}?,Expression,Expression)"/>
        [Pure]
        [return: NotNullIfNotNull("expression")]
        public static Expression? Replace(this Expression? expression, IDictionary<Expression, Expression> map)
        {
            ValidateArgument.For(map, nameof(map), v => v.NotNull()
                .ForEach(x => x, v => v
                    .For(x => x.Key, v => v.NotNull())
                    .For(x => x.Value, v => v.NotNull())));

            var visitor = new ReplaceVisitor(map);

            return visitor.Visit(expression);
        }

        #region Internal

        private sealed class ReplaceVisitor :
            ExpressionVisitor
        {
            private readonly IDictionary<Expression, Expression> _map;

            public ReplaceVisitor(IDictionary<Expression, Expression> map)
            {
                Contract.Assert(map is not null);

                _map = map!;
            }

            public ReplaceVisitor(Expression oldExpression, Expression newExpression) :
                this(new Dictionary<Expression, Expression> { { oldExpression, newExpression } })
            {
                Contract.Assert(oldExpression is not null);
                Contract.Assert(newExpression is not null);
            }

            public override Expression? Visit(Expression? node)
            {
                if ((node is not null) && _map.TryGetValue(node, out var replacement))
                {
                    return replacement;
                }

                return base.Visit(node);
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