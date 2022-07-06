using System;
using System.Linq.Expressions;
using System.Reflection;

using JetBrains.Annotations;

namespace ZySharp.Metaprogramming.Internal
{
    internal static class ExprExtensions
    {
        /// <summary>
        /// Checks if the given <see cref="MemberExpression"/> refers to the member of a compiler
        /// generated closure class that points to a captured variable.
        /// </summary>
        /// <param name="node">The expression to check.</param>
        /// <returns><c>True</c> if the expression refers to a closure member.</returns>
        [Pure]
        public static bool IsClosureMember(this MemberExpression? node)
        {
            if (node is null)
            {
                return false;
            }

            var declaringType = node!.Member.DeclaringType;

            return (declaringType is not null) && (declaringType.IsNestedPrivate) &&
                   (declaringType.Name.StartsWith("<>")) &&
                   (node.Expression is ConstantExpression { Value: not null }) &&
                   (node.Member.MemberType == MemberTypes.Field);
        }

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