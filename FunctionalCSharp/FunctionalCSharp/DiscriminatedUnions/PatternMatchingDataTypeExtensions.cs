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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FunctionalCSharp.DiscriminatedUnions
{
    public static class PatternMatchingDataTypeExtensions
    {     
        public static PatternMatching<a> With<a, b>(this PatternMatching<a> host, Expression<Func<a, Func<b, object>>> method, Func<b, object> handler)
        {
            return host.With<a>(
                t => IsMatch(t, method),
                t1 => handler.DynamicInvoke(GetValues(t1)));
        }
       
        public static PatternMatching<a> With<a, b, c>(this PatternMatching<a> host, Expression<Func<a, Func<b, c, object>>> method, Func<b, c, object> handler)
        {
            return host.With<a>(
                t => IsMatch(t, method),
                t1 => handler.DynamicInvoke(GetValues(t1)));
        }
        
        public static PatternMatching<a> With<a, b, c, d>(this PatternMatching<a> host, Expression<Func<a, Func<b, c, d, object>>> method, Func<b, c, d, object> handler)
        {
            return host.With<a>(
                t => IsMatch(t, method),
                t1 => handler.DynamicInvoke(GetValues(t1)));
        }

        public static PatternMatching<a> With<a, b, c, d, e>(this PatternMatching<a> host, Expression<Func<a, Func<b, c, d, e, object>>> method, Func<b, c, d, e, object> handler)
        {
            return host.With<a>(
                t => IsMatch(t, method),
                t1 => handler.DynamicInvoke(GetValues(t1)));
        }

        private static bool IsMatch<a>(a t, Expression method)
        {
            return t.GetType().Name == GetMethodName(method);
        }

        private static object[] GetValues<a>(a obj)
        {
            return obj.GetType().GetFields().Select(f => f.GetValue(obj)).ToArray();
        }

        private static string GetMethodName(Expression method)
        {
            return method.Match()
                .Lambda((args, body) => GetMethodName(body))
                .Unary(GetMethodName)
                .MethodCall((mi, ps) => GetMethodName(ps.ElementAt(2)))
                .Const(c => ((MethodInfo)c).Name)
                .Return<string>();
        }
    }
}