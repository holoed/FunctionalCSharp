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
using FunctionalCSharp.Tuples;
using NUnit.Framework;

namespace FunctionalCSharpTests.Tuples
{
    [TestFixture]
    public class TupleTests
    {
        [Test]
        public void New()
        {
            var tuple = Tuple.New(12, "435");
            Assert.AreEqual(12, tuple.First);
            Assert.AreEqual("435", tuple.Second);
        }

        [Test]
        public void ToStringTest()
        {
            var tuple = Tuple.New(12, "435");
            Assert.AreEqual("12, \"435\"", tuple.ToString());
        }

        [Test]
        public void EqualsTest()
        {
            Assert.AreEqual(Tuple.New(12, "435"), Tuple.New(12, "435"));
            Assert.AreNotEqual(Tuple.New(12, "435"), Tuple.New("435", 12));
            Assert.AreEqual(Tuple.New(new DateTime(2010, 1, 1), "5"), Tuple.New(new DateTime(2010, 1, 1), "5"));
        }
    }
}
