using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace System.FunctionalExtensions.Tests
{
    [TestFixture]
    class TestOptionExtensions
    {
        private static readonly Option<string> SomeOption = Option.Some("SomeOption");
        private static readonly Option<string> SomeNone = Option.None<string>();

        private static readonly Option<int> SomeIntOption = Option.Some(23);
        private static readonly Option<int> SomeIntNone = Option.None<int>();

        private static readonly Func<int, int> Add = i => i + 1;
            
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ValueWithExceptionFunction()
        {
            Func<Exception> f = () => new ArgumentException();
            Assert.That(SomeOption.Value(f), Is.EqualTo(SomeOption.Value));
            SomeNone.Value(f);
        }

        [Test]
        public void ValueWithDefaultValueFunction()
        {
            const string otherValue = "optionValue";
            Func<string> f = () => otherValue;

            Assert.That(SomeOption.Value(f), Is.EqualTo(SomeOption.Value));
            Assert.That(SomeNone.Value(f), Is.EqualTo(otherValue));
        }

        [Test]
        public void ValueWithDefaultValue()
        {
            const string otherValue = "optionValue";

            Assert.That(SomeOption.Value(otherValue), Is.EqualTo(SomeOption.Value));
            Assert.That(SomeNone.Value(otherValue), Is.EqualTo(otherValue));
        }

        [Test]
        public void ValueWithTypeDefaultValue()
        {
            Assert.That(SomeOption.ValueOrDefault(), Is.EqualTo(SomeOption.Value));
            Assert.That(SomeNone.ValueOrDefault(), Is.EqualTo(default(string)));
        }

        [Test]
        public void NoneYieldAEmptyEnumeration()
        {
            Assert.That(SomeNone.ToEnumerable(), Is.Empty);
        }

        [Test]
        public void SomeYieldASingletonEnumeration()
        {
            Assert.That(SomeOption.ToEnumerable(), Is.EquivalentTo(new List<string>() {SomeOption.Value}));
        }

        [Test]
        public void NoneCanYieldNullValue()
        {
            Assert.That(SomeOption.NullableValue(), Is.Not.Null);
            Assert.That(SomeNone.NullableValue(), Is.Null);
        }

        [Test]
        public void GetMappedValueOrDefaultValue()
        {
            const int defaultValue = -1;
            Func<int, int> add = (i) => i + 1;
            Assert.That(SomeIntOption.SelectValue(add, defaultValue), Is.EqualTo(add(SomeIntOption.Value)));
            Assert.That(SomeIntNone.SelectValue(add, defaultValue), Is.EqualTo(defaultValue));
        }

        [Test]
        public void GetMappedValueOrDefaultValueFromFunction()
        {
            Func<int> defaultValue = () => -1;
            Func<int, int> add = (i) => i + 1;
            Assert.That(SomeIntOption.SelectValue(add, defaultValue), Is.EqualTo(add(SomeIntOption.Value)));
            Assert.That(SomeIntNone.SelectValue(add, defaultValue), Is.EqualTo(defaultValue()));
        }

        [Test]
        public void OptionCanBeConvertedToNullable()
        {
            Assert.That(SomeIntOption.ToNullable(), Is.Not.Null.And.EqualTo(SomeIntOption.Value));
            Assert.That(SomeIntNone.ToNullable(), Is.Null);
        }

        [Test]
        public void NullableValueTypesCanBeConvertedToAnOption()
        {
            int? i = 23;
            int? inull = null;

            Assert.That(i.ToOption().IsSome, Is.True);
            Assert.That(i.ToOption().Value, Is.EqualTo(i));

            Assert.That(inull.ToOption().IsNone, Is.True);
        }

        [Test]
        public void ReferenceTypesCanBeConvertedToAnOption()
        {
            const string r = "foo";
            const string rnull = null;

            Assert.That(r.ToOption().IsSome, Is.Not.Null);
            Assert.That(r.ToOption().Value, Is.EqualTo(r));
            Assert.That(rnull.ToOption().IsNone, Is.True);
        }

        [Test]
        public void EnumerablesWithMinOneElementAreConvertedToAnSomeContainingTheFirstElement()
        {
            var singletonList = new List<int>() {23};
            var list = new List<int>() { 42, 23, 99};

            Assert.That(singletonList.ToOptionFromEnumerable().Value, Is.EqualTo(singletonList.First()));
            Assert.That(list.ToOptionFromEnumerable().Value, Is.EqualTo(list.First()));
        }

        [Test]
        public void EmptyEnumerablesAreConvertedToANone()
        {
            Assert.That(Enumerable.Empty<int>().ToOptionFromEnumerable().IsNone, Is.True);
        }

        [Test]
        public void CastValueToOption()
        {
            const string value = "foo";

            Assert.That(value.Cast<string>().IsSome, Is.True);
            Assert.That(value.Cast<string>().Value, Is.EqualTo(value));

            Assert.That(value.Cast<int>().IsNone, Is.True);
        }

        [Test]
        public void RunSideEffectsOnASome()
        {
            var effectDone = false;

            SomeOption.Run(delegate { effectDone = true; });
            Assert.That(effectDone, Is.True);
        }

        [Test]
        public void DontRunSideEffectsOnANone()
        {
            var effectDone = false;

            SomeNone.Run(delegate { effectDone = true; });
            Assert.That(effectDone, Is.False);
        }

        [Test]
        public void RunOrThrowSideEffectsOnASome()
        {
            var effectDone = false;

            SomeOption.RunOrThrow(delegate { effectDone = true; });
            Assert.That(effectDone, Is.True);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RunSideEffectsOnANoneRaiseTheDefaultException()
        {
            SomeNone.RunOrThrow(delegate { });
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void RunSideEffectsOnANoneRaiseTheSpecifiedException()
        {
            SomeNone.RunOrThrow(delegate { }, new ArgumentException());
        }

        [Test]
        public void RunSideEffectOnTrue()
        {
            var effectDone = false;

            Option.None<bool>().RunWhenTrue(delegate { effectDone = true; });
            Assert.That(effectDone, Is.False);

            Option.Some(false).RunWhenTrue(delegate { effectDone = true; });
            Assert.That(effectDone, Is.False);

            Option.Some(true).RunWhenTrue(delegate { effectDone = true; });
            Assert.That(effectDone, Is.True);
        }

        [Test]
        public void NestedOptionsCanBeCollapsed()
        {
            var nestedNoneNone = Option.None<Option<int>>();
            Assert.That(nestedNoneNone.Collapse().IsNone, Is.True);

            var nestedSomeNone = Option.Some(SomeNone);
            Assert.That(nestedSomeNone.Collapse().IsNone, Is.True);

            var nestedSomeSome = Option.Some(SomeOption);
            Assert.That(nestedSomeSome.Collapse(), Is.EqualTo(SomeOption));
        }

        [Test]
        public void OptionsCanBeCombinedAlternativly()
        {
            Assert.That(SomeOption.Or(SomeNone), Is.EqualTo(SomeOption));
            Assert.That(SomeNone.Or(SomeOption), Is.EqualTo(SomeOption));
            Assert.That(SomeNone.Or(SomeNone), Is.EqualTo(SomeNone));
            Assert.That(SomeOption.Or(Option.Some("blub")), Is.EqualTo(SomeOption));
        }

        [Test]
        public void OptionsCanBeCombinedAlternativlyByValue()
        {
            Assert.That(SomeOption.Or("blub"), Is.EqualTo(SomeOption));
            Assert.That(SomeNone.Or(SomeOption.Value), Is.EqualTo(SomeOption));
        }

        [Test]
        public void OptionsCanBeCombinedAlternativlyByFunctionValue()
        {
            Assert.That(SomeOption.Or(() => SomeNone), Is.EqualTo(SomeOption));
            Assert.That(SomeNone.Or(() => SomeOption), Is.EqualTo(SomeOption));
            Assert.That(SomeNone.Or(() => SomeNone), Is.EqualTo(SomeNone));
            Assert.That(SomeOption.Or(() => Option.Some("blub")), Is.EqualTo(SomeOption));
        }

        [Test]
        public void SingleOptionsCanBeProjected()
        {
            Assert.That(SomeIntOption.Select(Add).Value, Is.EqualTo(Add(SomeIntOption.Value)));
            Assert.That(SomeIntNone.Select(Add), Is.EqualTo(SomeIntNone));

            Assert.That(from o in SomeIntOption select Add(o), Is.EqualTo(Option.Some(Add(SomeIntOption.Value))));
            Assert.That(from n in SomeIntNone select Add(n), Is.EqualTo(SomeIntNone));
        }

        [Test]
        public void EnumerationOfOptionsCanBeProjected()
        {
            var list = new List<Option<int>>() {Option.Some(23), Option.None<int>(), Option.Some(42)};
            var expectedList = new List<Option<int>>() {Option.Some(24), Option.None<int>(), Option.Some(43)};

            Assert.That(list.Select(Add), Is.EquivalentTo(expectedList));
            Assert.That(from o in list select Add(o), Is.EquivalentTo(expectedList));
        }

        [Test]
        public void FilterOnlySomesInAnMixedEnumerableOfOptions()
        {
            var list = new List<Option<int>>() {Option.None<int>(), Option.Some(23), Option.None<int>(), Option.Some(42), Option.None<int>()};
            var expectedList = new List<int>() {23, 42};

            Assert.That(list.SelectValid(), Is.EquivalentTo(expectedList));
        }

        [Test]
        public void FilterAndTransformOnlySomesInAnMixedEnumerableOfOptions()
        {
            var list = new List<Option<int>>() {Option.None<int>(), Option.Some(23), Option.None<int>(), Option.Some(42), Option.None<int>()};
            var expectedList = new List<int>() {24, 43};

            Assert.That(list.SelectValid(Add), Is.EquivalentTo(expectedList));
        }

        [Test]
        public void ListCanBeFilteredWithPredicateReturningOptionBool()
        {
            var list = Enumerable.Range(1, 15);
            var list2 = Enumerable.Range(5, 15 - 5);
            var expectedList2 = Enumerable.Range(5, 5).ToList();

            Func<int, Option<bool>> filterFunc = i => i < 5 ? Option.None<bool>() : Option.Some(i < 10);

            Assert.That(list.WhereOption(filterFunc).IsNone, Is.True);
            Assert.That(list2.WhereOption(filterFunc).Value, Is.EquivalentTo(expectedList2));
        }

        [Test]
        public void ASingleOptionCanBeFiltered()
        {
            Assert.That(SomeNone.Where(s => true), Is.EqualTo(SomeNone));
            Assert.That(SomeOption.Where(s => s.Length == 0), Is.EqualTo(SomeNone));
            Assert.That(SomeOption.Where(s => s.Length > 0), Is.EqualTo(SomeOption));
        }

        [Test]
        public void ASingleOptionCanBeUsedInAMonadicContext()
        {
            var stringNone = Option.None<string>();
            Func<int, Option<string>> filterFunc = i => { return i < 5 ? Option.None<string>() : Option.Some(i.ToString()); };
            Func<int, string, int> f2 = (x, y) => x + Convert.ToInt32(y);

            Assert.That(SomeIntNone.SelectMany(filterFunc), Is.EqualTo(stringNone));
            Assert.That(Option.Some(2).SelectMany(filterFunc), Is.EqualTo(stringNone));
            Assert.That(Option.Some(10).SelectMany(filterFunc), Is.EqualTo(Option.Some("10")));

            Assert.That(SomeIntNone.SelectMany(filterFunc, f2), Is.EqualTo(SomeIntNone));
            Assert.That(Option.Some(4).SelectMany(filterFunc, f2), Is.EqualTo(SomeIntNone));
            Assert.That(Option.Some(8).SelectMany(filterFunc, f2), Is.EqualTo(Option.Some(16)));
        }

        [Test]
        public void AEnumerationOfOnlySomesCanBeSequenzedIntoASomeContainingAnEnumerationOfAllValues()
        {
            var expectedList = Enumerable.Range(1, 10);
            var list = expectedList.Select(Option.Some);

            Assert.That(list.Sequence().Value, Is.EqualTo(expectedList));
        }

        [Test]
        public void AEnumerationOfOptionsContainingOneOrMoreNonesAreSequencedToNone()
        {
            var expectedList = Enumerable.Range(1, 10);
            var list = expectedList.Select(Option.Some).ToList();
            list.Insert(4, Option.None<int>());

            Assert.That(list.Sequence(), Is.EqualTo(Option.None<IEnumerable<int>>()));
        }
    }
}
