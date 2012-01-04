using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace System.FunctionalExtensions.Tests
{
    [TestFixture]
    class TestEitherExtensions
    {
        private static readonly Either<string, int> SomeLeft = "some error".Left<string, int>();
        private static readonly Either<string, int> SomeRight = 23.Right<string, int>();
        private static readonly Either<int, string> OtherLeft = (-1).Left<int, string>();
        private static readonly Either<int, string> OtherRight = "some value".Right<int, string>();

        private static readonly Func<int, int> Add = i => i + 1;

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestLeftValueWithExceptionFunction()
        {
            Func<Exception> f = () => new ArgumentException();
            Assert.That(SomeLeft.Left(f), Is.EqualTo(SomeLeft.Left));
            SomeRight.Left(f);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestRightValueWithExceptionFunction()
        {
            Func<Exception> f = () => new ArgumentException();
            Assert.That(SomeRight.Right(f), Is.EqualTo(SomeRight.Right));
            SomeLeft.Right(f);
        }

        [Test]
        public void TestLeftValueWithDefaultValueFunction()
        {
            const string otherValue = "optionValue";
            Func<string> f = () => otherValue;

            Assert.That(SomeLeft.Left(f), Is.EqualTo(SomeLeft.Left));
            Assert.That(SomeRight.Left(f), Is.EqualTo(otherValue));
        }

        [Test]
        public void TestRightValueWithDefaultValueFunction()
        {
            const int otherValue = 42;
            Func<int> f = () => otherValue;

            Assert.That(SomeRight.Right(f), Is.EqualTo(SomeRight.Right));
            Assert.That(SomeLeft.Right(f), Is.EqualTo(otherValue));
        }

        [Test]
        public void TestLeftValueWithDefaultValue()
        {
            const string defaultValue = "optionValue";

            Assert.That(SomeLeft.Left(defaultValue), Is.EqualTo(SomeLeft.Left));
            Assert.That(SomeRight.Left(defaultValue), Is.EqualTo(defaultValue));
        }

        [Test]
        public void TestRightValueWithDefaultValue()
        {
            const int defaultValue = 42;

            Assert.That(SomeRight.Right(defaultValue), Is.EqualTo(SomeRight.Right));
            Assert.That(SomeLeft.Right(defaultValue), Is.EqualTo(defaultValue));
        }

        [Test]
        public void TestLeftValueWithTypeDefaultValue()
        {
            Assert.That(SomeLeft.LeftOrDefault(), Is.EqualTo(SomeLeft.Left));
            Assert.That(SomeRight.LeftOrDefault(), Is.EqualTo(default(string)));
        }

        [Test]
        public void TestRightValueWithTypeDefaultValue()
        {
            Assert.That(SomeRight.RightOrDefault(), Is.EqualTo(SomeRight.Right));
            Assert.That(SomeLeft.RightOrDefault(), Is.EqualTo(default(int)));
        }

        [Test]
        public void LeftToEnumerationOnLeftReturnsASingletonListWithTheLeftValue()
        {
            Assert.That(SomeLeft.LeftToEnumerable(), Is.EquivalentTo(new List<string>() {SomeLeft.Left}));
        }

        [Test]
        public void LeftToEnumerationOnRightReturnsAnEmptyList()
        {
            Assert.That(SomeRight.LeftToEnumerable(), Is.Empty);
        }

        [Test]
        public void RightToEnumerationOnRightReturnsASingletonListWithTheLeftValue()
        {
            Assert.That(SomeRight.RightToEnumerable(), Is.EquivalentTo(new List<int>() {SomeRight.Right}));
        }

        [Test]
        public void RightToEnumerationOnLeftReturnsAnEmptyList()
        {
            Assert.That(SomeLeft.RightToEnumerable(), Is.Empty);
        }

        [Test]
        public void NullableLeftOnLeftYieldLeftValue()
        {
            Assert.That(SomeLeft.NullableLeft(), Is.EqualTo(SomeLeft.Left));
        }

        [Test]
        public void NullableLeftOnRightYieldNull()
        {
            Assert.That(SomeRight.NullableLeft(), Is.Null);
        }

        [Test]
        public void NullableRightOnRightYieldRightValue()
        {
            Assert.That(OtherRight.NullableRight(), Is.EqualTo(OtherRight.Right));
        }

        [Test]
        public void NullableRightOnLeftYieldNull()
        {
            Assert.That(OtherLeft.NullableRight(), Is.Null);
        }

        [Test]
        public void LeftCanBeConvertedToNullable()
        {
            Assert.That(OtherLeft.LeftToNullable(), Is.Not.Null.And.EqualTo(OtherLeft.Left));
            Assert.That(OtherRight.LeftToNullable(), Is.Null);
        }

        [Test]
        public void RightCanBeConvertedToNullable()
        {
            Assert.That(SomeRight.RightToNullable(), Is.Not.Null.And.EqualTo(SomeRight.Right));
            Assert.That(SomeLeft.RightToNullable(), Is.Null);
        }

        [Test]
        public void GetMappedLeftValueOrDefaultValueFromFunction()
        {
            Func<string> defaultValue = () => "default";
            Func<string, string> addFoo = s => s + "foo";
            Assert.That(SomeLeft.SelectLeft(addFoo, defaultValue), Is.EqualTo(addFoo(SomeLeft.Left)));
            Assert.That(SomeRight.SelectLeft(addFoo, defaultValue), Is.EqualTo(defaultValue()));
        }

        [Test]
        public void GetMappedRightValueOrDefaultValueFromFunction()
        {
            Func<int> defaultValue = () => -1;
            Func<int, int> add = i => i + 23;
            Assert.That(SomeRight.SelectRight(add, defaultValue), Is.EqualTo(add(SomeRight.Right)));
            Assert.That(SomeLeft.SelectRight(add, defaultValue), Is.EqualTo(defaultValue()));
        }

        [Test]
        public void GetMappedLeftValueOrDefaultValue()
        {
            const string defaultValue = "default";
            Func<string, string> addFoo = s => s + "foo";
            Assert.That(SomeLeft.SelectLeft(addFoo, defaultValue), Is.EqualTo(addFoo(SomeLeft.Left)));
            Assert.That(SomeRight.SelectLeft(addFoo, defaultValue), Is.EqualTo(defaultValue));
        }

        [Test]
        public void GetMappedRightValueOrDefaultValue()
        {
            const int defaultValue = -1;
            Func<int, int> add = i => i + 23;
            Assert.That(SomeRight.SelectRight(add, defaultValue), Is.EqualTo(add(SomeRight.Right)));
            Assert.That(SomeLeft.SelectRight(add, defaultValue), Is.EqualTo(defaultValue));
        }

        [Test]
        public void RunSideEffectsOnARight()
        {
            var effectDone = false;

            SomeRight.Run(delegate { effectDone = true; });
            Assert.That(effectDone, Is.True);
        }

        [Test]
        public void DontRunSideEffectsOnALeft()
        {
            var effectDone = false;

            SomeLeft.Run(delegate { effectDone = true; });
            Assert.That(effectDone, Is.False);
        }

        [Test]
        public void RunSideEffectsOnARightDontThrowAnException()
        {
            var effectDone = false;

            SomeRight.RunOrThrow(delegate { effectDone = true; });
            Assert.That(effectDone, Is.True);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RunSideEffectsOnALeftRaiseTheDefaultException()
        {
            SomeLeft.RunOrThrow(delegate { });
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void RunSideEffectsOnALeftRaiseTheSpecifiedException()
        {
            SomeLeft.RunOrThrow(delegate { }, new ArgumentException());
        }

        [Test]
        public void RunSideEffectOnTrue()
        {
            var effectDone = false;

            (42.Left<int, bool>()).RunWhenTrue(delegate { effectDone = true; });
            Assert.That(effectDone, Is.False);

            (false.Right<string, bool>()).RunWhenTrue(delegate { effectDone = true; });
            Assert.That(effectDone, Is.False);

            (true.Right<string, bool>()).RunWhenTrue(delegate { effectDone = true; });
            Assert.That(effectDone, Is.True);
        }

        [Test]
        public void SingleEithersCanBeProjected()
        {
            Assert.That(SomeRight.Select(Add).Right, Is.EqualTo(Add(SomeRight.Right)));
            Assert.That(SomeLeft.Select(Add), Is.EqualTo(SomeLeft));

            Assert.That(from r in SomeRight select Add(r), Is.EqualTo(Add(SomeRight.Right).Right<string, int>()));
            Assert.That(from l in SomeLeft select Add(l), Is.EqualTo(SomeLeft));
        }

        [Test]
        public void EnumerationOfEithersCanBeProjected()
        {
            var list = new List<Either<string, int>>() { 23.Right<string, int>(), "error msg".Left<string, int>(), 42.Right<string, int>() };
            var expectedList = new List<Either<string, int>>() { 24.Right<string, int>(), "error msg".Left<string, int>(), 43.Right<string, int>() };

            Assert.That(list.Select(Add), Is.EquivalentTo(expectedList));
            Assert.That(from o in list select Add(o), Is.EquivalentTo(expectedList));
        }

        [Test]
        public void FilterOnlyRightsInAnMixedEnumerableOfOptions()
        {
            var list = new List<Either<string, int>>()
                           {
                               "first error".Left<string, int>(),
                               23.Right<string, int>(),
                               "error msg".Left<string, int>(),
                               42.Right<string, int>(),
                               "last error".Left<string, int>()
                           };
            var expectedList = new List<int>() { 23, 42 };

            Assert.That(list.SelectValid(), Is.EquivalentTo(expectedList));
        }

        [Test]
        public void FilterAndTransformOnlyRightsInAnMixedEnumerableOfOptions()
        {
            var list = new List<Either<string, int>>()
                           {
                               "first error".Left<string, int>(),
                               23.Right<string, int>(),
                               "error msg".Left<string, int>(),
                               42.Right<string, int>(),
                               "last error".Left<string, int>()
                           };
            var expectedList = new List<int>() { 24, 43 };

            Assert.That(list.SelectValid(Add), Is.EquivalentTo(expectedList));
        }

        [Test]
        public void ListCanBeFilteredWithPredicateReturningOptionBool()
        {
            var list = Enumerable.Range(1, 15);
            var list2 = Enumerable.Range(5, 15 - 5);
            var expectedList2 = Enumerable.Range(5, 5).ToList();

            Func<int, Either<string, bool>> filterFunc = i => i < 5 ? "smaller than 5".Left<string, bool>() : (i < 10).Right<string, bool>();

            Assert.That(list.WhereEither(filterFunc).Left, Is.EqualTo("smaller than 5"));
            Assert.That(list2.WhereEither(filterFunc).Right, Is.EquivalentTo(expectedList2));
        }

        [Test]
        public void ASingleEitherCanBeFiltered()
        {
            Assert.That(SomeLeft.Where(s => true), Is.EqualTo(SomeLeft));
            Assert.That(SomeRight.Where(s => s < 23), Is.EqualTo(default(string).Left<string, int>()));
            Assert.That(SomeRight.Where(s => s >= 23), Is.EqualTo(SomeRight));
        }

        [Test]
        public void ASingleEitherCanBeUsedInAMonadicContext()
        {
            var error = "smaller than 5".Left<string, string>();
            Func<int, Either<string, string>> filterFunc = i => i < 5 ? error : i.ToString().Right<string, string>();
            Func<int, string, string> f2 = (x, y) => (x + Convert.ToInt32(y)).ToString();

            Assert.That(SomeLeft.SelectMany(filterFunc).Left, Is.EqualTo(SomeLeft.Left));
            Assert.That(2.Right<string, int>().SelectMany(filterFunc), Is.EqualTo(error));
            Assert.That(10.Right<string, int>().SelectMany(filterFunc), Is.EqualTo("10".Right<string, string>()));

            Assert.That(SomeLeft.SelectMany(filterFunc, f2).Left, Is.EqualTo(SomeLeft.Left));
            Assert.That(4.Right<string, int>().SelectMany(filterFunc, f2), Is.EqualTo(error));
            Assert.That(8.Right<string, int>().SelectMany(filterFunc, f2), Is.EqualTo("16".Right<string, string>()));
        }

        [Test]
        public void AEnumerationOfOnlyRightsCanBeSequenzedIntoARightContainingAnEnumerationOfAllValues()
        {
            var expectedList = Enumerable.Range(1, 10);
            var list = expectedList.Select(Either.Right<string, int>);

            Assert.That(list.Sequence().Right, Is.EqualTo(expectedList));
        }

        [Test]
        public void AEnumerationOfEithersContainingOneOrMoreLeftsAreSequencedToLeft()
        {
            var expectedList = Enumerable.Range(1, 10);
            var list = expectedList.Select(Either.Right<string, int>).ToList();
            list.Insert(4, "error".Left<string, int>());

            Assert.That(list.Sequence(), Is.EqualTo("error".Left<string, IEnumerable<int>>()));
        }
    }
}
