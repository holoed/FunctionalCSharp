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
using System.Linq;

namespace FunctionalCSharp.Parser
{
    public struct ParseResult<TA, TB>
    {
        public TB Output;
        public TA Rest;
    }

    public static class ParseResult
    {
        public static ParseResult<TA, TB> Create<TA, TB>(TA rest, TB output)
        {
            return new ParseResult<TA, TB> {Output = output, Rest = rest};
        }


        public static ParseResult<IEnumerable<TA>, TA> Create<TA>(IEnumerable<TA> input)
        {
            return new ParseResult<IEnumerable<TA>, TA>
                       {Output = input.First(), Rest = input.Skip(1)};
        }
    }
}