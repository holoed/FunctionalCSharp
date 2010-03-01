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

namespace FunctionalCSharp
{
    public static class TuplesActivePatterns
    {
        public static PatternMatching<Tuple<T1,T2>> Tuple<T1,T2,TRet>(this PatternMatching<Tuple<T1,T2>> x, Func<T1,T2,TRet> action)
        {
            return x.With<Tuple<T1, T2>>(pair => action(pair.Item1, pair.Item2));
        }

        public static PatternMatching<Tuple<T1, T2, T3>> Tuple<T1, T2, T3, TRet>(this PatternMatching<Tuple<T1, T2, T3>> x, Func<T1, T2, T3, TRet> action)
        {
            return x.With<Tuple<T1, T2, T3>>(pair => action(pair.Item1, pair.Item2, pair.Item3));
        }
    }
}
