using System.Linq.Expressions;

using Xunit;

namespace ZySharp.Metaprogramming.Tests
{
    [Trait("Category", "Unit")]
    public sealed partial class Test
    {
        private static void AssertEqualExpression<TDelegate>(Expression<TDelegate> expected, Expression<TDelegate> actual)
        {
            Assert.Equal(expected.ToString(), actual.ToString());
        }
    }
}