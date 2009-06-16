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

namespace FunctionalCSharp
{
    public static class ListActivePatterns
    {
        public static PatternMatching<IEnumerable<T>> List<T>(this PatternMatching<IEnumerable<T>> x, Func<T, IEnumerable<T>, object> f)
        {
            var head = default(T);
            IEnumerator<T> enumerator = null;
            return x.With<IEnumerable<T>>(b => !IsNotEmpty(b, out head, out enumerator), c => f(head, Tail(enumerator)));
        }

        private static bool IsNotEmpty<T>(IEnumerable<T> seq, out T head, out IEnumerator<T> enumerator)
        {
            enumerator = seq.GetEnumerator();
            if (enumerator.MoveNext())
            {
                head = enumerator.Current;
                return false;
            }
            head = default(T);
            return true;
        }

        private static IEnumerable<T> Tail<T>(IEnumerator<T> enumerator)
        {
            while (enumerator.MoveNext())
                yield return enumerator.Current;
        }
    }
}
