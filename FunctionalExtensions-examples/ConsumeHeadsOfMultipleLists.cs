using System;
using System.Collections.Generic;
using System.Linq;
using System.FunctionalExtensions;
using NUnit.Framework;

namespace System.FunctionalExtensions.Examples
{
    static class ListExtensionExamples
    {
        /// <summary>
        /// Returns the first element of an enumerable as <c>Some</c>, or if the enumerable is empty,
        /// return <c>None</c>.
        /// </summary>
        public static Option<T> Head<T>(this IEnumerable<T> xs)
        {
            foreach (var x in xs)
            {
                return Option.Some(x);
            }
            return Option.None<T>();
        }
    }

    [TestFixture]
    internal class ConsumeHeadsOfMultipleLists
    {
        [Test]
        public void ConsumeHeadsSuccessfully()
        {
            var list1 = new List<int>() {1, 2, 3};
            var list2 = new List<int>() {23, 42};
            var list3 = new List<int>() {5000, 6000, 7000, 8000};

            Func<int, int, int, int> sum = (a, b, c) => a + b + c;

            // get the heads as Option<int> of all lists
            var result = from a in list1.Head()  // Some(1)
                         from b in list2.Head()  // Some(23)
                         from c in list3.Head()  // Some(5000)
                         select sum(a, b, c);    // all are Some's, so sum them up

            Assert.That(result.IsSome, Is.True);
            Assert.That(result.Value, Is.EqualTo(1 + 23 + 5000));
        }

        [Test]
        public void ConsumeHeadsFailing()
        {
            var list1 = new List<int>() {1, 2, 3};
            var list2 = Enumerable.Empty<int>();    // <-- empty list, no head available
            var list3 = new List<int>() {5000, 6000, 7000, 8000};

            Func<int, int, int, int> sum = (a, b, c) => a + b + c;

            // get the heads as Option<int> of all lists
            var result = from a in list1.Head()  // Some(1)
                         from b in list2.Head()  // ouch, we failed! -> None
                         from c in list3.Head()  // Some(5000)
                         select sum(a, b, c);    // we have a None, so fail the complete computation.
                                                 // sum() is never been called.

            Assert.That(result.IsNone, Is.True);
        }
    }
}
