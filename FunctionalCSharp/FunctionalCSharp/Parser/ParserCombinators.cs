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
    public static class ParserCombinators
    {
        public static IParser<IEnumerable<TA>, TA> Item<TA>()
        {
            return ObjectExpression
                .New<IParser<IEnumerable<TA>, TA>>()
                .With(o => o.Parse, (IEnumerable<TA> input) => input.Any() ? ParseResult.Create(input).Return() :
                                                                             Enumerable.Empty<ParseResult<IEnumerable<TA>, TA>>())
                .Return();
        }

        public static IParser<IEnumerable<TA>, TA> Sat<TA>(Predicate<TA> p)
        {
            return from x in Item<TA>()
                   where p(x)
                   select x;
        }

        public static IParser<TA, TB> Or<TA, TB>(this IParser<TA, TB> p, IParser<TA, TB> q)
        {
            return ObjectExpression
                .New<IParser<TA, TB>>()
                .With(o => o.Parse,
                      (TA input) =>
                          {
                              var x = p.Parse(input);
                              return x.Any() ? x : q.Parse(input);
                          })
                .Return();
        }

        public static IParser<IEnumerable<TA>, IEnumerable<TB>> Many1<TA, TB>(this IParser<IEnumerable<TA>, TB> p)
        {
            return from x in p
                   from xs in Many(p)
                   select x.Cons(xs);
        }

        public static IParser<IEnumerable<TA>, IEnumerable<TB>> Many<TA, TB>(this IParser<IEnumerable<TA>, TB> p)
        {
            return Many1(p).Or(ParserMonad.Return<IEnumerable<TA>, IEnumerable<TB>>(Enumerable.Empty<TB>()));
        }

        public static IParser<IEnumerable<TA>, IEnumerable<TB>> SepBy1<TA, TB>(this IParser<IEnumerable<TA>, TB> p, IParser<IEnumerable<TA>, TB> sep)
        {
            var q = from _ in sep
                    from y in p
                    select y;

            return from x in p
                   from xs in q.Many()
                   select x.Cons(xs);
        }

        public static IParser<IEnumerable<TA>, IEnumerable<TB>> SepBy<TA, TB>(this IParser<IEnumerable<TA>, TB> p, IParser<IEnumerable<TA>, TB> sep)
        {
            return SepBy1(p, sep).Or(ParserMonad.Return<IEnumerable<TA>, IEnumerable<TB>>(Enumerable.Empty<TB>()));
        }

        public static IParser<IEnumerable<TA>, TB> Chainl1<TA, TB>(this IParser<IEnumerable<TA>, TB> p, IParser<IEnumerable<TA>, Func<TB, TB, TB>> op)
        {
            Func<TB, IParser<IEnumerable<TA>, TB>> rest = null;
            rest = l => (from f in op
                         from r in p
                         from ret in rest(f(l, r))
                         select ret).Or(ParserMonad.Return<IEnumerable<TA>, TB>(l));
            return from l in p
                   from ret in rest(l)
                   select ret;
        }

        public static IParser<IEnumerable<TA>, TA> Chainl<TA>(this IParser<IEnumerable<TA>, TA> p, IParser<IEnumerable<TA>, Func<TA, TA, TA>> op, TA l)
        {
            return Chainl1(p, op).Or(ParserMonad.Return<IEnumerable<TA>, TA>(l));
        }
    }
}