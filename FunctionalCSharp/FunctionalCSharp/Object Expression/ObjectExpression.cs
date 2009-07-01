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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FunctionalCSharp
{
    public class ObjectExpression
    {
        public static ObjectExpression<T> New<T>()
        {
            return new ObjectExpression<T>();
        }
    }

    public struct Member
    {
        public string Key;
        public MethodInfo Info;
        public Delegate Implementation;
    }

    public class ObjectExpression<T>
    {
        private readonly IDictionary<string, Member> _impl = new Dictionary<string, Member>();

        public ObjectExpression<T> With(Expression<Action<T>> name, Action<T> impl)
        {
            With(name, (o => { impl(o); return default(T); }));
            return this;
        }

        public ObjectExpression<T> With<K>(Expression<Action<T>> name, Action<K> impl)
        {
            var methodCall = (MethodCallExpression)name.Body;
            var method = new Member { Key = methodCall.Method.MetadataToken.ToString(), Info = methodCall.Method, Implementation = impl };
            _impl.Add(BuildKey(method), method);
            return this;
        }

        public ObjectExpression<T> With<K>(Expression<Action<T>> name, Func<T, K> impl)
        {
            var methodCall = (MethodCallExpression)name.Body;
            var method = new Member { Key = methodCall.Method.MetadataToken.ToString(), Info = methodCall.Method, Implementation = impl };
            _impl.Add(BuildKey(method), method);
            return this;
        }

        public T Return()
        {
            return ObjectBuilder.Build<T>(_impl);
        }

        private static string BuildKey(Member member)
        {
            if (member.Info.IsGenericMethod)
                return member.Key + member.Info.GetGenericArguments().Select(item => item.FullName).Aggregate("", (x, y) => (x + y));
            return member.Key;
        }
    }
}