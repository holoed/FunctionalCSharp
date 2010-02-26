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
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Cons<T>(this T x, IEnumerable<T> xs)
        {
            yield return x;
            foreach (var y in xs)
                yield return y;
        }

        public static IEnumerable<T> Return<T>(this T t)
        {
            yield return t;
        }

        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
                action(item);
        }
    }
}


