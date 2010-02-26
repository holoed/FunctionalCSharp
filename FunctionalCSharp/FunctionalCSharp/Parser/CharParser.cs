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

namespace FunctionalCSharp.Parser
{
    public static class CharParser
    {
        public static IParser<T, string> AsString<T>(this IParser<T, IEnumerable<char>> p)
        {
            return from x in p
                   select new String(x.ToArray());
        }

        public static IParser<IEnumerable<char>, char> Lower()
        {
            return ParserCombinators.Sat<char>(Char.IsLower);
        }

        public static IParser<IEnumerable<char>, char> Upper()
        {
            return ParserCombinators.Sat<char>(Char.IsUpper);
        }

        public static IParser<IEnumerable<char>, char> Whitespace()
        {
            return ParserCombinators.Sat<char>(Char.IsWhiteSpace);
        }

        public static IParser<IEnumerable<char>, char> Digit()
        {
            return ParserCombinators.Sat<char>(Char.IsDigit);
        }

        public static IParser<IEnumerable<char>, char> Letter()
        {
            return Lower().Or(Upper());
        }

        public static IParser<IEnumerable<char>, IEnumerable<char>> Word()
        {
            return Letter().Many1();
        }

        public static IParser<IEnumerable<char>, IEnumerable<char>> Number()
        {
            return Digit().Many1();
        }

        public static IEnumerable<T> ParseString<T>(this IParser<IEnumerable<char>, T> p, string input)
        {
            return from x in p.Parse(input)
                   select x.Output;
        }        
    }
}