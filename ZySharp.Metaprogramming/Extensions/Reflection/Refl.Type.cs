using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using ZySharp.Validation;

namespace ZySharp.Metaprogramming.Extensions.Reflection
{
    /// <summary>
    /// Provides various general purpose extension methods for the <see cref="Type"/> class.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Determines whether the current <paramref name="type"/> is the same type as <paramref name="other"/> or
        /// a subclass of <paramref name="other"/>.
        /// </summary>
        /// <param name="type">The current type.</param>
        /// <param name="other">The other type.</param>
        /// <returns><c>True</c> if the current type is the same type as other or a subclass of other.</returns>
        public static bool IsSameOrSubclass(this Type type, Type other)
        {
            ValidateArgument.For(type, nameof(type), v => v.NotNull());
            ValidateArgument.For(other, nameof(other), v => v.NotNull());

            return (type == other) || (type.IsSubclassOf(other));
        }

        /// <summary>
        /// Determines if the given <paramref name="type"/> was constructed from a specific <paramref name="genericTypeDefinition"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="genericTypeDefinition">The type definition.</param>
        /// <returns><c>True</c> if the given type was constructed from the given generic type definition.</returns>
        public static bool IsConstructedFrom(this Type type, Type genericTypeDefinition)
        {
            ValidateArgument.For(type, nameof(type), v => v.NotNull());
            ValidateArgument.For(genericTypeDefinition, nameof(genericTypeDefinition), v => v.NotNull());

            if (!type.IsGenericType || type.IsGenericTypeDefinition || !genericTypeDefinition.IsGenericTypeDefinition)
            {
                return false;
            }

            return (type.GetGenericTypeDefinition() == genericTypeDefinition);
        }

        /// <summary>
        /// Returns all types derived from the given base <paramref name="type"/> that declared in the same assembly.
        /// </summary>
        /// <param name="type">The base type.</param>
        /// <returns>A list of all types derived from the given base type.</returns>
        public static IEnumerable<Type> GetDerivedTypes(this Type type)
        {
            ValidateArgument.For(type, nameof(type), v => v.NotNull());

            return GetDerivedTypes(type, type.Assembly);
        }

        /// <summary>
        /// Returns all types derived from the given base <paramref name="type"/> that declared in a specific
        /// <paramref name="assembly"/>.
        /// </summary>
        /// <param name="type">The base type.</param>
        /// <param name="assembly">The assembly to search for derived types.</param>
        /// <returns>A list of all types derived from the given base type.</returns>
        public static IEnumerable<Type> GetDerivedTypes(this Type type, Assembly assembly)
        {
            ValidateArgument.For(type, nameof(type), v => v.NotNull());
            ValidateArgument.For(assembly, nameof(assembly), v => v.NotNull());

            return assembly
                .GetTypes()
                .Where(t => (t != type) && type.IsAssignableFrom(t));
        }
    }
}