using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Xunit;

using ZySharp.Metaprogramming.Extensions.Expressions;

namespace ZySharp.Metaprogramming.Tests
{
    public sealed partial class TestExprExtensions
    {
        [Fact]
        public void ReplaceSingle()
        {
            var input = Lambda.Expr((int a, int b) => a + b);

            var nodeOld = input.Parameters.First();
            var nodeNew = Expression.Parameter(typeof(int), "x");
            var result = input.Replace(nodeOld, nodeNew)!;

            var expected = Lambda.Expr((int x, int b) => x + b);

            AssertEqualExpression(expected, result);
        }

        [Fact]
        public void ReplaceMultiple()
        {
            var input = Lambda.Expr((int a, int b) => a + b);

            var map = new Dictionary<Expression, Expression>
            {
                { input.Parameters[0], Expression.Parameter(typeof(int), "x") },
                { input.Parameters[1], Expression.Parameter(typeof(int), "y") },
            };
            var result = input.Replace(map)!;

            var expected = Lambda.Expr((int x, int y) => x + y);

            AssertEqualExpression(expected, result);
        }
    }
}