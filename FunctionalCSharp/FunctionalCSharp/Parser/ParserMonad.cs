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
    public static class ParserMonad
    {
        public static IParser<TA, TB> Return<TA, TB>(TB x)
        {
            return ObjectExpression
                .New<IParser<TA, TB>>()
                .With(o => o.Parse, (TA input) => ParseResult.Create(input, x).Return())
                .Return();
        }


        public static IParser<TA, TC> Select<TA, TB, TC>(this IParser<TA, TB> m, Func<TB, TC> selector)
        {
            return ObjectExpression
                .New<IParser<TA, TC>>()
                .With(o => o.Parse, (TA input) => m.Parse(input).Select(x => ParseResult.Create(x.Rest, selector(x.Output))))
                .Return();
        }


        public static IParser<TA, TD> SelectMany<TA, TB, TC, TD>(this IParser<TA, TB> m, Func<TB, IParser<TA, TC>> f, Func<TB, TC, TD> p)
        {
            return new SelectManyImpl<TA, TB, TC, TD>(m, f, p);
        }

        private class SelectManyImpl<TA, TB, TC, TD>: IParser<TA, TD>
        {
            private readonly IParser<TA, TB> _m;
            private readonly Func<TB, IParser<TA, TC>> _f;
            private readonly Func<TB, TC, TD> _p;

            public SelectManyImpl(IParser<TA, TB> m, Func<TB, IParser<TA, TC>> f, Func<TB, TC, TD> p)
            {
                _m = m;
                _f = f;
                _p = p;
            }

            public IEnumerable<ParseResult<TA, TD>> Parse(TA input)
            {
                return _m.Parse(input)
                    .Select(x => _f(x.Output)
                                     .Parse(x.Rest)
                                     .Select(y => ParseResult.Create(y.Rest, _p(x.Output, y.Output))))
                    .Aggregate(Enumerable.Empty<ParseResult<TA, TD>>(), (x, y) => x.Concat(y));
            }
        }


        public static IParser<TA, TB> Where<TA, TB>(this IParser<TA, TB> m, Predicate<TB> p)
        {
            return ObjectExpression
                .New<IParser<TA, TB>>()
                .With(o => o.Parse, (TA input) => m.Parse(input)
                                                     .Where(xi => p(xi.Output)))
                .Return();
        }
    }
}