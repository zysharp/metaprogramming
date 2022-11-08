using System.Linq.Expressions;

using Xunit;

namespace ZySharp.Metaprogramming.Tests;

[Trait("Category", "Unit")]
public sealed partial class TestExprExtensions
{
    private static void AssertEqualExpression<TDelegate>(Expression<TDelegate> expected, Expression<TDelegate> actual)
    {
        Assert.Equal(expected, actual, ExprEqualityComparer.Default);
    }
}
