using System.Diagnostics.CodeAnalysis;

namespace ZySharp.Metaprogramming
{
    /// <summary>
    /// Provides helper methods for declaration of function delegates and lambda expression.
    /// </summary>
    /// <remarks>
    /// C# language currently is not completely able to automatically infer the return type of anonymous methods
    /// when using a <c>var</c> declaration or when returning curried functions. The helper methods in the class
    /// can be used to work around this limitation.
    /// </remarks>
    public static partial class Lambda
    {
    }
}