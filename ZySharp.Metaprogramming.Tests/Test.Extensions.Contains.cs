using System;
using System.Linq;
using System.Linq.Expressions;

using Xunit;

using ZySharp.Metaprogramming.Extensions;

namespace ZySharp.Metaprogramming.Tests
{
    public sealed partial class Test
    {
        [Fact]
        public void TestContainsFound()
        {
            var input = Lambda.Expr((int x) => x == 42);

            var param = input.Parameters.First();
            var body = input.Body;

            Assert.True(body.Contains(param));
        }

        [Fact]
        public void TestContainsNotFound()
        {
            var input = Lambda.Expr((int x) => x == 42);

            var param = Expression.Parameter(typeof(int), "x");
            var body = input.Body;

            Assert.False(body.Contains(param));
        }
    }
}