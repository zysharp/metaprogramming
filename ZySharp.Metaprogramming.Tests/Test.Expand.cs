using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Xunit;

using ZySharp.Metaprogramming.Extensions;

namespace ZySharp.Metaprogramming.Tests
{
    public sealed partial class Test
    {
        [Fact]
        public void TestExpandDelegate()
        {
            var inner = Lambda.Expr((int x) => x == 42);
            var outer = Lambda.Expr((IEnumerable<int> x) => x.Where(inner.Compile()));

            var expanded = outer.Expand()!;
            var expected = Lambda.Expr((IEnumerable<int> x) => x.Where(x => x == 42));

            AssertEqualExpression(expected, expanded);
        }

        [Fact]
        public void TestExpandDelegateInline()
        {
            var outer = Lambda.Expr((IEnumerable<int> x) => x.Where(Lambda.Func((int x) => x == 42)));

            var expanded = outer.Expand()!;
            var expected = Lambda.Expr((IEnumerable<int> x) => x.Where(x => x == 42));

            AssertEqualExpression(expected, expanded);
        }

        [Fact]
        public void TestExpandDelegateNonCaptured()
        {
            var outer = Lambda.Expr((IEnumerable<int> x) => x.Where(GetPredicateDelegate()));

            var expanded = outer.Expand()!;
            var expected = outer;

            AssertEqualExpression(expected, expanded);
        }

        [Fact]
        public void TestExpandExpression()
        {
            var inner = Lambda.Expr((int x) => x == 42);
            var outer = Lambda.Expr((IQueryable<int> x) => x.Where(inner));

            var expanded = outer.Expand()!;
            var expected = Lambda.Expr((IQueryable<int> x) => x.Where(x => x == 42));

            AssertEqualExpression(expected, expanded);
        }

        [Fact]
        public void TestExpandExpressionInline()
        {
            var outer = Lambda.Expr((IQueryable<int> x) => x.Where(Lambda.Expr((int x) => x == 42)));

            var expanded = outer.Expand()!;
            var expected = Lambda.Expr((IQueryable<int> x) => x.Where(x => x == 42));

            AssertEqualExpression(expected, expanded);
        }

        [Fact]
        public void TestExpandExpressionNonCaptured()
        {
            var outer = Lambda.Expr((IQueryable<int> x) => x.Where(GetPredicateExpression()));

            var expanded = outer.Expand()!;
            var expected = outer;

            AssertEqualExpression(expected, expanded);
        }

        [Fact]
        public void TestExpandCapture()
        {
            var num42 = 42;
            var outer = Lambda.Expr((int x) => x == num42.Capture());

            var expanded = outer.Expand()!;
            var expected = Lambda.Expr((int x) => x == 42).Snapshot()!;

            AssertEqualExpression(expected, expanded);
        }

        [Fact]
        public void TestExpandInvoke()
        {
            var lower = Lambda.Expr((int x) => x >= 10);
            var upper = Lambda.Expr((int x) => x <= 42);
            var outer = Lambda.Expr((IQueryable<int> x) => x.Where(x => lower.Invoke(x) && upper.Invoke(x)));

            var expanded = outer.Expand()!;
            var expected = Lambda.Expr((IQueryable<int> x) => x.Where(x => x >= 10 && x <= 42));

            AssertEqualExpression(expected, expanded);
        }

        [Fact]
        public void TestExpandInvokeInline()
        {
            var lower = Lambda.Expr((int x) => x >= 10);
            var upper = Lambda.Expr((int x) => x <= 42);
            var outer = Lambda.Expr((IQueryable<int> x) => x.Where(x => Expr.Invoke(x => x >= 10, x) &&
                                                                        Expr.Invoke(x => x <= 42, x)));

            var expanded = outer.Expand()!;
            var expected = Lambda.Expr((IQueryable<int> x) => x.Where(x => x >= 10 && x <= 42));

            AssertEqualExpression(expected, expanded);
        }

        [Fact]
        public void TestExpandInvokeNested()
        {
            var number = Lambda.Expr(() => 42);
            var inner = Lambda.Expr((int x) => x == number.Invoke());
            var outer = Lambda.Expr((IQueryable<int> x) => x.Where(x => inner.Invoke(x)));

            var expanded = outer.Expand()!;
            var expected = Lambda.Expr((IQueryable<int> x) => x.Where(x => x == 42));

            AssertEqualExpression(expected, expanded);
        }

        [Fact]
        public void TestExpandInvokeFromMethod()
        {
            var outer = Lambda.Expr((IQueryable<int> x) => x.Where(x => GetPredicateExpression().Invoke(x)));

            var expanded = outer.Expand()!;
            var expected = Lambda.Expr((IQueryable<int> x) => x.Where(x => x == 42));

            AssertEqualExpression(expected, expanded);
        }

        [Fact]
        public void TestExpandInvokeFromMethodWithParam()
        {
            var num42 = 42;
            var outer = Lambda.Expr((IQueryable<int> x) => x.Where(x => GetPredicateExpressionWithParam(num42).Invoke(x)));

            var expanded = outer.Expand()!;
            num42 = 1337;
            expanded = expanded.Snapshot()!;
            var expected = Lambda.Expr((IQueryable<int> x) => x.Where(x => x == 42));

            AssertEqualExpression(expected, expanded);
        }

        #region Internal

        private static Func<int, bool> GetPredicateDelegate()
        {
            return x => x == 42;
        }

        private static Expression<Func<int, bool>> GetPredicateExpression()
        {
            return x => x == 42;
        }

        private static Expression<Func<int, bool>> GetPredicateExpressionWithParam(int number)
        {
            return x => x == number;
        }

        #endregion Internal
    }
}