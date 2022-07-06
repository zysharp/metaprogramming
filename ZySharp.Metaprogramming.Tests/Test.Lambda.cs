using Xunit;

namespace ZySharp.Metaprogramming.Tests
{
    public sealed partial class Test
    {
        [Fact]
        public void TestLambdaDelegateDeclaration()
        {
            var d = Lambda.Func((int x) => x == 42);

            Assert.True(d(42));
        }

        [Fact]
        public void TestLambdaDelegateAnonymousResultType()
        {
            var d = Lambda.Func((int x) => new { Value = x });

            Assert.Equal(42, d(42).Value);
        }

        [Fact]
        public void TestLambdaDelegateCurrying()
        {
            var d = Lambda.Func((int x) => Lambda.Func((int y) => x + y));

            Assert.Equal(42, d(40)(2));
        }

        [Fact]
        public void TestLambdaExpressionDeclaration()
        {
            var e = Lambda.Expr((int x) => x == 42);
            var d = e.Compile();

            Assert.True(d.Invoke(42));
        }

        [Fact]
        public void TestLambdaExpressionAnonymousResultType()
        {
            var e = Lambda.Expr((int x) => new { Value = x });
            var d = e.Compile();

            Assert.Equal(42, d(42).Value);
        }

        [Fact]
        public void TestLambdaExpressionCurrying()
        {
            var e = Lambda.Expr((int x) => Lambda.Func((int y) => x + y));
            var d = e.Compile();

            Assert.Equal(42, d(40)(2));
        }
    }
}