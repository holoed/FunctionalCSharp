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

namespace FunctionalCSharp.Parser
{
    public static class CalculatorCombinators
    {
        public static IParser<IEnumerable<char>, string> Calculator()
        {
            var addOp = CharParser.Token(ParserCombinators.Sat<char>(c => c == '+'))
                        .Select(_ => Calc((x, y) => x + y));
            var subOp = CharParser.Token(ParserCombinators.Sat<char>(c => c == '-'))
                        .Select(_ => Calc((x, y) => x - y));
            var mulOp = CharParser.Token(ParserCombinators.Sat<char>(c => c == '*'))
                        .Select(_ => Calc((x, y) => x * y));
            var divOp = CharParser.Token(ParserCombinators.Sat<char>(c => c == '/'))
                        .Select(_ => Calc((x, y) => x / y));

            var factor = CharParser.Token(CharParser.Number()).AsString();
            var term = factor.Chainl1(mulOp.Or(divOp));
            var exps = term.Chainl1(addOp.Or(subOp));
            return exps;
        }

        private static Func<string, string, string> Calc(Func<int, int, int> f)
        {
            return (lc, rc) => f(Int32.Parse(lc), Int32.Parse(rc)).ToString();
        }     
    }
}
