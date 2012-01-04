using System.Linq;
using NUnit.Framework;

namespace System.FunctionalExtensions.Examples
{
    [TestFixture]
    public class BasicComputingsInAnOptionalContext
    {
        [Test]
        public void SimpleAdditionOfASomeAndANone()
        {
            var goodValue = "Hello".ToOption();    // a Some<string> containing a value
            var badValue = Option.None<string>();  // a None<string> representing a bad computation

            // add both optional values in the optional computing context
            // which results in an optional value
            var concat = from good in goodValue
                         from bad in badValue
                         select good + bad;

            Assert.That(concat.IsNone, Is.True);
        }

        [Test]
        public void ExecuteSideEffects()
        {
            const string nullString = null;

            // nullString.ToOption() results in a None which will not be executed by run, because
            // no containing value is available.
            nullString.ToOption().Run(str =>
                                          {
                                              // str will never be null
                                              Assert.That(str, Is.Not.Null);
                                          });


            // "foo".ToOption() results in a Some("foo") on which a side effect can be executed,
            // because the Some contains a value to work on.
            var sideEffect = "";
            "foo".ToOption().Run(str => sideEffect = str);
            Assert.That(sideEffect, Is.Not.Null.And.EqualTo("foo"));
        }

        [Test]
        public void Guarding()
        {
            const string someName = "steve";

            // the guard (where clause) is true, thus the result is a Some("steve")
            var isItSteve = from name in someName.ToOption()
                            where name.Equals("steve")
                            select name;

            // the guard (where clause) is false, thus the result is a None
            var isItBill = from name in someName.ToOption()
                           where name.Equals("bill")
                           select name;

            Assert.That(isItSteve.IsSome, Is.True);
            Assert.That(isItBill.IsNone, Is.True);
        }

        [Test]
        public void ChainingOptionalValuesAndComputations()
        {
            var name1 = Option.None<string>();
            var name2 = Option.Some("some name");

            Assert.That(name1.Or(name2), Is.EqualTo(name2));
            Assert.That(name1.Or(() => name2), Is.EqualTo(name2));

            Assert.That(name1.Or("no name found").Value, Is.EqualTo("no name found"));
            Assert.That(name1.Or(() => "no name found".ToOption()).Value, Is.EqualTo("no name found"));
        }

        [Test]
        public void ExtractValuesFromOptions()
        {
            var possibleName = Option.None<string>();
            var realName = "the real name".ToOption();

            // value can return a default value in the None case
            Assert.That(possibleName.Value("default value"), Is.EqualTo("default value"));

            // the default value can be a function, good in cases when the default value will be very
            // expensive to calculate or must be done lazy
            Assert.That(possibleName.Value(() => "default value"), Is.EqualTo("default value"));

            // it's possible to return the default value of the type in a None case
            Assert.That(possibleName.ValueOrDefault(), Is.EqualTo(default(string)));

            // the value of a Some can also extracted without problems
            Assert.That(realName.Value, Is.EqualTo("the real name"));
            Assert.That(realName.Value("default value"), Is.EqualTo("the real name"));
            Assert.That(realName.Value(() => "default value"), Is.EqualTo("the real name"));
            Assert.That(realName.ValueOrDefault(), Is.EqualTo("the real name"));
        }

        [Test]
        public void AnOptionCanBeConvertedToAnEnumeration()
        {
            var none = Option.None<string>();
            var some = Option.Some("foo");

            Assert.That(none.ToEnumerable(), Is.Empty);
            Assert.That(some.ToEnumerable().ToList(), Has.Count.EqualTo(1).And.Contains("foo"));
        }
    }
}
