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
            return ObjectExpression
                .New<IParser<TA, TD>>()
                .With(o => o.Parse, (TA input) => 
                    m.Parse(input)
                     .Select(x => f(x.Output).Parse(x.Rest)
                                             .Select(y => ParseResult.Create(y.Rest, p(x.Output, y.Output))))
                     .Aggregate(Enumerable.Empty<ParseResult<TA, TD>>(), (x, y) => x.Concat(y)))
                .Return();
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