using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

using Microsoft.CSharp.RuntimeBinder;

using Xunit;

namespace ZySharp.Metaprogramming.Tests;

[Trait("Category", "Unit")]
public sealed class TestEqualityComparer
{
    [Fact]
    public void EqualsBinary()
    {
        var x = Lambda.Expr(() => 1 + 2);
        var y = Lambda.Expr(() => 1 + 2);
        AssertEqual(x.Body, y.Body);
        var z = Lambda.Expr(() => 1 - 2);
        AssertNotEqual(x.Body, z.Body);
    }

    [Fact]
    [SuppressMessage("Minor Code Smell", "S3220", Justification = "intended")]
    public void EqualsBlock()
    {
        var param = Expression.Parameter(typeof(int));

        var x = Expression.Block(new[] { param }, Expression.Increment(param));
        var y = Expression.Block(new[] { param }, Expression.Increment(param));
        AssertEqual(x, y);
        var z = Expression.Block(new[] { param }, Expression.Decrement(param));
        AssertNotEqual(x, z);
    }

    [Fact]
    public void EqualsConditional()
    {
        var x = Lambda.Expr((bool x) => x ? 1 : 2);
        var y = Lambda.Expr((bool x) => x ? 1 : 2);
        AssertEqual(x.Body, y.Body);
        var z = Lambda.Expr((bool x) => x ? 1 : 3);
        AssertNotEqual(x.Body, z.Body);
    }

    public static IEnumerable<object[]> DataEqualsConstant => new List<object[]>
    {
        new object[] { new(), new(), false }, // Reference type with default 'Equals' method
        new object[] {     1,     1, true  }, // Value type
        new object[] {     1,     2, false }, // Value type
        new object[] {   "1",   "1", true  }, // Reference type with custom 'Equals' method
        new object[] {   "1",   "2", false }  // Reference type with custom 'Equals' method
    };

    [Theory]
    [MemberData(nameof(DataEqualsConstant))]
    public void EqualsConstant(object? a, object? b, bool expected)
    {
        var x = Expression.Constant(a);
        var y = Expression.Constant(b);

        if (expected)
        {
            AssertEqual(x, y);
        }
        else
        {
            AssertNotEqual(x, y);
        }
    }

    [Fact]
    public void EqualsDebugInfo()
    {
        var doc = Expression.SymbolDocument("test.cs");

        var x = Expression.DebugInfo(doc, 1, 1, 2, 1);
        var y = Expression.DebugInfo(doc, 1, 1, 2, 1);
        AssertEqual(x, y);
        var z = Expression.DebugInfo(doc, 1, 1, 2, 2);
        AssertNotEqual(x, z);
    }

    [Fact]
    public void EqualsDefault()
    {
        var x = Expression.Default(typeof(int));
        var y = Expression.Default(typeof(int));
        AssertEqual(x, y);
        var z = Expression.Default(typeof(string));
        AssertNotEqual(x, z);
    }

    [Fact]
    public void EqualsDynamic()
    {
        dynamic obj = new ExpandoObject();
        obj.key = "value";

        var x = Expression.MakeDynamic(
            typeof(Func<CallSite, object, string>),
            Binder.Convert(0, typeof(string), typeof(TestEqualityComparer)),
            Expression.MakeDynamic(typeof(Func<CallSite, object, object>),
                Binder.GetMember(
                    CSharpBinderFlags.None,
                    "key",
                    typeof(TestEqualityComparer),
                    new[] { CSharpArgumentInfo.Create(0, null) }),
                Expression.Constant(obj)
            )
        );

        Assert.Throws<NotSupportedException>(() => ExprEqualityComparer.Default.Equals(x, x));
        Assert.Throws<NotSupportedException>(() => ExprEqualityComparer.Default.GetHashCode(x));
    }

    [Fact]
    public void EqualsExtension()
    {
        var x = new ExtensionExpression();

        Assert.Throws<NotImplementedException>(() => ExprEqualityComparer.Default.Equals(x, x));
        Assert.Throws<NotImplementedException>(() => ExprEqualityComparer.Default.GetHashCode(x));
    }

    [Fact]
    public void EqualsGoto()
    {
        var target = Expression.Label();

        var x = Expression.Goto(target);
        var y = Expression.Goto(target);
        AssertEqual(x, y);
        var z = Expression.Return(target);
        AssertNotEqual(x, z);
    }

    [Fact]
    public void EqualsIndex()
    {
        var a = Array.Empty<string>();
        var l = new List<string>();

        var x = Expression.MakeIndex(Expression.Constant(a), null, new[] { Expression.Constant(0) });
        var y = Expression.MakeIndex(Expression.Constant(a), null, new[] { Expression.Constant(0) });
        AssertEqual(x, y);
        var z = Expression.MakeIndex(Expression.Constant(l), typeof(List<string>).GetProperty("Item"), new[] { Expression.Constant(0) });
        AssertNotEqual(x, z);
    }

    [Fact]
    public void EqualsInvocation()
    {
        var x = Lambda.Expr((Func<int, bool> x) => x(0));
        var y = Lambda.Expr((Func<int, bool> x) => x(0));
        AssertEqual(x.Body, y.Body);
        var z = Lambda.Expr((Func<int, bool> x) => x(1));
        AssertNotEqual(x.Body, z.Body);
    }

    [Fact]
    public void EqualsLabel()
    {
        var target = Expression.Label();

        var x = Expression.Label(target, Expression.Constant(1));
        var y = Expression.Label(target, Expression.Constant(1));
        AssertEqual(x, y);
        var z = Expression.Label(target, Expression.Constant(2));
        AssertNotEqual(x, z);
    }

    [Fact]
    public void EqualsLambda()
    {
        var x = Lambda.Expr((int a, int b) => a + b);
        var y = Lambda.Expr((int a, int b) => a + b);
        AssertEqual(x.Body, y.Body);
        var z = Lambda.Expr((int a, int b) => a - b);
        AssertNotEqual(x.Body, z.Body);
    }

    [Fact]
    public void EqualsListInit()
    {
        var x = Expression.Lambda(() => new List<int> { 1 });
        var y = Expression.Lambda(() => new List<int> { 1 });
        AssertEqual(x.Body, y.Body);
        var z = Expression.Lambda(() => new List<int> { 2 });
        AssertNotEqual(x.Body, z.Body);
    }

    [Fact]
    public void EqualsLoop()
    {
        var x = Expression.Loop(Expression.Constant(1));
        var y = Expression.Loop(Expression.Constant(1));
        AssertEqual(x, y);
        var z = Expression.Loop(Expression.Constant(2));
        AssertNotEqual(x, z);
    }

    [Fact]
    public void EqualsMember()
    {
        var x = Lambda.Expr((List<int> x) => x.Count);
        var y = Lambda.Expr((List<int> x) => x.Count);
        AssertEqual(x.Body, y.Body);
        var z = Lambda.Expr((List<int> x) => x.Capacity);
        AssertNotEqual(x.Body, z.Body);
    }

    [Fact]
    public void EqualsMemberInit()
    {
        // MemberInit
        var x1 = Lambda.Expr(() => new TestNode { Data = new NodeData() });
        var y1 = Lambda.Expr(() => new TestNode { Data = new NodeData() });
        AssertEqual(x1.Body, y1.Body);
        var z1 = Lambda.Expr(() => new TestNode { Data = new NodeData { Name = "1" } });
        AssertNotEqual(x1.Body, z1.Body);

        // MemberMemberInit
        var x2 = Lambda.Expr(() => new TestNode { Data = { Name = "1" } });
        var y2 = Lambda.Expr(() => new TestNode { Data = { Name = "1" } });
        AssertEqual(x2.Body, y2.Body);
        var z2 = Lambda.Expr(() => new TestNode { Data = { Name = "2" } });
        AssertNotEqual(x2.Body, z2.Body);

        // MemberListInit
        var x3 = Lambda.Expr(() => new TestNode { Children = { new TestNode(), new TestNode() } });
        var y3 = Lambda.Expr(() => new TestNode { Children = { new TestNode(), new TestNode() } });
        AssertEqual(x3.Body, y3.Body);
        var z3 = Lambda.Expr(() => new TestNode { Children = { new TestNode() } });
        AssertNotEqual(x3.Body, z3.Body);
    }

    [Fact]
    public void EqualsMethodCall()
    {
        var x = Lambda.Expr(() => EqualityComparer<int>.Default.Equals(1, 1));
        var y = Lambda.Expr(() => EqualityComparer<int>.Default.Equals(1, 1));
        AssertEqual(x.Body, y.Body);
        var z = Lambda.Expr(() => EqualityComparer<int>.Default.Equals(1, 2));
        AssertNotEqual(x.Body, z.Body);
    }

    [Fact]
    public void EqualsNewArray()
    {
        var x = Lambda.Expr(() => new[] { 1 });
        var y = Lambda.Expr(() => new[] { 1 });
        AssertEqual(x.Body, y.Body);
        var z = Lambda.Expr(() => new[] { 2 });
        AssertNotEqual(x.Body, z.Body);
    }

#pragma warning disable CA1711

    [Fact]
    public void EqualsNew()
    {
        var x = Lambda.Expr(() => new { Source = "1" });
        var y = Lambda.Expr(() => new { Source = "1" });
        AssertEqual(x.Body, y.Body);
        var z = Lambda.Expr(() => new { Source = "2" });
        AssertNotEqual(x.Body, z.Body);
    }

#pragma warning restore CA1711

    [Fact]
    public void EqualsParameter()
    {
        var x = Lambda.Expr((int x) => x);
        var y = Lambda.Expr((int x) => x);
        AssertEqual(x.Body, y.Body);
        var z = Lambda.Expr((string x) => x);
        AssertNotEqual(x.Body, z.Body);
    }

    [Fact]
    public void TestEqualRuntimeVariables()
    {
        var x = Expression.RuntimeVariables(Expression.Parameter(typeof(int)));
        var y = Expression.RuntimeVariables(Expression.Parameter(typeof(int)));
        AssertEqual(x, y);
        var z = Expression.RuntimeVariables(Expression.Parameter(typeof(string)));
        AssertNotEqual(x, z);
    }

    [Fact]
    public void EqualsSwitch()
    {
        var case1 = Expression.SwitchCase(Expression.Constant(1), Expression.Constant(0));
        var case2 = Expression.SwitchCase(Expression.Constant(2), Expression.Constant(0));

        var x = Expression.Switch(Expression.Constant(1), Expression.Constant(0), case1);
        var y = Expression.Switch(Expression.Constant(1), Expression.Constant(0), case1);
        AssertEqual(x, y);
        var z = Expression.Switch(Expression.Constant(1), Expression.Constant(0), case2);
        AssertNotEqual(x, z);
    }

    [Fact]
    public void EqualsTry()
    {
        var c = Expression.Constant(0);

        var x = Expression.TryCatch(c, Expression.Catch(typeof(Exception), c));
        var y = Expression.TryCatch(c, Expression.Catch(typeof(Exception), c));
        AssertEqual(x, y);
        var z = Expression.TryCatch(c, Expression.Catch(typeof(NotImplementedException), c));
        AssertNotEqual(x, z);

        var v = Expression.TryCatchFinally(c, c, Expression.Catch(typeof(Exception), c));
        AssertNotEqual(x, v);
    }

    [Fact]
    public void EqualsTypeBinary()
    {
        var x = Lambda.Expr((object x) => x is string);
        var y = Lambda.Expr((object x) => x is string);
        AssertEqual(x.Body, y.Body);
        var z = Lambda.Expr((object x) => x as string);
        AssertNotEqual(x.Body, z.Body);
    }

    [Fact]
    public void EqualsUnary()
    {
        var x = Lambda.Expr((int x) => ~x);
        var y = Lambda.Expr((int x) => ~x);
        AssertEqual(x.Body, y.Body);
        var z = Lambda.Expr((bool x) => !x);
        AssertNotEqual(x.Body, z.Body);
    }

    [Fact]
    public void ReferenceEqualityParameter()
    {
        var param1 = Expression.Parameter(typeof(int));
        var param2 = Expression.Parameter(typeof(int));
        var param3 = Expression.Parameter(typeof(int));
        var param4 = Expression.Parameter(typeof(int));

        var x = Expression.Lambda(Expression.Add(param1, param2), param1, param2);
        var y = Expression.Lambda(Expression.Add(param3, param4), param3, param4);
        var z = Expression.Lambda(Expression.Add(param1, param2), param1, param4);

        AssertEqual(x, y);    // different references, but same local semantics
        AssertNotEqual(x, z); // different references and different local semantics
    }

    [Fact]
    public void ReferenceEqualityLabel()
    {
        var ret1 = Expression.Return(Expression.Label());
        var ret2 = Expression.Return(Expression.Label());
        var ret3 = Expression.Return(Expression.Label());
        var ret4 = Expression.Return(Expression.Label());

        var x = Expression.Block(ret1, ret2, ret1);
        var y = Expression.Block(ret3, ret4, ret3);
        var z = Expression.Block(ret1, ret2, ret4);

        AssertEqual(x, y);    // different references, but same local semantics
        AssertNotEqual(x, z); // different references and different local semantics
    }

    [Fact]
    public void IgnoreLabelName()
    {
        var x = Expression.Label(Expression.Label("x"));
        var y = Expression.Label(Expression.Label("y"));

        AssertNotEqual(x, y, new ExprEqualityComparer(ExprEqualityComparerFlags.None));
        AssertEqual(x, y, new ExprEqualityComparer(ExprEqualityComparerFlags.IgnoreLabelName));
    }

    [Fact]
    public void IgnoreLambdaName()
    {
        var delegateType = Expression.GetFuncType(typeof(int));

        var x = Expression.Lambda(delegateType, Expression.Constant(1), "x", Array.Empty<ParameterExpression>());
        var y = Expression.Lambda(delegateType, Expression.Constant(1), "y", Array.Empty<ParameterExpression>());

        AssertNotEqual(x, y, new ExprEqualityComparer(ExprEqualityComparerFlags.None));
        AssertEqual(x, y, new ExprEqualityComparer(ExprEqualityComparerFlags.IgnoreLambdaName));
    }

    private delegate int CustomFuncInt();

    [Fact]
    public void IgnoreLambdaType()
    {
        var x = Expression.Lambda<Func<int>>(Expression.Constant(1), Array.Empty<ParameterExpression>());
        var y = Expression.Lambda<CustomFuncInt>(Expression.Constant(1), Array.Empty<ParameterExpression>());

        AssertNotEqual(x, y, new ExprEqualityComparer(ExprEqualityComparerFlags.None));
        AssertEqual(x, y, new ExprEqualityComparer(ExprEqualityComparerFlags.IgnoreLambdaType));
    }

    [Fact]
    public void IgnoreParameterName()
    {
        var paramX = Expression.Parameter(typeof(int), "x");
        var paramY = Expression.Parameter(typeof(int), "y");

        var x = Expression.Lambda(Expression.Constant(1), paramX);
        var y = Expression.Lambda(Expression.Constant(1), paramY);

        AssertNotEqual(x, y, new ExprEqualityComparer(ExprEqualityComparerFlags.None));
        AssertEqual(x, y, new ExprEqualityComparer(ExprEqualityComparerFlags.IgnoreParameterName));
    }

    #region Internal

    private static void AssertEqual(Expression x, Expression y, IEqualityComparer<Expression>? comparer = null)
    {
        comparer ??= ExprEqualityComparer.Default;

        Assert.True(comparer.Equals(x, y));

        var hashX = comparer.GetHashCode(x);
        var hashY = comparer.GetHashCode(y);

        Assert.Equal(hashX, hashY);
    }

    private static void AssertNotEqual(Expression x, Expression y, IEqualityComparer<Expression>? comparer = null)
    {
        comparer ??= ExprEqualityComparer.Default;

        Assert.False(comparer.Equals(x, y));

        var hashX = comparer.GetHashCode(x);
        var hashY = comparer.GetHashCode(y);

        // This condition is not always true due to collisions, but chances for that are
        // super low..
        Assert.NotEqual(hashX, hashY);
    }

    [ExcludeFromCodeCoverage]
    private sealed class TestNode
    {
        public NodeData Data { get; set; } = new();

        public List<TestNode> Children { get; } = new();
    }

    [ExcludeFromCodeCoverage]
    private sealed class NodeData
    {
        public string Name { get; set; } = default!;
    }

    [ExcludeFromCodeCoverage]
    private sealed class ExtensionExpression :
        Expression
    {
        public override ExpressionType NodeType => ExpressionType.Extension;
    }

    #endregion Internal
}
