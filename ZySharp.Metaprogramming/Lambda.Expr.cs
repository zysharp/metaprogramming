using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

using JetBrains.Annotations;

namespace ZySharp.Metaprogramming
{
    public static partial class Lambda
    {
        /// <summary>
        /// Returns the given anonymous method as a lambda expression tree.
        /// </summary>
        /// <param name="lambda">The anonymous method.</param>
        /// <returns>The lambda expression tree.</returns>
        /// <remarks>
        /// If used in an expression tree, any call to <see cref="Expr{TResult}"/> is substituted for the actual
        /// expression when calling <see cref="Expr.Expand"/> on the tree.
        /// </remarks>
        [Pure, ExcludeFromCodeCoverage]
        public static Expression<Func<TResult>> Expr<TResult>(
            Expression<Func<TResult>> lambda)
        {
            return lambda;
        }

        /// <inheritdoc cref="Expr{TResult}"/>
        [Pure, ExcludeFromCodeCoverage]
        public static Expression<Func<T1, TResult>> Expr<T1, TResult>(
            Expression<Func<T1, TResult>> lambda)
        {
            return lambda;
        }

        /// <inheritdoc cref="Expr{TResult}"/>
        [Pure, ExcludeFromCodeCoverage]
        public static Expression<Func<T1, T2, TResult>> Expr<T1, T2, TResult>(
            Expression<Func<T1, T2, TResult>> lambda)
        {
            return lambda;
        }

        /// <inheritdoc cref="Expr{TResult}"/>
        [Pure, ExcludeFromCodeCoverage]
        public static Expression<Func<T1, T2, T3, TResult>> Expr<T1, T2, T3, TResult>(
            Expression<Func<T1, T2, T3, TResult>> lambda)
        {
            return lambda;
        }

        /// <inheritdoc cref="Expr{TResult}"/>
        [Pure, ExcludeFromCodeCoverage]
        public static Expression<Func<T1, T2, T3, T4, TResult>> Expr<T1, T2, T3, T4, TResult>(
            Expression<Func<T1, T2, T3, T4, TResult>> lambda)
        {
            return lambda;
        }

        /// <inheritdoc cref="Expr{TResult}"/>
        [Pure, ExcludeFromCodeCoverage]
        public static Expression<Func<T1, T2, T3, T4, T5, TResult>> Expr<T1, T2, T3, T4, T5, TResult>(
            Expression<Func<T1, T2, T3, T4, T5, TResult>> lambda)
        {
            return lambda;
        }

        // TODO: T16
    }
}