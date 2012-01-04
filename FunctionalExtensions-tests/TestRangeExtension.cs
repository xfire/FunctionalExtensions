
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using NUnit.Framework;

namespace System.FunctionalExtensions.Tests
{
    [TestFixture]
    class TestRangeExtension
    {
        [Test]
        public void CanCreateAnSimpleRangeByUsingTo()
        {
            Assert.That(1.To(10).ToList(), Is.EquivalentTo(new List<int>(){ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }));
        }

        [Test]
        public void CanCreateAnSimpleReversedRangeByUsingToWithFromBiggerThanTo()
        {
            Assert.That(10.To(1).ToList(), Is.EquivalentTo(new List<int>() { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 }));
        }

        [Test]
        public void CanCreateARangeByUsingToWithAStep()
        {
            Assert.That(1.To(10, 2).ToList(), Is.EquivalentTo(new List<int>() { 1, 3, 5, 7, 9 }));
        }

        [Test]
        public void CanCreateAnRevertedRangeByUsingToWithANegativeStep()
        {
            Assert.That(10.To(1, -2).ToList(), Is.EquivalentTo(new List<int>() { 10, 8, 6, 4, 2 }));
        }

        [Test]
        public void RangeWithFromEqualsToCreateASingletonList()
        {
            Assert.That(5.To(5).ToList(), Has.Count.EqualTo(1).And.Contains(5));
            Assert.That(5.To(5, 2).ToList(), Has.Count.EqualTo(1).And.Contains(5));
            Assert.That(5.To(5, -2).ToList(), Has.Count.EqualTo(1).And.Contains(5));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void RangeWithZeroStepThrowsAnException()
        {
            1.To(10, 0).ToList();
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void RangeWithFromBiggerThanToAndPositiveStepThrowsAnException()
        {
            10.To(1, 2).ToList();
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void RangeWithFromSmallerThanToAndNegativeStepThrowsAnException()
        {
            1.To(10, -1).ToList();
        }
    }
}
