using System.Linq;

using Xunit;

using ZySharp.Metaprogramming.Extensions.Expressions;

namespace ZySharp.Metaprogramming.Tests
{
    public sealed partial class TestExprExtensions
    {
        [Fact]
        public void Flatten()
        {
            var input = Lambda.Expr((int a, int b) => a + b);

            var result = input.Flatten().ToList();

            Assert.Equal(6, result.Count);

            // (a, b) => a + b
            Assert.Equal(0, result[0].Id);
            Assert.Equal(0, result[0].Level);
            Assert.Equal(input, result[0].Node);

            // a + b
            Assert.Equal(1, result[1].Id);
            Assert.Equal(1, result[1].Level);
            Assert.Equal(input.Body, result[1].Node);

            // a
            Assert.Equal(2, result[2].Id);
            Assert.Equal(2, result[2].Level);
            Assert.Equal(input.Parameters[0], result[2].Node);

            // b
            Assert.Equal(3, result[3].Id);
            Assert.Equal(2, result[3].Level);
            Assert.Equal(input.Parameters[1], result[3].Node);

            // a
            Assert.Equal(2, result[4].Id);
            Assert.Equal(1, result[4].Level);
            Assert.Equal(input.Parameters[0], result[4].Node);

            // b
            Assert.Equal(3, result[5].Id);
            Assert.Equal(1, result[5].Level);
            Assert.Equal(input.Parameters[1], result[5].Node);
        }
    }
}