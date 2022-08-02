using System;
using System.Linq.Expressions;

using Xunit;

using ZySharp.Metaprogramming.Extensions.Expressions;

namespace ZySharp.Metaprogramming.Tests
{
    public sealed partial class TestExprExtensions
    {
        [Fact]
        public void Snapshot()
        {
            var value = 42;

            var input = Lambda.Expr((int x) => x == value);
            var result = input.Snapshot();

            var param = Expression.Parameter(typeof(int), "x");
            var body = Expression.Equal(param, Expression.Constant(value));
            var expected = Expression.Lambda<Func<int, bool>>(body, param);

            AssertEqualExpression(expected, result);
        }

        [Fact]
        public void SnapshotIgnoreRegularMembers()
        {
            var instance = new TestClass { Field = 42 };

            var input = Lambda.Expr((int x) => x == instance.Field);
            var result = input.Snapshot();

            var param = Expression.Parameter(typeof(int), "x");
            var body = Expression.Equal(param, Expression.Field(Expression.Constant(instance), nameof(TestClass.Field)));
            var expected = Expression.Lambda<Func<int, bool>>(body, param);

            AssertEqualExpression(expected, result);
        }
    }
}