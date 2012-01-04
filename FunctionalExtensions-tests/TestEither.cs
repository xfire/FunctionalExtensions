using NUnit.Framework;

namespace System.FunctionalExtensions.Tests
{
    [TestFixture]
    class TestEither
    {
        private static readonly Either<string, int> SomeLeft = "some error".Left<string, int>();
        private static readonly Either<string, int> SomeLeftWithTheSameValue = "some error".Left<string, int>();
        private static readonly Either<string, int> SomeLeftWithAnotherValue = "foobar".Left<string, int>();
        private static readonly Either<string, int> SomeRight = 23.Right<string, int>();
        private static readonly Either<string, int> SomeRightWithTheSameValue = 23.Right<string, int>();
        private static readonly Either<string, int> SomeRightWithAnotherValue = 42.Right<string, int>();

        private static readonly Either<decimal, int> OtherLeft = (23.42m).Left<decimal, int>();
        private static readonly Either<string, string> OtherRight = "some value".Right<string, string>();

        [Test]
        [ExpectedException(typeof(EitherValueAccessException))]
        public void AccessingTheRightValueOfALeftEitherThrowsAnException()
        {
            var x = SomeLeft.Right;
        }

        [Test]
        [ExpectedException(typeof(EitherValueAccessException))]
        public void AccessingTheLeftValueOfARightEitherThrowsAnException()
        {
            var x = SomeRight.Left;
        }

        [Test]
        public void ALeftCanBeIdentified()
        {
            Assert.That(SomeLeft.IsLeft, Is.True);
            Assert.That(SomeLeft.IsRight, Is.False);
        }

        [Test]
        public void ARightCanBeIdentified()
        {
            Assert.That(SomeRight.IsRight, Is.True);
            Assert.That(SomeRight.IsLeft, Is.False);
        }

        [Test]
        public void LeftHasTheCorrectValue()
        {
            Assert.That(SomeLeft.Left, Is.EqualTo("some error"));
        }

        [Test]
        public void RightHasTheCorrectValue()
        {
            Assert.That(SomeRight.Right, Is.EqualTo(23));
        }

        [Test]
        public void TwoLeftsAreTheEqualable()
        {
            Assert.That(SomeLeft, Is.EqualTo(SomeLeft));
            Assert.That(SomeLeft.Equals(SomeLeft), Is.True);
            Assert.That(SomeLeft, Is.EqualTo(SomeLeftWithTheSameValue));
            Assert.That(SomeLeft, Is.Not.EqualTo(OtherLeft));

            Assert.That(SomeLeft.Equals((object)SomeLeft), Is.True);
            Assert.That(SomeLeft.Equals((object)null), Is.False);
            Assert.That(SomeLeft == SomeLeftWithTheSameValue, Is.True);
            Assert.That(SomeLeft != SomeLeftWithAnotherValue, Is.True);
        }

        [Test]
        public void TwoRightsAreTheEqualable()
        {
            Assert.That(SomeRight, Is.EqualTo(SomeRight));
            Assert.That(SomeRight.Equals(SomeRight), Is.True);
            Assert.That(SomeRight, Is.EqualTo(SomeRightWithTheSameValue));
            Assert.That(SomeRight, Is.Not.EqualTo(OtherRight));

            Assert.That(SomeRight.Equals((object)SomeRight), Is.True);
            Assert.That(SomeRight.Equals((object)null), Is.False);
            Assert.That(SomeRight == SomeRightWithTheSameValue, Is.True);
            Assert.That(SomeRight != SomeRightWithAnotherValue, Is.True);
        }

        [Test]
        public void LeftAndRightAreNotEqual()
        {
            Assert.That(SomeLeft, Is.Not.EqualTo(SomeRight));
            Assert.That(SomeRight, Is.Not.EqualTo(SomeLeft));
        }

        [Test]
        public void LeftsWithTheSameValueHaveTheSameHashcode()
        {
            Assert.That(SomeLeft.GetHashCode(), Is.EqualTo(SomeLeftWithTheSameValue.GetHashCode()));
        }

        [Test]
        public void RightsWithTheSameValueHaveTheSameHashcode()
        {
            Assert.That(SomeRight.GetHashCode(), Is.EqualTo(SomeRightWithTheSameValue.GetHashCode()));
        }

        [Test]
        public void ToStringOfLeftContainsTheLeftValue()
        {
            Assert.That(SomeLeft.ToString(), Is.StringContaining(SomeLeft.Left));
        }

        [Test]
        public void ToStringOfRightContainsTheRightValue()
        {
            Assert.That(SomeRight.ToString(), Is.StringContaining(SomeRight.Right.ToString()));
        }
    }
}
