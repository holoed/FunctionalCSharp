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

using System.Linq;
using FsCheck;
using FunctionalCSharp.Parser;
using NUnit.Framework;

namespace FunctionalCSharpTests.Parser
{
    [TestFixture]
    public class CharParserTests
    {
        [Test]
        public void Lower()
        {
            CollectionAssert.AreEqual(new[] { 'h' }, CharParser.Lower().Execute("hello"));
            CollectionAssert.AreEqual(new char[0], CharParser.Lower().Execute("Hello"));
        }

        [Test]
        public void Upper()
        {
            CollectionAssert.AreEqual(new[] { 'H' }, CharParser.Upper().Execute("Hello"));
            CollectionAssert.AreEqual(new char[0], CharParser.Upper().Execute("hello"));
        }

        [Test]
        public void Whitespace()
        {
            CollectionAssert.AreEqual(new[] { ' ' }, CharParser.Whitespace().Execute(" Hello"));
            CollectionAssert.AreEqual(new char[0], CharParser.Whitespace().Execute("Hello"));
        }

        [Test]
        public void Letter()
        {
            CollectionAssert.AreEqual(new[] { 'H' }, CharParser.Letter().Execute("Hello"));
            CollectionAssert.AreEqual(new char[0], CharParser.Letter().Execute("1234"));
        }

        [Test]
        public void Word()
        {
            CollectionAssert.AreEqual(new[] { "Hello" }, CharParser.Word().Execute("Hello Word"));
            CollectionAssert.AreEqual(new string[0], CharParser.Word().Execute("1234"));
        }

        [Test]
        public void Number()
        {
            CollectionAssert.AreEqual(new[] { "123" }, CharParser.Number().Execute("123 456"));
            CollectionAssert.AreEqual(new string[0], CharParser.Number().Execute("Hello World"));
        }

        [Test]
        public void Integer()
        {
            Spec.ForAny<int>(x => CharParser.Integer().Execute(x.ToString()).First() == x).Check(FsNUnit.Config);
        }    
    }
}


