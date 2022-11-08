using System;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ZySharp.Metaprogramming.Extensions.Expressions.Member;

/// <summary>
/// Provides various general purpose extension methods for the <see cref="MemberExpression"/> class.
/// </summary>
public static class MemberExprExtensions
{
    /// <summary>
    /// Checks if the given <see cref="MemberExpression"/> refers to the member of a compiler generated closure
    /// class that points to a captured variable.
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

        return (node is
        {
            Member:
            {
                MemberType: MemberTypes.Field,
                DeclaringType: { IsNestedPrivate: true } declaringType
            },
            Expression: ConstantExpression { Value: not null }
        })
        &&
        declaringType.IsDefined(typeof(CompilerGeneratedAttribute)) &&
        declaringType.Name.StartsWith("<>", StringComparison.Ordinal);
    }
}
