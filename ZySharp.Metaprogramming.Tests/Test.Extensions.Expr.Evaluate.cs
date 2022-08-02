using System;
using System.Linq.Expressions;

using Xunit;

using ZySharp.Metaprogramming.Extensions.Expressions;

namespace ZySharp.Metaprogramming.Tests
{
    public sealed partial class TestExprExtensions
    {
        [Fact]
        public void EvaluateConstant()
        {
            var value = 42;

            var input = Expression.Constant(value);
            var result = input.Evaluate();

            Assert.Equal(value, result);
        }

        [Fact]
        public void EvaluateField()
        {
            var instance = new TestClass
            {
                Field = 42
            };

            var input = Expression.Field(Expression.Constant(instance), nameof(TestClass.Field));
            var result = input.Evaluate();

            Assert.Equal(instance.Field, result);
        }

        [Fact]
        public void EvaluateProperty()
        {
            var instance = new TestClass
            {
                Prop = 42
            };

            var input = Expression.Property(Expression.Constant(instance), nameof(TestClass.Prop));
            var result = input.Evaluate();

            Assert.Equal(instance.Prop, result);
        }

        [Fact]
        public void EvaluateMethodCall()
        {
            var instance = new TestClass
            {
                Field = 42
            };

            var input = Expression.Call(Expression.Constant(instance),
                typeof(TestClass).GetMethod(nameof(TestClass.MethodCall))!, Array.Empty<Expression>());
            var result = input.Evaluate();

            Assert.Equal(instance.Field, result);
        }

        [Fact]
        public void EvaluateLambda()
        {
            var input = Lambda.Expr((int x) => x == 42);
            var result = input.Evaluate();

            Assert.Equal(typeof(Func<int, bool>), result!.GetType());
        }

        [Fact]
        public void EvaluateOther()
        {
            var input = Expression.Add(Expression.Constant(40), Expression.Constant(2));
            var result = input.Evaluate();

            Assert.Equal(42, result);
        }

        [Fact]
        public void EvaluateFailure()
        {
            var input = Expression.Add(Expression.Parameter(typeof(int)), Expression.Constant(0));

            Assert.Throws<EvaluationException>(() => input.Evaluate());
        }

        [Fact]
        public void EvaluateCast()
        {
            var value = 42;

            var input = Expression.Constant(value);
            var result = input.Evaluate<int>();

            Assert.Equal(value, result);
        }

        #region Internal

        private sealed class TestClass
        {
            public int Field;

            public int Prop { get; set; }

            public int MethodCall()
            {
                return Field;
            }
        }

        #endregion Internal
    }
}