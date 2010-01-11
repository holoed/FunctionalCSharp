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
    public struct Match
    {
        public object Value;
        public bool Success;
    }

    public static class PatternMatchingExtensions
    {
        public static PatternMatching<T> Match<T>(this T obj)
        {
            return new PatternMatching<T>(() => new Match{Value = obj, Success = false}); 
        }

        public static PatternMatching<T> Default<T>(this PatternMatching<T> x, Func<object, object> f)
        {
            return x.With<T>(b => true, b => f(b));
        }
    }

    public class PatternMatching<T>
    {
        private readonly Func<Match> _f;

        public PatternMatching(Func<Match> f)
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
                        return obj.Success
                                   ? obj
                                   : (obj.Value is TYPE_PATTERN && p((TYPE_PATTERN) obj.Value) 
                                          ? Success(f, obj) 
                                          : Fail(obj));
                    });
        }

        private static Match Fail(Match obj)
        {
            return new Match
            {
                Value = obj.Value,
                Success = false
            };
        }

        private static Match Success<TYPE_PATTERN>(Func<TYPE_PATTERN, object> f, Match obj)
        {
            return new Match
                       {
                           Value = f((TYPE_PATTERN) obj.Value),
                           Success = true
                       };
        }

        public PatternMatching<T> Any(Func<object> f)
        {
            return new PatternMatching<T>(
                () =>
                    {
                        var obj = _f();
                        return obj.Success ? obj : new Match {Value = f(), Success = true};
                    });
        }

        public TResult Return<TResult>()
        {
            var ret = _f();
            if (ret.Success && ret.Value is TResult)
                return (TResult)ret.Value;
            throw new MatchFailureException(String.Format("Failed to match: {0}", ret.Value.GetType()));
        }
    }

    public class MatchFailureException : Exception
    {
        public MatchFailureException(string message): base(message)
        {}
    }
}