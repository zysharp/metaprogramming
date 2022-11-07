using System;
using System.Linq.Expressions;

namespace ZySharp.Metaprogramming;

/// <summary>
/// Optional flags to modify the <see cref="ExprEqualityComparer"/> behavior.
/// </summary>
[Flags]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S2344", Justification = "won't fix")]
public enum ExprEqualityComparerFlags
{
    /// <summary>
    /// Do not use any flags.
    /// </summary>
    None = 0,

    /// <summary>
    /// The default flags for the <see cref="ExprEqualityComparer"/> parameter-less constructor.
    /// </summary>
    Default = IgnoreLabelName | IgnoreLambdaName | IgnoreLambdaType | IgnoreParameterName,

    /// <summary>
    /// The <see cref="LabelTarget.Name"/> property is used for debugging only. This flag configures the
    /// <see cref="ExprEqualityComparer"/> to ignore the name when comparing or hashing.
    /// </summary>
    IgnoreLabelName = 1 << 0,

    /// <summary>
    /// The <see cref="LambdaExpression.Name"/> property is used for debugging only. This flag configures the
    /// <see cref="ExprEqualityComparer"/> to ignore the name when comparing or hashing.
    /// </summary>
    IgnoreLambdaName = 1 << 1,

    /// <summary>
    /// The <see cref="LambdaExpression.Type"/> property might differ for two otherwise equal lambda expressions
    /// with the exact same signature. This is the case if e.g. a custom delegate type instead of
    /// e.g. <see cref="Func{TResult}"/> is used. This flag configures the <see cref="ExprEqualityComparer"/>
    /// to ignore the <b>node</b>-type when comparing or hashing. Note that the <b>return</b>-type is not
    /// ignored.
    /// </summary>
    IgnoreLambdaType = 1 << 2,

    /// <summary>
    /// The <see cref="ParameterExpression.Name"/> property is used for debugging only. This flag configures the
    /// <see cref="ExprEqualityComparer"/> to ignore the name when comparing or hashing.
    /// </summary>
    IgnoreParameterName = 1 << 3
}
