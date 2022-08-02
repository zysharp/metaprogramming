using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;

using ZySharp.Metaprogramming.Extensions.Expressions;
using ZySharp.Metaprogramming.Extensions.Reflection;
using ZySharp.Validation;

namespace ZySharp.Metaprogramming
{
    /// <summary>
    /// Implements the <see cref="IEqualityComparer{T}"/> interface for the <see cref="Expression"/> type to check
    /// for semantic equality.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The comparer is unable to process the <see cref="DynamicExpression"/> type.
    /// </para>
    /// <para>
    /// The comparer is unable to process extension expression types. Consider calling <see cref="Expression.Reduce"/>
    /// one them before passing the expression tree to the comparer.
    /// </para>
    /// <para>
    /// For <see cref="ConstantExpression"/> expressions the constant value is accessed using (if supported) the
    /// <see cref="IEqualityComparer{T}"/> interface or the default <see cref="object.Equals(object)"/> and
    /// <see cref="object.GetHashCode()"/> methods.
    /// </para>
    /// <para>
    /// Captured value-type references are converted to <see cref="ConstantExpression"/> nodes with a snapshot of
    /// their current values. See <see cref="ExprExtensions.Snapshot"/> for further information.
    /// </para>
    /// </remarks>
    public class ExprEqualityComparer :
        IEqualityComparer<Expression>
    {
        /// <summary>
        /// The equality comparer flags set during initialization.
        /// </summary>
        protected ExprEqualityComparerFlags Flags { get; }

        /// <summary>
        /// Returns the default <see cref="ExprEqualityComparer"/> instance.
        /// </summary>
        public static ExprEqualityComparer Default { get; } = new();

        /// <summary>
        /// Creates a new <see cref="ExprEqualityComparer"/> instance with the <see cref="ExprEqualityComparerFlags.Default"/> flags.
        /// </summary>
        public ExprEqualityComparer() :
            this(ExprEqualityComparerFlags.Default)
        {
        }

        /// <summary>
        /// Creates a new <see cref="ExprEqualityComparer"/> instance with custom <see cref="ExprEqualityComparerFlags"/>.
        /// </summary>
        /// <param name="flags">The flags.</param>
        public ExprEqualityComparer(ExprEqualityComparerFlags flags)
        {
            Flags = flags;
        }

        #region IEqualityComparer

        /// <inheritdoc cref="IEqualityComparer{T}.Equals(T,T)"/>
        /// <exception cref="NotSupportedException">If the expression type is not supported.</exception>
        /// <exception cref="NotImplementedException">If the expression type is unknown.</exception>
        [Pure]
        public bool Equals(Expression? x, Expression? y)
        {
            var context = new Context(this);
            return context.CompareExpr(x.Snapshot(), y.Snapshot());
        }

        /// <inheritdoc cref="IEqualityComparer{T}.GetHashCode(T)"/>
        /// <exception cref="NotSupportedException">If the expression type is not supported.</exception>
        /// <exception cref="NotImplementedException">If the expression type is unknown.</exception>
        [Pure]
        public int GetHashCode(Expression obj)
        {
            ValidateArgument.For(obj, nameof(obj), v => v.NotNull());

            var context = new Context(this);
            context.HashExpr(obj.Snapshot());
            return context.ToHashCode();
        }

        #endregion IEqualityComparer

        #region Compare Methods

        /// <summary>
        /// Determines whether expression <paramref name="x"/> and expression <paramref name="y"/> are semantically
        /// equal.
        /// </summary>
        /// <param name="context">The current equality comparer context.</param>
        /// <param name="x">The first expression.</param>
        /// <param name="y">The second expression.</param>
        /// <returns><c>True</c> if both expressions are semantically equal.</returns>
        /// <exception cref="NotSupportedException">If the expression type is not supported.</exception>
        /// <exception cref="NotImplementedException">If the expression type is unknown.</exception>
        protected virtual bool Compare(Context context, Expression? x, Expression? y)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());

            return (x, y) switch
            {
                (null, null) => true,
                (null, not null) => false,
                (not null, null) => false,
                (BinaryExpression e1, BinaryExpression e2) => CompareBinary(context, e1, e2),
                (BlockExpression e1, BlockExpression e2) => CompareBlock(context, e1, e2),
                (ConditionalExpression e1, ConditionalExpression e2) => CompareConditional(context, e1, e2),
                (ConstantExpression e1, ConstantExpression e2) => CompareConstant(context, e1, e2),
                (DebugInfoExpression e1, DebugInfoExpression e2) => CompareDebugInfo(context, e1, e2),
                (DefaultExpression e1, DefaultExpression e2) => CompareDefault(context, e1, e2),
                (DynamicExpression, DynamicExpression) => throw new NotSupportedException(),
                (GotoExpression e1, GotoExpression e2) => CompareGoto(context, e1, e2),
                (IndexExpression e1, IndexExpression e2) => CompareIndex(context, e1, e2),
                (InvocationExpression e1, InvocationExpression e2) => CompareInvocation(context, e1, e2),
                (LabelExpression e1, LabelExpression e2) => CompareLabel(context, e1, e2),
                (LambdaExpression e1, LambdaExpression e2) => CompareLambda(context, e1, e2),
                (ListInitExpression e1, ListInitExpression e2) => CompareListInit(context, e1, e2),
                (LoopExpression e1, LoopExpression e2) => CompareLoop(context, e1, e2),
                (MemberExpression e1, MemberExpression e2) => CompareMember(context, e1, e2),
                (MemberInitExpression e1, MemberInitExpression e2) => CompareMemberInit(context, e1, e2),
                (MethodCallExpression e1, MethodCallExpression e2) => CompareMethodCall(context, e1, e2),
                (NewArrayExpression e1, NewArrayExpression e2) => CompareNewArray(context, e1, e2),
                (NewExpression e1, NewExpression e2) => CompareNew(context, e1, e2),
                (ParameterExpression e1, ParameterExpression e2) => CompareParameter(context, e1, e2),
                (RuntimeVariablesExpression e1, RuntimeVariablesExpression e2) => CompareRuntimeVariables(context, e1, e2),
                (SwitchExpression e1, SwitchExpression e2) => CompareSwitch(context, e1, e2),
                (TryExpression e1, TryExpression e2) => CompareTry(context, e1, e2),
                (TypeBinaryExpression e1, TypeBinaryExpression e2) => CompareTypeBinary(context, e1, e2),
                (UnaryExpression e1, UnaryExpression e2) => CompareUnary(context, e1, e2),
                (not null, not null) when (x?.GetType() != y?.GetType()) => false,
                _ => throw new NotImplementedException()
            };
        }

        /// <inheritdoc cref="Compare"/>
        protected virtual bool CompareBase(Context context, Expression x, Expression y)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(x, nameof(x), v => v.NotNull());
            ValidateArgument.For(y, nameof(y), v => v.NotNull());

            var ignoreType = ((x is LambdaExpression) && Flags.HasFlag(ExprEqualityComparerFlags.IgnoreLambdaType));

            return context.Compare(x.CanReduce, y.CanReduce) &&
                   context.Compare(x.NodeType, y.NodeType) &&
                   context.Compare(x.Type, y.Type, ignoreType);
        }

        /// <inheritdoc cref="Compare"/>
        protected virtual bool CompareBinary(Context context, BinaryExpression x, BinaryExpression y)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(x, nameof(x), v => v.NotNull());
            ValidateArgument.For(y, nameof(y), v => v.NotNull());

            return CompareBase(context, x, y) &&
                   context.CompareExpr(x.Conversion, y.Conversion) &&
                   context.Compare(x.IsLifted, y.IsLifted) &&
                   context.Compare(x.IsLiftedToNull, y.IsLiftedToNull) &&
                   context.CompareExpr(x.Left, y.Left) &&
                   context.Compare(x.Method, y.Method) &&
                   context.CompareExpr(x.Right, y.Right);
        }

        /// <inheritdoc cref="Compare"/>
        protected virtual bool CompareBlock(Context context, BlockExpression x, BlockExpression y)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(x, nameof(x), v => v.NotNull());
            ValidateArgument.For(y, nameof(y), v => v.NotNull());

            return CompareBase(context, x, y) &&
                   context.CompareExpr(x.Expressions, y.Expressions) &&
                   context.CompareExpr(x.Result, y.Result) &&
                   context.CompareExpr(x.Variables, y.Variables);
        }

        /// <inheritdoc cref="Compare"/>
        protected virtual bool CompareConditional(Context context, ConditionalExpression x, ConditionalExpression y)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(x, nameof(x), v => v.NotNull());
            ValidateArgument.For(y, nameof(y), v => v.NotNull());

            return CompareBase(context, x, y) &&
                   context.CompareExpr(x.IfFalse, y.IfFalse) &&
                   context.CompareExpr(x.IfTrue, y.IfTrue) &&
                   context.CompareExpr(x.Test, y.Test);
        }

        /// <inheritdoc cref="Compare"/>
        protected virtual bool CompareConstant(Context context, ConstantExpression x, ConstantExpression y)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(x, nameof(x), v => v.NotNull());
            ValidateArgument.For(y, nameof(y), v => v.NotNull());

            return CompareBase(context, x, y) &&
                   context.Compare(x.Value, y.Value);
        }

        /// <inheritdoc cref="Compare"/>
        protected virtual bool CompareDebugInfo(Context context, DebugInfoExpression x, DebugInfoExpression y)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(x, nameof(x), v => v.NotNull());
            ValidateArgument.For(y, nameof(y), v => v.NotNull());

            return CompareBase(context, x, y) &&
                   context.Compare(x.Document, y.Document, CompareDocument) &&
                   context.Compare(x.EndColumn, y.EndColumn) &&
                   context.Compare(x.EndLine, y.EndLine) &&
                   context.Compare(x.IsClear, y.IsClear) &&
                   context.Compare(x.StartColumn, y.StartColumn) &&
                   context.Compare(x.StartLine, y.StartLine);
        }

        /// <inheritdoc cref="Compare"/>
        protected virtual bool CompareDefault(Context context, DefaultExpression x, DefaultExpression y)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(x, nameof(x), v => v.NotNull());
            ValidateArgument.For(y, nameof(y), v => v.NotNull());

            return CompareBase(context, x, y);
        }

        /// <inheritdoc cref="Compare"/>
        protected virtual bool CompareGoto(Context context, GotoExpression x, GotoExpression y)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(x, nameof(x), v => v.NotNull());
            ValidateArgument.For(y, nameof(y), v => v.NotNull());

            return CompareBase(context, x, y) &&
                   context.Compare(x.Kind, y.Kind) &&
                   context.Compare(x.Target, y.Target, CompareLabelTarget) &&
                   context.CompareExpr(x.Value, y.Value);
        }

        /// <inheritdoc cref="Compare"/>
        protected virtual bool CompareIndex(Context context, IndexExpression x, IndexExpression y)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(x, nameof(x), v => v.NotNull());
            ValidateArgument.For(y, nameof(y), v => v.NotNull());

            return CompareBase(context, x, y) &&
                   context.CompareExpr(x.Arguments, y.Arguments) &&
                   context.Compare(x.Indexer, y.Indexer) &&
                   context.CompareExpr(x.Object, y.Object);
        }

        /// <inheritdoc cref="Compare"/>
        protected virtual bool CompareInvocation(Context context, InvocationExpression x, InvocationExpression y)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(x, nameof(x), v => v.NotNull());
            ValidateArgument.For(y, nameof(y), v => v.NotNull());

            return CompareBase(context, x, y) &&
                   context.CompareExpr(x.Arguments, y.Arguments) &&
                   context.CompareExpr(x.Expression, y.Expression);
        }

        /// <inheritdoc cref="Compare"/>
        protected virtual bool CompareLabel(Context context, LabelExpression x, LabelExpression y)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(x, nameof(x), v => v.NotNull());
            ValidateArgument.For(y, nameof(y), v => v.NotNull());

            return CompareBase(context, x, y) &&
                   context.CompareExpr(x.DefaultValue, y.DefaultValue) &&
                   context.Compare(x.Target, y.Target, CompareLabelTarget);
        }

        /// <inheritdoc cref="Compare"/>
        protected virtual bool CompareLambda(Context context, LambdaExpression x, LambdaExpression y)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(x, nameof(x), v => v.NotNull());
            ValidateArgument.For(y, nameof(y), v => v.NotNull());

            return CompareBase(context, x, y) &&
                   context.CompareExpr(x.Body, y.Body) &&
                   context.Compare(x.Name, y.Name, Flags.HasFlag(ExprEqualityComparerFlags.IgnoreLambdaName)) &&
                   context.CompareExpr(x.Parameters, y.Parameters) &&
                   context.Compare(x.ReturnType, y.ReturnType) &&
                   context.Compare(x.TailCall, y.TailCall);
        }

        /// <inheritdoc cref="Compare"/>
        protected virtual bool CompareListInit(Context context, ListInitExpression x, ListInitExpression y)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(x, nameof(x), v => v.NotNull());
            ValidateArgument.For(y, nameof(y), v => v.NotNull());

            return CompareBase(context, x, y) &&
                   context.CompareMany(x.Initializers, y.Initializers, CompareElementInit) &&
                   context.CompareExpr(x.NewExpression, y.NewExpression);
        }

        /// <inheritdoc cref="Compare"/>
        protected virtual bool CompareLoop(Context context, LoopExpression x, LoopExpression y)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(x, nameof(x), v => v.NotNull());
            ValidateArgument.For(y, nameof(y), v => v.NotNull());

            return CompareBase(context, x, y) &&
                   context.CompareExpr(x.Body, y.Body) &&
                   context.Compare(x.BreakLabel, y.BreakLabel, CompareLabelTarget) &&
                   context.Compare(x.ContinueLabel, y.ContinueLabel, CompareLabelTarget);
        }

        /// <inheritdoc cref="Compare"/>
        protected virtual bool CompareMember(Context context, MemberExpression x, MemberExpression y)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(x, nameof(x), v => v.NotNull());
            ValidateArgument.For(y, nameof(y), v => v.NotNull());

            return CompareBase(context, x, y) &&
                   context.CompareExpr(x.Expression, y.Expression) &&
                   context.Compare(x.Member, y.Member);
        }

        /// <inheritdoc cref="Compare"/>
        protected virtual bool CompareMemberInit(Context context, MemberInitExpression x, MemberInitExpression y)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(x, nameof(x), v => v.NotNull());
            ValidateArgument.For(y, nameof(y), v => v.NotNull());

            return CompareBase(context, x, y) &&
                   context.CompareMany(x.Bindings, y.Bindings, CompareMemberBinding) &&
                   context.CompareExpr(x.NewExpression, y.NewExpression);
        }

        /// <inheritdoc cref="Compare"/>
        protected virtual bool CompareMethodCall(Context context, MethodCallExpression x, MethodCallExpression y)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(x, nameof(x), v => v.NotNull());
            ValidateArgument.For(y, nameof(y), v => v.NotNull());

            return CompareBase(context, x, y) &&
                   context.CompareExpr(x.Arguments, y.Arguments) &&
                   context.Compare(x.Method, y.Method) &&
                   context.CompareExpr(x.Object, y.Object);
        }

        /// <inheritdoc cref="Compare"/>
        protected virtual bool CompareNewArray(Context context, NewArrayExpression x, NewArrayExpression y)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(x, nameof(x), v => v.NotNull());
            ValidateArgument.For(y, nameof(y), v => v.NotNull());

            return CompareBase(context, x, y) &&
                   context.CompareExpr(x.Expressions, y.Expressions);
        }

        /// <inheritdoc cref="Compare"/>
        protected virtual bool CompareNew(Context context, NewExpression x, NewExpression y)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(x, nameof(x), v => v.NotNull());
            ValidateArgument.For(y, nameof(y), v => v.NotNull());

            return CompareBase(context, x, y) &&
                   context.CompareExpr(x.Arguments, y.Arguments) &&
                   context.Compare(x.Constructor, y.Constructor) &&
                   context.CompareMany(x.Members, y.Members);
        }

        /// <inheritdoc cref="Compare"/>
        protected virtual bool CompareParameter(Context context, ParameterExpression x, ParameterExpression y)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(x, nameof(x), v => v.NotNull());
            ValidateArgument.For(y, nameof(y), v => v.NotNull());

            if (!context.CompareReferenceId(x, y))
            {
                return false;
            }

            return CompareBase(context, x, y) &&
                   context.Compare(x.IsByRef, y.IsByRef) &&
                   context.Compare(x.Name, y.Name, Flags.HasFlag(ExprEqualityComparerFlags.IgnoreParameterName));
        }

        /// <inheritdoc cref="Compare"/>
        protected virtual bool CompareRuntimeVariables(Context context, RuntimeVariablesExpression x, RuntimeVariablesExpression y)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(x, nameof(x), v => v.NotNull());
            ValidateArgument.For(y, nameof(y), v => v.NotNull());

            return CompareBase(context, x, y) &&
                   context.CompareExpr(x.Variables, y.Variables);
        }

        /// <inheritdoc cref="Compare"/>
        protected virtual bool CompareSwitch(Context context, SwitchExpression x, SwitchExpression y)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(x, nameof(x), v => v.NotNull());
            ValidateArgument.For(y, nameof(y), v => v.NotNull());

            return CompareBase(context, x, y) &&
                   context.CompareMany(x.Cases, y.Cases, CompareSwitchCase) &&
                   context.Compare(x.Comparison, y.Comparison) &&
                   context.CompareExpr(x.DefaultBody, y.DefaultBody) &&
                   context.CompareExpr(x.SwitchValue, y.SwitchValue);
        }

        /// <inheritdoc cref="Compare"/>
        protected virtual bool CompareTry(Context context, TryExpression x, TryExpression y)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(x, nameof(x), v => v.NotNull());
            ValidateArgument.For(y, nameof(y), v => v.NotNull());

            return CompareBase(context, x, y) &&
                   context.CompareExpr(x.Body, y.Body) &&
                   context.CompareExpr(x.Fault, y.Fault) &&
                   context.CompareExpr(x.Finally, y.Finally) &&
                   context.CompareMany(x.Handlers, y.Handlers, CompareCatchBlock);
        }

        /// <inheritdoc cref="Compare"/>
        protected virtual bool CompareTypeBinary(Context context, TypeBinaryExpression x, TypeBinaryExpression y)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(x, nameof(x), v => v.NotNull());
            ValidateArgument.For(y, nameof(y), v => v.NotNull());

            return CompareBase(context, x, y) &&
                   context.CompareExpr(x.Expression, y.Expression) &&
                   context.Compare(x.TypeOperand, y.TypeOperand);
        }

        /// <inheritdoc cref="Compare"/>
        protected virtual bool CompareUnary(Context context, UnaryExpression x, UnaryExpression y)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(x, nameof(x), v => v.NotNull());
            ValidateArgument.For(y, nameof(y), v => v.NotNull());

            return CompareBase(context, x, y) &&
                   context.Compare(x.IsLifted, y.IsLifted) &&
                   context.Compare(x.IsLiftedToNull, y.IsLiftedToNull) &&
                   context.Compare(x.Method, y.Method) &&
                   context.CompareExpr(x.Operand, y.Operand);
        }

        private bool CompareDocument(Context context, SymbolDocumentInfo x, SymbolDocumentInfo y)
        {
            return context.Compare(x.DocumentType, y.DocumentType) &&
                   context.Compare(x.FileName, y.FileName) &&
                   context.Compare(x.Language, y.Language) &&
                   context.Compare(x.LanguageVendor, y.LanguageVendor);
        }

        private bool CompareLabelTarget(Context context, LabelTarget x, LabelTarget y)
        {
            if (!context.CompareReferenceId(x, y))
            {
                return false;
            }

            return context.Compare(x.Type, y.Type) &&
                   context.Compare(x.Name, y.Name, Flags.HasFlag(ExprEqualityComparerFlags.IgnoreLabelName));
        }

        private bool CompareElementInit(Context context, ElementInit x, ElementInit y)
        {
            return context.Compare(x.AddMethod, y.AddMethod) &&
                   context.CompareExpr(x.Arguments, y.Arguments);
        }

        private bool CompareMemberBinding(Context context, MemberBinding x, MemberBinding y)
        {
            if (x.GetType() != y.GetType())
            {
                return false;
            }

            if (!context.Compare(x.BindingType, y.BindingType) ||
                !context.Compare(x.Member, y.Member))
            {
                return false;
            }

            return (x, y) switch
            {
                (MemberAssignment mb1, MemberAssignment mb2) => context.CompareExpr(mb1.Expression, mb2.Expression),
                (MemberListBinding mb1, MemberListBinding mb2) => context.CompareMany(mb1.Initializers, mb2.Initializers, CompareElementInit),
                (MemberMemberBinding mb1, MemberMemberBinding mb2) => context.CompareMany(mb1.Bindings, mb2.Bindings, CompareMemberBinding),
                _ => throw new NotImplementedException()
            };
        }

        private bool CompareSwitchCase(Context context, SwitchCase x, SwitchCase y)
        {
            return context.CompareExpr(x.Body, y.Body) &&
                   context.CompareExpr(x.TestValues, y.TestValues);
        }

        private bool CompareCatchBlock(Context context, CatchBlock x, CatchBlock y)
        {
            return context.CompareExpr(x.Body, y.Body) &&
                   context.CompareExpr(x.Filter, y.Filter) &&
                   context.Compare(x.Test, y.Test) &&
                   context.CompareExpr(x.Variable, y.Variable);
        }

        #endregion Compare Methods

        #region Hash Methods

        /// <summary>
        /// Adds the given expression to the hash code.
        /// </summary>
        /// <param name="context">The current equality comparer context.</param>
        /// <param name="node">The expression node.</param>
        /// <exception cref="NotSupportedException">If the expression type is not supported.</exception>
        /// <exception cref="NotImplementedException">If the expression type unknown.</exception>
        protected virtual void HashExpression(Context context, Expression? node)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());

            switch (node)
            {
                case null:
                    context.Hash(0);
                    break;

                case BinaryExpression e:
                    HashBinary(context, e);
                    break;

                case BlockExpression e:
                    HashBlock(context, e);
                    break;

                case ConditionalExpression e:
                    HashConditional(context, e);
                    break;

                case ConstantExpression e:
                    HashConstant(context, e);
                    break;

                case DebugInfoExpression e:
                    HashDebugInfo(context, e);
                    break;

                case DefaultExpression e:
                    HashDefault(context, e);
                    break;

                case DynamicExpression:
                    throw new NotSupportedException();

                case GotoExpression e:
                    HashGoto(context, e);
                    break;

                case IndexExpression e:
                    HashIndex(context, e);
                    break;

                case InvocationExpression e:
                    HashInvocation(context, e);
                    break;

                case LabelExpression e:
                    HashLabel(context, e);
                    break;

                case LambdaExpression e:
                    HashLambda(context, e);
                    break;

                case ListInitExpression e:
                    HashListInit(context, e);
                    break;

                case LoopExpression e:
                    HashLoop(context, e);
                    break;

                case MemberExpression e:
                    HashMember(context, e);
                    break;

                case MemberInitExpression e:
                    HashMemberInit(context, e);
                    break;

                case MethodCallExpression e:
                    HashMethodCall(context, e);
                    break;

                case NewArrayExpression e:
                    HashNewArray(context, e);
                    break;

                case NewExpression e:
                    HashNew(context, e);
                    break;

                case ParameterExpression e:
                    HashParameter(context, e);
                    break;

                case RuntimeVariablesExpression e:
                    HashRuntimeVariables(context, e);
                    break;

                case SwitchExpression e:
                    HashSwitch(context, e);
                    break;

                case TryExpression e:
                    HashTry(context, e);
                    break;

                case TypeBinaryExpression e:
                    HashTypeBinary(context, e);
                    break;

                case UnaryExpression e:
                    HashUnary(context, e);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <inheritdoc cref="HashExpression"/>
        protected virtual void HashBase(Context context, Expression node)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(node, nameof(node), v => v.NotNull());

            var ignoreType = ((node is LambdaExpression) && Flags.HasFlag(ExprEqualityComparerFlags.IgnoreLambdaType));

            context.Hash(node.CanReduce);
            context.Hash(node.NodeType);
            context.Hash(node.Type, ignoreType);
        }

        /// <inheritdoc cref="HashExpression"/>
        protected virtual void HashBinary(Context context, BinaryExpression node)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(node, nameof(node), v => v.NotNull());

            HashBase(context, node);
            context.HashExpr(node.Conversion);
            context.Hash(node.IsLifted);
            context.Hash(node.IsLiftedToNull);
            context.HashExpr(node.Left);
            context.Hash(node.Method);
            context.HashExpr(node.Right);
        }

        /// <inheritdoc cref="HashExpression"/>
        protected virtual void HashBlock(Context context, BlockExpression node)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(node, nameof(node), v => v.NotNull());

            HashBase(context, node);
            context.HashExpr(node.Expressions);
            context.HashExpr(node.Result);
            context.HashExpr(node.Variables);
        }

        /// <inheritdoc cref="HashExpression"/>
        protected virtual void HashConditional(Context context, ConditionalExpression node)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(node, nameof(node), v => v.NotNull());

            HashBase(context, node);
            context.HashExpr(node.IfFalse);
            context.HashExpr(node.IfTrue);
            context.HashExpr(node.Test);
        }

        /// <inheritdoc cref="HashExpression"/>
        protected virtual void HashConstant(Context context, ConstantExpression node)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(node, nameof(node), v => v.NotNull());

            HashBase(context, node);
            context.Hash(node.Value);
        }

        /// <inheritdoc cref="HashExpression"/>
        protected virtual void HashDebugInfo(Context context, DebugInfoExpression node)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(node, nameof(node), v => v.NotNull());

            HashBase(context, node);
            context.Hash(node.EndColumn);
            context.Hash(node.EndLine);
            context.Hash(node.Document, HashDocument);
            context.Hash(node.IsClear);
            context.Hash(node.StartColumn);
            context.Hash(node.StartLine);
        }

        /// <inheritdoc cref="HashExpression"/>
        protected virtual void HashDefault(Context context, DefaultExpression node)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(node, nameof(node), v => v.NotNull());

            HashBase(context, node);
        }

        /// <inheritdoc cref="HashExpression"/>
        protected virtual void HashGoto(Context context, GotoExpression node)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(node, nameof(node), v => v.NotNull());

            HashBase(context, node);
            context.Hash(node.Kind);
            context.Hash(node.Target, HashLabelTarget);
            context.HashExpr(node.Value);
        }

        /// <inheritdoc cref="HashExpression"/>
        protected virtual void HashIndex(Context context, IndexExpression node)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(node, nameof(node), v => v.NotNull());

            HashBase(context, node);
            context.HashExpr(node.Arguments);
            context.Hash(node.Indexer);
            context.HashExpr(node.Object);
        }

        /// <inheritdoc cref="HashExpression"/>
        protected virtual void HashInvocation(Context context, InvocationExpression node)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(node, nameof(node), v => v.NotNull());

            HashBase(context, node);
            context.HashExpr(node.Arguments);
            context.HashExpr(node.Expression);
        }

        /// <inheritdoc cref="HashExpression"/>
        protected virtual void HashLabel(Context context, LabelExpression node)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(node, nameof(node), v => v.NotNull());

            HashBase(context, node);
            context.HashExpr(node.DefaultValue);
            context.Hash(node.Target, HashLabelTarget);
        }

        /// <inheritdoc cref="HashExpression"/>
        protected virtual void HashLambda(Context context, LambdaExpression node)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(node, nameof(node), v => v.NotNull());

            HashBase(context, node);
            context.HashExpr(node.Body);
            context.Hash(node.Name, Flags.HasFlag(ExprEqualityComparerFlags.IgnoreLambdaName));
            context.HashExpr(node.Parameters);
            context.Hash(node.ReturnType);
            context.Hash(node.TailCall);
        }

        /// <inheritdoc cref="HashExpression"/>
        protected virtual void HashListInit(Context context, ListInitExpression node)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(node, nameof(node), v => v.NotNull());

            HashBase(context, node);
            context.HashMany(node.Initializers, HashElementInit);
            context.HashExpr(node.NewExpression);
        }

        /// <inheritdoc cref="HashExpression"/>
        protected virtual void HashLoop(Context context, LoopExpression node)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(node, nameof(node), v => v.NotNull());

            HashBase(context, node);
            context.HashExpr(node.Body);
            context.Hash(node.BreakLabel, HashLabelTarget);
            context.Hash(node.ContinueLabel, HashLabelTarget);
        }

        /// <inheritdoc cref="HashExpression"/>
        protected virtual void HashMember(Context context, MemberExpression node)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(node, nameof(node), v => v.NotNull());

            HashBase(context, node);
            context.HashExpr(node.Expression);
            context.Hash(node.Member);
        }

        /// <inheritdoc cref="HashExpression"/>
        protected virtual void HashMemberInit(Context context, MemberInitExpression node)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(node, nameof(node), v => v.NotNull());

            HashBase(context, node);
            context.HashMany(node.Bindings, HashMemberBinding);
            context.HashExpr(node.NewExpression);
        }

        /// <inheritdoc cref="HashExpression"/>
        protected virtual void HashMethodCall(Context context, MethodCallExpression node)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(node, nameof(node), v => v.NotNull());

            HashBase(context, node);
            context.HashExpr(node.Arguments);
            context.Hash(node.Method);
            context.HashExpr(node.Object);
        }

        /// <inheritdoc cref="HashExpression"/>
        protected virtual void HashNewArray(Context context, NewArrayExpression node)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(node, nameof(node), v => v.NotNull());

            HashBase(context, node);
            context.HashExpr(node.Expressions);
        }

        /// <inheritdoc cref="HashExpression"/>
        protected virtual void HashNew(Context context, NewExpression node)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(node, nameof(node), v => v.NotNull());

            HashBase(context, node);
            context.HashExpr(node.Arguments);
            context.Hash(node.Constructor);
            context.HashMany(node.Members!);
        }

        /// <inheritdoc cref="HashExpression"/>
        protected virtual void HashParameter(Context context, ParameterExpression node)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(node, nameof(node), v => v.NotNull());

            HashBase(context, node);
            context.HashReferenceId(node);
            context.Hash(node.IsByRef);
            context.Hash(node.Name, Flags.HasFlag(ExprEqualityComparerFlags.IgnoreParameterName));
        }

        /// <inheritdoc cref="HashExpression"/>
        protected virtual void HashRuntimeVariables(Context context, RuntimeVariablesExpression node)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(node, nameof(node), v => v.NotNull());

            HashBase(context, node);
            context.HashExpr(node.Variables);
        }

        /// <inheritdoc cref="HashExpression"/>
        protected virtual void HashSwitch(Context context, SwitchExpression node)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(node, nameof(node), v => v.NotNull());

            HashBase(context, node);
            context.HashMany(node.Cases, HashSwitchCase);
            context.Hash(node.Comparison);
            context.HashExpr(node.DefaultBody);
            context.HashExpr(node.SwitchValue);
        }

        /// <inheritdoc cref="HashExpression"/>
        protected virtual void HashTry(Context context, TryExpression node)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(node, nameof(node), v => v.NotNull());

            HashBase(context, node);
            context.HashExpr(node.Body);
            context.HashExpr(node.Fault);
            context.HashExpr(node.Finally);
            context.HashMany(node.Handlers, HashCatchBlock);
        }

        /// <inheritdoc cref="HashExpression"/>
        protected virtual void HashTypeBinary(Context context, TypeBinaryExpression node)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(node, nameof(node), v => v.NotNull());

            HashBase(context, node);
            context.HashExpr(node.Expression);
            context.Hash(node.TypeOperand);
        }

        /// <inheritdoc cref="HashExpression"/>
        protected virtual void HashUnary(Context context, UnaryExpression node)
        {
            ValidateArgument.For(context, nameof(context), v => v.NotNull());
            ValidateArgument.For(node, nameof(node), v => v.NotNull());

            HashBase(context, node);
            context.Hash(node.IsLifted);
            context.Hash(node.IsLiftedToNull);
            context.Hash(node.Method);
            context.HashExpr(node.Operand);
        }

        private void HashDocument(Context context, SymbolDocumentInfo value)
        {
            context.Hash(value.DocumentType);
            context.Hash(value.FileName);
            context.Hash(value.Language);
            context.Hash(value.LanguageVendor);
        }

        private void HashLabelTarget(Context context, LabelTarget value)
        {
            context.HashReferenceId(value);
            context.Hash(value.Name, Flags.HasFlag(ExprEqualityComparerFlags.IgnoreLabelName));
            context.Hash(value.Type);
        }

        private void HashElementInit(Context context, ElementInit value)
        {
            context.Hash(value.AddMethod);
            context.HashExpr(value.Arguments);
        }

        private void HashMemberBinding(Context context, MemberBinding value)
        {
            context.Hash(value.GetType());

            context.Hash(value.BindingType);
            context.Hash(value.Member);

            switch (value)
            {
                case MemberAssignment mb:
                    context.HashExpr(mb.Expression);
                    break;

                case MemberListBinding mb:
                    context.HashMany(mb.Initializers, HashElementInit);
                    break;

                case MemberMemberBinding mb:
                    context.HashMany(mb.Bindings, HashMemberBinding);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        private void HashSwitchCase(Context context, SwitchCase value)
        {
            context.HashExpr(value.Body);
            context.HashExpr(value.TestValues);
        }

        private void HashCatchBlock(Context context, CatchBlock value)
        {
            context.HashExpr(value.Body);
            context.HashExpr(value.Filter);
            context.Hash(value.Test);
            context.HashExpr(value.Variable);
        }

        #endregion Hash Methods

        #region Context

        /// <summary>
        /// The expression equality comparer context.
        /// </summary>
        protected sealed class Context
        {
            private readonly ExprEqualityComparer _comparer;
            private readonly IDictionary<object, int> _referenceIds = new Dictionary<object, int>();
            private readonly IDictionary<object, object> _referenceMapping = new Dictionary<object, object>();
            private HashCode _hash = new();

            /// <summary>
            /// Creates a new <see cref="Context"/> instance that uses the given <paramref name="comparer"/>.
            /// </summary>
            /// <param name="comparer">The <see cref="ExprEqualityComparer"/> instance.</param>
            internal Context(ExprEqualityComparer comparer)
            {
                _comparer = comparer;
            }

            #region Compare Methods

            /// <summary>
            /// Determines whether the specified objects are equal by using the default <see cref="EqualityComparer{T}"/> instance.
            /// </summary>
            /// <typeparam name="T">The type of the objects.</typeparam>
            /// <param name="x">The first object.</param>
            /// <param name="y">The second object.</param>
            /// <param name="ignore">Set <c>true</c> to ignore this comparison and return <c>true</c>.</param>
            /// <returns><c>True</c> if both objects are equal.</returns>
            /// <remarks>
            /// If both objects are <c>null</c> they are considered equal.
            /// If only one object is <c>null</c> they are considered different.
            /// </remarks>
            public bool Compare<T>(T? x, T? y, bool ignore = false)
            {
                return Compare(x, y, (_, a, b) => EqualityComparer<T>.Default.Equals(a, b), ignore);
            }

            /// <summary>
            /// Determines whether the specified objects are equal by using the provided <paramref name="compare"/> delegate.
            /// </summary>
            /// <typeparam name="T">The type of the objects.</typeparam>
            /// <param name="x">The first object.</param>
            /// <param name="y">The second object.</param>
            /// <param name="compare">The compare delegate.</param>
            /// <param name="ignore">Set <c>true</c> to ignore this comparison and return <c>true</c>.</param>
            /// <returns><c>True</c> if both objects are equal.</returns>
            /// <exception cref="InvalidOperationException">If used to compare <see cref="Expression"/> objects.</exception>
            /// <remarks>
            /// If both objects are <c>null</c> they are considered equal and the <paramref name="compare"/> delegate
            /// is not invoked.
            /// If only one object is <c>null</c> they are considered different and the <paramref name="compare"/>
            /// delegate is not invoked.
            /// If the same reference is passed for both objects the <paramref name="compare"/> delegate is invoked
            /// anyways.
            /// </remarks>
            public bool Compare<T>(T? x, T? y, Func<Context, T, T, bool> compare, bool ignore = false)
            {
                ValidateArgument.For(compare, nameof(compare), v => v.NotNull());

                if (typeof(T).IsSameOrSubclass(typeof(Expression)))
                {
                    throw new InvalidOperationException();
                }

                if ((x is null) && (y is null))
                {
                    return true;
                }

                if ((x is null) || (y is null))
                {
                    return false;
                }

                return ignore || compare(this, x, y);
            }

            /// <summary>
            /// Determines whether the specified collections are equal by using the default <see cref="EqualityComparer{T}"/> instance
            /// for each element.
            /// </summary>
            /// <typeparam name="T">The type of the collection elements.</typeparam>
            /// <param name="x">The first collection.</param>
            /// <param name="y">The second collection.</param>
            /// <returns><c>True</c> if both collections are equal.</returns>
            /// <exception cref="InvalidOperationException">If used to compare <see cref="Expression"/> objects.</exception>
            /// <remarks>
            /// If both collections are <c>null</c> they are considered equal.
            /// If one collection is <c>null</c> and the other collection does not have any element they are considered equal.
            /// If only one collection is <c>null</c> and the other collection contains elements they are considered different.
            /// </remarks>
            public bool CompareMany<T>(IReadOnlyCollection<T>? x, IReadOnlyCollection<T>? y)
            {
                return CompareMany(x, y, (_, a, b) => EqualityComparer<T>.Default.Equals(a, b));
            }

            /// <summary>
            /// Determines whether the specified collections are equal by using the provided <paramref name="compare"/> delegate for
            /// each element.
            /// </summary>
            /// <typeparam name="T">The type of the collection elements.</typeparam>
            /// <param name="x">The first collection.</param>
            /// <param name="y">The second collection.</param>
            /// <param name="compare"></param>
            /// <returns><c>True</c> if both collections are equal.</returns>
            /// <exception cref="InvalidOperationException">If used to compare <see cref="Expression"/> objects.</exception>
            /// <remarks>
            /// If both collections are <c>null</c> they are considered equal and the <paramref name="compare"/> delegate is not
            /// invoked.
            /// If one collection is <c>null</c> and the other collection does not have any element they are considered equal and
            /// the <paramref name="compare"/> delegate is not invoked.
            /// If only one collection is <c>null</c> and the other collection contains elements they are considered different and
            /// the <paramref name="compare"/> delegate is not invoked.
            /// If the same reference is passed for both collections the <paramref name="compare"/> delegate is invoked
            /// anyways.
            /// </remarks>
            public bool CompareMany<T>(IReadOnlyCollection<T>? x, IReadOnlyCollection<T>? y, Func<Context, T, T, bool> compare)
            {
                ValidateArgument.For(compare, nameof(compare), v => v.NotNull());

                if (typeof(T).IsSameOrSubclass(typeof(Expression)))
                {
                    throw new InvalidOperationException();
                }

                return (x, y) switch
                {
                    (null, null) => true,
                    (null, not null) when !y.Any() => true,
                    (not null, null) when !x.Any() => true,
                    (null, not null) => false,
                    (not null, null) => false,
                    ({ } a, { } b) when (a.Count == b.Count) => x.Zip(y, (a, b) => Compare(a, b, compare)).All(r => r),
                    _ => false
                };
            }

            /// <summary>
            /// Determines whether expression <paramref name="x"/> and expression <paramref name="y"/> are semantically
            /// equal.
            /// </summary>
            /// <param name="x">The first expression.</param>
            /// <param name="y">The second expression.</param>
            /// <returns><c>True</c> if both expressions are semantically equal.</returns>
            /// <exception cref="NotSupportedException">If the expression type is not supported.</exception>
            /// <exception cref="NotImplementedException">If the expression type is unknown.</exception>
            public bool CompareExpr(Expression? x, Expression? y)
            {
                return _comparer.Compare(this, x, y);
            }

            /// <summary>
            /// Determines whether all expressions in list <paramref name="x"/> and the expressions in list
            /// <paramref name="y"/> are semantically equal.
            /// </summary>
            /// <param name="x">The first list of expressions.</param>
            /// <param name="y">The second list of expressions.</param>
            /// <returns><c>True</c> if all expressions in the lists are semantically equal.</returns>
            /// <exception cref="NotSupportedException">If the expression type is not supported.</exception>
            /// <exception cref="NotImplementedException">If the expression type is unknown.</exception>
            public bool CompareExpr(IReadOnlyCollection<Expression?>? x, IReadOnlyCollection<Expression?>? y)
            {
                return (x, y) switch
                {
                    (null, null) => true,
                    (null, not null) when !y.Any() => true,
                    (not null, null) when !x.Any() => true,
                    (null, not null) => false,
                    (not null, null) => false,
                    ({ } a, { } b) when (a.Count == b.Count) => x.Zip(y, CompareExpr).All(r => r),
                    _ => false
                };
            }

            /// <summary>
            /// Determines whether object <paramref name="x"/> always maps to the same object <paramref name="y"/> and
            /// vice versa.
            /// </summary>
            /// <param name="x">The first object.</param>
            /// <param name="y">The second object.</param>
            /// <returns><c>True</c> if the first object maps to the second object and vice versa.</returns>
            public bool CompareReferenceId(object? x, object? y)
            {
                if ((x is null) && (y is null))
                {
                    return true;
                }

                if ((x is null) || (y is null))
                {
                    return false;
                }

                if (!_referenceMapping.TryGetValue(x, out var mapXtoY))
                {
                    mapXtoY = y;
                    _referenceMapping[x] = y;
                }

                if (!_referenceMapping.TryGetValue(y, out var mapYtoX))
                {
                    mapYtoX = x;
                    _referenceMapping[y] = x;
                }

                return (mapXtoY == y) && (mapYtoX == x);
            }

            #endregion Compare Methods

            #region Hash Methods

            /// <summary>
            /// Adds a single value to the hash code using the default <see cref="EqualityComparer{T}"/> instance to calculate
            /// the hash code.
            /// </summary>
            /// <typeparam name="T">The type of the value.</typeparam>
            /// <param name="value">The value.</param>
            /// <param name="ignore">Set <c>true</c> to ignore this hash operation.</param>
            /// <remarks>
            /// If the value is <c>null</c> the value <c>0</c> is added to the hash.
            /// </remarks>
            public void Hash<T>(T? value, bool ignore = false)
            {
                Hash(value, (context, value) => context._hash.Add(value), ignore);
            }

            /// <summary>
            /// Adds a single value to the hash code using the provided <paramref name="hash"/> delegate to calculate the hash code.
            /// </summary>
            /// <typeparam name="T">The type of the value.</typeparam>
            /// <param name="value">The value.</param>
            /// <param name="hash">The hash delegate.</param>
            /// <param name="ignore">Set <c>true</c> to ignore this hash operation.</param>
            /// <remarks>
            /// If the value is <c>null</c> the value <c>0</c> is added to the hash and the <paramref name="hash"/> delegate is
            /// not invoked.
            /// </remarks>
            public void Hash<T>(T? value, Action<Context, T> hash, bool ignore = false)
            {
                ValidateArgument.For(hash, nameof(hash), v => v.NotNull());

                if (ignore)
                {
                    return;
                }

                if (value is null)
                {
                    _hash.Add(0);
                    return;
                }

                hash(this, value);
            }

            /// <summary>
            /// Adds a multiple values to the hash code using the default <see cref="EqualityComparer{T}"/> instance to calculate
            /// the hash code for each element in the collection.
            /// </summary>
            /// <typeparam name="T">The type of the value.</typeparam>
            /// <param name="collection">The collection.</param>
            /// <remarks>
            /// If the collection is <c>null</c> or does not contain elements the value <c>0</c> is added to the hash.
            /// For each <c>null</c> element in the collection the value <c>0</c> is added to the hash.
            /// </remarks>
            public void HashMany<T>(IReadOnlyCollection<T?>? collection)
            {
                HashMany(collection, (context, value) => context.Hash(value));
            }

            /// <summary>
            /// Adds a multiple values to the hash code using the provided <paramref name="hash"/> delegate to calculate the hash
            /// code for each element in the collection.
            /// </summary>
            /// <typeparam name="T">The type of the value.</typeparam>
            /// <param name="collection">The collection.</param>
            /// <param name="hash">The hash delegate.</param>
            /// <remarks>
            /// If the collection is <c>null</c> or does not contain elements the value <c>0</c> is added to the hash.
            /// For each <c>null</c> element in the collection the value <c>0</c> is added to the hash.
            /// </remarks>
            public void HashMany<T>(IReadOnlyCollection<T?>? collection, Action<Context, T> hash)
            {
                ValidateArgument.For(hash, nameof(hash), v => v.NotNull());

                if ((collection is null) || !collection.Any())
                {
                    _hash.Add(0);
                    return;
                }

                _hash.Add(collection.Count);
                foreach (var value in collection)
                {
                    Hash(value, hash);
                }
            }

            /// <summary>
            /// Adds the given expression to the hash code.
            /// </summary>
            /// <param name="node">The expression node.</param>
            /// <exception cref="NotSupportedException">If the expression type is not supported.</exception>
            /// <exception cref="NotImplementedException">If the expression type unknown.</exception>
            public void HashExpr(Expression? node)
            {
                _comparer.HashExpression(this, node);
            }

            /// <summary>
            /// Adds the given expressions to the hash code.
            /// </summary>
            /// <param name="nodes">The expression nodes.</param>
            /// <exception cref="NotSupportedException">If the expression type is not supported.</exception>
            /// <exception cref="NotImplementedException">If the expression type unknown.</exception>
            public void HashExpr(IReadOnlyCollection<Expression?>? nodes)
            {
                if ((nodes is null) || !nodes.Any())
                {
                    Hash(0);
                    return;
                }

                _hash.Add(nodes.Count);
                foreach (var node in nodes)
                {
                    HashExpr(node);
                }
            }

            /// <summary>
            /// Adds the unique id of the given reference to the hash code.
            /// </summary>
            /// <param name="obj">The object reference.</param>
            public void HashReferenceId(object? obj)
            {
                if (obj is null)
                {
                    Hash(0);
                    return;
                }

                if (!_referenceIds.TryGetValue(obj, out var id))
                {
                    id = _referenceIds.Count + 1;
                    _referenceIds[obj] = id;
                }

                Hash(id);
            }

            /// <summary>
            /// Returns the final hash code.
            /// </summary>
            /// <returns>The final hash code.</returns>
            public int ToHashCode()
            {
                return _hash.ToHashCode();
            }

            #endregion Hash Methods
        }

        #endregion Context
    }
}