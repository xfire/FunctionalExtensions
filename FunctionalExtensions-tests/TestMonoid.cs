using NUnit.Framework;

namespace System.FunctionalExtensions.Tests
{
    [TestFixture]
    class TestMonoid
    {
        [Test]
        public void CompareTwoOrderingFunctionsWithOrderingMonoid()
        {
            Func<string, string, int> direct = (x, y) => x.CompareTo(y);
            Func<string, string, int> byLength = (x, y) => x.Length.CompareTo(y.Length);

            Func<string, string, int> f = (x, y) => Monoids.Ordering.Mappend(byLength(x, y), direct(x, y));

            Assert.That(f("zen", "ant"), Is.GreaterThan(0));
            Assert.That(f("zen", "ants"), Is.LessThan(0));
        }
    }
}
