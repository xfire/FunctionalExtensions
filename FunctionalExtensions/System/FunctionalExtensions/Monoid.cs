using System.Linq;
using System.Collections.Generic;

namespace System.FunctionalExtensions
{
    public interface Monoid<T>
    {
        T Mempty { get; }
        T Mappend(T a, T b);
        T Mconcat(IEnumerable<T> xs);
    }

    public static class Monoids
    {
        public static Monoid<T> Create<T>(T mempty, Func<T, T, T> mappend)
        {
            return new NormalMonoid<T>(mempty, mappend);
        }

        public static Monoid<int> IntAdd = Create(0, (x, y) => x + y);
        public static Monoid<int> IntMul = Create(1, (x, y) => x * y);
        public static Monoid<string> String = Create("", (x, y) => x + y);
        public static Monoid<bool> All = Create(true, (x, y) => x && y);
        public static Monoid<bool> Any = Create(false, (x, y) => x || y);
        public static Monoid<int> Ordering = Create(0, (x, y) => (x != 0) ? x : y);

        public static Monoid<IEnumerable<T>> Enumerable<T>()
        {
            return Create(Enumerable.Empty<T>(), (xs, ys) => xs.Concat(ys));
        }
        public static Monoid<Option<T>> Option<T>()
        {
            return Create(Option.None<T>(), (x, y) => x.IsNone ? y : x);
        }
    }

    class NormalMonoid<T> : Monoid<T>
    {
        private readonly T _mempty;
        private readonly Func<T, T, T> _mappend;

        public NormalMonoid(T mempty, Func<T, T, T> mappend)
        {
            _mempty = mempty;
            _mappend = mappend;
        }

        public T Mempty { get { return _mempty; } }

        public T Mappend(T a, T b)
        {
            return _mappend(a, b);
        }

        public T Mconcat(IEnumerable<T> xs)
        {
            return xs.Aggregate(_mempty, _mappend);
        }
    }
}
