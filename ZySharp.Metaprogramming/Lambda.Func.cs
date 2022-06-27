using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace ZySharp.Metaprogramming
{
    public static partial class Lambda
    {
        /// <summary>
        /// Returns the given anonymous method as a lambda function delegate.
        /// </summary>
        /// <param name="lambda">The anonymous method.</param>
        /// <returns>The lambda delegate.</returns>
        /// <remarks>
        /// If used in an expression tree, any call to <see cref="Func{TResult}"/> is replaced with the actual
        /// delegate when calling <see cref="Expr.Expand"/> on the tree.
        /// </remarks>
        [Pure, ExcludeFromCodeCoverage]
        public static Func<TResult> Func<TResult>(
            Func<TResult> lambda)
        {
            return lambda;
        }

        /// <inheritdoc cref="Func{TResult}"/>
        [Pure, ExcludeFromCodeCoverage]
        public static Func<T1, TResult> Func<T1, TResult>(
            Func<T1, TResult> lambda)
        {
            return lambda;
        }

        /// <inheritdoc cref="Func{TResult}"/>
        [Pure, ExcludeFromCodeCoverage]
        public static Func<T1, T2, TResult> Func<T1, T2, TResult>(
            Func<T1, T2, TResult> lambda)
        {
            return lambda;
        }

        /// <inheritdoc cref="Func{TResult}"/>
        [Pure, ExcludeFromCodeCoverage]
        public static Func<T1, T2, T3, TResult> Func<T1, T2, T3, TResult>(
            Func<T1, T2, T3, TResult> lambda)
        {
            return lambda;
        }

        /// <inheritdoc cref="Func{TResult}"/>
        [Pure, ExcludeFromCodeCoverage]
        public static Func<T1, T2, T3, T4, TResult> Func<T1, T2, T3, T4, TResult>(
            Func<T1, T2, T3, T4, TResult> lambda)
        {
            return lambda;
        }

        /// <inheritdoc cref="Func{TResult}"/>
        [Pure, ExcludeFromCodeCoverage]
        public static Func<T1, T2, T3, T4, T5, TResult> Func<T1, T2, T3, T4, T5, TResult>(
            Func<T1, T2, T3, T4, T5, TResult> lambda)
        {
            return lambda;
        }

        // TODO: T16
    }
}