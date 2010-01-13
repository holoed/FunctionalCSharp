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
using System.Linq.Expressions;
using System.Reflection;

namespace FunctionalCSharp
{
    public static class ObjectBuilderExtensions
    {
        public static void AddMethod<T>(this ObjectBuilder host, Expression<Func<T, Action>> method, Action handler)
        {
            host.AddMethod(GetMethodInfo(method), handler);
            return;
        }

        public static void AddMethod<T, T1>(this ObjectBuilder host, Expression<Func<T, Action<T1>>> method, Action<T1> handler)
        {
            host.AddMethod(GetMethodInfo(method), handler);
            return;
        }

        public static void AddMethod<T, T1, T2>(this ObjectBuilder host, Expression<Func<T, Action<T1, T2>>> method, Action<T1, T2> handler)
        {
            host.AddMethod(GetMethodInfo(method), handler);
            return;
        }

        public static void AddMethod<T, T1, T2, T3>(this ObjectBuilder host, Expression<Func<T, Action<T1, T2, T3>>> method, Action<T1, T2, T3> handler)
        {
            host.AddMethod(GetMethodInfo(method), handler);
            return;
        }

        public static void AddMethod<T, T1, T2, T3, T4>(this ObjectBuilder host, Expression<Func<T, Action<T1, T2, T3, T4>>> method, Action<T1, T2, T3, T4> handler)
        {
            host.AddMethod(GetMethodInfo(method), handler);
            return;
        }

        public static void AddMethod<T, T1>(this ObjectBuilder host, Expression<Func<T, Func<T1>>> method, Func<T1> handler)
        {
            host.AddMethod(GetMethodInfo(method), handler);
            return;
        }

        public static void AddMethod<T, T1, T2>(this ObjectBuilder host, Expression<Func<T, Func<T1, T2>>> method, Func<T1, T2> handler)
        {
            host.AddMethod(GetMethodInfo(method), handler);
            return;
        }

        public static void AddMethod<T, T1, T2, T3>(this ObjectBuilder host, Expression<Func<T, Func<T1, T2, T3>>> method, Func<T1, T2, T3> handler)
        {
            host.AddMethod(GetMethodInfo(method), handler);
            return;
        }

        public static void AddMethod<T, T1, T2, T3, T4>(this ObjectBuilder host, Expression<Func<T, Func<T1, T2, T3, T4>>> method, Func<T1, T2, T3, T4> handler)
        {
            host.AddMethod(GetMethodInfo(method), handler);
            return;
        }

        public static void AddMethod<T, T1, T2, T3, T4, T5>(this ObjectBuilder host, Expression<Func<T, Func<T1, T2, T3, T4, T5>>> method, Func<T1, T2, T3, T4, T5> handler)
        {
            host.AddMethod(GetMethodInfo(method), handler);
            return;
        }

        private static MethodInfo GetMethodInfo(Expression exp)
        {
            return exp.Match()
                      .With<MemberExpression>(m => ((PropertyInfo)m.Member).GetGetMethod())
                      .With<LambdaExpression>(l => GetMethodInfo(l.Body))
                      .With<UnaryExpression>(u => GetMethodInfo(u.Operand))
                      .With<MethodCallExpression>(m => GetMethodInfo(m.Arguments[2]))
                      .With<ConstantExpression>(c => c.Value)
                      .Return<MethodInfo>();
        }
    }
}
