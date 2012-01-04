using System.FunctionalExtensions;
using NUnit.Framework;

namespace System.FunctionalExtensions.Tests
{
    [TestFixture]
    public class TestOption
    {
        private static readonly Option<int> SomeOption = Option.Some(23);
        private static readonly Option<int> SomeOptionWithTheSameValue = Option.Some(23);
        private static readonly Option<int> SomeOtherOption = Option.Some(42);
        private static readonly Option<int> SomeNone = Option.None<int>();
        private static readonly Option<int> SomeNoneOfTheSameType = Option.None<int>();
        private static readonly Option<decimal> OtherNone = Option.None<decimal>();

        [Test]
        [ExpectedException(typeof(OptionValueAccessException))]
        public void NoneHasNoValue()
        {
            var a = SomeNone.Value;
        }

        [Test]
        public void NoneCanBeIdentified()
        {
            Assert.That(SomeNone.IsNone, Is.True);
            Assert.That(SomeNone.IsSome, Is.False);
        }

        [Test]
        public void SomeCanBeIdentified()
        {
            Assert.That(SomeOption.IsNone, Is.False);
            Assert.That(SomeOption.IsSome, Is.True);
        }

        [Test]
        public void SomeHasTheCorrectValue()
        {
            const string someValue = "value";
            Assert.That(Option.Some(someValue).Value, Is.EqualTo(someValue));
        }

        [Test]
        public void TwoNonesAreEqualable()
        {
            Assert.That(SomeNone, Is.EqualTo(SomeNone));
            Assert.That(SomeNone.Equals(SomeNone), Is.True);
            Assert.That(SomeNone, Is.EqualTo(SomeNoneOfTheSameType));
            Assert.That(SomeNone, Is.Not.EqualTo(OtherNone));

            Assert.That(SomeNone.Equals((object)SomeNone), Is.True);
            Assert.That(SomeNone.Equals((object)null), Is.False);
            Assert.That(SomeNone == SomeNoneOfTheSameType, Is.True);
        }

        [Test]
        public void TwoSomesAreEqualable()
        {
            Assert.That(SomeOption, Is.EqualTo(SomeOption));
            Assert.That(SomeOption.Equals(SomeOption), Is.True);
            Assert.That(SomeOption, Is.EqualTo(SomeOptionWithTheSameValue));
            Assert.That(SomeOption, Is.Not.EqualTo(SomeOtherOption));

            Assert.That(SomeOption.Equals((object)SomeOption), Is.True);
            Assert.That(SomeOption.Equals((object)null), Is.False);
            Assert.That(SomeOption == SomeOptionWithTheSameValue, Is.True);
            Assert.That(SomeOption != SomeOtherOption, Is.True);
        }

        [Test]
        [ExpectedException(typeof(SomeInitializedWithNullException))]
        public void SomeCanNotBeInitializedWithNull()
        {
            const string nullValue = null;
            Option.Some<string>(nullValue);
        }

        [Test]
        public void SomeAndNoneAreNotEqual()
        {
            Assert.That(SomeOption, Is.Not.EqualTo(SomeNone));
            Assert.That(SomeNone, Is.Not.EqualTo(SomeOption));
        }

        [Test]
        public void AllNonesHaveTheSameHashcode()
        {
            Assert.That(SomeNone.GetHashCode(), Is.EqualTo(OtherNone.GetHashCode()));
        }

        [Test]
        public void ToStringOfNoneContainsTheWordNone()
        {
            Assert.That(SomeNone.ToString().ToLower(), Is.StringContaining("none"));
        }

        [Test]
        public void ToStringOfSomeContainsTheValue()
        {
            Assert.That(SomeOption.ToString(), Is.StringContaining(SomeOption.Value.ToString()));
        }
    }
}
