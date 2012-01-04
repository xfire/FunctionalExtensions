
using System.Collections.Generic;

namespace System.FunctionalExtensions
{
    public static class RangeExtension
    {
        /// <summary>
        /// Generates a sequence of integral numbers within a specified range
        /// [<paramref name="from"/>..<paramref name="to"/>].
        /// </summary>
        public static IEnumerable<int> To(this int @from, int to)
        {
            return @from > to ? @from.To(to, -1) : @from.To(to, 1);
        }

        /// <summary>
        /// Generates a sequence of integral numbers within a specified range
        /// [<paramref name="from"/>..<paramref name="to"/>] using the specified <paramref name="step"/>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <para><paramref name="step"/> is 0</para>
        /// <para><paramref name="step"/> is positive but <paramref name="from"/> is bigger than <paramref name="to"/></para>
        /// <para><paramref name="step"/> is negative but <paramref name="from"/> is smaller than <paramref name="to"/></para>
        /// </exception>
        public static IEnumerable<int> To(this int @from, int to, int step)
        {
            if(step == 0) throw new ArgumentException();
            if(step > 0 && @from > to) throw new ArgumentOutOfRangeException();
            if(step < 0 && @from < to) throw new ArgumentOutOfRangeException();

            if (step > 0)
            {
                for (var i = @from; i <= to; i += step)
                {
                    yield return i;
                }
            }
            else
            {
                for (var i = @from; i >= to; i += step)
                {
                    yield return i;
                }
            }
        }
    }
}
