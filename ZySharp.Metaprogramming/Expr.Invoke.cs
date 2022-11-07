using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;

using ZySharp.Validation;

namespace ZySharp.Metaprogramming;

public static partial class Expr
{
#pragma warning disable IDE0079
#pragma warning disable S2436, S107

    /// <summary>
    /// Compiles and invokes the given <paramref name="expression"/>.
    /// </summary>
    /// <param name="expression">The <see cref="Expression"/> to invoke.</param>
    /// <returns>The result of the expression.</returns>
    /// <remarks>
    /// If used in an expression tree, any call to <see cref="Invoke{TResult}"/> is substituted for the result
    /// of the expression invocation when calling <see cref="Expand"/> on the tree.
    /// </remarks>
    [Pure, ExcludeFromCodeCoverage]
    public static TResult Invoke<TResult>(
        this Expression<Func<TResult>> expression)
    {
        ValidateArgument.For(expression, nameof(expression), v => v.NotNull());
        return expression.Compile().Invoke();
    }

    /// <inheritdoc cref="Invoke{TResult}"/>
    [Pure, ExcludeFromCodeCoverage]
    public static TResult Invoke<T1, TResult>(
        this Expression<Func<T1, TResult>> expression,
        T1 arg1)
    {
        ValidateArgument.For(expression, nameof(expression), v => v.NotNull());
        return expression.Compile().Invoke(arg1);
    }

    /// <inheritdoc cref="Invoke{TResult}"/>
    [Pure, ExcludeFromCodeCoverage]
    public static TResult Invoke<T1, T2, TResult>(
        this Expression<Func<T1, T2, TResult>> expression,
        T1 arg1, T2 arg2)
    {
        ValidateArgument.For(expression, nameof(expression), v => v.NotNull());
        return expression.Compile().Invoke(arg1, arg2);
    }

    /// <inheritdoc cref="Invoke{TResult}"/>
    [Pure, ExcludeFromCodeCoverage]
    public static TResult Invoke<T1, T2, T3, TResult>(
        this Expression<Func<T1, T2, T3, TResult>> expression,
        T1 arg1, T2 arg2, T3 arg3)
    {
        ValidateArgument.For(expression, nameof(expression), v => v.NotNull());
        return expression.Compile().Invoke(arg1, arg2, arg3);
    }

    /// <inheritdoc cref="Invoke{TResult}"/>
    [Pure, ExcludeFromCodeCoverage]
    public static TResult Invoke<T1, T2, T3, T4, TResult>(
        this Expression<Func<T1, T2, T3, T4, TResult>> expression,
        T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        ValidateArgument.For(expression, nameof(expression), v => v.NotNull());
        return expression.Compile().Invoke(arg1, arg2, arg3, arg4);
    }

    /// <inheritdoc cref="Invoke{TResult}"/>
    [Pure, ExcludeFromCodeCoverage]
    public static TResult Invoke<T1, T2, T3, T4, T5, TResult>(
        this Expression<Func<T1, T2, T3, T4, T5, TResult>> expression,
        T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
    {
        ValidateArgument.For(expression, nameof(expression), v => v.NotNull());
        return expression.Compile().Invoke(arg1, arg2, arg3, arg4, arg5);
    }

    /// <inheritdoc cref="Invoke{TResult}"/>
    [Pure, ExcludeFromCodeCoverage]
    public static TResult Invoke<T1, T2, T3, T4, T5, T6, TResult>(
        this Expression<Func<T1, T2, T3, T4, T5, T6, TResult>> expression,
        T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
    {
        ValidateArgument.For(expression, nameof(expression), v => v.NotNull());
        return expression.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6);
    }

    /// <inheritdoc cref="Invoke{TResult}"/>
    [Pure, ExcludeFromCodeCoverage]
    public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, TResult>(
        this Expression<Func<T1, T2, T3, T4, T5, T6, T7, TResult>> expression,
        T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
    {
        ValidateArgument.For(expression, nameof(expression), v => v.NotNull());
        return expression.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
    }

    /// <inheritdoc cref="Invoke{TResult}"/>
    [Pure, ExcludeFromCodeCoverage]
    public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
        this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>> expression,
        T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
    {
        ValidateArgument.For(expression, nameof(expression), v => v.NotNull());
        return expression.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
    }

    /// <inheritdoc cref="Invoke{TResult}"/>
    [Pure, ExcludeFromCodeCoverage]
    public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(
        this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression,
        T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
    {
        ValidateArgument.For(expression, nameof(expression), v => v.NotNull());
        return expression.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
    }

    /// <inheritdoc cref="Invoke{TResult}"/>
    [Pure, ExcludeFromCodeCoverage]
    public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(
        this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression,
        T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
    {
        ValidateArgument.For(expression, nameof(expression), v => v.NotNull());
        return expression.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
    }

    /// <inheritdoc cref="Invoke{TResult}"/>
    [Pure, ExcludeFromCodeCoverage]
    public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(
        this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> expression,
        T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
    {
        ValidateArgument.For(expression, nameof(expression), v => v.NotNull());
        return expression.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
    }

    /// <inheritdoc cref="Invoke{TResult}"/>
    [Pure, ExcludeFromCodeCoverage]
    public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(
        this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> expression,
        T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
    {
        ValidateArgument.For(expression, nameof(expression), v => v.NotNull());
        return expression.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
    }

    /// <inheritdoc cref="Invoke{TResult}"/>
    [Pure, ExcludeFromCodeCoverage]
    public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(
        this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>> expression,
        T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
    {
        ValidateArgument.For(expression, nameof(expression), v => v.NotNull());
        return expression.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
    }

    /// <inheritdoc cref="Invoke{TResult}"/>
    [Pure, ExcludeFromCodeCoverage]
    public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(
        this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>> expression,
        T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
    {
        ValidateArgument.For(expression, nameof(expression), v => v.NotNull());
        return expression.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
    }

    /// <inheritdoc cref="Invoke{TResult}"/>
    [Pure, ExcludeFromCodeCoverage]
    public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(
        this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>> expression,
        T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
    {
        ValidateArgument.For(expression, nameof(expression), v => v.NotNull());
        return expression.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
    }

    /// <inheritdoc cref="Invoke{TResult}"/>
    [Pure, ExcludeFromCodeCoverage]
    public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(
        this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>> expression,
        T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
    {
        ValidateArgument.For(expression, nameof(expression), v => v.NotNull());
        return expression.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
    }

#pragma warning restore S2436, S107
#pragma warning restore IDE0079
}
