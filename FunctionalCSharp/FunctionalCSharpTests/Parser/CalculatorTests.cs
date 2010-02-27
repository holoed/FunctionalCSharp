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

using System.Collections.Generic;
using FunctionalCSharp.Parser;
using NUnit.Framework;

namespace FunctionalCSharpTests.Parser
{
    [TestFixture]
    public class CalculatorTests
    {
        [Test]
        public void Add()
        {
            var expected = new[] { "5" };
            CollectionAssert.AreEqual(expected, Calculate("2+3"));
            CollectionAssert.AreEqual(expected, Calculate("2 +3"));
            CollectionAssert.AreEqual(expected, Calculate("2+ 3"));
            CollectionAssert.AreEqual(expected, Calculate("2 + 3"));
        }

        [Test]
        public void Sub()
        {
            CollectionAssert.AreEqual(new[] { "-1" }, Calculate("2-3"));
            CollectionAssert.AreEqual(new[] { "-1" }, Calculate("2 - 3"));
            CollectionAssert.AreEqual(new[] { "-1" }, Calculate("2  - 3"));
            CollectionAssert.AreEqual(new[] { "-1" }, Calculate("2 -  3"));
        }

        [Test]
        public void Div()
        {
            CollectionAssert.AreEqual(new[] { "3" }, Calculate("9/3"));
            CollectionAssert.AreEqual(new[] { "3" }, Calculate("9 /3"));
            CollectionAssert.AreEqual(new[] { "3" }, Calculate("9/ 3"));
            CollectionAssert.AreEqual(new[] { "3" }, Calculate("9   / 3"));
        }

        [Test]
        public void Mul()
        {
            CollectionAssert.AreEqual(new[] { "27" }, Calculate("9*3"));
            CollectionAssert.AreEqual(new[] { "27" }, Calculate("9  *3"));
            CollectionAssert.AreEqual(new[] { "27" }, Calculate("9* 3"));
            CollectionAssert.AreEqual(new[] { "27" }, Calculate("9 * 3"));
        }

        [Test]
        public void AddMul()
        {
            CollectionAssert.AreEqual(new[] { "29" }, Calculate("2+9*3"));
            CollectionAssert.AreEqual(new[] { "21" }, Calculate("2 * 9 + 3"));
        }

        [Test]
        public void AddMulDivSub()
        {
            CollectionAssert.AreEqual(new[] { "9" }, Calculate("3 + 4 * 2 - 4 / 2"));
        }

        private static IEnumerable<string> Calculate(string exp)
        {
            return CalculatorCombinators.Calculator().Execute(exp);
        }
    }
}
