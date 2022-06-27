using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

using Xunit;

using ZySharp.Metaprogramming.Extensions.Expressions;

namespace ZySharp.Metaprogramming.Tests
{
    public sealed partial class TestExprExtensions
    {
        [Fact]
        public void Reduce()
        {
            var input = Expression.Block(
                Expression.MakeBinary(ExpressionType.AddAssign,
                    Expression.Parameter(typeof(int)),
                    Expression.Constant(2)
                )
            );

            Assert.NotEqual(input, input.ReduceRecursive());
            Assert.Equal(input, input.ReduceExtensionsRecursive());
        }

        [Fact]
        public void ReduceExtensions()
        {
            var input = Expression.Block(
                new ExtensionExpression()
            );

            Assert.NotEqual(input, input.ReduceRecursive());
            Assert.NotEqual(input, input.ReduceExtensionsRecursive());
        }

        #region Internal

        [ExcludeFromCodeCoverage]
        private sealed class ExtensionExpression :
            Expression
        {
            public override ExpressionType NodeType => ExpressionType.Extension;

            public override bool CanReduce => true;

            public override Type Type => typeof(int);

            public override Expression Reduce()
            {
                return Constant(0);
            }
        }

        #endregion Internal
    }
}