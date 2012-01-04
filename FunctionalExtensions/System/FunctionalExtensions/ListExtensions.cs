using System.Collections.Generic;
using System.Linq;

namespace System.FunctionalExtensions
{
    public static class ListExtensions
    {
        /// <summary>
        /// Extract the first element of a list, or return <c>None</c> if the list is empty.
        /// </summary>
        public static Option<T> Head<T>(this IEnumerable<T> xs)
        {
            foreach (var x in xs)
            {
                return Option.Some(x);
            }
            return Option.None<T>();
        }

        /// <summary>
        /// Extract the last element of a list, or return <c>None</c> if the list is empty.
        /// </summary>
        public static Option<T> LastOptional<T>(this IEnumerable<T> xs)
        {
            if (!xs.Any())
            {
                return Option.None<T>();
            }
            return Option.Some(xs.Last());
        }

        /// <summary>
        /// Extract the elements after the head of a list.
        /// </summary>
        public static IEnumerable<T> Tail<T>(this IEnumerable<T> xs)
        {
            return xs.Skip(1);
        }

        /// <summary>
        /// Return all the elements of a list except the last one.
        /// </summary>
        public static IEnumerable<T> Init<T>(this IEnumerable<T> xs)
        {
            return xs.Take(xs.Count() - 1);
        }

        /// <summary>
        /// Flatten the specified list of lists.
        /// </summary>
        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> xss)
        {
            return xss.SelectMany(xs => xs);
        }

        /// <summary>
        /// The intersperse function takes an element and a list and `intersperses' that element
        /// between the elements of the list.
        /// </summary>
        public static IEnumerable<T> Intersperse<T>(this IEnumerable<T> xs, T element)
        {
            var e = xs.GetEnumerator();
            var isValid = e.MoveNext();
            while (isValid)
            {
                yield return e.Current;
                isValid = e.MoveNext();
                if (isValid)
                {
                    yield return element;
                }
            }
        }

        /// <summary>
        /// Intercalate inserts the list ys in between the lists in xss and concatenates the result.
        /// </summary>
        public static IEnumerable<T> Intercalate<T>(this IEnumerable<IEnumerable<T>> xss, IEnumerable<T> ys)
        {
            return Intersperse(xss, ys).Flatten();
        }

        /// <summary>
        /// The transpose function transposes the rows and columns of its argument.
        /// </summary>
        public static IEnumerable<IEnumerable<T>> Transpose<T>(this IEnumerable<IEnumerable<T>> xss)
        {
            var es = xss.Select(xs => xs.GetEnumerator()).ToList();

            bool next;
            do
            {
                next = false;
                var step = new List<T>();
                foreach (var e in es.Where(e => e.MoveNext()))
                {
                    next = true;
                    step.Add(e.Current);
                }
                if (next)
                {
                    yield return step;
                }
            } while (next);
        }

        /// <summary>
        /// And returns the conjunction of a Boolean list.
        /// </summary>
        public static bool And(this IEnumerable<bool> xs)
        {
            return xs.Aggregate(true, (acc, v) => acc && v);
        }

        /// <summary>
        /// Or returns the disjunction of a Boolean list.
        /// </summary>
        public static bool Or(this IEnumerable<bool> xs)
        {
            return xs.Aggregate(false, (acc, v) => acc || v);
        }

        /// <summary>
        /// Iterate returns an infinite list of repeated applications of func to startValue.
        /// </summary>
        public static IEnumerable<T> Iterate<T>(this T startValue, Func<T, T> func)
        {
            var value = startValue;
            while (true)
            {
                yield return value;
                value = func(value);
            }
        }

        /// <summary>
        /// Test whether an enumerable is empty or not.
        /// </summary>
        public static bool IsEmpty<T>(this IEnumerable<T> xs)
        {
            return !xs.Any();
        }

        /// <summary>
        /// Cycle ties a finite list into a circular one, or equivalently, the infinite repetition of the original
        /// list. It is the identity on infinite lists.
        /// Warning: Cycle on an empty list, returns an empty list.
        /// </summary>
        public static IEnumerable<T> Cycle<T>(this IEnumerable<T> xs)
        {
            if (xs.IsEmpty())
            {
                yield break;
            }
            while (true)
            {
                foreach (var x in xs)
                {
                    yield return x;
                }
            }
        }

        /// <summary>
        /// SplitAt returns a tuple where first element is xs prefix of length n and the second element is the
        /// remainder of the list.
        /// </summary>
        public static Tuple<IEnumerable<T>, IEnumerable<T>> SplitAt<T>(this IEnumerable<T> xs, int n)
        {
            return Tuple.Create(xs.Take(n), xs.Skip(n));
        }

        /// <summary>
        /// Span, applied to a predicate and a list, returns a tuple where first element is longest prefix
        /// (possibly empty) of elements that satisfy the predicate and second element is the remainder of the list.
        /// </summary>
        public static Tuple<IEnumerable<T>, IEnumerable<T>> Span<T>(this IEnumerable<T> xs, Func<T, bool> predicate)
        {
            return Tuple.Create(xs.TakeWhile(predicate), xs.SkipWhile(predicate));
        }

        /// <summary>
        /// Break, applied to a predicate and a list, returns a tuple where first element is longest prefix
        /// (possibly empty) of elements that do not satisfy the predicate and second element is the remainder of the list.
        /// </summary>
        public static Tuple<IEnumerable<T>, IEnumerable<T>> Break<T>(this IEnumerable<T> xs, Func<T, bool> predicate)
        {
            return xs.Span(x => !predicate(x));
        }

        /// <summary>
        /// Returns true if the value is not in the list.
        /// </summary>
        public static bool ContainsNot<T>(this IEnumerable<T> xs, T value)
        {
            return !xs.Contains(value);
        }

        /// <summary>
        /// The partition function takes a predicate a list and returns the pair of lists of elements which do
        /// and do not satisfy the predicate.
        /// </summary>
        public static Tuple<IEnumerable<T>, IEnumerable<T>> Partition<T>(this IEnumerable<T> xs, Func<T, bool> predicate)
        {
            return Tuple.Create(xs.Where(predicate), xs.Where(x => !predicate(x)));
        }

        /// <summary>
        /// Zip takes two lists and returns a list of corresponding pairs. If one input list is short, excess
        /// elements of the longer list are discarded.
        /// </summary>
        public static IEnumerable<Tuple<A, B>> Zip<A, B>(this IEnumerable<A> xs, IEnumerable<B> ys)
        {
            return xs.Zip(ys, Tuple.Create);
        }

        /// <summary>
        /// Zip takes three lists and returns a list of corresponding pairs. If one input list is short, excess
        /// elements of the longer list are discarded.
        /// </summary>
        public static IEnumerable<Tuple<A, B, C>> Zip<A, B, C>(this IEnumerable<A> xs, IEnumerable<B> ys, IEnumerable<C> zs)
        {
            var ex = xs.GetEnumerator();
            var ey = ys.GetEnumerator();
            var ez = zs.GetEnumerator();

            while (true)
            {
                if (ex.MoveNext() && ey.MoveNext() && ez.MoveNext())
                {
                    yield return Tuple.Create(ex.Current, ey.Current, ez.Current);
                }
                else
                {
                    yield break;
                }
            }
        }

        /// <summary>
        /// Zip takes four lists and returns a list of corresponding pairs. If one input list is short, excess
        /// elements of the longer list are discarded.
        /// </summary>
        public static IEnumerable<Tuple<A, B, C, D>> Zip<A, B, C, D>(this IEnumerable<A> ws, IEnumerable<B> xs, IEnumerable<C> ys, IEnumerable<D> zs)
        {
            var ew = ws.GetEnumerator();
            var ex = xs.GetEnumerator();
            var ey = ys.GetEnumerator();
            var ez = zs.GetEnumerator();

            while (true)
            {
                if (ew.MoveNext() && ex.MoveNext() && ey.MoveNext() && ez.MoveNext())
                {
                    yield return Tuple.Create(ew.Current, ex.Current, ey.Current, ez.Current);
                }
                else
                {
                    yield break;
                }
            }
        }

        /// <summary>
        /// Execute an side effect on each item in the enumerable <paramref name="xs"/>. (<see cref="List{T}.ForEach"/>)
        /// </summary>
        public static void ForEach<T>(this IEnumerable<T> xs, Action<T> sideEffect)
        {
            foreach (var x in xs)
            {
                sideEffect(x);
            }
        }

        /// <summary>
        /// Return a string representation of the given <see cref="IEnumerable{T}"/> <paramref name="xs"/>.
        /// </summary>
        public static string AsString<T>(this IEnumerable<T> xs, string prefix = "[", string suffix = "]", string seperator = ", ")
        {
            return prefix + string.Join(seperator, xs) + suffix;
        }
    }
}
