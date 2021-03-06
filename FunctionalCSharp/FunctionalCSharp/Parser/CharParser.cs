﻿#region License

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

        public static IParser<IEnumerable<char>, IEnumerable<char>> Whitespaces()
        {
            return Whitespace().Many();
        }

        public static IParser<IEnumerable<char>, char> Digit()
        {
            return ParserCombinators.Sat<char>(Char.IsDigit);
        }

        public static IParser<IEnumerable<char>, char> Symbol()
        {
            return ParserCombinators.Sat<char>(Char.IsPunctuation);
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

        public static IParser<IEnumerable<char>, b> Token<b>(IParser<IEnumerable<char>, b> p)
        {
            return from x in p
                   from y in Whitespaces()
                   select x;
        }

        public static IParser<IEnumerable<char>, char> Symbol(char symbol)
        {
            return from x in Symbol()
                   where x == symbol
                   select x;
        }

        public static IParser<IEnumerable<char>, int> Integer()
        {
            Func<int, int> negate = x => -x;
            var op = (from _ in Symbol('-')
                      select negate).Or(ParserMonad.Return<IEnumerable<char>, Func<int, int>>(x => x));

            return from f in op
                   from n in Number().AsString()
                   select f(Int32.Parse(n));
        }

        public static IEnumerable<T> Execute<T>(this IParser<IEnumerable<char>, T> p, IEnumerable<char> input)
        {
            return from x in p.Parse(input)
                   select x.Output;
        }        
    }
}