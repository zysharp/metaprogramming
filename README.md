# ZySharp Metaprogramming

![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)
[![Build](https://github.com/zysharp/metaprogramming/actions/workflows/ci.yaml/badge.svg)](https://github.com/zysharp/metaprogramming/actions/workflows/ci.yaml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=zysharp_metaprogramming&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=zysharp_metaprogramming)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=zysharp_metaprogramming&metric=coverage)](https://sonarcloud.io/summary/new_code?id=zysharp_metaprogramming)
[![NuGet](https://img.shields.io/nuget/v/ZySharp.Metaprogramming.svg)](https://nuget.org/packages/ZySharp.Metaprogramming)
[![Nuget](https://img.shields.io/nuget/dt/ZySharp.Metaprogramming.svg)](https://nuget.org/packages/ZySharp.Metaprogramming)

A modern C# metaprogramming library.

## Enhanced Lambda Type Inference

The C# language currently is not completely able to automatically infer the return type of anonymous methods when using a `var` declaration or when returning curried functions. The helper methods in the `ZySharp.Metaprogramming.Lambda` class can be used to work around this limitation.

```csharp
// Expression<Func<>>
var e = Lambda.Expr((int x) => x == 42);
var e = Lambda.Expr((int x) => new { Value = x });
var e = Lambda.Expr((int x) => Lambda.Func((int y) => x + y));

// Func<>
var d = Lambda.Func((int x) => x == 42)
var d = Lambda.Func((int x) => new { Value = x });
var d = Lambda.Func((int x) => Lambda.Func((int y) => x + y));
```

## Expression Concatenation and Invocation

When creating dynamic LINQ expressions, it is often useful to use foreign lambda expressions inside an outer expression. While this is somewhat possible (`.Compile().Invoke(...)`), most third-party libraries (e.g. `LINQ to Entities`) are not able to understand the resulting expression tree. The `ZySharp.Metaprogramming.Expr` class provides a set of simple functions to solve this problem.

### Expression Concatenation

```csharp
// Expression<Func<>>
var inner = Lambda.Expr((int x) => x == 42);
var outer = Lambda.Expr((IQueryable<int> x) => x.Where(inner));
                                               //  :. Expression<Func<int, bool>>
var result = outer.Expand(); // x => x.Where(x => x == 42)

// Func<>
var inner = Lambda.Expr((int x) => x == 42);
var outer = Lambda.Expr((IEnumerable<int> x) => x.Where(inner.Compile()));
                                                //  :. Func<int, bool>
var result = outer.Expand(); // x => x.Where(x => x == 42)
```

### Expression Invocation

```csharp
var lower = Lambda.Expr((int x) => x >= 10);
var upper = Lambda.Expr((int x) => x <= 42);
var outer = Lambda.Expr((IQueryable<int> x) => x.Where(x => lower.Invoke(x) && upper.Invoke(x)));
var result = outer.Expand(); // x => x.Where(x => x >= 10 && x <= 42)
```

### Dynamic Expression Invocation

In the following example, `GetPredicateExpressionWithParam()` is evaluated at runtime (when `.Expand()` is called). The resulting expression is then integrated into the outer expression tree. During this process, all captured variables (`num42`) are also evaluated.

```csharp
private static Expression<Func<int, bool>> GetPredicateExpressionWithParam(int number)
{
    return x => x == number;
}

var num42 = 42;
var outer = Lambda.Expr((IQueryable<int> x) => x.Where(x => GetPredicateExpressionWithParam(num42).Invoke(x)));
var result = outer.Expand(); // x => x.Where(x => x == 42)
```

## Expression Tree Comparison

By default, expression trees can only be compared by reference. To be able to e.g. store semantically equivalent expression trees in a dictionary, the `ZySharp.Metaprogramming.ExprEqualityComparer` implements the `IEqualityComparer<T>` interface for the `Expression` class.

```csharp
var x = Lambda.Expr((int a, int b) => a + b);
var y = Lambda.Expr((int c, int d) => c + d);
ExprEqualityComparer.Default.Equals(x, y); // true

var z = Lambda.Expr((int a, int b) => a + a);
ExprEqualityComparer.Default.Equals(x, z); // false
```

All built-in `Expression` types except `DynamicExpression` are supported.

## Versioning

Versions follow the [semantic versioning scheme](https://semver.org/).

## License

ZySharp.Metaprogramming is licensed under the MIT license.
