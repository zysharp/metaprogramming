using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

using ZySharp.Metaprogramming.Extensions;

namespace ZySharp.Metaprogramming
{
    /// <summary>
    /// This exception is thrown by <see cref="ExprExtensions.Evaluate"/> if the passed expression could not get
    /// evaluated correctly.
    /// </summary>
    [Serializable, ExcludeFromCodeCoverage]
    public class EvaluationException :
        Exception
    {
        /// <inheritdoc cref="Exception()"/>
        public EvaluationException()
        {
        }

        /// <inheritdoc cref="Exception(string)"/>
        public EvaluationException(string message) : base(message)
        {
        }

        /// <inheritdoc cref="Exception(string,Exception)"/>
        public EvaluationException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <inheritdoc cref="Exception(SerializationInfo,StreamingContext)"/>
        protected EvaluationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}