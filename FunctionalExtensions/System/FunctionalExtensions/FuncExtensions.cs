
namespace System.FunctionalExtensions
{
    public static class FuncExtensions
    {
        /// <summary>
        /// Convert a value of type <typeparamref name="T"/> to a 0-ary function return the value.
        /// </summary>
        public static Func<T> Const<T>(this T value)
        {
            return () => value;
        } 

        /// <summary>
        /// Flip the arguments of an 2-ary function.
        /// </summary>
        public static Func<TB, TA, TC> Flip<TA, TB, TC>(this Func<TA, TB, TC> f)
        {
            return (x, y) => f(y, x);
        }

        /// <summary>
        /// Curries a 1-ary function, eg. return a function which takes the first parameter
        /// and which returns the constant function.
        /// </summary>
        public static Func<TA, Func<TB>> Curry<TA, TB>(this Func<TA, TB> f)
        {
            return x => () => f(x);
        }

        /// <summary>
        /// Curries a 1-ary function, and apply <paramref name="value"/> to the first parameter.
        /// </summary>
        /// <returns>A constanct function which returns the result.</returns>
        public static Func<TB> Curry<TA, TB>(this Func<TA, TB> f, TA value)
        {
            return () => f(value);
        }

        /// <summary>
        /// Curries a 2-ary function, eg. return a function which takes the first parameter
        /// and which returns another function wich takes the second parameter and returns
        /// the result.
        /// </summary>
        public static Func<TA, Func<TB, TC>> Curry<TA, TB, TC>(this Func<TA, TB, TC> f)
        {
            return x => y => f(x, y);
        }

        /// <summary>
        /// Curries a 2-ary function, and apply <paramref name="value"/> to the first parameter.
        /// </summary>
        /// <returns>A function which takes the second parameter of the original function <paramref name="f"/> an which returns the result.</returns>
        public static Func<TB, TC> Curry<TA, TB, TC>(this Func<TA, TB, TC> f, TA value)
        {
            return x => f(value, x);
        }

        /// <summary>
        /// Curries a 3-ary function, eg. return a function which takes the first parameter
        /// and which returns another function wich takes the other parameters and returns
        /// the result.
        /// </summary>
        public static Func<TA, Func<TB, TC, TD>> Curry<TA, TB, TC, TD>(this Func<TA, TB, TC, TD> f)
        {
            return a => (b, c) => f(a, b, c);
        }

        /// <summary>
        /// Curries a 3-ary function, eg. return a function which takes the first parameter
        /// and which returns another function wich takes the other parameters and returns
        /// the result.
        /// </summary>
        public static Func<TB, TC, TD> Curry<TA, TB, TC, TD>(this Func<TA, TB, TC, TD> f, TA value)
        {
            return (x, y) => f(value, x, y);
        }

        /// <summary>
        /// Compose two 1-ary functions, e.g. return a new function which call <paramref name="f"/> with the
        /// result of <paramref name="g"/> applied to the parameter.
        /// </summary>
        public static Func<TA, TC> Compose<TA, TB, TC>(this Func<TB, TC> f, Func<TA, TB> g)
        {
            return x => f(g(x));
        }

        /// <summary>
        /// Execute the function <paramref name="f"/> and return the result as <c>Right</c>. All catched
        /// <see cref="Exception"/>'s are returned as <c>Left</c>.
        /// </summary>
        public static Either<Exception, TR> TryCatch<TR>(this Func<TR> f)
        {
            try
            {
                return f().Right<Exception, TR>();
            }
            catch (Exception e)
            {
                return e.Left<Exception, TR>();
            }
        }

        /// <summary>
        /// Execute the function <paramref name="f"/> and return the result as <c>Right</c>. All catched
        /// <see cref="Exception"/>'s are returned as <c>Left</c>.
        /// </summary>
        public static Either<Exception, TR> TryCatch<TA, TR>(this Func<TA, TR> f, TA a)
        {
            return f.Curry(a).TryCatch();
        }

        /// <summary>
        /// Execute the function <paramref name="f"/> and return the result as <c>Right</c>. All catched
        /// <see cref="Exception"/>'s are returned as <c>Left</c>.
        /// </summary>
        public static Either<Exception, TR> TryCatch<TA, TB, TR>(this Func<TA, TB, TR> f, TA a, TB b)
        {
            return f.Curry(a).Curry(b).TryCatch();
        }

        /// <summary>
        /// Execute the function <paramref name="f"/> and return the result as <c>Right</c>. All catched
        /// <see cref="Exception"/>'s are returned as <c>Left</c>.
        /// </summary>
        public static Either<Exception, TR> TryCatch<TA, TB, TC, TR>(this Func<TA, TB, TC, TR> f, TA a, TB b, TC c)
        {
            return f.Curry(a).Curry(b).Curry(c).TryCatch();
        }

        /// <summary>
        /// Map a function over a 0-ary function.
        /// </summary>
        public static Func<TB> Select<TA, TB>(this Func<TA> f, Func<TA, TB> g)
        {
            return () => g(f());
        }

        /// <summary>
        /// Map a function over a 1-ary function.
        /// </summary>
        public static Func<TA, TC> Select<TA, TB, TC>(this Func<TA, TB> f, Func<TB, TC> g)
        {
            return x => g(f(x));
        }

        /// <summary>
        /// Monadic bind.
        /// </summary>
        public static Func<TB> SelectMany<TA, TB>(this Func<TA> f, Func<TA, Func<TB>> g)
        {
            return g(f());
        }

        /// <summary>
        /// Monadic bind.
        /// </summary>
        public static Func<TC> SelectMany<TA, TB, TC>(this Func<TA> k, Func<TA, Func<TB>> p, Func<TA, TB, TC> f)
        {
            return SelectMany<TA, TC>(k, x => Select<TB, TC>(p(x), y => f(x, y)));
        }

        /// <summary>
        /// Monadic bind.
        /// </summary>
        public static Func<TC, TB> SelectMany<TA, TB, TC>(this Func<TC, TA> f, Func<TA, Func<TC, TB>> g)
        {
            return x => g(f(x))(x);
        }

        /// <summary>
        /// Monadic bind.
        /// </summary>
        public static Func<TC, TD> SelectMany<TA, TB, TC, TD>(this Func<TC, TA> f, Func<TA, Func<TC, TB>> p, Func<TA, TB, TD> k)
        {
            return SelectMany<TA, TD, TC>(f, x => Select<TC, TB, TD>(p(x), y => k(x, y)));
        }

        /// <summary>
        /// Arrow: Create a function which will feed it's input (<typeparamref name="TA"/>) to the functions <paramref name="f"/>
        /// and <paramref name="g"/> and return a <see cref="Tuple{TB, TC}"/> with the results of <paramref name="f"/> and
        /// <paramref name="g"/>.
        /// </summary>
        public static Func<TA, Tuple<TB, TC>> And<TA, TB, TC>(this Func<TA, TB> f, Func<TA, TC> g)
        {
            return a => Tuple.Create(f(a), g(a));
        }

        /// <summary>
        /// Arrow: Create a function which will use the functions <paramref name="f"/> and <paramref name="g"/> to transform
        /// the values in the the input <see cref="Tuple{TA,TB}"/> to the return value <see cref="Tuple{TC, TD}"/>.
        /// </summary>
        public static Func<Tuple<TA, TB>, Tuple<TC, TD>> Product<TA, TB, TC, TD>(this Func<TA, TC> f, Func<TB, TD> g)
        {
            return ab => Tuple.Create(f(ab.Item1), g(ab.Item2));
        }

        /// <summary>
        /// Arrow: Create a function which will apply the function <paramref name="f"/> to the first value of its given
        /// <see cref="Tuple{TA, TC}"/> parameter.
        /// </summary>
        public static Func<Tuple<TA, TC>, Tuple<TB, TC>> First<TA, TB, TC>(this Func<TA, TB> f)
        {
            return ac => Tuple.Create(f(ac.Item1), ac.Item2);
        }

        /// <summary>
        /// Arrow: Create a function which will apply the function <paramref name="f"/> to the first value of its given
        /// <see cref="Tuple{TC, TA}"/> parameter.
        /// </summary>
        public static Func<Tuple<TC, TA>, Tuple<TC, TB>> Second<TA, TB, TC>(this Func<TA, TB> f)
        {
            return ca => Tuple.Create(ca.Item1, f(ca.Item2));
        }

    }
}
