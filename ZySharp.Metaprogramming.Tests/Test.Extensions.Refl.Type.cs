using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Xunit;

using ZySharp.Metaprogramming.Extensions.Reflection;

namespace ZySharp.Metaprogramming.Tests
{
    public sealed partial class TestReflExtensions
    {
        [Fact]
        public void IsSameOrSubclass()
        {
            Assert.True(typeof(string).IsSameOrSubclass(typeof(string)));
            Assert.True(typeof(string).IsSameOrSubclass(typeof(object)));
            Assert.False(typeof(string).IsSameOrSubclass(typeof(int)));
        }

        [Fact]
        public void IsConstructedFrom()
        {
            Assert.True(typeof(Func<int>).IsConstructedFrom(typeof(Func<>)));
            Assert.False(typeof(Func<int>).IsConstructedFrom(typeof(Func<,>)));
            Assert.False(typeof(Func<int>).IsConstructedFrom(typeof(Action<>)));
            Assert.False(typeof(int).IsConstructedFrom(typeof(Func<>)));
            Assert.False(typeof(Func<int>).IsConstructedFrom(typeof(int)));
        }

        [Fact]
        public void GetDerivedTypes()
        {
            var expected = typeof(DerivedIntList);
            var types = typeof(List<int>).GetDerivedTypes(expected.Assembly);

            Assert.Contains(expected, types);
        }

        #region Internal

        [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Intended")]
        private sealed class DerivedIntList : List<int>
        {
        }

        #endregion Internal
    }
}