using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;

namespace System.FunctionalExtensions.Tests
{
    [TestFixture]
    class TestFuncExtensions
    {
        private static readonly Func<int> ConstantFunction = 23.Const();
        private static readonly Func<int, int> Add2 = i => i + 2; 
        private static readonly Func<int, int> Mul10 = i => i * 10;
        private static readonly Func<int, int, int> Mul = (x, y) => x*y; 
        private static readonly Func<int, int, int> AddMul = (x, y) => (x + y)*x;
        private static readonly Func<int, int, int, int> AddMulSub = (x, y, z) => ((x + y) * x) - z;
        private static readonly Func<int, string> Int2Str = i => i.ToString();
        
        [Test]
        public void AValueCanBeConvertedToAConstantFunctionWhichAlwaysReturnTheValue()
        {
            var constant = 23.Const();
            Assert.That(constant(), Is.EqualTo(23));
        }

        [Test]
        public void ArgumentsOfAn2AryFunctionCanBeFlipped()
        {
            Assert.That(AddMul.Flip()(5, 10), Is.Not.EqualTo(AddMul(5, 10)));
            Assert.That(AddMul.Flip()(5, 10), Is.EqualTo((10 + 5) * 10));
        }

        [Test]
        public void A1AryFunctionCanBeCurried()
        {
            Func<int, Func<int>> curried = Add2.Curry();
            Assert.That(curried(2)(), Is.EqualTo(Add2(2)));

            Func<int> curriedAndApplied = Add2.Curry(2);
            Assert.That(curriedAndApplied(), Is.EqualTo(Add2(2)));
        }

        [Test]
        public void A2AryFunctionCanBeCurried()
        {
            Func<int, Func<int, int>> curried = AddMul.Curry();
            Assert.That(curried(2)(4), Is.EqualTo(AddMul(2, 4)));

            Func<int, int> curriedAndApplied = AddMul.Curry(2);
            Assert.That(curriedAndApplied(4), Is.EqualTo(AddMul(2, 4)));
        }

        [Test]
        public void A3AryFunctionCanBeCurried()
        {
            Func<int, Func<int, int, int>> curried = AddMulSub.Curry();
            Assert.That(curried(2)(4, 6), Is.EqualTo(AddMulSub(2, 4, 6)));

            Func<int, int, int> curriedAndApplied = AddMulSub.Curry(2);
            Assert.That(curriedAndApplied(4, 6), Is.EqualTo(AddMulSub(2, 4, 6)));
        }

        [Test]
        public void TwoSimpleFunctionsCanBeComposed()
        {
            var f = Add2.Compose(Mul10);
            var f2 = Mul10.Compose(Add2);
            Assert.That(f(5), Is.EqualTo(52));
            Assert.That(f2(5), Is.EqualTo(70));
        }

        [Test]
        public void ComposeTwoFunctionOnEnumerationOfIntegers()
        {
            Func<IEnumerable<int>, IEnumerable<int>> sort = xs => xs.OrderBy(x=>x);
            Func<IEnumerable<int>, IEnumerable<int>> reverse = xs => xs.Reverse();

            var testData = new List<int>() { 4, 1, 5, 3, 2 };
            var descSort = reverse.Compose(sort);

            Assert.That(descSort(testData), Is.EquivalentTo(5.To(1)));
            Assert.That(descSort(testData), Is.EquivalentTo(reverse(sort(testData))));
        }

        [Test]
        public void ComposeFunctionsWithVariousComplexerTypes()
        {
            Func<int, Option<string>> f = i => i.ToString().ToOption();
            Func<Option<string>, Either<int, int>> g = os => os.Value.Length.Right<int, int>();

            var c = g.Compose(f);

            Assert.That(c(12345).Right, Is.EqualTo(5));
        }

        [Test]
        public void ExceptionsWhileExecuting0AryFunctionsCanBeHandledWithAnEither()
        {
            Func<int> throws = () => { throw new ArgumentException(); };

            Assert.That(ConstantFunction.TryCatch(), Is.EqualTo(ConstantFunction().Right<Exception, int>()));
            Assert.That(throws.TryCatch().Left, Is.InstanceOf<ArgumentException>());
        }

        [Test]
        public void ExceptionsWhileExecuting1AryFunctionsCanBeHandledWithAnEither()
        {
            Func<int, int> throws = i => { throw new ArgumentException(); };

            Assert.That(Add2.TryCatch(2), Is.EqualTo(Add2(2).Right<Exception, int>()));
            Assert.That(throws.TryCatch(2).Left, Is.InstanceOf<ArgumentException>());
        }

        [Test]
        public void ExceptionsWhileExecuting2AryFunctionsCanBeHandledWithAnEither()
        {
            Func<int, int, int> throws = (x, y) => { throw new ArgumentException(); };

            Assert.That(Mul.TryCatch(2, 3), Is.EqualTo(Mul(2, 3).Right<Exception, int>()));
            Assert.That(throws.TryCatch(2, 3).Left, Is.InstanceOf<ArgumentException>());
        }

        [Test]
        public void ExceptionsWhileExecuting3AryFunctionsCanBeHandledWithAnEither()
        {
            Func<int, int, int, int> throws = (x, y, z) => { throw new ArgumentException(); };

            Assert.That(AddMulSub.TryCatch(2, 3, 4), Is.EqualTo(AddMulSub(2, 3, 4).Right<Exception, int>()));
            Assert.That(throws.TryCatch(2, 3, 4).Left, Is.InstanceOf<ArgumentException>());
        }

        [Test]
        public void MapAFunctionOverAConstantFunction()
        {
            Assert.That(ConstantFunction.Select(Int2Str)(), Is.EqualTo("23"));
        }

        [Test]
        public void MapAFunctionOverA1AryFunction()
        {
            var add2AndThenToString = Add2.Select(Int2Str);
            Assert.That(add2AndThenToString(5), Is.EqualTo("7"));
        }

        [Test]
        public void SelectManyFor0AryFunctionsWork()
        {
            var expected = ConstantFunction() * ConstantFunction();

            Assert.That(ConstantFunction.SelectMany(x => (x*x).Const())(), Is.EqualTo(expected));

            var v = from a in ConstantFunction
                    from b in (a*a).Const()
                    select b;
            Assert.That(v(), Is.EqualTo(expected));
        }
        
        [Test]
        public void SelectManyFor1AryFunctionsWork()
        {
            const int expected = (2 + 5)*5;

            Assert.That(Add2.SelectMany(x => Mul.Curry(x))(5), Is.EqualTo(expected));

            var v = from a in Add2
                    from b in Mul.Curry(a)
                    select b;
            Assert.That(v(5), Is.EqualTo(expected));
        }

        [Test]
        public void Two1AryFunctionsCanBeUsedToCreateATupeFromASingleValue()
        {
            Func<int, string> ab = i => i.ToString();
            Func<int, bool> ac = i => i > 5;

            Assert.That(ab.And(ac)(5).Item1, Is.EqualTo("5"));
            Assert.That(ab.And(ac)(3).Item2, Is.False);
        }

        [Test]
        public void AFunctionCanBeAppliedToBothValuesOfATuple()
        {
            var t = Tuple.Create("foo", true);
            Func<string, string> ab = s => s.ToUpper();
            Func<bool, int> cd = b => b ? 23 : 42;

            var result = ab.Product(cd)(t);

            Assert.That(result.Item1, Is.EqualTo("FOO"));
            Assert.That(result.Item2, Is.EqualTo(23));
        }

        [Test]
        public void AFunctionCanBeAppliedToTheFirstValueOfATuple()
        {
            var t = Tuple.Create("foo", true);
            Func<string, string> ab = s => s.ToUpper();

            var result = ab.First<string, string, bool>()(t);

            Assert.That(result.Item1, Is.EqualTo("FOO"));
            Assert.That(result.Item2, Is.True);
        }

        [Test]
        public void AFunctionCanBeAppliedToTheSecondValueOfATuple()
        {
            var t = Tuple.Create("foo", false);
            Func<bool, int> cd = b => b ? 23 : 42;

            var result = cd.Second<bool, int, string>()(t);

            Assert.That(result.Item1, Is.EqualTo("foo"));
            Assert.That(result.Item2, Is.EqualTo(42));
        }

    }
}
