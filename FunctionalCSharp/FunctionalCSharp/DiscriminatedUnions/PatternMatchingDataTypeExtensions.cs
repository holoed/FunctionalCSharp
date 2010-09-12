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
                t1 => handler((b) GetValues(t1)[0]));
        }
       
        public static PatternMatching<a> With<a, b, c>(this PatternMatching<a> host, Expression<Func<a, Func<b, c, object>>> method, Func<b, c, object> handler)
        {
            return host.With<a>(
                t => IsMatch(t, method),
                t1 =>
                    {
                        var values = GetValues(t1);
                        return handler((b) values[0], (c) values[1]);
                    });
        }
        
        public static PatternMatching<a> With<a, b, c, d>(this PatternMatching<a> host, Expression<Func<a, Func<b, c, d, object>>> method, Func<b, c, d, object> handler)
        {
            return host.With<a>(
                t => IsMatch(t, method),
                t1 =>
                    {
                        var values = GetValues(t1);
                        return handler((b)values[0], (c)values[1], (d)values[2]);
                    });
        }

        public static PatternMatching<a> With<a, b, c, d, e>(this PatternMatching<a> host, Expression<Func<a, Func<b, c, d, e, object>>> method, Func<b, c, d, e, object> handler)
        {
            return host.With<a>(
                t => IsMatch(t, method),
                t1 =>
                    {
                        var values = GetValues(t1);
                        return handler((b)values[0], (c)values[1], (d)values[2], (e)values[3]);
                    });
        }

        private static bool IsMatch<a>(a t, Expression method)
        {
            return t.GetType().Name == GetMethodName(method);
        }

        private static object[] GetValues<a>(a obj)
        {
            var dataType = obj as IDataType;
            return dataType != null ? dataType.Values : obj.GetType().GetFields().Select(f => f.GetValue(obj)).ToArray();
        }

        private static string GetMethodName(Expression method)
        {
            var lambda = (LambdaExpression) method;
            var unary = (UnaryExpression) lambda.Body;
            var methodCall = (MethodCallExpression) unary.Operand;
            var constant = (ConstantExpression) methodCall.Arguments[2];
            var methodInfo = (MethodInfo) constant.Value;
            return methodInfo.Name;
        }
    }
}