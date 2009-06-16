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
    public static class MaybeActivePatterns
    {
        public static PatternMatching<Maybe<T>> Some<T>(this PatternMatching<Maybe<T>> x, Func<T, object> f)
        {
            return x.With<Maybe<T>>(b => b is Some<T>, b => f(Value(b)));
        }

        public static PatternMatching<Maybe<T>> Some<T>(this PatternMatching<Maybe<T>> x, T expected, Func<object> f)
        {
            return x.With<Maybe<T>>(b => b is Some<T> && Value(b).Equals(expected), _ => f());
        }

        public static PatternMatching<Maybe<T>> None<T>(this PatternMatching<Maybe<T>> x, Func<object> f)
        {
            return x.With<Maybe<T>>(b => b is None<T>, b => f());
        }

        private static T Value<T>(Maybe<T> b)
        {
            return ((Some<T>)b).Value;
        }
    }
}
