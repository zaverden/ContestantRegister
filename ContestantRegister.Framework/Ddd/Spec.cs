﻿using System;
using System.Linq.Expressions;
using ContestantRegister.Framework.Extensions;

namespace ContestantRegister.Framework.Ddd
{
    public class Spec<T>
    {
        public static bool operator false(Spec<T> spec) => false;

        public static bool operator true(Spec<T> spec) => false;

        public static Spec<T> operator &(Spec<T> spec1, Spec<T> spec2)
            => new Spec<T>(spec1._expression.And(spec2._expression));

        public static Spec<T> operator |(Spec<T> spec1, Spec<T> spec2)
            => new Spec<T>(spec1._expression.Or(spec2._expression));

        public static Spec<T> operator !(Spec<T> spec)
            => new Spec<T>(spec._expression.Not());

        public static implicit operator Expression<Func<T, bool>>(Spec<T> spec)
            => spec._expression;

        public static implicit operator Spec<T>(Expression<Func<T, bool>> expression)
            => new Spec<T>(expression);

        private readonly Expression<Func<T, bool>> _expression;

        public Spec(Expression<Func<T, bool>> expression)
        {
            _expression = expression ?? throw new ArgumentNullException(nameof(expression));
        }

        // только для того, чтобы было похоже на спецификацию по евангелию, на практике не используется
        public bool IsSatisfiedBy(T obj) => _expression.AsFunc()(obj);
    }
}
