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
        public static IParser<IEnumerable<char>, int> Calculator()
        {
            var addOp = OpParser('+', (x, y) => x + y);
            var subOp = OpParser('-', (x, y) => x - y);
            var mulOp = OpParser('*', (x, y) => x * y);
            var divOp = OpParser('/', (x, y) => x / y);

            IParser<IEnumerable<char>, int> expr = null;

            var factor =
                Integer().Or(
                    from l in CharParser.Token(CharParser.Symbol('('))
                    from n in expr
                    from r in CharParser.Token(CharParser.Symbol(')'))
                    select n);

            var term = factor.Chainl1(mulOp.Or(divOp));
            expr = term.Chainl1(addOp.Or(subOp));
            return expr;
        }

        private static IParser<IEnumerable<char>, int> Integer()
        {
            return CharParser.Token(CharParser.Integer());
        }

        private static IParser<IEnumerable<char>, Func<int, int, int>> OpParser(char op, Func<int, int, int> calc)
        {
            return CharParser.Token(ParserCombinators.Sat<char>(c => c == op)).Select(_ => calc);
        }
    }
}
