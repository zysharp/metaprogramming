using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using JetBrains.Annotations;

namespace ZySharp.Metaprogramming.Extensions
{
    public static partial class ExprExtensions
    {
        /// <summary>
        /// Evaluates the given expression and returns the resulting value.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>The result of the expression evaluation.</returns>
        /// <exception cref="EvaluationException">If the expression could not get evaluated correctly.</exception>
        /// <remarks>
        /// For <see cref="LambdaExpression"/> consider using <see cref="Delegate.DynamicInvoke"/> instead.
        /// </remarks>
        [Pure]
        public static object? Evaluate(this Expression? expression)
        {
            try
            {
                return expression switch
                {
                    null => null,
                    ConstantExpression constant => constant.Value,
                    MemberExpression { Member: FieldInfo field } member => field.GetValue(member.Expression.Evaluate()),
                    MemberExpression { Member: PropertyInfo property } member => property.GetValue(member.Expression.Evaluate()),
                    MethodCallExpression call => call.Method.Invoke(call.Object.Evaluate(), call.Arguments.Select(Evaluate).ToArray()),
                    _ => Expression.Lambda(expression).Compile().DynamicInvoke()
                };
            }
            catch (Exception e)
            {
                throw new EvaluationException(string.Format(CultureInfo.InvariantCulture,
                        Resources.CouldNotEvaluateExpression, expression?.ToString() ?? "null"),
                    e);
            }
        }

        /// <summary>
        /// Evaluates the given expression and returns the resulting value. This method throws an exception if the
        /// </summary>
        /// <typeparam name="T">The result value type.</typeparam>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns>The result of the expression evaluation.</returns>
        /// <exception cref="EvaluationException">If the expression could not get evaluated correctly.</exception>
        /// <exception cref="InvalidCastException">If the result value could not be converted to the desired type.</exception>
        /// <remarks>
        /// For <see cref="LambdaExpression"/> consider using <see cref="Delegate.DynamicInvoke"/> instead.
        /// </remarks>
        [Pure]
        public static T? Evaluate<T>(this Expression? expression)
        {
            return (T?)expression.Evaluate();
        }
    }
}