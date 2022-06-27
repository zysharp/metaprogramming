using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace ZySharp.Metaprogramming
{
    /// <summary>
    /// Provides the <see cref="Invoke{TResult}"/> and <see cref="Expand{TDelegate}"/> extensions used for expression
    /// tree concatenation.
    /// </summary>
    public static partial class Expr
    {
        /// <summary>
        /// Captures a constant snapshot of the passed value-type reference.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">A reference of the value to capture.</param>
        /// <returns>The current value of the passed value-type reference.</returns>
        /// <remarks>
        /// If used in an expression tree, any call to <see cref="Capture{T}"/> will be substituted for a constant
        /// snapshot of <paramref name="value"/> when calling <see cref="Expand"/> on the tree.
        /// </remarks>
        [Pure, ExcludeFromCodeCoverage]
        public static T Capture<T>(this ref T value)
            where T : struct
        {
            return value;
        }
    }
}