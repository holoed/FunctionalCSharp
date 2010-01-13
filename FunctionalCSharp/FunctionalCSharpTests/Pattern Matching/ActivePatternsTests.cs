#region License

/* ****************************************************************************
 * Copyright (c) Edmondo Pentangelo. 
 *
 * This source code is subject to terms and conditions of the Microsoft Public License. 
 * A copy of the license can be found in the License.html file at the root of this distribution. 
 * By using this source code in any fashion, you are agreeing to be bound by the terms of the 
 * Microsoft Public License.
 *
 * You must not remove this notice, or any other, from this software.
 * ***************************************************************************/

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FunctionalCSharp;
using FunctionalCSharp.Tuples;
using NUnit.Framework;

namespace FunctionalCSharpTests
{
    [TestFixture]
    public class ActivePatternsTests
    {
        [Test]
        public void MatchExpressionWithActivePatterns()
        {
            Expression<Func<int, int, int, int>> f = (x, y, z) => (x + y) * z;

            Func<Expression, string> toString = null;
            toString = exp =>
                       exp.Match()
                           .Lambda((args, body) => toString(body))
                           .Param((name) => name)
                           .Add((l, r) => String.Format("({0} + {1})", toString(l), toString(r)))
                           .Mult((l, r) => String.Format("{0} * {1}", toString(l), toString(r)))
                           .Return<string>();

            Assert.AreEqual("(x + y) * z", toString(f));
        }

        [Test]
        public void MatchEvenOddWithArithmeticActivePatterns()
        {
            Func<int, bool> even = null;
            Func<int, bool> odd = null;
            even = exp => exp.Match()
                              .With(n => 0,     _ => true)
                              .With(n => n + 1, n => odd(n))
                              .Return<bool>();
            odd = exp => !even(exp);

            for (var i = 0; i < 10; i++)
            {
                if (i % 2 == 0)
                {
                    Assert.IsTrue(even(i));
                    Assert.IsFalse(odd(i));
                }
                else
                {
                    Assert.IsFalse(even(i));
                    Assert.IsTrue(odd(i));
                }
            }
        }

        [Test]
        public void MatchSomeOrNoneWithActivePatterns()
        {
            Func<Maybe<int>, int> 
            f = n => n.Match()
                      .Some(x => x)
                      .None(() => 42)
                      .Return<int>();
            Assert.AreEqual(5, f(5.Some()));
            Assert.AreEqual(42, f(Maybe.None<int>()));
        }

        [Test]
        public void MatchSomeLiteralOrNoneWithActivePatterns()
        {
            Func<Maybe<int>, bool>
            isFortyTwo = n => n.Match()
                               .Some(42, () => true)
                               .Some(    x  => false)
                               .None(    () => false)
                               .Return<bool>();
            Assert.IsTrue(isFortyTwo(42.Some()));
            Assert.IsFalse(isFortyTwo(35.Some()));
            Assert.IsFalse(isFortyTwo(Maybe.None<int>()));
        }

        [Test]
        public void MatchNumbers()
        {
            Func<int, bool>
            isFiveNotTwelve = n => n.Match()
                                    .Value(5, () => true)
                                    .Value(12, () => false)
                                    .Return<bool>();
            Assert.IsTrue(isFiveNotTwelve(5));
            Assert.IsFalse(isFiveNotTwelve(12));

            try
            {
                isFiveNotTwelve(42);
                Assert.Fail();
            }
            catch (MatchFailureException e)
            { Assert.AreEqual("Failed to match: System.Int32", e.Message); }
        }

        [Test]
        public void MatchStrings()
        {
            Func<string, bool>
            IsTrueOrFalse = n => n.Match()
                                  .Value("Yes", () => true)
                                  .Value("False", () => false)
                                  .Return<bool>();
            Assert.IsTrue(IsTrueOrFalse("Yes"));
            Assert.IsFalse(IsTrueOrFalse("False"));

            try
            {
                IsTrueOrFalse("Foo");
                Assert.Fail();
            }
            catch (MatchFailureException e)
            { Assert.AreEqual("Failed to match: System.String", e.Message); }
        }

        [Test]
        public void MatchSequences()
        {
            Func<Func<char, char>, Func<IEnumerable<char>, IEnumerable<char>>> map = null;
            map = f => s => s.Match()
                             .List((x, xs) => f(x).Cons(map(f)(xs)))
                             .Any(() => s)
                             .Return<IEnumerable<char>>();
            var toUpper = map(Char.ToUpper);

            Assert.AreEqual("HELLO WORLD", toUpper("Hello World").Aggregate("", (x, y) => x + y));
        }

        [Test]
        public void MatchTuples()
        {
            var value = Tuple.New(42, "Hello");
            Assert.AreEqual(42, value.Match()
                                     .Tuple((x, _) => x)
                                     .Return<int>());
            Assert.AreEqual("Hello", value.Match()
                                     .Tuple((_, s) => s)
                                     .Return<string>());
        }

        [Test]
        public void MatchTuples3()
        {
            var value = Tuple.New(42, "Hello", new DateTime());
            Assert.AreEqual(42, value.Match()
                                     .Tuple((x, y, z) => x)
                                     .Return<int>());
            Assert.AreEqual("Hello", value.Match()
                                     .Tuple((x, y, z) => y)
                                     .Return<string>());
            Assert.AreEqual(new DateTime(), value.Match()
                                                 .Tuple((x, y, z) => z)
                                                 .Return<DateTime>());
        }
    }

    public static class Extensions
    {
        public static IEnumerable<T> Cons<T>(this T head, IEnumerable<T> tail)
        {
            yield return head;
            foreach (var item in tail)
                yield return item;
        }

    }
}

