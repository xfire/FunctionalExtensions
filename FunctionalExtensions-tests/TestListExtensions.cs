using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace System.FunctionalExtensions.Tests
{
    [TestFixture]
    public class TestListExtensions
    {
        static readonly Option<int> None = Option.None<int>();
        static readonly IEnumerable<int> Empty = new List<int>();
        static readonly IEnumerable<int> Single = new List<int>() { 23 };
        static readonly IEnumerable<int> List = new List<int>() { 23, 42 };
        static readonly IEnumerable<IEnumerable<int>> NestedEmpty = new List<IEnumerable<int>>();
        static readonly IEnumerable<IEnumerable<int>> NestedSingle = new List<IEnumerable<int>>() { new List<int>() { 23 } };
        static readonly IEnumerable<IEnumerable<int>> NestedLists = new List<IEnumerable<int>>() {
            Enumerable.Range(1, 3),
            Enumerable.Range (4, 1),
            Enumerable.Range (5, 2),
            Enumerable.Range (7, 4),
        };
        static readonly IEnumerable<bool> EmptyBools = new List<bool>();
        static readonly IEnumerable<bool> AllTrue = new List<bool>() { true, true, true };
        static readonly IEnumerable<bool> AllFalse = new List<bool>() { false, false, false };
        static readonly IEnumerable<bool> MixedBools = new List<bool>() { true, false, true };

        [Test]
        public void HeadOfEmptyListIsNone()
        {
            Assert.That(Empty.Head(), Is.EqualTo(None));
        }

        [Test]
        public void HeadOfFullListIsSome()
        {
            Assert.That(List.Head(), Is.EqualTo(Option.Some(23)));
        }

        [Test]
        public void LastOfEmptyListIsNone()
        {
            Assert.That(Empty.LastOptional(), Is.EqualTo(None));
        }

        [Test]
        public void LastOfFullListIsSome()
        {
            Assert.That(List.LastOptional(), Is.EqualTo(Option.Some(42)));
        }

        [Test]
        public void TailOfEmptyListIsTheEmptyList()
        {
            Assert.That(Empty.Tail(), Is.EquivalentTo(Empty));
        }

        [Test]
        public void TailOfListWithOnlyOneElementIsTheEmptyList()
        {
            Assert.That(Single.Tail(), Is.EquivalentTo(Empty));
        }

        [Test]
        public void TailOfListWithMultipleElementsIsTheListWithoutTheFirstItem()
        {
            Assert.That(List.Tail().ToList(), Has.Count.EqualTo(1).And.Contains(42));
        }

        [Test]
        public void InitOfEmptyListIsTheEmptyList()
        {
            Assert.That(Empty.Init(), Is.EquivalentTo(Empty));
        }

        [Test]
        public void InitOfListWithExactlyOneElementIsTheEmptyList()
        {
            Assert.That(Single.Init(), Is.EquivalentTo(Empty));
        }

        [Test]
        public void InitOfListWithMultipleElementsIsTheListWithoutTheLastElement()
        {
            Assert.That(List.Init().ToList(), Has.Count.EqualTo(1).And.Contains(23));
        }

        [Test]
        public void IntersperseAnEmptyListResultsInAnEmptyList()
        {
            Assert.That(Empty.Intersperse(-1), Is.EqualTo(Empty));
        }

        [Test]
        public void IntersperseAListWithExactlyOneElementResultsInTheSameList()
        {
            Assert.That(Single.ToList(), Has.Count.EqualTo(1).And.Contains(23));
        }

        [Test]
        public void IntersperseAListWithMultipleElementsResultsInAListWithTheSpecialElementBetweenEachListElement()
        {
            Assert.That(List.Intersperse(-1), Is.EquivalentTo(new List<int>() { 23, -1, 42 }));
        }

        [Test]
        public void FlattenAnEmptyListResultsInAnEmptyList()
        {
            var nested = new List<IEnumerable<int>>();
            Assert.That(nested.Flatten(), Is.EqualTo(Empty));
        }

        [Test]
        public void FlattenANestedListResultsInASingleListContainingAllElements()
        {
            var expected = Enumerable.Range(1, 10);
            Assert.That(NestedLists.Flatten(), Is.EquivalentTo(expected));
        }

        [Test]
        public void IntercalateElementsIntoAnEmptyListResultsInAnEmptyList()
        {
            var empty = new List<IEnumerable<int>>();
            Assert.That(empty.Intercalate(List), Is.EqualTo(Empty));
        }

        [Test]
        public void IntercalateAnEmptyListIntoAFilledListResultsInTheFilledListUnmodified()
        {
            var expected = Enumerable.Range(1, 10);
            Assert.That(NestedLists.Intercalate(Empty), Is.EquivalentTo(expected));
        }

        [Test]
        public void IntercalateElementsIntoAFilledListResultsInAListWithTheSpecialElementsBetweenEachListElement()
        {
            var expected = new List<int>() { 1, 2, 3, 23, 42, 4, 23, 42, 5, 6, 23, 42, 7, 8, 9, 10 };
            Assert.That(NestedLists.Intercalate(List), Is.EquivalentTo(expected));
        }

        [Test]
        public void TransposeAnEmptyNestedListResultsInAnEmptyNestedList()
        {
            Assert.That(NestedEmpty.Transpose(), Is.EquivalentTo(NestedEmpty));
        }

        [Test]
        public void TransposeAnSingleNestedList()
        {
            Assert.That(NestedSingle.Transpose(), Is.EquivalentTo(NestedSingle));
        }

        [Test]
        public void TransposeAnNestedList()
        {
            var expected = new List<IEnumerable<int>>()
                               {
                                   new List<int>() {1,4,5,7},
                                   new List<int>() {2,6,8},
                                   new List<int>() {3,9},
                                   new List<int>() {10},
                               };
            Assert.That(NestedLists.Transpose(), Is.EquivalentTo(expected));
        }

        [Test]
        public void AndListOfBooleans()
        {
            Assert.That(EmptyBools.And(), Is.True);
            Assert.That(AllTrue.And(), Is.True);
            Assert.That(MixedBools.And(), Is.False);
        }

        [Test]
        public void OrListOfBooleans()
        {
            Assert.That(EmptyBools.Or(), Is.False);
            Assert.That(AllFalse.Or(), Is.False);
            Assert.That(AllTrue.Or(), Is.True);
            Assert.That(MixedBools.Or(), Is.True);
        }

        [Test]
        public void IterateOnAddOneReturnsAListWithAscendingNumbers()
        {
            const int n = 11;
            Assert.That(0.Iterate(x => x + 1).Take(n), Is.EquivalentTo(Enumerable.Range(0, n)));
        }

        [Test]
        public void CycleOnAnEmptyListReturnTheEmptyList()
        {
            Assert.That(Empty.Cycle(), Is.EqualTo(Empty));
        }

        [Test]
        public void CycleReturnsTheListInfinitely()
        {
            Assert.That(List.Cycle().Take(List.Count() * 2), Is.EquivalentTo(List.Concat(List)));
        }

        [Test]
        public void CheckWhetherAListIsEmptyOrNot()
        {
            Assert.That(Empty.IsEmpty(), Is.True);
            Assert.That(List.IsEmpty(), Is.False);
        }

        [Test]
        public void SplitAnEmptyListResultsInATupleOfTwoEmptyLists()
        {
            Assert.That(Empty.SplitAt(2).Item1, Is.EquivalentTo(Empty));
            Assert.That(Empty.SplitAt(2).Item2, Is.EquivalentTo(Empty));
        }

        [Test]
        public void SplitAListAtAGivenPosition()
        {
            var list = Enumerable.Range(1, 3);
            var longList = Enumerable.Range(1, 5);

            Assert.That(list.SplitAt(1).Item1, Is.EquivalentTo(new List<int>() { 1 }));
            Assert.That(list.SplitAt(1).Item2, Is.EquivalentTo(new List<int>() { 2, 3 }));

            Assert.That(list.SplitAt(3).Item1, Is.EquivalentTo(list));
            Assert.That(list.SplitAt(3).Item2, Is.EquivalentTo(Empty));

            Assert.That(list.SplitAt(4).Item1, Is.EquivalentTo(list));
            Assert.That(list.SplitAt(4).Item2, Is.EquivalentTo(Empty));

            Assert.That(list.SplitAt(0).Item1, Is.EquivalentTo(Empty));
            Assert.That(list.SplitAt(0).Item2, Is.EquivalentTo(list));

            Assert.That(list.SplitAt(-1).Item1, Is.EquivalentTo(Empty));
            Assert.That(list.SplitAt(-1).Item2, Is.EquivalentTo(list));

            Assert.That(longList.SplitAt(3).Item1, Is.EquivalentTo(new List<int>() { 1, 2, 3 }));
            Assert.That(longList.SplitAt(3).Item2, Is.EquivalentTo(new List<int>() { 4, 5 }));
        }

        [Test]
        public void SpanOnAnEmptyListResultsInAnTupleOfTwoEmptyLists()
        {
            Assert.That(Empty.Span(x => x > 2).Item1, Is.EquivalentTo(Empty));
            Assert.That(Empty.Span(x => x > 2).Item2, Is.EquivalentTo(Empty));
        }

        [Test]
        public void SpanAListWithAPredicate()
        {
            var shortList = new List<int>() { 1, 2, 3 };
            var longList = new List<int>() { 1, 2, 3, 4, 1, 2, 3, 4 };

            Assert.That(shortList.Span(x => x < 9).Item1, Is.EquivalentTo(shortList));
            Assert.That(shortList.Span(x => x < 9).Item2, Is.EquivalentTo(Empty));

            Assert.That(shortList.Span(x => x < 0).Item1, Is.EquivalentTo(Empty));
            Assert.That(shortList.Span(x => x < 0).Item2, Is.EquivalentTo(shortList));

            Assert.That(longList.Span(x => x < 3).Item1, Is.EquivalentTo(new List<int>() { 1, 2 }));
            Assert.That(longList.Span(x => x < 3).Item2, Is.EquivalentTo(new List<int>() { 3, 4, 1, 2, 3, 4 }));
        }

        [Test]
        public void BreakOnAnEmptyListResultsInAnTupleOfTwoEmptyLists()
        {
            Assert.That(Empty.Break(x => x > 2).Item1, Is.EquivalentTo(Empty));
            Assert.That(Empty.Break(x => x > 2).Item2, Is.EquivalentTo(Empty));
        }

        [Test]
        public void BreakAListWithAPredicate()
        {
            var shortList = new List<int>() { 1, 2, 3 };
            var longList = new List<int>() { 1, 2, 3, 4, 1, 2, 3, 4 };

            Assert.That(shortList.Break(x => x < 9).Item1, Is.EquivalentTo(Empty));
            Assert.That(shortList.Break(x => x < 9).Item2, Is.EquivalentTo(shortList));

            Assert.That(shortList.Break(x => x > 9).Item1, Is.EquivalentTo(shortList));
            Assert.That(shortList.Break(x => x > 9).Item2, Is.EquivalentTo(Empty));

            Assert.That(longList.Break(x => x > 3).Item1, Is.EquivalentTo(new List<int>() { 1, 2, 3 }));
            Assert.That(longList.Break(x => x > 3).Item2, Is.EquivalentTo(new List<int>() { 4, 1, 2, 3, 4 }));
        }

        [Test]
        public void AValueCanBeTestedIfItIsNotInAList()
        {
            Assert.That(Empty.ContainsNot(23), Is.True);
            Assert.That(List.ContainsNot(23), Is.False);
            Assert.That(List.ContainsNot(-1), Is.True);
        }

        [Test]
        public void PartitioningAnEmptyListResultsInAnEmptyList()
        {
            Assert.That(Empty.Partition(x => x > 3).Item1, Is.EquivalentTo(Empty));
            Assert.That(Empty.Partition(x => x > 3).Item2, Is.EquivalentTo(Empty));
        }

        [Test]
        public void AListCanBePartitionedByAPredicate()
        {
            var numbers = Enumerable.Range(1, 10);
            var even = numbers.Where(x => x % 2 == 0);
            var odd = numbers.Where(x => x % 2 != 0);

            Assert.That(numbers.Partition(x => x % 2 == 0).Item1, Is.EquivalentTo(even));
            Assert.That(numbers.Partition(x => x % 2 == 0).Item2, Is.EquivalentTo(odd));
        }

        [Test]
        public void ZippingTwoListsOnOfThemEmptyReturnsInAnEmptyList()
        {
            var empty = new List<Tuple<int, int>>();
            Assert.That(List.Zip(Empty), Is.EquivalentTo(empty));
            Assert.That(Empty.Zip(List), Is.EquivalentTo(empty));
            Assert.That(Empty.Zip(Empty), Is.EquivalentTo(empty));
        }

        [Test]
        public void ZippingTwoListsResultsInAListOfTuples()
        {
            var xs = Enumerable.Range(1, 4);
            var ys = Enumerable.Range(10, 50).Where(x => x % 10 == 0);
            var expected = new List<Tuple<int, int>>()
                               {
                                   Tuple.Create(1,10),
                                   Tuple.Create(2,20),
                                   Tuple.Create(3,30),
                                   Tuple.Create(4,40)
                               };
            Assert.That(xs.Zip(ys).ToList(), Is.EquivalentTo(expected));
        }

        [Test]
        public void ZippingThreeListsOnOfThemEmptyReturnsInAnEmptyList()
        {
            var empty = new List<Tuple<int, int, int>>();
            foreach (var xs in new List<IEnumerable<int>>() { List, Empty })
            {
                Assert.That(xs.Zip(List, Empty), Is.EquivalentTo(empty));
                Assert.That(xs.Zip(Empty, List), Is.EquivalentTo(empty));
                Assert.That(xs.Zip(Empty, Empty), Is.EquivalentTo(empty));
            }
        }

        [Test]
        public void ZippingThreeListsResultsInAListOfTuples()
        {
            var xs = Enumerable.Range(1, 4);
            var ys = Enumerable.Range(10, 80).Where(x => x % 10 == 0);
            var zs = Enumerable.Range(100, 500).Where(x => x % 100 == 0);
            var expected = new List<Tuple<int, int, int>>()
            {
                Tuple.Create(1,10, 100),
                Tuple.Create(2,20, 200),
                Tuple.Create(3,30, 300),
                Tuple.Create(4,40, 400) 
            };
            Assert.That(xs.Zip(ys, zs).ToList(), Is.EquivalentTo(expected));
        }

        [Test]
        public void ZippingFourListsOnOfThemEmptyReturnsInAnEmptyList()
        {
            var empty = new List<Tuple<int, int, int, int>>();
            foreach (var xs in new List<IEnumerable<int>>() { List, Empty })
            {
                Assert.That(xs.Zip(List, List, Empty), Is.EquivalentTo(empty));
                Assert.That(xs.Zip(List, Empty, List), Is.EquivalentTo(empty));
                Assert.That(xs.Zip(List, Empty, Empty), Is.EquivalentTo(empty));
                Assert.That(xs.Zip(Empty, List, Empty), Is.EquivalentTo(empty));
                Assert.That(xs.Zip(Empty, Empty, List), Is.EquivalentTo(empty));
                Assert.That(xs.Zip(Empty, Empty, Empty), Is.EquivalentTo(empty));
            }
        }
        
        [Test]
        public void ZippingFourListsResultsInAListOfTuples()
        {
            var xs = Enumerable.Range(1, 6);
            var ys = Enumerable.Range(10, 80).Where(x => x % 10 == 0);
            var zs = Enumerable.Range(100, 500).Where(x => x % 100 == 0);
            var ws = Enumerable.Range(1000, 5000).Where(x => x % 1000 == 0);
            var expected = new List<Tuple<int, int, int, int>>()
            {
                Tuple.Create(1, 10, 100, 1000),
                Tuple.Create(2, 20, 200, 2000),
                Tuple.Create(3, 30, 300, 3000),
                Tuple.Create(4, 40, 400, 4000),
                Tuple.Create(5, 50, 500, 5000) 
            };
            Assert.That(xs.Zip(ys, zs, ws).ToList(), Is.EquivalentTo(expected));
        }

        [Test]
        public void WeCanExecuteASideEffectOnEachElementOfAnEnumerable()
        {
            var list = Enumerable.Range(1, 10);
            var result = new List<int>();
            list.ForEach(result.Add);
            Assert.That(result, Is.EquivalentTo(list));
        }

        [Test]
        public void AnEnumerableCanBeConvertedToAStringRepresentation()
        {
            Assert.That(1.To(5).AsString(), Is.EqualTo("[1, 2, 3, 4, 5]"));
            Assert.That(1.To(3).AsString("{", "}", "_"), Is.EqualTo("{1_2_3}"));
            Assert.That(Enumerable.Empty<int>().AsString(), Is.EqualTo("[]"));
        }
    }
}
