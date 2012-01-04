using System.Collections.Generic;
using NUnit.Framework;

namespace System.FunctionalExtensions.Examples
{
    static class LookupAndParseWithoutErrorReasonExtensions
    {
        /// <summary>
        /// Extension method which will lookup the <paramref name="key"/> in the <paramref name="dictionary"/>
        /// and return the corresponding value as <c>Some(value)</c>.
        /// <para>If the <paramref name="key"/> is not in the <paramref name="dictionary"/>, <c>None</c> will be returned.</para>
        /// </summary>
        public static Option<TValue> Lookup<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            return dictionary.ContainsKey(key) ? Option.Some(dictionary[key]) : Option.None<TValue>();
        }
    }

    [TestFixture]
    class LookupAndParseWithoutErrorReason
    {
        /// <summary>Some test data</summary>
        private static readonly IDictionary<string, string> Dict = new Dictionary<string, string>()
                                                        {
                                                            {"someValue", "23"},
                                                            {"badValue", "abcd"}
                                                        };

        /// <summary>
        /// Try to parse a <c>string</c> as <c>int</c>.
        /// Will return the parsed integer as <c>Some(int)</c> or if the string can not be parsed <c>None</c>.
        /// </summary>
        static Option<int> Parse(string str)
        {
            int i;
            if (int.TryParse(str, out i))
            {
                return i.ToOption();
            }
            else
            {
                return Option.None<int>();
            }
        }

        [Test]
        public void LookupAndParseSucceed()
        {
            var result = from s in Dict.Lookup("someValue")  // OK: get string "23"
                         from i in Parse(s)                  // OK: "23" -> 23
                         select i * 1000;                    // OK: return 23 * 1000 as option

            Assert.That(result, Is.EqualTo(Option.Some(23000)));
        }

        [Test]
        public void LookupFailedAndParseSucceed()
        {
            var result = from s in Dict.Lookup("notExistingValue")  // FAIL: key does not exists in dictionary
                         from i in Parse(s)                         // -
                         select i * 1000;                           // -

            Assert.That(result, Is.EqualTo(Option.None<int>()));
        }

        [Test]
        public void LookupSucceedAndParseFailed()
        {
            var result = from s in Dict.Lookup("badValue")  // OK
                         from i in Parse(s)                 // FAIL: can not parse "abcd" as integer
                         select i * 1000;                   // -

            Assert.That(result, Is.EqualTo(Option.None<int>()));
        }
    }
}
