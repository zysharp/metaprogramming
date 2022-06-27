using System;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;

namespace ZySharp.Metaprogramming.Extensions.Expressions.MethodCall
{
    /// <summary>
    /// Provides various general purpose extension methods for the <see cref="MethodCallExpression"/> class.
    /// </summary>
    public static class MethodCallExprExtensions
    {
        /// <summary>
        /// Checks if the given <see cref="MethodCallExpression"/> targets a specific method.
        /// </summary>
        /// <param name="node">The expression to check.</param>
        /// <param name="declaringType">The declaring type of the method to check for.</param>
        /// <param name="name">The name of the method to check for.</param>
        /// <param name="includeSubclasses">Set <c>true</c> to allow </param>
        /// <returns><c>True</c> if the expression targets the specified method.</returns>
        [Pure]
        public static bool TargetsMethod(this MethodCallExpression? node, Type declaringType, string name,
            bool includeSubclasses = false)
        {
            if (node is null)
            {
                return false;
            }

            if (node.Method.Name != name)
            {
                return false;
            }

            if (node.Method.DeclaringType == declaringType)
            {
                return true;
            }

            if (!includeSubclasses || (node.Method.DeclaringType is null))
            {
                return false;
            }

            return node.Method.DeclaringType.IsSubclassOf(declaringType);
        }
    }
}