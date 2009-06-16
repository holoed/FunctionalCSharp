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
using System.Linq.Expressions;
using FunctionalCSharp;
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
    }
}

