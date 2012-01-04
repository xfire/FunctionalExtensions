using System.Collections.Generic;
using NUnit.Framework;

namespace System.FunctionalExtensions.Examples
{
    static class LookupAndParseWithErrorReasonExtensions
    {
        /// <summary>
        /// Extension method which will lookup the <paramref name="key"/> in the <paramref name="dictionary"/>
        /// and return the corresponding value as <c>Right(value)</c>.
        /// <para>If the <paramref name="key"/> is not in the <paramref name="dictionary"/>, <c>Left(error message)</c>
        /// will be returned.</para>
        /// </summary>
        public static Either<string, TValue> LookupWithError<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            return dictionary.ContainsKey(key) ? dictionary[key].Right<string, TValue>() : "can not find key".Left<string, TValue>();
        }
    }

    [TestFixture]
    class LookupAndParseWithErrorReason
    {
        /// <summary>Some test data</summary>
        private static readonly IDictionary<string, string> Dict = new Dictionary<string, string>()
                                                        {
                                                            {"someValue", "23"},
                                                            {"badValue", "abcd"}
                                                        };

        /// <summary>
        /// Try to parse a <c>string</c> as <c>int</c>.
        /// Will return the parsed integer as <c>Right(integer value)</c> or if the string can not be parsed <c>Left(error message)</c>.
        /// </summary>
        static Either<string, int> Parse(string str)
        {
            int i;
            if (int.TryParse(str, out i))
            {
                return i.Right<string, int>();
            }
            else
            {
                return "can not parse integer".Left<string, int>();
            }
        }

        [Test]
        public void LookupAndParseSucceed()
        {
            var result = from s in Dict.LookupWithError("someValue")  // OK: get string "23"
                         from i in Parse(s)                           // OK: "23" -> 23
                         select i * 1000;                             // OK: return 23 * 1000 as option

            Assert.That(result.IsRight, Is.True);
            Assert.That(result.Right, Is.EqualTo(23000));
        }

        [Test]
        public void LookupFailedAndParseSucceed()
        {
            var result = from s in Dict.LookupWithError("notExistingValue")  // FAIL: key does not exists in dictionary
                         from i in Parse(s)                                  // -
                         select i * 1000;                                    // -

            Assert.That(result.IsLeft, Is.True);
            Assert.That(result.Left, Is.EqualTo("can not find key"));
        }

        [Test]
        public void LookupSucceedAndParseFailed()
        {
            var result = from s in Dict.LookupWithError("badValue")  // OK
                         from i in Parse(s)                          // FAIL: can not parse "abcd" as integer
                         select i * 1000;                            // -

            Assert.That(result.IsLeft, Is.True);
            Assert.That(result.Left, Is.EqualTo("can not parse integer"));
        }
    }
}
