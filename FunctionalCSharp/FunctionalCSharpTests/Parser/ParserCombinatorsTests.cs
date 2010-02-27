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
using FunctionalCSharp.Parser;
using NUnit.Framework;

namespace FunctionalCSharpTests.Parser
{
    [TestFixture]
    public class ParserCombinatorsTests
    {
        [Test]
        public void Item()
        {
            CollectionAssert.AreEqual(new[] { 'H' }, ParserCombinators.Item<char>().Execute("Hello"));
            CollectionAssert.AreEqual(new[] { '1' }, ParserCombinators.Item<char>().Execute("1234"));
        }

        [Test]
        public void Sat()
        {
            CollectionAssert.AreEqual(new char[0], ParserCombinators.Sat<char>(c => c == 'X').Execute("Hello"));
            CollectionAssert.AreEqual(new[] { 'H' }, ParserCombinators.Sat<char>(c => c == 'H').Execute("Hello"));
            CollectionAssert.AreEqual(new[] { '1' }, ParserCombinators.Sat<char>(c => c == '1').Execute("1234"));
        }

        [Test]
        public void Or()
        {
            var parser = ParserCombinators.Sat<char>(c => c == '1').Or(ParserCombinators.Sat<char>(c => c == '2'));
            CollectionAssert.AreEqual(new[] { '1' }, parser.Execute("1234"));
            CollectionAssert.AreEqual(new[] { '2' }, parser.Execute("2345"));
        }

        [Test]
        public void Many()
        {
            var parser = ParserCombinators.Item<char>().Many().AsString();
            CollectionAssert.AreEqual(new[] { "Hello World" }, parser.Execute("Hello World"));
            CollectionAssert.AreEqual(new[] {""}, parser.Execute(""));            
        }

        [Test]
        public void SepBy()
        {
            var parser = ParserCombinators.Sat<char>(Char.IsLetter).SepBy(
                ParserCombinators.Sat<char>(Char.IsWhiteSpace));
            CollectionAssert.AreEqual(new[]{ new[]{'H', 'e', 'l', 'l', 'o'} }, parser.Execute("H e l l o")); 
        }

        [Test]
        public void Chainl1()
        {
            Func<string, string, string> f = (x, y) => String.Format("({0}{1})",x,y);
            var parser = ParserCombinators.Sat<char>(Char.IsLetter).Many().AsString().Chainl1(
                         ParserCombinators.Sat<char>(Char.IsWhiteSpace).Select(_ => f));
            CollectionAssert.AreEqual(new[] { "((((He)l)l)o)" }, parser.Execute("H e l l o"));
        }
    }
}


