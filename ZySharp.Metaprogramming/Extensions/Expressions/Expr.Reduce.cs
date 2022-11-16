using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;

namespace ZySharp.Metaprogramming.Extensions.Expressions;

public static partial class ExprExtensions
{
    /// <summary>
    /// Recursively reduces all nodes of the given expression tree to simpler expressions.
    /// </summary>
    /// <param name="expression">The expression tree to reduce.</param>
    /// <returns>The reduced expression tree.</returns>
    /// <remarks>
    /// Nodes are ignored if <see cref="Expression.CanReduce"/> is <c>false</c> while an exception is thrown if
    /// a reduced node does not satisfies certain invariants (see <see cref="Expression.ReduceAndCheck()"/>).
    /// </remarks>
    [Pure]
    [return: NotNullIfNotNull(nameof(expression))]
    public static Expression? ReduceRecursive(this Expression? expression)
    {
        return new ReduceVisitor(false).Visit(expression);
    }

    /// <summary>
    /// Recursively reduces all extension nodes of the given expression tree to known expressions.
    /// </summary>
    /// <param name="expression">The expression tree to reduce.</param>
    /// <returns>The reduced expression tree.</returns>
    /// <remarks>
    /// Nodes are ignored if <see cref="Expression.CanReduce"/> is <c>false</c> while an exception is thrown if
    /// a reduced node does not satisfies certain invariants (see <see cref="Expression.ReduceAndCheck()"/>).
    /// </remarks>
    [Pure]
    [return: NotNullIfNotNull(nameof(expression))]
    public static Expression? ReduceExtensionsRecursive(this Expression? expression)
    {
        return new ReduceVisitor(true).Visit(expression);
    }

    #region Internal

    private sealed class ReduceVisitor :
        ExpressionVisitor
    {
        private readonly bool _extensionsOnly;

        public ReduceVisitor(bool extensionsOnly)
        {
            _extensionsOnly = extensionsOnly;
        }

        public override Expression? Visit(Expression? node)
        {
            if ((node is not null) && (node.CanReduce) &&
                ((!_extensionsOnly) || (node.NodeType == ExpressionType.Extension)))
            {
                return base.Visit(node.ReduceAndCheck());
            }

            return base.Visit(node);
        }
    }

    #endregion Internal
}
