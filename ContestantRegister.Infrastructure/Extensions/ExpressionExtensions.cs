using System;
using System.Linq.Expressions;

namespace ContestantRegister.Framework.Extensions
{
    public static class ExpressionExtensions
    {
        public static Func<TIn, TOut> AsFunc<TIn, TOut>(this Expression<Func<TIn, TOut>> expr)
            => CompiledExpressions<TIn, TOut>.AsFunc(expr);
    }
}
