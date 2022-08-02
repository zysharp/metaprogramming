using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using ZySharp.Metaprogramming.Extensions.Expressions;
using ZySharp.Metaprogramming.Extensions.Expressions.Member;
using ZySharp.Metaprogramming.Extensions.Expressions.MethodCall;

namespace ZySharp.Metaprogramming
{
    public static partial class Expr
    {
        /// <summary>
        /// Expands the given expression tree.
        /// </summary>
        /// <typeparam name="TDelegate">The type of the lambda expression delegate.</typeparam>
        /// <param name="expression">The input expression tree.</param>
        /// <returns>The expanded expression tree.</returns>
        /// <remarks>
        /// This method expands the given <paramref name="expression"/> tree by replacing any method call expression
        /// targeting one of the following methods:
        /// <see cref="Lambda.Expr{T}"/>,
        /// <see cref="LambdaExpression.Compile()"/>,
        /// <see cref="Capture{T}"/>,
        /// <see cref="Invoke{TResult}"/>.
        /// </remarks>
        [Pure]
        [return: NotNullIfNotNull("expression")]
        public static Expression<TDelegate>? Expand<TDelegate>(this Expression<TDelegate>? expression)
        {
            return Expand((Expression?)expression) as Expression<TDelegate>;
        }

        /// <inheritdoc cref="Expand{TDelegate}"/>
        [Pure]
        [return: NotNullIfNotNull("expression")]
        public static Expression? Expand(this Expression? expression)
        {
            return new ExpandVisitor().Visit(expression);
        }

        #region Visitor

#pragma warning disable S125

        private sealed class ExpandVisitor :
            ExpressionVisitor
        {
            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                // Expand calls to `Capture` ...

                if (node.TargetsMethod(typeof(Metaprogramming.Expr), nameof(Metaprogramming.Expr.Capture)))
                {
                    var target = node.Arguments.First();
                    Contract.Assert(target is MemberExpression);

                    return Expression.Constant(target.Evaluate(), node.Type);
                }

                // Expand calls to `Invoke`

                // var p = Lambda.Expr((int i) => i > 1);
                // var e = Lambda.Expr((IQueryable<int> q) => q.Where(x => p.Invoke(x)));
                //                                                :. Expression
                // => q => q.Where(i => i > 1)

                // Completely evaluates 'p'. For e.g. method calls, a snapshot of value type arguments
                // might indirectly get captured by the resulting expression:

                // public static Expression<Func<int, bool>> GetPredicate(int threshold)
                // {
                //     return i => i > threshold;
                // }
                //
                // var t = 1;
                // var e = Lambda.Expr((IQueryable<int> q) => q.Where(x => GetPredicate(t).Invoke(x)));
                //                                                :. Expression
                // => q => q.Where(i => i > 1)
                //                          :. Actually a closure member, but a different one than `t`

                // Otherwise captured variables will remain references to the closure fields:

                // var t = 1;
                // var p = Lambda.Expr((int i) => i > t);
                // var e = Lambda.Expr((IQueryable<int> q) => q.Where(x => p.Invoke(x)));
                //                                                :. Expression
                // => q => q.Where(i => i > t)

                if (node.TargetsMethod(typeof(Metaprogramming.Expr), nameof(Metaprogramming.Expr.Invoke)))
                {
                    var target = node.Arguments.First();
                    var lambda = EvaluateTarget(target);

                    Contract.Assert(lambda.Parameters.Count == (node.Arguments.Count - 1));

                    var parameterMap = new Dictionary<Expression, Expression>();
                    for (var i = 0; i < lambda.Parameters.Count; i++)
                    {
                        parameterMap.Add(lambda.Parameters[i], Visit(node.Arguments[i + 1]));
                    }

                    var body = lambda.Body.Replace(parameterMap);

                    return Visit(body)!;
                }

                // Expand calls to `Compile` ...

                // var p = Lambda.Expr((int i) => i > 1);
                // var e = Lambda.Expr((IEnumerable<int> q) => q.Where(p.Compile()));
                //                                                 :. Func
                // => q => q.Where(i => i > 1)

                // If 'p' is not a captured variable inside a closure, no transformation will be
                // applied.

                if ((node.TargetsMethod(typeof(LambdaExpression), nameof(LambdaExpression.Compile), true)) &&
                    (node.Object is MemberExpression member) && (member.IsClosureMember()))
                {
                    return VisitMember(member);
                }

                // Expand calls to `Lambda.Expr` and `Lambda.Func` ...

                if (node.TargetsMethod(typeof(Lambda), nameof(Lambda.Expr)) ||
                    node.TargetsMethod(typeof(Lambda), nameof(Lambda.Func)))
                {
                    return node.Arguments.First();
                }

                return base.VisitMethodCall(node);
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                if (!node.IsClosureMember() || !ReturnsExpression(node))
                {
                    return base.VisitMember(node);
                }

                // Collapse captured expression variables...

                // var p = Lambda.Expr((int i) => i > 1);
                // var e = Lambda.Expr((IQueryable<int> q) => q.Where(p));
                //                                                :. Expression
                // => q => q.Where(i => i > 1)

                // If 'p' is not a captured variable inside a closure, no transformation will be
                // applied.

                var closure = (ConstantExpression)node.Expression!;
                var field = (FieldInfo)node.Member;

                if ((field.GetValue(closure.Value) is Expression expression))
                {
                    return Visit(expression)!;
                }

                return base.VisitMember(node);
            }

            /// <summary>
            /// Checks if the given <paramref name="node"/> refers to a member that returns an <see cref="Expression"/>
            /// type.
            /// </summary>
            /// <param name="node">The expression to check.</param>
            /// <returns><c>True</c>, if the expression refers to a member that returns an expression type.</returns>
            private static bool ReturnsExpression(MemberExpression node)
            {
                return node.Type.IsSubclassOf(typeof(Expression));
            }

            /// <summary>
            /// Evaluates the <paramref name="target"/> expression passed to <see cref="Metaprogramming.Expr.Invoke{TResult}"/> calls
            /// and returns the resulting lambda expression tree.
            /// </summary>
            /// <param name="target">The target expression to evaluate.</param>
            /// <returns>The resulting lambda expression tree.</returns>
            /// <exception cref="InvalidOperationException"></exception>
            private static LambdaExpression EvaluateTarget(Expression? target)
            {
                if (target is UnaryExpression { NodeType: ExpressionType.Quote, Operand: LambdaExpression lambda })
                {
                    return lambda;
                }

                var eval = (object?)null;
                try
                {
                    eval = target.Evaluate();
                }
                catch (EvaluationException)
                {
                    // Suppress `EvaluationException` and return `null` instead
                }

                if (eval is not LambdaExpression result)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture,
                        Resources.CouldNotEvaluateTarget, target?.ToString() ?? "null"));
                }

                return result;
            }
        }

#pragma warning restore S125

        #endregion Visitor
    }
}