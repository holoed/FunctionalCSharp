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
    public static class PatternMatchingExtensions
    {
        public static PatternMatching<T> Match<T>(this T obj)
        {
            return new PatternMatching<T>(() => obj); 
        }
    }

    public class PatternMatching<T>
    {
        private readonly Func<object> _f;

        public PatternMatching(Func<object> f)
        {
            _f = f;
        }

        public PatternMatching<T> With<TYPE_PATTERN>(Func<TYPE_PATTERN, object> f)
        {
            return With(_ => true, f);
        }

        public PatternMatching<T> With<TYPE_PATTERN>(Func<TYPE_PATTERN, bool> p, Func<TYPE_PATTERN, object> f)
        {
            return new PatternMatching<T>(
                () =>
                {
                    var obj = _f();
                    return obj is TYPE_PATTERN && p((TYPE_PATTERN)obj) ? f((TYPE_PATTERN)obj) : obj;
                });
        }

        public TResult Return<TResult>()
        {
            var ret = _f();
            if (ret is TResult)
                return (TResult)ret;
            throw new MatchFailureException(String.Format("Failed to match: {0}", ret.GetType()));
        }
    }

    public class MatchFailureException : Exception
    {
        public MatchFailureException(string message): base(message)
        {}
    }
}